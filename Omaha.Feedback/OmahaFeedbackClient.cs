using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Omaha.Feedback.Proto;
using Omaha.Feedback.Proto.Extension;
using Omaha.Feedback.Proto.Math;
using Omaha.Feedback.Proto.Web;

using ProtoBuf;

namespace Omaha.Feedback
{
    //TODO: use streams! do not load the hole request into memory!
    public class OmahaFeedbackClient
    {
        private string OmahaServer { get; set; }
        private bool IsTsl { get; set; }
        private HttpClient HttpClient { get; set; }
        
        public OmahaFeedbackClient(bool isTsl, string omahaServer)
        {
            OmahaServer = omahaServer;
            HttpClient = new HttpClient();
            IsTsl = isTsl;
        }
        ~OmahaFeedbackClient()
        {
            HttpClient.Dispose();
        }

        private async Task<bool> SendProtocolBufferRequest(object obj)
        {
            OmahaLogProvider.GetInstance(Omaha.OmahaConstants.CompanyName, OmahaConstants.AppName, Omaha.OmahaConstants.LogLevel).Info("send protocol buffer request");
            
            string filename = Guid.NewGuid().ToString() + ".tmp";
            using (FileStream ms = new FileStream(filename, FileMode.CreateNew))
            {
                Serializer.Serialize(ms, obj);

                var request = new HttpRequestMessage(HttpMethod.Post, (IsTsl ? "https://" : "http://") + OmahaServer + OmahaConstants.OmahaFeedbackPage );
                ms.Seek(0, SeekOrigin.Begin);
                request.Content = new StreamContent(ms);
                request.Content.Headers.Add("Content-Type", "application/x-protobuf");

                var result = (await HttpClient.SendAsync(request));
                File.Delete(filename);
                return result.IsSuccessStatusCode;
            }
        }

        [Obsolete("This mehtod presuppose a wide knowledge about the used contract, use OmahaFeedbackClient::SendFeedback() instead")]
        public async Task<bool> SendExtensionSubmit(ExtensionSubmit extensionSubmit)
        {
            return await SendProtocolBufferRequest(extensionSubmit);
        }
        [Obsolete("This mehtod presuppose a wide knowledge about the used contract, use OmahaFeedbackClient::SendFeedback() instead")]
        public async Task<bool> SendSuggestQuery(SuggestQuery suggestQuery)
        {
            return await SendProtocolBufferRequest(suggestQuery);
        }

        public async Task<bool> SendFeedback(OmahaFeedback feedback)
        { return await SendFeedback(feedback.Description, feedback.Email, feedback.Screenshot?.Image, feedback.Screenshot?.Height ?? 0, feedback.Screenshot?.Width ?? 0, feedback.AdditionalFile, feedback.SystemInfoJson); }
        public async Task<bool> SendFeedback(string description, string email, InternetMedia image, int imageHeight, int imageWidth, InternetMedia[] additionalFile, string systemInfoJson)
        {
            var webData = new WebData()
            {
                Annotations = new Annotation[] { },
                ProductSpecificDatas = new ProductSpecificData[] { },
                ProductSpecificBinaryDataNames = new string[] { }
            };
            var commonData = new CommonData()
            {
                Description = description,
                UserEmail = email
            };

            PostedScreenshot screenShot = null;
            if (image != null)
                screenShot = new PostedScreenshot()
                {
                    BinaryContent = image.Data,
                    MimeType = image.MimeType,
                    Dimensions = new Dimensions()
                    {
                        Height = imageHeight,
                        Width = imageWidth
                    }
                };
            
            var baseFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + Omaha.OmahaConstants.CompanyName + "\\" + OmahaConstants.AppName + "\\";
            var tmpFolderPath = baseFolderPath + Guid.NewGuid();
            while (Directory.Exists(tmpFolderPath))
            { tmpFolderPath = baseFolderPath + Guid.NewGuid(); }
            Directory.CreateDirectory(tmpFolderPath);

            foreach (var file in OmahaLogProvider.GetLogFiles())
            {
                File.Copy(file.Value, tmpFolderPath + "\\" + file.Key.Replace("\\", "") + ".log");
            }
            foreach (var addition in additionalFile)
            {
                File.WriteAllBytes(tmpFolderPath + "\\" + addition.MimeType, addition.Data);
            }

            var zipFileName = baseFolderPath + Guid.NewGuid() + ".zip";
            while (File.Exists(zipFileName))
            { zipFileName = baseFolderPath + Guid.NewGuid() + ".zip"; }

            ZipFile.CreateFromDirectory(tmpFolderPath, zipFileName);

            var blackBox = new PostedBlackbox()
            {
                MimeType = "multipart",
                Data = File.ReadAllBytes(zipFileName)
            };
            File.Delete(zipFileName);
            foreach (var file in Directory.GetFiles(tmpFolderPath))
            {
                File.Delete(file);
            }
            Directory.Delete(tmpFolderPath);

            return await SendProtocolBufferRequest(new ExtensionSubmit()
            {
                CommonData = commonData,
                WebData = webData,
                TypeId = 0,
                ProductSpecificBinaryDatas = new[] {
                    new ProductSpecificBinaryData()
                    {
                        MimeType = "json",
                        Data = Encoding.Default.GetBytes(string.IsNullOrEmpty(systemInfoJson) ? "{}" : systemInfoJson),
                        Name = "system_data.json"
                    },
                },
                Scrennshot = screenShot,
                Blackbox = blackBox
            });
        }
        public async Task<bool> SendFeedback(string description, string email, InternetMedia image, int imageHeight, int imageWidth, string systemInfoJson)
        { return await SendFeedback(description, email, image, imageHeight, imageWidth, null, systemInfoJson); }
        public async Task<bool> SendFeedback(string description, string email, string systemInfoJson)
        {
            return await SendFeedback(description, email, null, 0, 0, null, systemInfoJson);
        }
        public async Task<bool> SendFeedback(string description, string email, InternetMedia[] additionalFile)
        { return await SendFeedback(description, email, null, 0, 0, additionalFile); }
        public async Task<bool> SendFeedback(string description, string email, InternetMedia image, int imageHeight, int imageWidth, InternetMedia[] additionalFile)
        { return await SendFeedback(description, email, image, imageHeight, imageWidth, additionalFile, string.Empty); }
        public async Task<bool> SendFeedback(string description, string email, InternetMedia image, int imageHeight, int imageWidth)
        { return await SendFeedback(description, email, image, imageHeight, imageWidth, null, string.Empty); }
        public async Task<bool> SendFeedback(string description, string email)
        { return await SendFeedback(description, email, null, 0, 0); }
    }
}
