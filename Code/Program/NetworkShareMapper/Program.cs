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
            LogWriter myLogWriter = new LogWriter("Program");
            myLogWriter.LogWrite("Intune Network Drive Mapping has started.");
            PolicyRetrival myPolicyRetrival = new PolicyRetrival();
            if (!myPolicyRetrival.isEnabled() && args.Contains("Startmenu"))
            {
                myLogWriter.LogWrite("Has been started from start menu but configuration is missing. Will display user hint.",2);
                UnconfiguredNoticeWindow myUnconfiguredNoticeWindow = new UnconfiguredNoticeWindow();
                myUnconfiguredNoticeWindow.ShowDialog();
            }

            NetworkDriveMapping myNetworkDriveMapping = new NetworkDriveMapping(myPolicyRetrival);
            myNetworkDriveMapping.Execute();
            myLogWriter.LogWrite("Exiting Intune Network Drive Mapping", 2);
        }
    }
}
