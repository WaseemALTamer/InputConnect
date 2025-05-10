using System;
using System.Text.Json.Serialization;


namespace InputConnect.Structures
{
    public class MessageCommand
    {
        [JsonPropertyName("Type")]
        public string? Type { get; set; }

        [JsonPropertyName("Command")]
        public object? Command { get; set; }

        [JsonPropertyName("Time")]
        public string? Time { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // this will get you the time when creating an instance of the class

    }
}
