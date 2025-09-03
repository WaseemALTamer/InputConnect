using System.Text.Json.Serialization;
using System.Collections.Generic;




namespace InputConnect.Structures
{
    // this class will hold the data that will be used to be send from one
    // device  to another, the data is none sensitive  data  but  they  are
    // encreapted for added scurity


    public class IntialData
    {
        [JsonPropertyName("Screens")]
        public List<Bounds>? Screens { get; set; }

    }
}
