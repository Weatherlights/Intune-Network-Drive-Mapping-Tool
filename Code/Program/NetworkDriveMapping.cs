using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDriveMapping
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
            foreach (string policyName in myPolicyRetrival.retrivePolicyNames())
            {
                NetworkDriveMappingPolicy policy = myPolicyRetrival.GetPolicyByName(policyName);
                DriveSettings.MapNetworkDrive(policy.driveLetter, policy.uncPath, true);
            }
        }
    }
}
