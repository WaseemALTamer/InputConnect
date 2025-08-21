using InputConnect.Structures;
using InputConnect.Network;
using System;
using Avalonia.Controls;
using System.Threading.Tasks;



namespace InputConnect.Connections
{
    public static class Manager
    {
        // this class will be used to grab the connection and the disconnect messages along with all the
        // and will proform the actions of connecting and disconnecting also all  the  command meessages
        // will be passed through this class which will also filter the message  out  incase  they  have
        // different SequenceNumber or a different token


        // your EstablishConnection  varable  should  return  a  Connection  struct  linking back to the
        // connection list so you can keep track of it and then it is less work later on

        public static Action? OnConnectedConnectionAdded; // this will only work if the connection is accespted
        public static Action? ActionOnIncomingConnection; // this will be used for the UI to act upone seeing connection
                                                          // incoming


        static Manager() {
            MessageManager.OnConnect += OnIncomingConnection; // map the connect message to our function
        }



        // this function is to be used to make a connection with another device on the network
        public static Connection? EstablishConnection(string IP, string Token, string? MacAdress, string? Devicename=null){ 
            // we only need the ip to make the connections with a device on the network and is listening
            // the token is not shared and is only used to send the  message over  this will  be used to
            // send a connection request nothing more nothing less, it will also  add the  connection to
            // the connection list but with a <pending> state waiting for response

            MessageUDP messageUDP = new MessageUDP { 
                MessageType = Network.Constants.MessageTypes.Connect,
                Text = Encryptor.Encrypt(Constants.PassPhase, Token), // encreapt the text for the other device to try to decreapt it
                IsEncrypted = true,
            };


            var newConnection = new Connection{
                State = Constants.StatePending,
                DeviceName = Devicename,
                MacAddress = MacAdress,
                SequenceNumber = 0,
                Token = Token,
            };



            for (int i = 0; i < Devices.ConnectionList.Count; i++) {
                if (Devices.ConnectionList[i].MacAddress == MacAdress) {
                    Devices.ConnectionList[i] = newConnection;
                    ConnectionUDP.Send(IP, messageUDP);
                    return newConnection;
                }
            }
                
            // it is advices that you add your connection manually rather than let this function add it for you
            // this way you can move control over it later on on the device page
            
            Devices.ConnectionList.Add(newConnection);
            ConnectionUDP.Send(IP, messageUDP);
            return newConnection;
        }


        public static void OnIncomingConnection(MessageUDP Message) {  // this function is to be maped to the Message manager for the Connect messages

            // connections that are not 
            

            if (SharedData.IncomingConnection.Message != null) {
                // this means we already got a conenction to deal with so that connection is rejected for now

                CloseIncomingConnection(Message, "Busy trying to connect to another device try again later "); // reject
                return; // move on
            } 

            SharedData.IncomingConnection.Message = Message; // pass the message to the SharedData namespace and wait until AcceptIncomingConnection gets triggered

            if (ActionOnIncomingConnection != null) {
                ActionOnIncomingConnection.Invoke(); // wait for other parts of the code to establish the connection through AcceptIncomingConnection with the token
            }
        }


        // this function only trys one to decreapt it if you want to try more than once then write the code your self
        // I actually did check the UI -> ConnectionReplay.cs file for refrence on how to do so 
        public static Connection? AcceptIncomingConnection(MessageUDP Message, string Token) {
            // the token is only there to check if it works no more no less
            // we will also check if the token work with  the  UI, we  will
            // also check it again but this time it is  one time  and if it 
            // wrong then we reject is straight away


            // we stoping doing the "SharedData.IncomingConnection.Clear();" in this function simply do it after this function is ran whenever you call it


            if (Message == null || Message.Text == null) { 
                return null; 
            }
            if (Encryptor.Decrypt(Message.Text, Token) != Constants.PassPhase) {
                CloseIncomingConnection(Message, "Token mismatch"); // reject it
                //SharedData.IncomingConnection.Clear(); // remove the the message
                return null; // move on
            }

            MessageUDP messageUDP = new MessageUDP{
                MessageType = Network.Constants.MessageTypes.Accept,
                Text = Encryptor.Encrypt(Constants.PassPhase, Token), // replay with the pass phase for absolutely no reason
                IsEncrypted = true,
            };

            if (Message.IP != null){
                ConnectionUDP.Send(Message.IP, messageUDP); // notify the other device to add the connection now
            }

            // now we can Check the message and add the connection to the list
            var newConnection = new Connection{
                State = Constants.StateConnected,
                DeviceName = Message.DeviceName,
                MacAddress = Message.MacAddress,
                SequenceNumber = 0,
                Token = Token,
            };

            for (int i = 0; i < Devices.ConnectionList.Count; i++){ // check if connection already exists
                if (Devices.ConnectionList[i].MacAddress == Message.MacAddress){ // if it does then we will overwrite it with the new connection
                    Devices.ConnectionList[i] = newConnection;
                    //SharedData.IncomingConnection.Clear(); // remove the message
                    return newConnection;
                }
            }


            Devices.ConnectionList.Add(newConnection); // added the new connection
            if (OnConnectedConnectionAdded != null) OnConnectedConnectionAdded.Invoke();
            //SharedData.IncomingConnection.Clear();
            return newConnection;
        }


        public static void CloseIncomingConnection(MessageUDP Message, string? Reason = null) {

            if (Message == null) return;

            MessageUDP messageUDP = new MessageUDP{
                MessageType = Network.Constants.MessageTypes.Decline,
                Text = Reason,
            };


            for (int i = 0; i < Devices.ConnectionList.Count; i++){ 
                // check if connection already exists
                var device = Devices.ConnectionList[i];
                if (device.MacAddress == Message.MacAddress){ 
                    // if it does then we will overwrite it with the new connection
                    Devices.ConnectionList.Remove(device);

                    // note that we dont break out because there is an issue  that causes
                    // two connections i have no clue where it is but simply leaving this
                    // function to not break out will fix this problem, the  problem have
                    // been fixed through it might arrise again if you  try  to  add more
                    // only add your connection once

                    break;
                }
            }

            if (Message.IP != null){
                ConnectionUDP.Send(Message.IP, messageUDP);
            }
        }


        public static async Task RequestInitialData(Connection connection) {
            // this function will block the function that your trigger this function 
            // from until it get the data if  you want  to run  other  this function 
            // works as follows:
            // request_data -> wait_for_data -> get_data -> return

            if (connection.MacAddress == null) return;

            MessageUDP newMessage = new MessageUDP{
                MessageType = Network.Constants.MessageTypes.IntialDataRequest,
            };

            ConnectionUDP.Send(MessageManager.MacToIP[connection.MacAddress], newMessage);

            var expiryTime = DateTime.Now.AddMilliseconds(3000); // 3 seconds

            while (DateTime.Now < expiryTime){
                if (connection.Screens != null) return;
                await Task.Delay(1);
            }

            return;
        }


    }
}
