using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System;


namespace InputConnect.Network
{

    public static class Device{


        public static string? IP = GetMyIP();
        public static string? Subnet = GetPublicSubnet();
        public static string? MacAdress = GetMyMacAdress();
        public static string? DeviceName = Environment.UserName;



        public static string? GetMyIP()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()){
                if (ni.OperationalStatus != OperationalStatus.Up ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;

                var ipProps = ni.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses){
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(addr.Address))
                    {
                        return addr.Address.ToString();
                    }
                }
            }

            return null;
        }

        public static string? GetPublicSubnet(){
            if (IP == null) return null;
            string[] parts = IP.Split('.');
            if (parts.Length != 4)
                return null;
            return $"{parts[0]}.{parts[1]}.{parts[2]}";
        }

        public static string? GetMyMacAdress() {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet or Wireless interfaces that are up
                if (nic.OperationalStatus == OperationalStatus.Up &&
                    (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    PhysicalAddress address = nic.GetPhysicalAddress();
                    byte[] bytes = address.GetAddressBytes();
                    string Mac = string.Join(":", Array.ConvertAll(bytes, b => b.ToString("X2")));
                    return string.Join(":", Array.ConvertAll(bytes, b => b.ToString("X2")));
                }
            }
            return null;
        }

        public static void UpdateDeviceData() {
            IP = GetMyIP();
            Subnet = GetPublicSubnet();
            MacAdress = GetMyMacAdress();
            DeviceName = Environment.UserName;
        }
    }
}
