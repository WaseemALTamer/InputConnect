

using System.Text.Json.Serialization;



namespace InputConnect.Commands
{
    public class Audio
    {


        [JsonPropertyName("Buffer")]
        public byte[]? Buffer { get; set; }


        [JsonPropertyName("BytesRecorded")]
        public int? BytesRecorded { get; set; }


        [JsonPropertyName("Frequency")]
        public int? Frequency { get; set; }


        [JsonPropertyName("Channal")]
        public int? Channal { get; set; }


        [JsonPropertyName("Sample")]
        public int? Sample { get; set; }


        [JsonPropertyName("Format")]
        public int? Format { get; set; }

    }
}