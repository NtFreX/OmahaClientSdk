using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;

using Microsoft.Win32;

using Omaha.Update.Helper;

namespace Omaha.Update
{
    public class InstallationContext
    {
        private bool IsMachineInstallation { get; set; }
        private Guid AppId { get; set; }
        private string OmahaName { get; set; }

        public InstallationContext(bool isMachineInstallation, string omahaName, Guid appId)
        {
            IsMachineInstallation = isMachineInstallation;
            AppId = appId;
            OmahaName = omahaName;

            //TODO: implement the user context functionallity
            if (!IsMachineInstallation) throw new NotImplementedException();
        }

        //Get as user possible
        //set requires admin privilegues
        public string Channel
        {
            get
            {
                var value = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\" + (WindowsHelper.Is64BitOperatingSystem ? "WOW6432Node\\" : "") + OmahaName + @"\Update\ClientState\{" + AppId.ToString().ToUpper() + "}", "channel", OmahaConstants.DefaultBrand);
                return value?.ToString();
            }
            set
            {
                var baseKeyName = @"HKEY_LOCAL_MACHINE\Software\" + (WindowsHelper.Is64BitOperatingSystem ? "WOW6432Node\\" : "") + OmahaName + @"\Update\ClientState\{" + AppId.ToString().ToUpper() + "}";
                WriteRegistryValue(baseKeyName, "channel", value);
            }
        }

        public Guid UserId
        {
            get
            {
                var value = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\" + (WindowsHelper.Is64BitOperatingSystem ? "WOW6432Node\\" : "") + OmahaName + @"\Update", "uid", null);
                return value == null ? Guid.Empty : new Guid(value.ToString());
            }
        }
        
        private void WriteRegistryValue(string baseKeyName, string key, string value)
        {
            if(string.IsNullOrEmpty(baseKeyName)) throw new ArgumentNullException(nameof(baseKeyName));
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            if (Thread.CurrentPrincipal.IsInRole(WindowsBuiltInRole.Administrator.ToString()))
                Registry.SetValue(baseKeyName, key, value);
            else
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo("reg.exe", "ADD " + baseKeyName + " /v " + key + " /f /d " + value)
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "runas",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    },
                };
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
