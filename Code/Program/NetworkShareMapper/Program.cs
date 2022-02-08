using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkShareMapper
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            PolicyRetrival myPolicyRetrival = new PolicyRetrival();
            if (!myPolicyRetrival.isEnabled() && args.Contains("Startmenu"))
            {
                UnconfiguredNoticeWindow myUnconfiguredNoticeWindow = new UnconfiguredNoticeWindow();
                myUnconfiguredNoticeWindow.ShowDialog();
            }

            NetworkDriveMapping myNetworkDriveMapping = new NetworkDriveMapping(myPolicyRetrival);
            myNetworkDriveMapping.Execute();
            
        }
    }
}
