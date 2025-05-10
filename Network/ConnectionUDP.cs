using InputConnect.Network.Constants;
using InputConnect.Structures;
using System.Threading.Tasks;
using InputConnect.Setting;
using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using System.Net;
using System;
using System.Net.Mail;
using Tmds.DBus.Protocol;





namespace InputConnect.Network
{
    // this class will be mainly used to send  and  comunicate  with other  devices
    // this class will be ran as soon as the application starts and is not optional
    // through it other devices will be able to detect you this  file  will get its
    // thread

    public static class ConnectionUDP{
        
        public static UdpClient? Client; // we all act as clients and there is no real server
        public static Task? backgroundTaskThread;

        

        static ConnectionUDP() {
            backgroundTaskThread = new Task(async () => await Task.WhenAll(Listen(), AdvertiseDevice()));
            backgroundTaskThread.Start();
        }



        public static void SendUDP(string targetIP, MessageUDP message){
            try{
                if (Client != null){
                    
                    message.IP = Device.IP;
                    message.Port = Config.Port;
                    message.MacAddress = Device.MacAdress;
                    message.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    
                    if (message.IsEncrypted == null) message.IsEncrypted = false; // this ensures we dont give a null in the encrypted bool to other devices

                    string _message = JsonSerializer.Serialize(message);
                    byte[] data = Encoding.UTF8.GetBytes(_message);
                    int _statusCode = Client.Send(data, data.Length, targetIP, Config.Port);
                    if (_statusCode == 117) EstablishConnection(); // 117 means our connection has been denied
                }
            }
            catch (Exception ex){
                Console.WriteLine($"Unexpected error: {ex.Message}.");
            }
        }

        public static bool IsListening = false;
        private static async Task Listen(){
            
            IsListening = true;
            while (true){
                if (Client == null){
                    EstablishConnection();
                    await Task.Delay(3000);
                    continue;
                }
                if (!IsListening) {
                    await Task.Delay(3000);
                    continue; 
                }

                try{
                    if (Client != null) {
                        UdpReceiveResult result = await Client.ReceiveAsync();
                        string message = Encoding.UTF8.GetString(result.Buffer);
                        MessageManager.ProccessMessageUDP(message);
                    }
                }
                catch (ObjectDisposedException){
                    Console.WriteLine("Send failed — UDP client was disposed.");
                    EstablishConnection();
                }
                catch (Exception ex){
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }


        public static bool IsAdvertising = false;
        private static async Task AdvertiseDevice() {
            
            IsAdvertising = true;
            while (true) {
                if (Client == null){
                    EstablishConnection();
                    await Task.Delay(3000);
                    continue;
                }
                if (!IsAdvertising){
                    await Task.Delay(3000);
                    continue;
                }

                MessageUDP AdvertismentMessage = new MessageUDP{
                    MessageType = MessageTypes.Advertisement,
                    Text = Config.DeviceName, // We use the Config.DeviceName this will be the name of the device
                                              // if the name hs not been set by the user
                };



                try{
                    SendUDP(Config.BroadCastAddress, AdvertismentMessage);
                }
                catch (Exception ex){
                    Console.WriteLine($"Failed to send to {Config.BroadCastAddress}: {ex.Message}");
                    break;
                }

                
                await Task.Delay(Config.AdvertisementInterval);
            }
        }

        public static void EstablishConnection() {
            Device.UpdateDeviceData(); // we update the ip and the subnet to check if they changed
            if (Device.MacAdress == null) {
                if (Client != null) { 
                    Client.Close(); 
                    Client = null;
                }
            }
            if (Device.MacAdress != null && Device.IP != null) {
                Client = new UdpClient(new IPEndPoint(IPAddress.Any, Config.Port));
            }
        }
    }
}