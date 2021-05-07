using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkShareMapper
{
    public class DriveSettings
    {
        private enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        }
        private enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        }
        private enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010
        }
        private enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct NETRESOURCE
        {
            public ResourceScope oResourceScope;
            public ResourceType oResourceType;
            public ResourceDisplayType oDisplayType;
            public ResourceUsage oResourceUsage;
            public string sLocalName;
            public string sRemoteName;
            public string sComments;
            public string sProvider;
        }
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2
            (ref NETRESOURCE oNetworkResource, string sPassword,
            string sUserName, int iFlags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2
            (string sLocalName, uint iFlags, int iForce);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
    [MarshalAs(UnmanagedType.LPTStr)] string localName,
    [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
    ref int length);

        public static void MapNetworkDrive(string sDriveLetter, string sNetworkPath, bool isPersistent)
        {
            //Checks if the last character is \ as this causes error on mapping a drive.
            if (sNetworkPath.Substring(sNetworkPath.Length - 1, 1) == @"\")
            {
                sNetworkPath = sNetworkPath.Substring(0, sNetworkPath.Length - 1);
            }

            NETRESOURCE oNetworkResource = new NETRESOURCE();
            oNetworkResource.oResourceType = ResourceType.RESOURCETYPE_DISK;
            oNetworkResource.sLocalName = sDriveLetter + ":";
            oNetworkResource.sRemoteName = sNetworkPath;
            if (isPersistent)
                oNetworkResource.oResourceScope = ResourceScope.RESOURCE_REMEMBERED;

            //If Drive is already mapped disconnect the current 
            //mapping before adding the new mapping
            if (IsDriveMapped(sDriveLetter))
            {
                if (GetMappedDriveLocation(sDriveLetter) != sNetworkPath )
                    DisconnectNetworkDrive(sDriveLetter, true);
            }

            WNetAddConnection2(ref oNetworkResource, null, null, 0);
        }

        public static int DisconnectNetworkDrive(string sDriveLetter, bool bForceDisconnect)
        {
            if (bForceDisconnect)
            {
                return WNetCancelConnection2(sDriveLetter + ":", 0, 1);
            }
            else
            {
                return WNetCancelConnection2(sDriveLetter + ":", 0, 0);
            }
        }

        public static string GetMappedDriveLocation(string sDriveLetter)
        {
            var sb = new StringBuilder(512);
            var size = sb.Capacity;
            var error = WNetGetConnection(sDriveLetter + ":", sb, ref size);
            if (error != 0)
                throw new Win32Exception(error, "WNetGetConnection failed");
            string networkpath = sb.ToString();
            return networkpath;
        }

        public static bool IsDriveMapped(string sDriveLetter)
        {
            string[] DriveList = Environment.GetLogicalDrives();
            for (int i = 0; i < DriveList.Length; i++)
            {
                if (sDriveLetter + ":\\" == DriveList[i].ToString())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
