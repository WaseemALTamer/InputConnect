using System.Collections.Generic;
using InputConnect.Structures;



namespace InputConnect.Connections
{
    public static class Devices{
        public static List<Connection> ConnectionList = new List<Connection>(); // this is used to store all the connected devices to you
        public static List<MessageUDP> CommandMessageList = new List<MessageUDP>(); // this List will only contain the messages that are relevant




        // i should start a thread right here nad it is for excuating command messages only
    }
}
