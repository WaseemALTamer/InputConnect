using System.Text.Json.Serialization;


namespace InputConnect.Structures
{
    // This struct class is a general idea of a message that we are going to take to send
    // over udp network

    public class MessageUDP{
        [JsonPropertyName("MessageType")]
        public string? MessageType { get; set; }

        [JsonPropertyName("MacAdress")]
        public string? MacAddress { get; set; }

        [JsonPropertyName("Text")]
        public string? Text { get; set; }

        [JsonPropertyName("IP")]
        public string? IP { get; set; }

        [JsonPropertyName("Port")]
        public int? Port { get; set; }

        [JsonPropertyName("Time")]
        public string? Time { get; set; }

        [JsonPropertyName("IsEncrypted")]
        public bool? IsEncrypted { get; set; }

    }
}
