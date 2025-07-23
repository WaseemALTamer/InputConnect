


using System.Text.Json.Serialization;

namespace InputConnect.Commands
{
    public class Mouse
    {
        // this is used to tell the other device where on screen they should be

        [JsonPropertyName("X")]
        public double? X { get; set; } // this will be a precentage from 1-100

        [JsonPropertyName("Y")]
        public double? Y { get; set; } // this will be a precentage from 1-100

        [JsonPropertyName("ScrollDelta")]
        public double? ScrollDelta { get; set; }

        [JsonPropertyName("MouseHide")]
        public bool? MouseHide { get; set; } // this is used to tell the other deivces if there mouse should be hidden

        [JsonPropertyName("MousePress")]
        public int? MouseButtonPress { get; set; }


        [JsonPropertyName("MouseRelease")]
        public int? MouseButtonRelease { get; set; }

    }
}