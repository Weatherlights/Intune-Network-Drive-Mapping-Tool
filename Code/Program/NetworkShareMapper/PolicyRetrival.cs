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


        public PolicyRetrival()
        {
            policyStoreKey = Registry.CurrentUser.OpenSubKey(policyLocation, false);
        }

        public List<NetworkDriveMappingPolicy> Policies
        {
            get
            {
                List<NetworkDriveMappingPolicy> policies = new List<NetworkDriveMappingPolicy>();
                if (this.retrivePolicyNames2() != null) // This is the new configuration which is more flexible. If it is used it is prefered over the old configuration.
                {
                    foreach (string policyName in this.retrivePolicyNames2())
                    {
                        policies.Add(this.GetPolicyByName2(policyName));
                    }
                }
                else if (this.retrivePolicyNames1() != null)
                {
                    foreach (string policyName in this.retrivePolicyNames1())
                    {
                        policies.Add(this.GetPolicyByName1(policyName));
                    }
                }
                return policies;
            }
        }

        private string[] retrivePolicyNames1()
        {
            RegistryKey myPolicyKey = Registry.CurrentUser.OpenSubKey(policyLocation + "\\Policies");
            if (myPolicyKey != null)
            {
                return myPolicyKey.GetValueNames();
            } else
            {
                return null;
            }
        }

        private string[] retrivePolicyNames2()
        {
            RegistryKey myPolicyKey = Registry.CurrentUser.OpenSubKey(policyLocation + "\\Policies2");
            if (myPolicyKey != null)
            {
                return myPolicyKey.GetSubKeyNames();
            }
            else
            {
                return null;
            }
        }

        private NetworkDriveMappingPolicy GetPolicyByName1(string name)
        {
            NetworkDriveMappingPolicy policy = null;
            using (RegistryKey policyPolicyKey = Registry.CurrentUser.OpenSubKey(policyLocation + "\\Policies"))
            {
                policy = new NetworkDriveMappingPolicy();
                if (policyPolicyKey.GetValueNames().Contains(name))
                {
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
                }
            }
            return policy;
        }

        private NetworkDriveMappingPolicy GetPolicyByName2(string name)
        {
            NetworkDriveMappingPolicy policy = null;
            using (RegistryKey policyPolicyKey = Registry.CurrentUser.OpenSubKey(policyLocation + "\\Policies2\\" + name))
            {
                policy = new NetworkDriveMappingPolicy();

                policy.driveLetter = name;
                policy.uncPath = (string)policyPolicyKey.GetValue("Path");
                if ( policyPolicyKey.GetValueNames().Contains("Username") && policyPolicyKey.GetValueNames().Contains("Password"))
                {
                    policy.Username = (string)policyPolicyKey.GetValue("Username");
                    policy.Password = (string)policyPolicyKey.GetValue("Password");
                }
            }
            return policy;
        }

        private bool TestRegistryKeyValue(string AttributeName)
        {
            if (policyStoreKey != null)
                if (policyStoreKey.GetValueNames().Contains(AttributeName))
                    return true;
            return false;
        }


        public bool isEnabled()
        {
            
            try
            {
                if (TestRegistryKeyValue("Enabled")) 
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
                if (TestRegistryKeyValue("MapPersistent"))
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
                if (TestRegistryKeyValue("RefreshInterval"))
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
                if (TestRegistryKeyValue("RetryCount"))
                    return (int)policyStoreKey.GetValue("RetryCount");
                else
                    return 15;
            }
            catch (Exception e)
            {
                return 15;
            }
        }

        public int getUpdateInterval()
        {
            try
            {
                if (TestRegistryKeyValue("UpdateInterval"))
                    return (int)policyStoreKey.GetValue("UpdateInterval");
                else
                    return 10800;
            }
            catch (Exception e)
            {
                return 10800;
            }
        }

        public bool isNetworkTestEnabled()
        {
            try
            {
                if (TestRegistryKeyValue("NetworkTestEnabled"))
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
