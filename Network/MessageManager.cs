using System.Collections.Generic;
using InputConnect.Structures;
using System.Text.Json;
using System;
using InputConnect.Commands;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Tmds.DBus.Protocol;



namespace InputConnect.Network
{
    public static class MessageManager{
        // This Class is designed to take the Message decode it and  store it
        // in a public varable which other varables  can use it this will not 
        // decreapt the message or do anything, it will  only  determin  what
        // type of a message it is then it will  simply  just  run a function
        // you stored for  messages  when  they  are  proccessed feel free to
        // minpulate the data that the MessageManager holds


        // these Actions are so you can subscribe to them

        public static Action? OnAdvertisement;

        public static Action<MessageUDP>? OnConnect;

        public static Action<MessageUDP>? OnAccept;

        public static Action<MessageUDP>? OnDecline;

        public static Action<MessageUDP>? OnCommand;

        public static Action<Mouse>? OnCommandMouse;

        public static Action<Keyboard>? OnCommandKeyboard;

        public static Action<Audio>? OnCommandAudio;

        public static Action? OnIntailDataReuqest;

        public static Action<IntialData>? OnIntailData;




        public static List<MessageUDP> Advertisements = new List<MessageUDP>(); // this is used to collect all the advertisement messages

        public static Dictionary<string, string> MacToIP = new Dictionary<string, string>(); // this is a dictionary that will hold mac to
                                                                                             // ips this elemnate the  dynamic ips problem


        public static void ProccessMessageUDP(string message) {
            MessageUDP? _message = JsonSerializer.Deserialize<MessageUDP>(message);
            if (_message == null) return;

            UpdateMacToIP(_message);

            if (_message.MessageType == Constants.MessageTypes.Advertisement) ProccessAdvertisement(_message);

            if (_message.MessageType == Constants.MessageTypes.Connect) ProccessConnect(_message);
            if (_message.MessageType == Constants.MessageTypes.Accept) ProccessAccept(_message);
            if (_message.MessageType == Constants.MessageTypes.Decline) ProccessDecline(_message);

            if (_message.MessageType == Constants.MessageTypes.Command) ProccessCommand(_message);

            if (_message.MessageType == Constants.MessageTypes.IntialDataRequest) PorccessIntailDataReuqest(_message);
            if (_message.MessageType == Constants.MessageTypes.IntialData) PorccessIntailData(_message);
        }


        public static void UpdateMacToIP(MessageUDP message) {
            // this function main perpose is to link the macs to there ips

            if (message == null ||
                message.IP == null||
                message.MacAddress == null)
                return;

            MacToIP[message.MacAddress] = message.IP;
        }

        public static Structures.Connection? MessageToConnection(MessageUDP message) {
            foreach (var connection in Connections.Devices.ConnectionList){
                if (connection.MacAddress == message.MacAddress)
                    return connection;
            }
            return null;
        }

        private static void ProccessAdvertisement(MessageUDP message){
            // this filters out the messages that we dont need and are too old
            for (int i = Advertisements.Count - 1; i >= 0; i--){
                var _message = Advertisements[i];
                if (_message.Time == null) continue;
                var time = DateTime.Parse(_message.Time);

                if ((DateTime.Now - time) > TimeSpan.FromSeconds(Setting.Config.AdvertiseTimeSpan)){
                    Advertisements.Remove(_message);
                    continue;
                }
            }

            // update the new message
            for (int _index = 0; _index < Advertisements.Count; _index++){
                if (Advertisements[_index].IP == message.IP) {
                    Advertisements[_index] = message;
                    if (OnAdvertisement != null) OnAdvertisement.Invoke();
                    return;
                }
            }
            Advertisements.Add(message);
            if (OnAdvertisement != null) OnAdvertisement.Invoke();
        }


        private static void ProccessConnect(MessageUDP message){
            if (OnConnect != null) OnConnect.Invoke(message);
        }

        private static void ProccessAccept(MessageUDP message) {
            // this function will run when the other device replay to your
            // connection  request  and they  accept your  connection this
            // function is will add  there connection  into the connection
            // list, this relise on the MacAdress

            for (int i = 0; i < Connections.Devices.ConnectionList.Count; i++) { 
                var device = Connections.Devices.ConnectionList[i];
                if (device.MacAddress == message.MacAddress) {
                    device.State = Connections.Constants.StateConnected;
                    break;
                }

            }



            if (OnAccept != null) OnAccept.Invoke(message);


        }


        private static void ProccessDecline(MessageUDP message)
        {
            // this function will run when the other device replay to your
            // connection  request  and they  accept your  connection this
            // function is will add  there connection  into the connection
            // list, this relise on the MacAdress

            for (int i = 0; i < Connections.Devices.ConnectionList.Count; i++){
                var device = Connections.Devices.ConnectionList[i];
                if (device.MacAddress == message.MacAddress){
                    device.State = Connections.Constants.StateRejection;
                    Connections.Devices.ConnectionList.Remove(device);
                    break;
                }
            }

            


            if (OnDecline != null) OnDecline.Invoke(message);


        }


        private static void ProccessCommand(MessageUDP message) {

            if (OnCommand != null) OnCommand.Invoke(message); // this will not ontain a proccessed message

            Structures.Connection? DeviceConnection = null;

            if (message.Text != null) {
                if (message.IsEncrypted == true) {
                    // loop through the connections and get the password out with the connection
                    foreach (var connection in Connections.Devices.ConnectionList) {
                        if (connection.MacAddress == message.MacAddress) {
                            DeviceConnection = connection;
                            var text = Encryptor.Decrypt(message.Text, connection.Token);
                            message.Text = text;
                            break;
                        }

                    }
                }

                if (message.Text == null || DeviceConnection == null) return;
                var commandMessage = JsonSerializer.Deserialize<MessageCommand>(message.Text);

                if (commandMessage != null &&
                    commandMessage.Type == Commands.Constants.CommandTypes.Mouse &&
                    commandMessage.Command != null) 
                {

                    var Command = JsonSerializer.Deserialize<Commands.Mouse>(commandMessage.Command);

                    if (OnCommandMouse != null &&
                        Command != null &&
                        DeviceConnection.MouseState == Connections.Constants.Receive)
                    { 
                        OnCommandMouse.Invoke(Command); 
                    }

                }


                if (commandMessage != null &&
                    commandMessage.Type == Commands.Constants.CommandTypes.Keyboard &&
                    commandMessage.Command != null)
                {

                    var Command = JsonSerializer.Deserialize<Commands.Keyboard>(commandMessage.Command);

                    if (OnCommandKeyboard != null &&
                        Command != null &&
                        DeviceConnection.KeyboardState == Connections.Constants.Receive)
                    {
                        OnCommandKeyboard.Invoke(Command);
                    }

                }


            }


            
        }


        private static void PorccessIntailDataReuqest(MessageUDP message){
            // find the connection
            Structures.Connection? connection = MessageToConnection(message);
            if (connection == null) return;


            MessageUDP newMessage = new MessageUDP { 
                MessageType = Constants.MessageTypes.IntialData,
                MacAddress = message.MacAddress,
                IsEncrypted = true,
            };

            IntialData data = new IntialData();
            data.Screens = new List<Bounds>();

            // add the data 
            if (SharedData.Device.Screens != null) {
                foreach (var screen in SharedData.Device.Screens){
                    data.Screens.Add(new Bounds(screen.Bounds.X,
                                                screen.Bounds.Y,
                                                screen.Bounds.Width,
                                                screen.Bounds.Height));
                }
            }

            // finnished adding the data




            var EncryptedData = Encryptor.Encrypt(JsonSerializer.Serialize<IntialData>(data), connection.Token);
            newMessage.Text = EncryptedData;


            if (message.IP != null)
                ConnectionUDP.Send(message.IP, newMessage);

            if (OnIntailDataReuqest != null) OnIntailDataReuqest.Invoke();
        }

        private static void PorccessIntailData(MessageUDP message)
        {
            if (message.Text == null) return;
            Structures.Connection? connection = MessageToConnection(message);
            if (connection == null) return;


            if (message.IsEncrypted == true) {
                message.Text = Encryptor.Decrypt(message.Text, connection.Token);
                if (message.Text == null) return; // we return because token is incorrect
            }

            IntialData? data = JsonSerializer.Deserialize<IntialData>(message.Text);

            if (data == null) return;

            connection.Screens = data.Screens;



            if (OnIntailData != null) OnIntailData.Invoke(data);
        }


    }
}