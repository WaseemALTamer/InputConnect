using InputConnect.Structures;
using InputConnect.Network;
using System;
using System.Reflection.Metadata;
using InputConnect.UI.Holders;


namespace InputConnect.Connections
{
    public static class Manager
    {
        // this class will be used to grab the connection and the disconnect messages along with all the
        // and will proform the actions of connecting and disconnecting also all  the  command meessages
        // will be passed through this class which will also filter the message  out  incase  they  have
        // different SequenceNumber or a different token


        public static Action? ActionOnIncomingConnection; // this will be used for the UI to act upone seeing connection
                                                          // incoming


        static Manager() {
            MessageManager.OnConnect += OnIncomingConnection; // map the connect message to our function
        }



        // this function is to be used to make a connection with another device on the network
        public static void EstablishConnection(string IP, string Token){ // we only need the ip to make the connections with a device on the network and is listening
                                                                         // the token is not shared and is only used to send the message over

            MessageUDP messageUDP = new MessageUDP { 
                MessageType = Network.Constants.MessageTypes.Connect,
                Text = Encryptor.Encrypt(Constants.PassPhase, Token), // encreapt the text for the other device to try to decreapt it
                IsEncrypted = true,
            };
            ConnectionUDP.SendUDP(IP, messageUDP);
        }


        public static void OnIncomingConnection(MessageUDP Message) {  // this function is to be maped to the Message manager for the Connect messages

            // connections that are not 
            

            if (SharedData.IncomingConnection.Message != null) {
                // this means we already got a conenction to deal with so that connection is rejected for now

                RejectIncomingConnection(Message, "Busy trying to connect to another device try again later "); // reject
                return; // move on
            } 

            SharedData.IncomingConnection.Message = Message; // pass the message to the SharedData namespace and wait until AcceptIncomingConnection gets triggered

            if (ActionOnIncomingConnection != null) {
                ActionOnIncomingConnection.Invoke(); // wait for other parts of the code to establish the connection through AcceptIncomingConnection with the token
            }
        }


        // this function only trys one to decreapt it if you want to try more than once then write the code your self
        // I actually did check the UI -> ConnectionReplay.cs file for refrence on how to do so 
        public static void AcceptIncomingConnection(MessageUDP Message, string Token) { // the token is only there to check if it works no more no less
                                                                                        // we will also check if the token work with  the  UI, we  will
                                                                                        // also check it again but this time it is  one time  and if it 
                                                                                        // wrong then we reject is straight away

            if (Message == null || Message.Text == null) { 
                return; 
            }
            if (Encryptor.Decrypt(Message.Text, Token) != Constants.PassPhase) {
                RejectIncomingConnection(Message, "Token mismatch"); // reject it
                SharedData.IncomingConnection.Clear(); // remove the the message
                return; // move on
            }

            MessageUDP messageUDP = new MessageUDP{
                MessageType = Network.Constants.MessageTypes.Accept,
                Text = Encryptor.Encrypt(Constants.PassPhase, Token), // replay with the pass phase for absolutely no reason
                IsEncrypted = true,
            };
            if (Message != null && Message.IP != null){
                ConnectionUDP.SendUDP(Message.IP, messageUDP); // notify the other device to add the connection now
            }

            // now we can Check the message and add the connection to the list
            if (Message == null) return;

            var newConnection = new Connection{
                DeviceName = Message.DeviceName,
                MacAdress = Message.MacAddress,
                SequenceNumber = 0,
                Token = Token,
            };

            for (int i = 0; i < Devices.ConnectionList.Count; i++){ // check if connection already exists
                if (Devices.ConnectionList[i].MacAdress != Message.MacAddress){ // if it does then we will overwrite it with the new connection
                    Devices.ConnectionList[i] = newConnection;
                    SharedData.IncomingConnection.Message = null; // remove the message
                    return;
                }
            }




            Devices.ConnectionList.Add(newConnection); // added the new connection

            SharedData.IncomingConnection.Message = null; // remove the the message
        }

        public static void RejectIncomingConnection(MessageUDP Message, string? Reason = null) {
            
            MessageUDP messageUDP = new MessageUDP{
                MessageType = Network.Constants.MessageTypes.Decline,
                Text = Reason,
            };
            if (Message != null && Message.IP != null) {
                ConnectionUDP.SendUDP(Message.IP, messageUDP);
            }
        }
    }
}
