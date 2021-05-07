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

        public NetworkDriveMapping()
        {
            myPolicyRetrival = new PolicyRetrival();
        }

        public void Execute()
        {
            NetworkChangeDetector myNetworkChangeDetector = new NetworkChangeDetector();
            int retryCount = 0;
            while (true)
            {
                int sleepTime = myPolicyRetrival.getRefreshInterval();
                bool isMapPersistent = myPolicyRetrival.isMapPersistent();
                if (myPolicyRetrival.isNetworkTestEnabled())
                {
                    if (myNetworkChangeDetector.CheckNetworkChange())
                        retryCount = myPolicyRetrival.getRetryCount();
                } else
                {
                    retryCount = 1;
                }


                if (retryCount > 0)
                {
                    if (myPolicyRetrival.isEnabled())
                        foreach (string policyName in myPolicyRetrival.retrivePolicyNames())
                        {
                            NetworkDriveMappingPolicy policy = myPolicyRetrival.GetPolicyByName(policyName);
                            if (policy.driveLetter != null && policy.uncPath != null)
                                try
                                {
                                    DriveSettings.MapNetworkDrive(policy.driveLetter, policy.uncPath, isMapPersistent);
                                }
                                catch (Exception e)
                                {
                                    // do nothing
                                }
                        }
                    retryCount--;
                }
                Thread.Sleep(sleepTime);
            }
        }
    }
}
