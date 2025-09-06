using InputConnect.Structures;
using InputConnect.Network;
using System.Text.Json;
using SharpHook.Data;
using SharpHook;
using Tmds.DBus.Protocol;
using System.Collections.Generic;
using System.Linq;




namespace InputConnect.Controllers
{
    public static class Keyboard
    {
        // this will make a refrence of the GlobalHook The global hook is always running
        public static TaskPoolGlobalHook Hook = Controllers.Hook.GlobalHook;
        public static EventSimulator EventSimulator = Controllers.Hook.GlobalEventSimulator;
        public static Structures.Connection? KeyboardTargetConnection;


        public static bool ActiveTransmition = true; // setting this false will pause all the transmition


        // this Start function should be ran after the hook is ran
        public static void Start() {
            Hook.KeyPressed += OnKeyPressed;
            Hook.KeyReleased += OnKeyReleased;


            MessageManager.OnCommandKeyboard += RecieveKeyboardCommnad;
            Controllers.Hook.OnTargetConnectionChange += TransmitAllKeyRelease;
        }


        // Event handler for key presses
        private static void OnKeyPressed(object? sender, KeyboardHookEventArgs e){
            if (ActiveTransmition) {
                TransmitKeyPress(e.Data.KeyCode);
            }

        }

        private static void OnKeyReleased(object? sender, KeyboardHookEventArgs e){
            if (ActiveTransmition){
                TransmitKeyRelease(e.Data.KeyCode);
            }
        }



        public static void PressKey(KeyCode key){
            if (EventSimulator == null) return;

            // Simulate a key press (down)
            EventSimulator.SimulateKeyPress(key);
        }

        public static void ReleaseKey(KeyCode key){
            if (EventSimulator == null) return;

            // Simulate a key release (up)
            EventSimulator.SimulateKeyRelease(key);
        }



        public static void TransmitKeyPress(KeyCode key) {

            if (ActiveTransmition == false) return;

            Structures.Connection? target = KeyboardTargetConnection;

            if (target == null) {
                target = Controllers.Hook.TargetConnection;
            }

            if (target == null) return;


            var _command = new Commands.Keyboard{
                KeyPress = key
            };

            var _commandMessage = new MessageCommand{
                Type = Commands.Constants.CommandTypes.Keyboard,
                SequenceNumber = target.SequenceNumber + 1,
                Command = JsonSerializer.Serialize(_command)
            };
            target.SequenceNumber += 1;


            var messageudp = new MessageUDP{
                MessageType = Network.Constants.MessageTypes.Command,
                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), target.PasswordKey),
                IsEncrypted = true
            };

            if (target.MacAddress != null &&
                MessageManager.MacToIP.TryGetValue(target.MacAddress, out var ip))
            {
                ConnectionUDP.Send(ip, messageudp);
            }

        }



        public static void TransmitKeyRelease(KeyCode key) {
            
            if (ActiveTransmition == false) return;
            
            Structures.Connection? target = KeyboardTargetConnection;

            if (target == null) {
                target = Controllers.Hook.TargetConnection;
            }

            if (target == null) return;


            var _command = new Commands.Keyboard{
                KeyRelease = key
            };

            var _commandMessage = new MessageCommand{
                Type = Commands.Constants.CommandTypes.Keyboard,
                SequenceNumber = target.SequenceNumber + 1,
                Command = JsonSerializer.Serialize(_command)
            };
            target.SequenceNumber += 1;

            var messageudp = new MessageUDP{
                MessageType = Network.Constants.MessageTypes.Command,
                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), target.PasswordKey),
                IsEncrypted = true
            };

            if (target.MacAddress != null &&
                MessageManager.MacToIP.TryGetValue(target.MacAddress, out var ip))
            {
                ConnectionUDP.Send(ip, messageudp);
            }


        }



        public static void TransmitAllKeyRelease()
        {

            if (ActiveTransmition == false) return;

            foreach (var connection in Connections.Devices.ConnectionList){
                if (connection.KeyboardState == Connections.Constants.Transmit){
                    var _command = new Commands.Keyboard{
                        // we send a message which contain nothing represeting releasing all keys
                    };

                    var _commandMessage = new MessageCommand{
                        Type = Commands.Constants.CommandTypes.Keyboard,
                        SequenceNumber = connection.SequenceNumber + 1,
                        Command = JsonSerializer.Serialize(_command)
                    };
                    connection.SequenceNumber += 1;

                    var messageudp = new MessageUDP{
                        MessageType = Network.Constants.MessageTypes.Command,
                        Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.PasswordKey),
                        IsEncrypted = true
                    };

                    if (connection.MacAddress != null &&
                        MessageManager.MacToIP.TryGetValue(connection.MacAddress, out var ip)){
                        ConnectionUDP.Send(ip, messageudp);
                    }


                }
            }
        }


        private static List<KeyCode> pressedKeys = new List<KeyCode>();

        private static void ReleaseAllKeys(){
            foreach (var key in pressedKeys){
                ReleaseKey(key);
            }
            pressedKeys.Clear();
        }

        public static void RecieveKeyboardCommnad(Commands.Keyboard? command)
        {
            if (command == null) return;

            // If both KeyPress and KeyRelease are null, release all keys
            if (command.KeyPress == null && command.KeyRelease == null){
                ReleaseAllKeys();
                return;
            }

            // Handle key press
            if (command.KeyPress != null)
            {
                KeyCode key = (KeyCode)command.KeyPress;
                PressKey(key);

                if (!pressedKeys.Contains(key)){
                    pressedKeys.Add(key);
                }
            }

            // Handle key release
            if (command.KeyRelease != null)
            {
                KeyCode key = (KeyCode)command.KeyRelease;
                if (pressedKeys.Contains(key)){
                    pressedKeys.Remove(key);
                    ReleaseKey(key);
                }
            }
        }
    }
}
