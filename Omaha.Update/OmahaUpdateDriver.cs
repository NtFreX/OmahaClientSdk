using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

using GoogleUpdate3Lib;

using NLog;
using Omaha.Update.Enums;
using Omaha.Update.Exceptions;
using Omaha.Update.Helper;
using Omaha.Update.Properties;

namespace Omaha.Update
{
    public class OmahaUpdateDriver
    {
        private Logger Logger { get; set; }
        private Action<int, string> OnProgress { get; set; }
        private bool InstallUpdateIfPossible { get; set; }
        private string Locale { get; set; }
        private uint? ElevationWindowHandle { get; set; }
        private int AllowedRetries { get; set; }
        private bool IsSystemLevelInstall { get; set; }
        private int LastReportedProgress { get; set; }
        private OmahaUpdateErrorCode ErrorCode { get; set; }
        private OmahaUpdateUpgradeStatus Status { get; set; }
        private int InstallerExitCode { get; set; }
        private string HtmlErrorMessage { get; set; }
        private string NewVersion { get; set; }
        private System.Exception Exception { get; set; }
        private Guid AppGuid { get; set; }
        private IGoogleUpdate3Web GoogleUpdate { get; set; }
        private IAppBundleWeb AppBundle { get; set; }
        private IAppWeb App { get; set; }

        public OmahaUpdateDriver(string locale, uint? elevationWindowHandle, Guid appGuid, bool installUpdateIfPossible, Action<int, string> onProgress)
        {
            AllowedRetries = OmahaConstants.AllowedRetries;
            Locale = locale;
            ElevationWindowHandle = elevationWindowHandle;
            AppGuid = appGuid;
            ErrorCode = OmahaUpdateErrorCode.OmahaUpdateNoError;
            Status = OmahaUpdateUpgradeStatus.UpgradeError;
            InstallerExitCode = -1;
            LastReportedProgress = 0;
            IsSystemLevelInstall = false;
            HtmlErrorMessage = string.Empty;
            NewVersion = string.Empty;
            Exception = null;
            InstallUpdateIfPossible = installUpdateIfPossible;
            OnProgress = onProgress;
        }

        private void HandleUpdateError(OmahaUpdateErrorCode errorCode, Exception exception, int installerExitCode, string errorString)
        {
            Status = OmahaUpdateUpgradeStatus.UpgradeError;
            ErrorCode = errorCode;
            Exception = exception;
            InstallerExitCode = installerExitCode;
            ExceptionHelper.ThrowAccordingException(Exception, errorString);
        }


        private OmahaUpdateErrorCode CanUpdateCurrentApp(string executable)
        {
            //TODO: implement
            return OmahaUpdateErrorCode.OmahaUpdateNoError;

            //AppDistribution distribution = AppDistribution.GetDistribution();
            //string userExePath = Installer.GetAppInstallPath(false, distribution),
            //    machineExePath = Installer.GetAppInstallPath(true, distribution);
            //if(!executablePath.Equals(userExePath) &&
            //    !executablePath.Equals(machineExePath))
            //    return OmahaUpdateErrorCode.CANNOT_UPGRADE_IN_THIS_DIRECTORY;
            //return OmahaUpdateErrorCode.OMAHA_UPDATE_NO_ERROR;
        }

        private void InitialiseGoogleUpdate(bool systemLevelInstalation)
        {
            try
            {
                var googleUpdateClsid = systemLevelInstalation
                    ? OmahaConstants.ClsidOmahaUpdate3WebMachineClass
                    : OmahaConstants.ClsidOmahaUpdate3WebUserClass;
                //Get COM Type from registry with clsid and cast it into an IGoogleUpdate3Web Interface
                Logger.Info("creating IGoogleUpdate3Web with clsid {" + googleUpdateClsid + "}");
                var comType = Type.GetTypeFromCLSID(googleUpdateClsid, true);
                GoogleUpdate = (IGoogleUpdate3Web)Activator.CreateInstance(comType);

                if (GoogleUpdate == null)
                    throw new Exception("an error occured while initialising the google update com object");
            }
            catch (Exception exce)
            {
                ExceptionHelper.ThrowAccordingException(exce, "could not initialise IGoogleUpdate3Web");
            }
        }

        private void PrepareUpdateCheck()
        {
            try
            {
                if (GoogleUpdate == null)
                {
                    try
                    {
                        if (!PathHelper.Get(BasePathKey.DirExe, out string executableFilePath))
                            Logger.Error("couldn not get the executeable file path");
                        Logger.Debug("searching for update for " + executableFilePath);

                        IsSystemLevelInstall = !InstallationHelper.IsPerUserInstall(executableFilePath);
                        Logger.Info("is system level install " + IsSystemLevelInstall);

                        ErrorCode = CanUpdateCurrentApp(executableFilePath);
                        Logger.Info("can update " + ErrorCode);

                        if (ErrorCode != OmahaUpdateErrorCode.OmahaUpdateNoError)
                            ExceptionHelper.ThrowAccordingException(Marshal.GetExceptionForHR(-3), "can not update current app");

                        InitialiseGoogleUpdate(IsSystemLevelInstall);
                    }
                    catch (Exception exce)
                    {
                        ErrorCode = OmahaUpdateErrorCode.OmahaUpdateOndemandClassNotFound;
                        ExceptionHelper.ThrowAccordingException(exce, "could not initialise the google update com object");
                    }

                    Logger.Info("successfully created googleupdate com object");
                }

                // The class was created, so all subsequent errors are reported as:
                ErrorCode = OmahaUpdateErrorCode.OmahaUpdateOndemandClassReportedError;

                if (AppBundle == null)
                {
                    try
                    {
                        Logger.Debug("creating appbundle");
                        AppBundle = (IAppBundleWeb) GoogleUpdate?.createAppBundleWeb();
                        try
                        {
                            Logger.Debug("setting language " + Locale);
                            if(!string.IsNullOrEmpty(Locale) && AppBundle != null)
                                AppBundle.displayLanguage = Locale;
                        }
                        catch(Exception exce) { Logger.Info(new Exception("could not set the language to the appbundle", exce)); } 

                        Logger.Debug("initialising appbundle");
                        try { AppBundle?.initialize(); }
                        catch (Exception exce) { ExceptionHelper.ThrowAccordingException(exce, "could not initialise the appbundle"); }

                        try
                        {
                            Logger.Debug("setting parentHWND " + (ElevationWindowHandle?.ToString() ?? string.Empty));
                            if (ElevationWindowHandle != null && AppBundle != null)
                                AppBundle.parentHWND = ElevationWindowHandle.Value;
                        } catch(Exception exce) { Logger.Info(new Exception("could not set the parent window handle", exce)); }

                        Logger.Info("creating the installed app {" + AppGuid + "}");
                        try { AppBundle?.createInstalledApp("{" + AppGuid + "}"); }
                        catch (Exception exce) { ExceptionHelper.ThrowAccordingException(exce, "could not create the installed app with the guid {" + AppGuid + "}"); }
                    }
                    catch (Exception ex) { ExceptionHelper.ThrowAccordingException(ex, "could not create the appbundle"); }
                }

                if (App == null)
                {
                    // TODO: support multiple apps in one bundle
                    Logger.Debug("getting the app com object");
                    try { App = (IAppWeb)AppBundle?[0]; }
                    catch(Exception exce) { ExceptionHelper.ThrowAccordingException(exce, "could not get the app from the appbundle"); }

                    Logger.Debug("checking for update");
                    try { AppBundle?.checkForUpdate(); }
                    catch(Exception exce) { ExceptionHelper.ThrowAccordingException(exce, "the appbundle could not check for updates"); }
                }
            }
            catch (Exception exce) { ExceptionHelper.ThrowAccordingException(exce, "could not prepare the update check"); }
        }

        private OmahaUpdateResult BeginUpdateCheck()
        {
            Logger = OmahaLogProvider.GetInstance(Omaha.OmahaConstants.CompanyName, OmahaConstants.AppName, Omaha.OmahaConstants.LogLevel);
            var errorCode = OmahaUpdateErrorCode.OmahaUpdateNoError;
            
            Logger.Info("the update check has been started");
            try
            {
                try
                {
                    PrepareUpdateCheck();
                    InstallerExitCode = -1;
                    HtmlErrorMessage = string.Empty;

                    try { PollGoogleUpdate(); }
                    catch (Exception ex) { ExceptionHelper.ThrowAccordingException(ex, "could not poll the google update"); }
                }
                catch (Exception exce) { ExceptionHelper.ThrowAccordingException(exce, "could not execute an update"); }
            }
            catch (UsingExternalUpdaterException exce)
            {
                // This particular transient error is worth retrying.
                if (AllowedRetries > 0)
                {
                    --AllowedRetries;
                    Thread.Sleep(OmahaConstants.RetryIntervalSeconds*1000);
                    BeginUpdateCheck();
                }
                else Exception = exce;
            }
            catch (Exception e) { Exception = e; }
            
            if (App != null) Marshal.FinalReleaseComObject(App);
            App = null;
            if (AppBundle != null) Marshal.FinalReleaseComObject(AppBundle);
            AppBundle = null;
            if (GoogleUpdate != null) Marshal.FinalReleaseComObject(GoogleUpdate);
            GoogleUpdate = null;

            Driver = null;
            GC.Collect();

            if (Exception != null)
            {
                Logger.Error(Exception);
            }

            if (Exception?.GetType() == typeof(UsingExternalUpdaterException))
            {
                HtmlErrorMessage = Resources.ResourceManager.GetString("IDS_ABOUT_BOX_EXTERNAL_UPDATE_IS_RUNNING");
            }

            var htmlErrorMessage = "<a href='http://www.google.com/search?q=" + ErrorCode + "'>" + ErrorCode + "</a>";
            if (InstallerExitCode != -1)
                htmlErrorMessage = InstallerExitCode + " " + htmlErrorMessage;
            if (IsSystemLevelInstall)
                htmlErrorMessage += Environment.NewLine + " -- system level";

            if (string.IsNullOrEmpty(Exception?.Message))
                HtmlErrorMessage = Resources.ResourceManager.GetString("IDS_ABOUT_BOX_ERROR_UPDATE_CHECK_FAILED")?
                        .Replace("%0", htmlErrorMessage) ?? string.Empty;
            else
                HtmlErrorMessage = Resources.ResourceManager.GetString("IDS_ABOUT_BOX_ERROR_UPDATE_CHECK_FAILED")?
                    .Replace("%0", Exception.Message)
                    .Replace("%1", htmlErrorMessage) ?? string.Empty;

            return new OmahaUpdateResult
            {
                ErrorCode = errorCode,
                Exception = Exception,
                InstallerExitCode = InstallerExitCode,
                Status = Status,
                HtmlErrorMessage = HtmlErrorMessage
            };
        }
        private void PollGoogleUpdate()
        {
            try
            {
                Logger.Debug("polling google update");

                bool isWorking = true;
                while (isWorking)
                {
                    isWorking = false;
                    ICurrentState state = null;
                    var stateValue = CurrentState.StateInit;
                    var errorCode = OmahaUpdateErrorCode.OmahaUpdateNoError;
                    var installerExitCode = -1;
                    var errrorString = string.Empty;
                    var upgradeStatus = OmahaUpdateUpgradeStatus.UpgradeError;
                    var newVersion = string.Empty;
                    var progress = 0;

                    GetCurrentState(ref state, ref stateValue);
                    if (IsErrorState(state, stateValue, ref errorCode, ref installerExitCode, ref errrorString))
                        HandleUpdateError(errorCode, new Exception("an error state occured", Exception), installerExitCode, errrorString);
                    else if (IsFinalState(state, stateValue, ref upgradeStatus, ref newVersion))
                    {
                        Status = upgradeStatus;
                        ErrorCode = ErrorCode <= 0 ? OmahaUpdateErrorCode.OmahaUpdateNoError : ErrorCode;
                        HtmlErrorMessage = string.Empty;
                        if (!string.IsNullOrEmpty(newVersion))
                            NewVersion = newVersion;
                        InstallerExitCode = -1;
                    }
                    else if (IsIntermediateState(state, stateValue, ref newVersion, ref progress))
                    {
                        isWorking = true;

                        var gotNewVersion = string.IsNullOrEmpty(NewVersion) && !string.IsNullOrEmpty(newVersion);
                        if (gotNewVersion) NewVersion = newVersion;
                        // Give the caller this status update if it differs from the last one given.
                        if (gotNewVersion || progress != LastReportedProgress)
                        {
                            LastReportedProgress = progress;

                            OnProgress?.Invoke(LastReportedProgress, newVersion);
                        }

                        Thread.Sleep(OmahaConstants.OmahaUpdatePollIntervalMs);
                    }
                }
            } catch(Exception exce) {ExceptionHelper.ThrowAccordingException(exce, "could not poll the update"); }
        }
        private void GetCurrentState(ref ICurrentState currentState, ref CurrentState stateValue)
        {
            try
            {
                Logger.Debug("getting current state");
                currentState = (ICurrentState)App.currentState;
                stateValue = (CurrentState)currentState.stateValue;
            }
            catch (Exception exce)
            {
                ExceptionHelper.ThrowAccordingException(exce, "could not get the current state");
            }
        }
        private bool IsErrorState(ICurrentState currentState, CurrentState stateValue, ref OmahaUpdateErrorCode errorCode, ref int installerExitCode, ref string errrorString)
        {
            if (stateValue == CurrentState.StateError)
            {
                Logger.Debug("the state value indicates an error");
                // In general, errors reported by Google Update fall under this category
                // (see special case below).
                errorCode = OmahaUpdateErrorCode.OmahaUpdateErrorUpdating;
                // In general, the exit code of Chrome's installer is unknown (see special
                // case below).
                installerExitCode = -1;
                // Report the error_code provided by Google Update if possible, or the
                // reason it wasn't possible otherwise.

                long hresultErrorCode = 0;
                try { hresultErrorCode = currentState.errorCode; }
                catch (Exception exce) { ExceptionHelper.ThrowAccordingException(exce, "could not read the error code from the current state"); }

                // Special cases:
                // - Use a custom error code if Google Update repoted that the update was
                //   disabled by a Group Policy setting.
                // - Extract the exit code of Chrome's installer if Google Update repoted
                //   that the update failed because of a failure in the installer.
                long code = 0;
                switch (hresultErrorCode)
                {
                    case OmahaConstants.GoopdateEAppUpdateDisabledByPolicy:
                        errorCode = OmahaUpdateErrorCode.OmahaUpdateDisabledByPolicy;
                        break;
                    case OmahaConstants.GoopdateEAppUpdateDisabledByPolicyManual:
                        errorCode = OmahaUpdateErrorCode.OmahaUpdateDisabledByPolicyAutoOnly;
                        break;
                    case OmahaConstants.GoopdateinstallEInstallerFailed:
                        try { code = currentState.installerResultCode; }
                        catch (Exception exce) { code = exce.HResult; }
                        break;
                }
                installerExitCode = (int)code;

                try { errrorString = currentState.completionMessage; }
                catch(Exception exce) { Logger.Info(new Exception("could not get the completion message", exce)); }

                return true;
            }

            if (stateValue == CurrentState.StateUpdateAvailable && InstallUpdateIfPossible)
            {
                // TODO: move to PollGoogleUpdate

                Logger.Info("starting the installation");
                try { AppBundle.install(); }
                catch
                {
                    Logger.Error("an exception happened while installing");
                    // Report a failure to start the install as a general error while trying
                    // to interact with Google Update.
                    errorCode = OmahaUpdateErrorCode.OmahaUpdateOndemandClassReportedError;
                    installerExitCode = -1;
                    return true;
                }
                // Return false for handling in IsIntermediateState.
            }
            return false;
        }
        private bool IsFinalState(ICurrentState currentState, CurrentState stateValue, ref OmahaUpdateUpgradeStatus upgradeStatus, ref string newVersion)
        {
            try
            {
                if (stateValue == CurrentState.StateUpdateAvailable && !InstallUpdateIfPossible)
                {
                    upgradeStatus = OmahaUpdateUpgradeStatus.UpgradeIsAvailable;
                    newVersion = currentState.availableVersion;
                    return true;
                }
                if (stateValue == CurrentState.StateInstallComplete)
                {
                    upgradeStatus = OmahaUpdateUpgradeStatus.UpgradeSuccessful;
                    return true;
                }
                if (stateValue == CurrentState.StateNoUpdate)
                {
                    upgradeStatus = OmahaUpdateUpgradeStatus.UpgradeAlreadyUpToDate;
                    return true;
                }
            }
            catch (Exception exce) { ExceptionHelper.ThrowAccordingException(exce, "could not check if the state is a final state"); }
            return false;
        }
        private bool IsIntermediateState(ICurrentState currentState, CurrentState stateValue, ref string newVersion, ref int progress)
        {
            try
            {
                // ERROR will have been handled in IsErrorState. UPDATE_AVAILABLE, and
                // NO_UPDATE will have been handled in IsFinalState if not doing an install,
                // as will STATE_INSTALL_COMPLETE when doing an install. All other states
                // following UPDATE_AVAILABLE will only happen when an install is to be done.
                progress = 0;

                switch (stateValue)
                {
                    case CurrentState.StateInit:
                    case CurrentState.StateWaitingToCheckForUpdate:
                    case CurrentState.StateCheckingForUpdate:
                        // There is no news to report yet.
                        break;
                    case CurrentState.StateUpdateAvailable:
                        newVersion = currentState.availableVersion;
                        break;
                    case CurrentState.StateWaitingToDownload:
                    case CurrentState.StateRetryingDownload:
                        break;

                    case CurrentState.StateDownloading:
                        ulong totalBytes = currentState.totalBytesToDownload;
                        // 0-50 is downloading.
                        progress =
                            int.Parse(
                                Math.Round(((double)currentState.bytesDownloaded / (double)totalBytes) * 50.0, 0).ToString(CultureInfo.InvariantCulture));
                        break;
                    case CurrentState.StateDownloadComplete:
                    case CurrentState.StateExtracting:
                    case CurrentState.StateApplyingDifferentialPatch:
                    case CurrentState.StateReadyToInstall:
                    case CurrentState.StateWaitingToInstall:
                        progress = 50;
                        break;
                    case CurrentState.StateInstalling:
                        progress = 50;
                        long installProgress = currentState.installProgress;
                        if (installProgress >= 0 && installProgress <= 100)
                        {
                            // 50-100 is installing.
                            progress = int.Parse(Math.Round(50.0 + installProgress / 2.0, 0).ToString());
                        }
                        break;
                    case CurrentState.StateInstallComplete:
                    case CurrentState.StatePaused:
                    case CurrentState.StateNoUpdate:
                    case CurrentState.StateError:
                    default:
                        //UMA_HISTOGRAM_SPARSE_SLOWLY("GoogleUpdate.UnexpectedState", state_value);
                        return false;

                }
            }
            catch(Exception exce) { ExceptionHelper.ThrowAccordingException(exce, "could not check if state is intermediate state"); }
            return true;
        }

        private static OmahaUpdateDriver Driver { get; set; }
        public static OmahaUpdateResult RunUpdateCheck(Guid appGuid, string locale, bool installUpdateIfPossible, uint? elevationWindowHandle, Action<int, string> onProgress)
        {
            if (Driver != null) return null;
            Driver = new OmahaUpdateDriver(locale, elevationWindowHandle, appGuid, installUpdateIfPossible, onProgress);
            return Driver.BeginUpdateCheck();
        }
    }
}
