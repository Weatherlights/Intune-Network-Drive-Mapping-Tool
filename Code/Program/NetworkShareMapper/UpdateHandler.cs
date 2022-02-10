using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Store;


namespace NetworkShareMapper
{
    internal class UpdateHandler
    {
        private StoreContext context = null;
        private LogWriter myLogWriter = null;
        IReadOnlyList<StorePackageUpdate> storePackageUpdates = null;

        public UpdateHandler()
        {
            myLogWriter = new LogWriter("UpdateHandler");
        }

        private void SetRestartOnUpdate()
        {

            myLogWriter.LogWrite("Will now set application restart after installation.");
            uint res = RelaunchHelper.RegisterApplicationRestart(null, RelaunchHelper.RestartFlags.RESTART_NO_HANG);
            if (res != 0)
            {
                myLogWriter.LogWrite("Restart could not be set.");
            }
            else
            {
                myLogWriter.LogWrite("Registered app for a restart.");
            }

        }

        public async Task<bool> SearchAndDownloadUpdates()
        {
            myLogWriter.LogWrite("Begin SearchAndDownloadUpdates");
            bool updateReadyToInstall = false;
            try
            {
                if (context == null)
                {
                    context = StoreContext.GetDefault();
                }

                storePackageUpdates =
                    await context.GetAppAndOptionalStorePackageUpdatesAsync();
                myLogWriter.LogWrite("Search for Updates finished. Found " + storePackageUpdates.Count + " Updates.");

                if (storePackageUpdates.Count > 0)
                {
                    if (!context.CanSilentlyDownloadStorePackageUpdates)
                    {
                        myLogWriter.LogWrite("CanSilentlyDownloadStorePackageUpdates returned false. Will abort.", 2);
                        updateReadyToInstall = false;
                    }
                    else
                    {
                        myLogWriter.LogWrite("Will now download updates.");

                        StorePackageUpdateResult downloadResult =
                               await context.TrySilentDownloadStorePackageUpdatesAsync(storePackageUpdates);

                        switch (downloadResult.OverallState)
                        {
                            case StorePackageUpdateState.Completed:
                                myLogWriter.LogWrite("Download has been finished successfully.");
                                updateReadyToInstall = true;
                                break;
                            case StorePackageUpdateState.Canceled:
                                myLogWriter.LogWrite("Update canceled: Canceled by the user.", 2);
                                updateReadyToInstall = false;
                                break;
                            case StorePackageUpdateState.ErrorLowBattery:
                                myLogWriter.LogWrite("Update canceled: Battery level to low to download update.", 2);
                                updateReadyToInstall = false;
                                break;
                            case StorePackageUpdateState.ErrorWiFiRecommended:
                                myLogWriter.LogWrite("Update canceled: The user is recommended to use a wifi to update.", 2);
                                updateReadyToInstall = false;
                                break;
                            case StorePackageUpdateState.ErrorWiFiRequired:
                                myLogWriter.LogWrite("Update canceled: A wifi connection is required to perform the update.", 2);
                                updateReadyToInstall = false;
                                break;
                            case StorePackageUpdateState.OtherError:
                                myLogWriter.LogWrite("Update canceled: Unknown error.", 3);
                                updateReadyToInstall = false;
                                break;
                            default:
                                updateReadyToInstall = false;
                                break;
                        }
                    }
                }
            } catch (Exception e)
            {
                myLogWriter.LogWrite("Update search could not be completed. Exception: " + e.ToString(), 2);
            }
            return updateReadyToInstall;
        }


        public async Task<bool> InstallUpdate()
        {
            bool updatesInstalledSuccessfully = false;
            myLogWriter.LogWrite("Begin InstallUpdate");
            try
            {
                    SetRestartOnUpdate();
                    myLogWriter.LogWrite("Update is now installing.");
                    StorePackageUpdateResult downloadResult =
                        await context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);
                    myLogWriter.LogWrite("Installation of updates completed.");
                    switch (downloadResult.OverallState)
                    {
                        case StorePackageUpdateState.Completed:
                            myLogWriter.LogWrite("Update installed successfully.", 1);
                        updatesInstalledSuccessfully = true;
                        break;
                        case StorePackageUpdateState.Canceled:
                            myLogWriter.LogWrite("Update was canceled by the user.", 2);
                        updatesInstalledSuccessfully = false;
                        break;
                        case StorePackageUpdateState.ErrorWiFiRequired:
                            myLogWriter.LogWrite("No wifi connection available.", 2);
                        updatesInstalledSuccessfully = false;
                        break;
                    case StorePackageUpdateState.ErrorWiFiRecommended:
                            myLogWriter.LogWrite("A wifi connection is recommended.", 2);
                        updatesInstalledSuccessfully = false;
                        break;
                    case StorePackageUpdateState.ErrorLowBattery:
                        updatesInstalledSuccessfully = false;
                        break;
                    case StorePackageUpdateState.OtherError:
                            myLogWriter.LogWrite("Update failed because of unknown error.", 3);
                        updatesInstalledSuccessfully = false;
                        break;
                    default:
                            myLogWriter.LogWrite("Update installed successfully.", 1);
                        updatesInstalledSuccessfully = false;
                        break;

                }
                
            } catch (Exception e)
            {
                myLogWriter.LogWrite("Update search could not be completed. Exception: " + e.ToString(), 2);
            }
            return updatesInstalledSuccessfully;

        }
    }
}
