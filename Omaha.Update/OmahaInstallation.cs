using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;

using Microsoft.Win32;

namespace Omaha.Update
{
    public class OmahaInstallation
    {
        private bool IsMachineInstallation { get; set; }
        private Guid AppId { get; set; }
        private string OmahaName { get; set; }

        public OmahaInstallation(bool isMachineInstallation, string omahaName, Guid appId)
        {
            IsMachineInstallation = isMachineInstallation;
            AppId = appId;
            OmahaName = omahaName;

            //TODO: make omaha work in user context
            if (!IsMachineInstallation) throw new NotImplementedException();
        }

        //Get as user possible
        //set requires admin privilegues
        public string Channel
        {
            get
            {
                var value = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\" + (Windows.Is64BitOperatingSystem ? "WOW6432Node\\" : "") + OmahaName + @"\Update\ClientState\{" + AppId.ToString().ToUpper() + "}", "channel", OmahaConstants.DefaultBrand);
                return value?.ToString();
            }
            set
            {
                string baseKeyName = @"HKEY_LOCAL_MACHINE\Software\" + (Windows.Is64BitOperatingSystem ? "WOW6432Node\\" : "") + OmahaName + @"\Update\ClientState\{" + AppId.ToString().ToUpper() + "}";
                WriteRegistryValue(baseKeyName, "channel", value);
            }
        }

        public Guid UserId
        {
            get
            {
                var value = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\" + (Windows.Is64BitOperatingSystem ? "WOW6432Node\\" : "") + OmahaName + @"\Update", "uid", null);
                return value == null ? Guid.Empty : new Guid(value.ToString());
            }
        }

        /// <summary>
        /// Writes the registry value.
        /// </summary>
        /// <param name="baseKeyName">Name of the base key.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="SystemException"></exception>
        private void WriteRegistryValue(string baseKeyName, string key, string value)
        {
            if(string.IsNullOrEmpty(baseKeyName)) throw new ArgumentNullException(nameof(baseKeyName));
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            if (Thread.CurrentPrincipal.IsInRole(WindowsBuiltInRole.Administrator.ToString()))
                Registry.SetValue(baseKeyName, key, value);
            else
            {
                Process p = new Process()
                {
                    StartInfo = new ProcessStartInfo("reg.exe", "ADD " + baseKeyName + " /v " + key + " /f /d " + value)
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "runas",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    },
                };
                p.Start();
                p.WaitForExit();
            }
        }
    }
}
