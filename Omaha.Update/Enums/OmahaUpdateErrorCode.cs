namespace Omaha.Update.Enums
{
    public enum OmahaUpdateErrorCode : long
    {
        // The upgrade completed successfully (or hasn't been started yet).
        OmahaUpdateNoError = 0,
        // Google Update only supports upgrading if Chrome is installed in the default
        // location. This error will appear for developer builds and with
        // installations unzipped to random locations.
        CannotUpgradeInThisDirectory = 1,
        // Failed to create Google Update JobServer COM class. DEPRECATED.
        // GOOGLE_UPDATE_JOB_SERVER_CREATION_FAILED = 2,
        // Failed to create Google Update OnDemand COM class.
        OmahaUpdateOndemandClassNotFound = 3,
        // Google Update OnDemand COM class reported an error during a check for
        // update (or while upgrading).
        OmahaUpdateOndemandClassReportedError = 4,
        // A call to GetResults failed. DEPRECATED.
        // GOOGLE_UPDATE_GET_RESULT_CALL_FAILED = 5,
        // A call to GetVersionInfo failed. DEPRECATED
        // GOOGLE_UPDATE_GET_VERSION_INFO_FAILED = 6,
        // An error occurred while upgrading (or while checking for update).
        // Check the Google Update log in %TEMP% for more details.
        OmahaUpdateErrorUpdating = 7,
        // Updates can not be downloaded because the administrator has disabled all
        // types of updating.
        OmahaUpdateDisabledByPolicy = 8,
        // Updates can not be downloaded because the administrator has disabled
        // manual (on-demand) updates.  Automatic background updates are allowed.
        OmahaUpdateDisabledByPolicyAutoOnly = 9,
        NumErrorCodes
    }
}
