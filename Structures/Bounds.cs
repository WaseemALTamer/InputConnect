using System.Text.Json.Serialization;





namespace InputConnect.Structures
{
    public class Bounds
    {

        [JsonPropertyName("X")]
        public int X { get; }

        [JsonPropertyName("Y")]
        public int Y { get; }

        [JsonPropertyName("Width")]
        public int Width { get; }

        [JsonPropertyName("Height")]
        public int Height { get; }

        public Bounds(int x = 0, int y = 0, int width = 0, int height = 0){
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
