using InputConnect.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.SharedData
{
    public static class IncomingConnection{

        // this class is mode only for the communcation between the
        // Connection.Mangaer and the  ConnectionReplay UI class so
        // they can share the data among one another

        public static MessageUDP? Message;




        public static string? Token;



        public static void Clear() {

            Message = null;
            Token = null;

        }
    }
}
