using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkDriveMapping
{

    
    class PolicyRetrival
    {
        private string policyLocation = "Software\\Policies\\weatherlights.com\\NetworkDriveMapping";
        private RegistryKey policyStoreKey = null;

        public PolicyRetrival()
        {
            policyStoreKey = Registry.CurrentUser.OpenSubKey(policyLocation, false);
        }

        public string[] retrivePolicyNames()
        {
            return policyStoreKey.GetSubKeyNames();
        }

        public NetworkDriveMappingPolicy GetPolicyByName(string name)
        {
            RegistryKey myPolicyKey = Registry.CurrentUser.OpenSubKey(policyLocation + "\\" + name);
            NetworkDriveMappingPolicy policy = new NetworkDriveMappingPolicy();
            policy.driveLetter = (string) myPolicyKey.GetValue("DriveLetter");
            policy.uncPath = (string) myPolicyKey.GetValue("uncPath");
            return policy;
        }

    }
}
