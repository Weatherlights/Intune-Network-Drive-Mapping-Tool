using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NetworkDriveMapping
{
    public partial class NetworkDriveMappingService : ServiceBase
    {
        private NetworkDriveMapping myNetworkDriveMapping = null;
        public NetworkDriveMappingService()
        {
            InitializeComponent();
        }

        private void OnTimer(object sender, ElapsedEventArgs args)
        {
            myNetworkDriveMapping.Execute();
        }

        protected override void OnStart(string[] args)
        {
            myNetworkDriveMapping = new NetworkDriveMapping();
            Timer timer = new Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
        }
    }
}
