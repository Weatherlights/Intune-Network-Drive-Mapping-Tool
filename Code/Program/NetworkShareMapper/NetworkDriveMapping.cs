using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkShareMapper
{
    class NetworkDriveMapping
    {
        private PolicyRetrival myPolicyRetrival = null;
        private UpdateHandler myUpdateHandler = null;
        private LogWriter myLogWriter = null;
        private bool isStartedFromStartmenu = false;

        public NetworkDriveMapping(PolicyRetrival policyRetrival)
        {
            myPolicyRetrival = policyRetrival;
            myUpdateHandler = new UpdateHandler();
            myLogWriter = new LogWriter("NetworkDriveMapping");
        }

        private void MapDrives()
        {
            if (myPolicyRetrival.isEnabled())
                try
                {
                    bool isMapPersistent = myPolicyRetrival.isMapPersistent();
                    List<NetworkDriveMappingPolicy> policies = new List<NetworkDriveMappingPolicy>();
                    if (myPolicyRetrival.retrivePolicyNames2() != null) // This is the new configuration which is more flexible. If it is used it is prefered over the old configuration.
                    {
                        foreach (string policyName in myPolicyRetrival.retrivePolicyNames2())
                        {
                            policies.Add(myPolicyRetrival.GetPolicyByName2(policyName));
                        }
                    }
                    else if (myPolicyRetrival.retrivePolicyNames() != null)
                    {
                        foreach (string policyName in myPolicyRetrival.retrivePolicyNames())
                        {
                            policies.Add(myPolicyRetrival.GetPolicyByName(policyName));
                        }
                    }
                    foreach (NetworkDriveMappingPolicy policy in policies)
                    {
                        if (policy.driveLetter != null && policy.uncPath != null)
                        try
                        {
                            DriveSettings.MapNetworkDrive(policy.driveLetter, policy.uncPath, isMapPersistent, policy.Username, policy.Password);
                            //myLogWriter.LogWrite("Mapped networkdrive " + policy.driveLetter + " to " + policy.uncPath);
                        }
                        catch (Exception e)
                        {
                            myLogWriter.LogWrite("Failed to map networkdrive " + policy.driveLetter + " to " + policy.uncPath + "\nException: " + e.ToString(), 2);
                            // do nothing
                        }
                    }
                }
                catch (Exception e)
                {
                    myLogWriter.LogWrite("An unknown error occured.\nException: " + e.ToString(), 3);
                }
        }

        public void Execute()
        {
            DateTime dateLastUpdate = DateTime.Now;
            NetworkChangeDetector myNetworkChangeDetector = new NetworkChangeDetector();
            myLogWriter.LogWrite("Initialized NetworkChangeDetector");

            Task<bool> searchUpdateTask = null;
            Task<bool> installUpdateTask = null;

            myLogWriter.LogWrite("Initialized UpdateTask");
            int retryCount = 0;
            bool shouldRun = true;
            int updateInterval = myPolicyRetrival.getUpdateInterval();
            while (shouldRun)
            {
                int sleepTime = myPolicyRetrival.getRefreshInterval();
                
                if (myPolicyRetrival.isNetworkTestEnabled())
                {
                    if (myNetworkChangeDetector.CheckNetworkChange())
                    {
                        myLogWriter.LogWrite("CheckNetworkChange() indicated a network change.");
                        retryCount = myPolicyRetrival.getRetryCount();
                    }
                }
                else
                {
                    retryCount = 1;
                }

                if (retryCount > 0)
                {
                    MapDrives();
                    Thread.Sleep(sleepTime);
                }
                else
                {
                    if (retryCount == 0)
                        myLogWriter.LogWrite("Will now go to sleep.");

                    Thread.Sleep(sleepTime);
                    bool updatesReadyToInstall = false;
                    if (searchUpdateTask == null)
                        searchUpdateTask = myUpdateHandler.SearchAndDownloadUpdates();
                    if (searchUpdateTask.IsCompleted)
                        updatesReadyToInstall = searchUpdateTask.Result;

                    if (updatesReadyToInstall)
                    {
                        if (installUpdateTask == null)
                            installUpdateTask = myUpdateHandler.InstallUpdate();
                        if (installUpdateTask.IsCompleted)
                            if (installUpdateTask.Result)
                            {
                                shouldRun = false;
                                myLogWriter.LogWrite("Will now restart Intune Network Drive Mapping to install updates.", 1);
                            }
                    }
                    DateTime currentDate = DateTime.Now;
                    long elapsedTicks = currentDate.Ticks - dateLastUpdate.Ticks;
                    TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

                    if ((elapsedSpan.TotalSeconds >= updateInterval) && (updateInterval > 60))
                    {
                        dateLastUpdate = DateTime.Now;
                        installUpdateTask = null;
                        searchUpdateTask = null;
                    }

                }
                retryCount--;

            }
        }
    }
}
