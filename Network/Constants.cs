using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.Network.Constants
{
    // this file will contain all the constant values for the network
    public static class MessageTypes{

        public static string Advertisement = "Advertisement"; // addvertisement is simply used to braodcast that you are looking for incoming connections
                                                              // it also helps other devices discover you on there application if they are lessining

        public static string Connect = "Connect"; // this message type is to intaillise a connections and connected both devices togather
                                                  // it is only used when connecting to another device

        public static string Disconnect = "Disconnect"; // used to disconnect a connection

        public static string Command = "Command"; // this message is a command messge which tells the other devices to excuate a mouse momvent, keyboard
                                                  // keypress and more, this need to be encreapted and each message needs  a special  pattern it is only
                                                  // used after astaplishing a secur UDP protocol



    }
}
