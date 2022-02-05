using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkShareMapper
{


    class PolicyRetrival
    {
        private string policyLocation = "Software\\Policies\\weatherlights.com\\NetworkDriveMapping";
        private RegistryKey policyStoreKey = null;
        private RegistryKey policyPolicyKey = null;

        public PolicyRetrival()
        {
            policyStoreKey = Registry.CurrentUser.OpenSubKey(policyLocation, false);
        }

        public string[] retrivePolicyNames()
        {
            RegistryKey myPolicyKey = Registry.CurrentUser.OpenSubKey(policyLocation + "\\Policies");
            return myPolicyKey.GetValueNames();
        }

        public NetworkDriveMappingPolicy GetPolicyByName(string name)
        {
            if ( policyPolicyKey == null )
                policyPolicyKey = Registry.CurrentUser.OpenSubKey(policyLocation + "\\Policies");
            NetworkDriveMappingPolicy policy = new NetworkDriveMappingPolicy();
            string myPolicyValue = (string)policyPolicyKey.GetValue(name);
            string[] myPolicyValueArray = myPolicyValue.Split(';');
            if (myPolicyValueArray.Length > 1)
            {
                policy.driveLetter = myPolicyValueArray[0];
                string myPathWithVariables = myPolicyValueArray[1];
                policy.uncPath = Environment.ExpandEnvironmentVariables(myPathWithVariables);
                if (myPolicyValueArray.Length > 3)
                {
                    policy.Username = myPolicyValueArray[2];
                    policy.Password = myPolicyValueArray[3];
                }
            }
            return policy;
            
        }

        public bool isEnabled()
        {
            try
            {
                if (policyStoreKey.GetValueNames().Contains("Enabled"))
                {
                    int value = (int)policyStoreKey.GetValue("Enabled");
                    if (value > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            } catch (Exception e)
            {
                return false;
            }
        }

        public bool isMapPersistent()
        {
            try
            {
                if (policyStoreKey.GetValueNames().Contains("MapPersistent"))
                {
                    int value = (int)policyStoreKey.GetValue("MapPersistent");
                    if (value > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public int getRefreshInterval()
        {
            try
            {
                if (policyStoreKey.GetValueNames().Contains("RefreshInterval"))
                    return (int)policyStoreKey.GetValue("RefreshInterval");
                else
                    return 10000;
            }
            catch (Exception e)
            {
                return 10000;
            }
        }

        public int getRetryCount()
        {
            try
            {
                if (policyStoreKey.GetValueNames().Contains("RetryCount"))
                    return (int)policyStoreKey.GetValue("RetryCount");
                else
                    return 15;
            }
            catch (Exception e)
            {
                return 15;
            }
        }

        public bool isNetworkTestEnabled()
        {
            try
            {
                if (policyStoreKey.GetValueNames().Contains("NetworkTestEnabled"))
                {
                    int value = (int)policyStoreKey.GetValue("NetworkTestEnabled");
                    if (value > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            catch (Exception e)
            {
                return true;
            }
        }

    }
}
