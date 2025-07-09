using System.Text.Json.Serialization;
using System;



namespace InputConnect.Structures
{
    public class MessageCommand // this class will be serilsed using json and will be send in the MessageUDP.Text as a string
    {
        [JsonPropertyName("Type")]
        public string? Type { get; set; } // type of command if this is not filled then the command is disregraded

        [JsonPropertyName("Command")]
        public string? Command { get; set; } // serilsed json check the type before deserlising it

        [JsonPropertyName("Time")]
        public string? Time { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // this will get you the time when creating an instance of the class

        [JsonPropertyName("SequenceNumber")]
        public ulong SequenceNumber { get; set; } // it is really important to fill the sequenceNumber otherwise
                                                  // the reciver will think your a "replay attacker"
    }
}
