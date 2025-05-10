using System.Text.Json.Serialization;


namespace InputConnect.Structures
{
    public class ConfigStruct {
        // this file is not named config because i dont want it to class with the
        // public static class config i can do it but it  will just take changing
        // a word for more than 132 lines of code and  other people  may find not 
        // relise there is two different config classes, they  both  should share
        // the exact same varables other wise the  config part  will not function
        // as intended, notice how the values of this  struct if the  same as the
        // Config file when created

        [JsonPropertyName("DeviceName")]
        public string? DeviceName { get; set; } = Network.Device.DeviceName;

        [JsonPropertyName("ApplicationName")]
        public string ApplicationName { get; set; } = "InputConnect";

        [JsonPropertyName("BroadCastAddress")]
        public string BroadCastAddress { get; set; } = "255.255.255.255";

        [JsonPropertyName("Port")]
        public int Port { get; set; } = 39393;

        [JsonPropertyName("Tick")]
        public int Tick { get; set; } = 8;

        [JsonPropertyName("AdvertisementInterval")]
        public int AdvertisementInterval { get; set; } = 3000;

        [JsonPropertyName("AdvertiseTimeSpan")]
        public int AdvertiseTimeSpan { get; set; } = 10;

        [JsonPropertyName("TransitionDuration")]
        public int TransitionDuration { get; set; } = 200;

        [JsonPropertyName("CornerRadius")]
        public int CornerRadius { get; set; } = 10;

        [JsonPropertyName("TransitionHover")]
        public int TransitionHover { get; set; } = 100;

        [JsonPropertyName("FontSize")]
        public int FontSize { get; set; } = 20;

    }


}
