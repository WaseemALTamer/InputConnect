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

        // Application
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
        
        // Network
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


        // UI

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
        public int MouseScrollStrength { get; set; } = 120; // this is the defult mouse scroll stength it is 120 on
                                                            // windows and some linux distros

        [JsonPropertyName("VirtualScreenCriticalRegionSize")]
        public int VirtualScreenCriticalRegionSize { get; set; } = 2; // this is the creatical region for each virtual mointor
                                                                     // which  extend  beyond its  size  and overlabs  with a
                                                                     // physcal mointor which then the mouse can shift to the
                                                                     // virtual mointor


        [JsonPropertyName("MouseUpdateTickRate")]
        public int MouseUpdateTickRate { get; set; } = 8; // this is the tick rate to update the mouse on the other deivce
                                                          // this value is only valued when you are  in the absorber mouse
                                                          // tracker


        // Audio

        [JsonPropertyName("OutputAudioDevice")]
        public string OutputAudioDevice { get; set; } = "";

        [JsonPropertyName("AudioFrequency")]
        public ushort AudioFrequency { get; set; } = 48000; // this is the univsral tageted audio frequency

        [JsonPropertyName("AudioBufferSize")]
        public ushort AudioBufferSize { get; set; } = 10240; // this is the buffer size that is sent over the network
                                                             // the buffer will have as many channels as you need but
                                                             // keep in mind if your device  can generate  the buffer
                                                             // size then a smaller one will be taken if a larger one
                                                             // is generated then it will  be sliced  to achieve this
                                                             // buffer size

        [JsonPropertyName("AudioChannals")]
        public byte AudioChannals { get; set; } = 2; // this is the Audio channels that we aim to get 1 left 1 right
                                                     // desktops audi loopback devices usually  dont use more than 2
                                                     // and if you sure you have more then you are  free to increase
                                                     // the number but it may cause errors and the application might
                                                     // not function as recommended

    }


}
