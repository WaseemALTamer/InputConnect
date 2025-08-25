using SharpHook.Data;
using System.Text.Json.Serialization;





namespace InputConnect.Commands
{
    public class Keyboard{

        // the key matrix approch have been change to simple just sending the key events from before



        [JsonPropertyName("KeyPress")]
        public KeyCode? KeyPress { get; set; }

        [JsonPropertyName("KeyRelease")]
        public KeyCode? KeyRelease { get; set; }

    }
}
