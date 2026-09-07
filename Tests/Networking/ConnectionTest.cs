using InputConnect.Connections;
using InputConnect.Structures;
using System.Threading.Tasks;
using System;
using System.Threading;




namespace InputConnect.Tests
{
    
    public class ConnectionTest : ITest{
        public string Name => "connection";

        PasswordKey passwordKey = new PasswordKey("TestPassword");


        Connection? newConnection;

        public void AcceptConnection(){

            Console.WriteLine("Connection request recived");

            if (SharedData.IncomingConnection.Message == null ) return;

            newConnection = Connections.Manager.AcceptIncomingConnection(SharedData.IncomingConnection.Message,
                                                                            passwordKey);

            Console.WriteLine($"Connection status: {newConnection?.State}");

        }


        public async Task<int> Initialize(){

            // clear all the connections first

            Connections.Devices.ConnectionList.Clear(); // clear the connection list
            
            // simulate an incoming connection
            

            if (Network.Device.IP == null) {
                Console.WriteLine("IP address is not avaliable");
                return -1;
            }


            Manager.ActionOnIncomingConnection += AcceptConnection; // sub to the function to catch the incoming connection with our selfs
            

            await Task.Delay(500); // wait for the network reciver to boot up

            

            Console.WriteLine($"sending the connection request to {Network.Device.MacAdress} on ip {Network.Device.IP}");

            newConnection = Connections.Manager.EstablishConnection(Network.Device.IP,
                                                                        passwordKey,
                                                                        Network.Device.MacAdress,
                                                                        Network.Device.DeviceName);

            // check if the new connection is pending
            if (newConnection?.State == Connections.Constants.StatePending){
                Console.WriteLine("Connection pending status passed");
            }

            // we wait to catch the connection for 3 seconds if we do in those 3 seconds then we pass the test
            var timeout = TimeSpan.FromSeconds(3);
            var startTime = DateTime.UtcNow;
            
            while (newConnection?.State == Connections.Constants.StatePending){
                if (DateTime.UtcNow - startTime >= timeout){
                    break;
                }
                await Task.Delay(100);
            }

            if (newConnection?.State == Connections.Constants.StateConnected){
                Console.WriteLine("established fully accepted connection status passed");
                return 1;
            }

            Manager.ActionOnIncomingConnection -= AcceptConnection; // unsub a the test incase you want to run it again

            // clear the connections for the next session and save it
            Connections.Devices.ConnectionList.Clear();
            AppData.SaveConnections();

            return -1;
        }
    }


}


