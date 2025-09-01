




namespace InputConnect.Setting
{
    static public class Config{

        // No IpAdressNeeded as it will be  dinamically graped
        // Grap the ip using the Device.cs file with the class

        public static string? DeviceName = SharedData.Device.DeviceName; // this is a Device name  that you set your  self from
                                                                         // setting if there is no Name then we take the device
                                                                         // name

        public static string ApplicationName = "InputConnect"; // this is just the name of the application
                                                               // nothing more nothing less

        public static string BroadCastAddress = "255.255.255.255"; // incase you are not aware this is a boardcast address
                                                                   // that you can only send a  message to it once and all
                                                                   // the devices will use recieve the message this doesnt
                                                                   // need to change underneath  any  circumstances  other
                                                                   // than if the address was changed to something else by
                                                                   // universally

        public static int Port = 39393; // the port number should be the same for all devices
                                        // other wise the  application  wont communicate with
                                        // other devices on the network

        public static int Tick = 8;     // this is used for animations  each tick will occure
                                        // every 8ms apart this  is  also  is  being used for
                                        // dispatch timers for varying tasks

        public static int AdvertisementInterval = 3000; // this is used to tell  the function that advertise the
                                                        // device to advertise it  every <AdvertisementInterval>
                                                        // the number is in milliseconds
        
        public static int AdvertiseTimeSpan = 10;       // this  is  used  to tell the app that all  the  devices
                                                        // that  had  an  advertisement that is  10s old  is  not
                                                        // relievent and should be removed the time is in seconds

        public static int TransitionDuration = 200;     // this is  used  for  Transitioning for  one  holder to
                                                        // the value is in ms

        public static int CornerRadius = 10;     // this value is used for the Holder corner Radius
                                                 // this is a generic var that anycontainer can use

        public static int TransitionHover = 100; // this is used for a hover  effect  duration,
                                                 // the time is in ms

        public static int FontSize = 20;  // this is for all the text font size of the application


        public static int PopupTimeout = 10000; // this will be used for popus that use a timeout


        public static int TransitionReferenceDistance = 1000; // this will be used for animation that
                                                                // vary in distance

        public static int TimeOutDuration = 3000; // this will control how long it takes to wait for another
                                                  // device message before timming out and returning nothing

        // Mouse 

        public static int MouseScrollStrength = 120;


        public static int MouseUpdateTickRate = 8; // this is the tick rate to update the mouse on the other deivce
                                                   // this value is only valued when you are  in the absorber mouse
                                                   // tracker


        // audio

        public static ushort AudioFrequency = 48000;


        public static ushort AudioBufferSize = 10240;


        public static byte AudioChannals = 2;
        

    }
}
