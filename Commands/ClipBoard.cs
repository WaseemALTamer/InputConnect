using SharpHook.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InputConnect.Commands
{



    public class ClipBoard
    {

        [JsonPropertyName("Text")]
        public string? Text { get; set; }




    }
}
