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

        public static List<MessageUDP> Advertisements = new List<MessageUDP>(); // this is used to collect all the advertisement messages
        public static Action? OnAdvertisement;


        public static Action<MessageUDP>? OnConnect; // it will pass the message as <MessageUDP> so you can
                                                     // run your logic in it

        public static void ProccessMessageUDP(string message) {
            MessageUDP? _message = JsonSerializer.Deserialize<MessageUDP>(message);
            if (_message != null && _message.MessageType == Constants.MessageTypes.Advertisement) ProccessAdvertisement(_message);
            if (_message != null && _message.MessageType == Constants.MessageTypes.Connect) ProccessConnect(_message);
        }

        private static void ProccessAdvertisement(MessageUDP message){
            for (int _index = 0; _index < Advertisements.Count; _index++){
                if (Advertisements[_index].IP == message.IP) {
                    Advertisements[_index] = message;
                    if (OnAdvertisement != null) OnAdvertisement.Invoke();
                    return;
                }
            }
            Advertisements.Add(message);
        }

        private static void ProccessConnect(MessageUDP message){
            if (OnConnect != null) OnConnect.Invoke(message);
        }
    }
}