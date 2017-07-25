namespace Omaha.Update.Enums
{
    public enum OmahaUpdateUpgradeStatus : long
    {
        // The upgrade has started. DEPRECATED.
        // UPGRADE_STARTED = 0,
        // A check for upgrade has been initiated. DEPRECATED.
        // UPGRADE_CHECK_STARTED = 1,
        // An update is available.
        UpgradeIsAvailable = 2,
        // The upgrade happened successfully.
        UpgradeSuccessful = 3,
        // No need to upgrade, Chrome is up to date.
        UpgradeAlreadyUpToDate = 4,
        // An error occurred.
        UpgradeError = 5,
        NumUpgradeStatus
    }
}
