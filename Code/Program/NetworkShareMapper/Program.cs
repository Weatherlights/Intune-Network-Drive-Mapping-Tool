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
        static void Main(string[] args)
        {
            NetworkDriveMapping myNetworkDriveMapping = new NetworkDriveMapping();

                myNetworkDriveMapping.Execute();
            
        }
    }
}
