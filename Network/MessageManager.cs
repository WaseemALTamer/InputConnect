using System.Collections.Generic;
using InputConnect.Structures;
using System.Text.Json;
using System;



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



        public static List<MessageUDP> Advertisements = new List<MessageUDP>(); // this is used to collect all the advertisement messages


        public static void ProccessMessageUDP(string message) {
            MessageUDP? _message = JsonSerializer.Deserialize<MessageUDP>(message);
            if (_message != null && _message.MessageType == Constants.MessageTypes.Advertisement) ProccessAdvertisement(_message);
            if (_message != null && _message.MessageType == Constants.MessageTypes.Connect) ProccessConnect(_message);
            if (_message != null && _message.MessageType == Constants.MessageTypes.Accept) ProccessAccepte(_message);
            if (_message != null && _message.MessageType == Constants.MessageTypes.Decline) ProccessDecline(_message);
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

        private static void ProccessAccepte(MessageUDP message) {
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


    }
}