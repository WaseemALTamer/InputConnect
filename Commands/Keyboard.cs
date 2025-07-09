using System.Text.Json.Serialization;





namespace InputConnect.Commands
{
    public class Keyboard{


        [JsonPropertyName("KeyMatrix")]
        public string? KeyMatrix { get; set; } // this needs to be changed later

    }
}
