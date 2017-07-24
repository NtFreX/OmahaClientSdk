using Omaha.Update.Enum;

namespace Omaha.Update
{
    public class OmahaUpdateResult
    {
        public OmahaUpdateErrorCode ErrorCode { get; set; }
        public int InstallerExitCode { get; set; }
        public string HtmlErrorMessage { get; set; }
        public System.Exception Exception { get; set; }
        public OmahaUpdateUpgradeStatus Status { get; set; }
    }
}
