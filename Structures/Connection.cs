using InputConnect.Controllers.Audio;
using System.Collections.Generic;
using System.Text.Json.Serialization;





namespace InputConnect.Structures
{
    public class Connection{
        // connections only need to be made for commands to be sent over all the other message types
        // doesnt not need encreaption, there is more than one type of command message

        [JsonPropertyName("State")]
        public string? State { get; set; } // used to indecate the state of the connection <pending, connected, rejected>

        [JsonPropertyName("DeviceName")]
        public string? DeviceName { get; set; }

        [JsonPropertyName("MacAddress")]
        public string? MacAddress { get; set; } // this will be used to identify the Connection and the  messages they send
                                                // simply  mimicking  someone  MacAdress  isn't  going  to cut  it  as  the
                                                // application has many layers of identification and filtring attackers for
                                                // replay attack, though if they decreapt the message then change the token
        
        [JsonPropertyName("SequenceNumber")]
        public ulong SequenceNumber { get; set; } = 0; // this is used to track command messages we using long  data type which
                                                       // gives us 19 quintillion messages before  our application  will  break
                                                       // if anyone breaks the application that way, cheers bro  never  thought
                                                       // my application will run on a quantum computer

        // out outdated apporch used the screens approch
        [JsonPropertyName("Bounds")]
        public Bounds? Bounds { get; set; } // this is used to specify what part of your screen you want this application to work

        [JsonPropertyName("Screens")]
        public List<Bounds>? Screens { get; set; } // this is the acutal physical display of the other device with its actual bounds
        
        [JsonPropertyName("VirtualScreens")]
        public List<Bounds>? VirtualScreens { get; set; } // this will be the virtual screens created to go to the next device the bounds
                                                          // are completely imagenery, and need to be converted back to the screen bounds
                                                          // before being sent to the next device
        [JsonPropertyName("AudioQueue")]
        public AudioQueue? AudioQueue { get; set; } // this will  provide a thread free  Queue system in order
                                                    // to keep  track of the  audio  comming from  the network
                                                    // for the  connection which then the audio can be mearged
                                                    // togather for  Mixing incoming data, after  creating the
                                                    // save connection freacher for  quick  startup  reconnect
                                                    // distory the AudioQueue for the connection before saving
                                                    // there is no need to keep track  of the bytes they  mean
                                                    // nothing when saving the connection


        [JsonPropertyName("Logs")]
        public string? Logs { get; set; } // this will hold the logs for certain messages that are sent and
                                          // data on the progress for the connection
        [JsonPropertyName("KeyboardState")]

        public string? KeyboardState { get; set; } // this can be either a Reciver, Transmitter or None
        
        [JsonPropertyName("MouseState")]
        public string? MouseState { get; set; } // this can be either a Reciver, Transmitter or None
        
        [JsonPropertyName("AudioState")]
        public string? AudioState { get; set; } // this can be either a Reciver, Transmitter or None

        [JsonPropertyName("PasswordKey")]
        public PasswordKey? PasswordKey { get; set; } // this will contain the data to encreapt data effienctly
                                                      // this methode allows  us to  store the key and the salt
                                                      // along with it to make it less processing intensive

    }
}