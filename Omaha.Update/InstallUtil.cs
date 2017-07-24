using System;

namespace Omaha.Update
{
    public class InstallUtil
    {
        public static bool IsPerUserInstall(string executable)
        {
            string installFolder;
            if (IntPtr.Size == 8) //is64BitProcess
                installFolder = Environment.GetEnvironmentVariable("PROGRAMFILES(x86)");
            else
                installFolder = Environment.GetEnvironmentVariable("PROGRAMFILES");

            if (!executable.ToLower().StartsWith(installFolder.ToLower()))
                return true;
            return false;
        }
    }
}
