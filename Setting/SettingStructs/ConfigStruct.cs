using System.Text.Json.Serialization;


namespace InputConnect.SettingStruct
{
    public class ConfigStruct
    {
        // this file is not named config because i dont want it to class with the
        // public static class config i can do it but it  will just take changing
        // a word for more than 132 lines of code and  other people  may find not 
        // relise there is two different config classes, they  both  should share
        // the exact same varables other wise the  config part  will not function
        // as intended, notice how the values of this  struct if the  same as the
        // Config file when created

        // No IpAdressNeeded as it will be  dinamically graped
        // Grap the ip using the Device.cs file with the class


        [JsonPropertyName("DeviceName")]
        public string? DeviceName { get; set; } = SharedData.Device.DeviceName;  // this is a Device name  that you set your  self from
                                                                         // setting if there is no Name then we take the device
                                                                         // name
        [JsonPropertyName("ApplicationName")]
        public string ApplicationName { get; set; } = "InputConnect"; // this is just the name of the application
                                                                             // nothing more nothing less
        [JsonPropertyName("BroadCastAddress")]
        public string BroadCastAddress { get; set; } = "255.255.255.255"; // incase you are not aware this is a boardcast address
                                                                                 // that you can only send a  message to it once and all
                                                                                 // the devices will use recieve the message this doesnt
                                                                                 // need to change underneath  any  circumstances  other
                                                                                 // than if the address was changed to something else by
                                                                                 // universally
        [JsonPropertyName("Port")]
        public int Port { get; set; } = 39393;   // the port number should be the same for all devices
                                                        // other wise the  application  wont communicate with
                                                        // other devices on the network
        [JsonPropertyName("Tick")]
        public int Tick { get; set; } = 8;     // this is used for animations  each tick will occure
                                                      // every 8ms apart this  is  also  is  being used for
                                                      // dispatch timers for varying tasks
        [JsonPropertyName("AdvertisementInterval")]
        public int AdvertisementInterval { get; set; } = 3000; // this is used to tell  the function that advertise the
                                                                      // device to advertise it  every <AdvertisementInterval>
                                                                      // the number is in milliseconds
        [JsonPropertyName("AdvertiseTimeSpan")]
        public int AdvertiseTimeSpan { get; set; } = 10;       // this  is  used  to tell the app that all  the  devices
                                                                      // that  had  an  advertisement that is  10s old  is  not
                                                                      // relievent and should be removed the time is in seconds
        [JsonPropertyName("TransitionDuration")]
        public int TransitionDuration { get; set; } = 200;     // this is  used  for  Transitioning for  one  holder to
                                                                      // the value is in ms
        [JsonPropertyName("CornerRadius")]
        public int CornerRadius { get; set; } = 10;     // this value is used for the Holder corner Radius
                                                               // this is a generic var that anycontainer can use
        [JsonPropertyName("TransitionHover")]
        public int TransitionHover { get; set; } = 100; // this is used for a hover  effect  duration,
                                                               // the time is in ms
        [JsonPropertyName("FontSize")]
        public int FontSize { get; set; } = 20;  // this is for all the text font size of the application

        [JsonPropertyName("PopupTimeout")]
        public int PopupTimeout { get; set; } = 10000; // this will be used for popus that use a timeout



        [JsonPropertyName("TransitionReferenceDistance")]
        public int TransitionReferenceDistance { get; set; } = 1000; // this will be used for animation that
                                                                     // vary in distance
        [JsonPropertyName("TimeOutDuration")]
        public int TimeOutDuration { get; set; } = 3000; // this will control how long it takes to wait for another
                                                         // device message before timming out and returning nothing

        // Mouse 
        [JsonPropertyName("MouseScrollStrength")]
        public int MouseScrollStrength { get; set; } = 120;

        [JsonPropertyName("MouseUpdateTickRate")]
        public int MouseUpdateTickRate { get; set; } = 8; // this is the tick rate to update the mouse on the other deivce
                                                          // this value is only valued when you are  in the absorber mouse
                                                          // tracker


        // audio
        [JsonPropertyName("AudioFrequency")]
        public ushort AudioFrequency { get; set; } = 48000;

        [JsonPropertyName("AudioBufferSize")]
        public ushort AudioBufferSize { get; set; } = 10240;

        [JsonPropertyName("AudioChannals")]
        public byte AudioChannals { get; set; } = 2;

    }


}
