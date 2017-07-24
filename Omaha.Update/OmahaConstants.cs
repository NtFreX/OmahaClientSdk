using System;
using GoogleUpdate3Lib;

namespace Omaha.Update
{
    [SystemInfo]
    public class OmahaConstants
    {
        public const int AllowedRetries = 1;
        public const int RetryIntervalSeconds = 5;
        public const int OmahaUpdatePollIntervalMs = 250;
        public const string AppName = "Omaha.Update";
        public const string DefaultBrand = "stable";
        
        public const long GoopdateEAppUpdateDisabledByPolicy = 0x80040813;
        public const long GoopdateEAppUpdateDisabledByPolicyManual = 0x8004081f;
        public const long GoopdateinstallEInstallerFailed = 0x80040902;
        public const long GoopdateEAppUsingExternalUpdater = 0xA043081D;

        public static readonly Guid ClsidOmahaUpdate3WebMachineClass = typeof (GoogleUpdate3WebMachineClassClass).GUID;
        public static readonly Guid ClsidOmahaUpdate3WebUserClass = typeof(GoogleUpdate3WebUserClassClass).GUID;
        public static readonly Guid OmahaAppId = new Guid("430FD4D0-B729-4F61-AA34-91526481799D");
    }
}
