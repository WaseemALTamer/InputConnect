using InputConnect.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.Connections
{
    public static class Devices{
        public static List<Connection> ConnectionList = new List<Connection>(); // this is used to store all the connected devices to you
        public static List<MessageUDP> CommandMessage = new List<MessageUDP>(); // this List will only contain the messages that are relevant
    }
}
