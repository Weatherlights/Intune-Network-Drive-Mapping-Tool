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

        public async void InstallUpdate()
        {
            myLogWriter.LogWrite("Begin InstallUpdate");
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }
            
            IReadOnlyList<StorePackageUpdate> storePackageUpdates =
               await context.GetAppAndOptionalStorePackageUpdatesAsync();
            myLogWriter.LogWrite("Search for Updates finished. Found " + storePackageUpdates.Count + " Updates.");
            // Start the silent installation of the packages. Because the packages have already
            // been downloaded in the previous method, the following line of code just installs
            // the downloaded packages.
            if (storePackageUpdates.Count > 0)
            {
                if (!context.CanSilentlyDownloadStorePackageUpdates)
                {
                    myLogWriter.LogWrite("CanSilentlyDownloadStorePackageUpdates returned false. Will abort.",2);
                    return;
                }
                SetRestartOnUpdate();
                myLogWriter.LogWrite("Update is now installing.");
                StorePackageUpdateResult downloadResult =
                    await context.TrySilentDownloadAndInstallStorePackageUpdatesAsync(storePackageUpdates);
                myLogWriter.LogWrite("Installation of updates completed.");
                switch (downloadResult.OverallState)
                {
                    // If the user cancelled the installation or you can't perform the installation  
                    // for some other reason, try again later. The RetryInstallLater method is not  
                    // implemented in this example, you should implement it as needed for your own app.
                    case StorePackageUpdateState.Canceled:
                        myLogWriter.LogWrite("Update was canceled by the user.",2);
                        return;
                    case StorePackageUpdateState.ErrorWiFiRequired:
                        myLogWriter.LogWrite("No wifi connection available.", 2);
                        return;
                    case StorePackageUpdateState.ErrorWiFiRecommended:
                        myLogWriter.LogWrite("A wifi connection is recommended.", 2);
                        return;
                    case StorePackageUpdateState.ErrorLowBattery:
                        myLogWriter.LogWrite("Update failed because of low battery.", 2);
                        return;
                    case StorePackageUpdateState.OtherError:
                        myLogWriter.LogWrite("Update failed because of unknown error.", 3);
                        return;
                    default:
                        myLogWriter.LogWrite("Update installed successfully.", 1);
                        break;
                }
            }
        }
    }
}
