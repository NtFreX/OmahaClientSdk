namespace Omaha.Update.Enums
{
    public enum CurrentState : long
    {
        StateInit = 1,
        StateWaitingToCheckForUpdate = 2,
        StateCheckingForUpdate = 3,
        StateUpdateAvailable = 4,
        StateWaitingToDownload = 5,
        StateRetryingDownload = 6,
        StateDownloading = 7,
        StateDownloadComplete = 8,
        StateExtracting = 9,
        StateApplyingDifferentialPatch = 10,
        // TODO(omaha3): Should we move STATE_DOWNLOAD_COMPLETE here and eliminate
        // STATE_READY_TO_INSTALL?
        StateReadyToInstall = 11,
        StateWaitingToInstall = 12,
        StateInstalling = 13,
        StateInstallComplete = 14,
        StatePaused = 15,
        StateNoUpdate = 16,
        StateError = 17,
    }
}
