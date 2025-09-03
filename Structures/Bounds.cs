using System.Text.Json.Serialization;





namespace InputConnect.Structures
{
    public class Bounds
    {

        [JsonPropertyName("X")]
        public int X { get; set; }

        [JsonPropertyName("Y")]
        public int Y { get; set; }

        [JsonPropertyName("Width")]
        public int Width { get; set; }

        [JsonPropertyName("Height")]
        public int Height { get; set; }

        public Bounds(int x = 0, int y = 0, int width = 0, int height = 0){
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
