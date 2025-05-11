


namespace InputConnect.SharedData
{
    public static class TargetedDevice {
        // this file will contain all the data  that  will be shared from
        // one Class, the  varables are made to be set and used somewhere
        // else this is not the file  that is used to connect to  another
        // devices, this file will be used to target an Advertised Device
        // nothing more and mothing less at least not anymore


        // the next 3 var are data on the device
        public static string? IP;
        public static int? Port;
        public static string? MacAddress;
        public static string? DeviceName;




        public static string? Token;

    }
}
