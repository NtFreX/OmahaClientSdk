using System;
using Omaha.Update.Enums;

namespace Omaha.Update
{
    public class OmahaUpdateResult
    {
        public OmahaUpdateErrorCode ErrorCode { get; set; }
        public int InstallerExitCode { get; set; }
        public string HtmlErrorMessage { get; set; }
        public Exception Exception { get; set; }
        public OmahaUpdateUpgradeStatus Status { get; set; }
    }
}
