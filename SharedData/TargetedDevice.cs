


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
        public static int? Port; // the port is still provided just in case
        public static string? MACAddress;
        public static string? DeviceName;




        public static string? Token;    // it  is  used to  encrypt the data
                                        // this is  something that the  user
                                        // types on  the devices  tht  he is
                                        // trying to connect  to assume that
                                        // you try to connect to 3 different
                                        // devices then all of  them  should
                                        // have the same token 



    }
}
