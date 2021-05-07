using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkShareMapper
{
    class NetworkChangeDetector
    {
        IPAddress[] latestAdresses = null;

        public NetworkChangeDetector()
        {

        }

        public bool CheckNetworkChange()
        {
            bool result = true;

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            if ( latestAdresses != null )
            if (this.CompareAdressList(latestAdresses, host.AddressList))
               result = false;
            latestAdresses = host.AddressList;

            return result;
        }

        private bool CompareAdressList(IPAddress[] ListA, IPAddress[] ListB)
        {

            if (ListA.Length != ListB.Length)
                return false;

            foreach (IPAddress entry in ListA)
                if (!ListB.Contains(entry))
                    return false;
            return true;
        }
    }
}
