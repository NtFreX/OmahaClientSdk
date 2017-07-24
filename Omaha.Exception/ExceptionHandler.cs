using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace Omaha.Exception
{
    public class ExceptionHandler
    {
        [DllImport("Dbghelp.dll")]
        static extern bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId, IntPtr hFile, int DumpType, ref MinidumpExceptionInformation ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct MinidumpExceptionInformation
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            public int ClientPointers;
        }

        private static readonly int MiniDumpWithFullMemory = 2;

        /// <summary>
        /// Writes the mini dump.
        /// requires full trust for the immediate caller. This member cannot be used by partially trusted or transparent code.
        /// </summary>
        /// <param name="companyName">Name of the company.</param>
        /// <param name="appName">Name of the application.</param>
        /// <returns>success indicator</returns>
        [SecurityCritical]
        public static bool WriteMiniDump(string companyName, string appName)
        {
            string filePath;
            return WriteMiniDump(companyName, appName, out filePath);
        }

        [SecurityCritical]
        public static bool WriteMiniDump(string companyName, string appName, out string miniDumpFilePath)
        {
            try
            {
                string dumpPAth = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), companyName + "\\" + appName);
                IoHelper.CreateDirectoryIfNotExists(dumpPAth);
                miniDumpFilePath = dumpPAth + "\\" + DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss") + ".dmp";
                FileStream file = new FileStream(miniDumpFilePath, FileMode.Create);
                MinidumpExceptionInformation info = new MinidumpExceptionInformation();
                info.ClientPointers = 1;
                info.ExceptionPointers = Marshal.GetExceptionPointers();
                info.ThreadId = GetCurrentThreadId();

                // A full memory dump is necessary in the case of a managed application, other wise no information
                // regarding the managed code will be available
                MiniDumpWriteDump(
                    GetCurrentProcess(),
                    GetCurrentProcessId(),
                    file.SafeFileHandle.DangerousGetHandle(),
                    MiniDumpWithFullMemory,
                    ref info,
                    IntPtr.Zero,
                    IntPtr.Zero);
                file.Close();
                return true;
            }
            catch { /*IGNORE*/ }
            miniDumpFilePath = string.Empty;
            return false;
        }

        //TODO: the field "stacktrace_json" is problematic.. if a crash with this field exists on the server the crashes cannot be viewn anymore => exception
        public static async Task<bool> UploadCrash(bool isTsl, string omahaServer, string miniDumpFile, Guid appId, Guid userId, string stackTrace, string stackTraceJson, string signature)
        {
            var baseFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + Omaha.OmahaConstants.CompanyName + "\\" + OmahaConstants.AppName + "\\";
            var tempFolderPath = baseFolderPath + Guid.NewGuid() + "\\";
            var tarFileName = Guid.NewGuid() + ".tar";
            try
            {
                IoHelper.CreateDirectoryIfNotExists(tempFolderPath);
                File.Copy(miniDumpFile, tempFolderPath + Path.GetFileName(miniDumpFile));

                //This format cannot be read by the omaha server
                //ZipFile.CreateFromDirectory(tempFolderPath, baseFolderPath + tarFileName);

                DirectoryInfo directoryOfFilesToBeTarred = new DirectoryInfo(tempFolderPath);
                FileInfo[] filesInDirectory = directoryOfFilesToBeTarred.GetFiles();
                var tarArchiveName = baseFolderPath + tarFileName;
                using (Stream targetStream = new GZipOutputStream(File.Create(tarArchiveName)))
                {
                    using (TarArchive tarArchive = TarArchive.CreateOutputTarArchive(targetStream, TarBuffer.DefaultBlockFactor))
                    {
                        foreach (FileInfo fileToBeTarred in filesInDirectory)
                        {
                            TarEntry entry = TarEntry.CreateEntryFromFile(fileToBeTarred.FullName);
                            tarArchive.WriteEntry(entry, true);
                        }
                    }
                }

                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 0, 1, 0);
                    
                    var request = new HttpRequestMessage(
                        HttpMethod.Post,
                        (isTsl ? "https://" : "http://") + omahaServer + OmahaConstants.OmahaCrashPage);

                    var content = new MultipartFormDataContent();
                    content.Add(new ByteArrayContent(File.ReadAllBytes(baseFolderPath + tarFileName)), "upload_file_minidump", tarFileName);
                    content.Add(new StringContent("{" + appId + "}"), "appid");
                    content.Add(new StringContent("{" + userId + "}"), "userid");
                    content.Add(new StringContent(signature), "signature");
                    content.Add(new StringContent(stackTrace), "stacktrace");
                    content.Add(new StringContent(stackTraceJson), "stacktrace_json");
                    request.Content = content;

                    var result = await client.SendAsync(request);
                    return result.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                try
                {
                    foreach (string file in Directory.GetFiles(tempFolderPath))
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(tempFolderPath);
                    File.Delete(baseFolderPath + tarFileName);
                } catch { /*IGNORE*/ }
            }
        }
    }
}
