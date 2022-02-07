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

        public NetworkDriveMapping()
        {
            myPolicyRetrival = new PolicyRetrival();
            myUpdateHandler = new UpdateHandler();
            myLogWriter = new LogWriter("NetworkDriveMapping");
        }

        public void Execute()
        {
            NetworkChangeDetector myNetworkChangeDetector = new NetworkChangeDetector();
            myLogWriter.LogWrite("Initialized NetworkChangeDetector");
            Task updateTask = new Task(myUpdateHandler.InstallUpdate);
            myLogWriter.LogWrite("Initialized UpdateTask");
            int retryCount = 0;
            while (true)
            {
                int sleepTime = myPolicyRetrival.getRefreshInterval();;
                bool isMapPersistent = myPolicyRetrival.isMapPersistent();
                if (myPolicyRetrival.isNetworkTestEnabled())
                {
                    if (myNetworkChangeDetector.CheckNetworkChange())
                    {
                        myLogWriter.LogWrite("CheckNetworkChange() indicated a network change.");
                        retryCount = myPolicyRetrival.getRetryCount();
                    }
                } else
                {
                    retryCount = 1;
                }
                
                if (retryCount > 0)
                {
                    //myLogWriter.LogWrite("retryCount is " + retryCount);
                    if (myPolicyRetrival.isEnabled())
                        try
                        {   
                            foreach (string policyName in myPolicyRetrival.retrivePolicyNames())
                            {  
                                NetworkDriveMappingPolicy policy = myPolicyRetrival.GetPolicyByName(policyName);
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
                    retryCount--;
                } else
                {

                    if (updateTask.Status == TaskStatus.Created)
                    {
                        updateTask.Start();
                    }
                   
                }
                Thread.Sleep(sleepTime);
            }
        }
    }
}
