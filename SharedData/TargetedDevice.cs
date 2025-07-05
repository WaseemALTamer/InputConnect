using Avalonia.Controls;
using InputConnect.Structures;




namespace InputConnect.SharedData
{
    public static class TargetedDevice {
        // this file will contain all the data  that  will be shared from
        // one Class, the  varables are made to be set and used somewhere
        // else this is not the file  that is used to connect to  another
        // devices, this file will be used to target an Advertised Device
        // nothing more and mothing less at least not anymore



        // ip address approch is not used rather use the  dictionary that
        // contain the mac to ip address in MessageManager
        
        // public static string? IP; 

        public static string? MacAddress;
        public static string? DeviceName;



        public static Connection? Connection; // this will hold our current connection



        public static string? Token;

        public static void Clear() {
            MacAddress = null; 
            DeviceName = null;
            Connection = null;
            Token = null;
        }

    }
}
