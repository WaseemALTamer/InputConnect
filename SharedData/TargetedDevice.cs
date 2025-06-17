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



        public static string? IP;
        public static string? MacAddress;
        public static string? DeviceName;
        public static string? State;



        public static Connection? Connection; // this will hold our current connection



        public static string? Token;

        public static void Clear() {
            IP = null; 
            MacAddress = null; 
            DeviceName = null; 
            State = null; 
            Connection = null;
            Token = null;
        }

    }
}
