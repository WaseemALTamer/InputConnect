using InputConnect.Network;
using InputConnect.Structures;
using SharpHook;
using SharpHook.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

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

            if (target.MacAddress != null){
                ConnectionUDP.Send(MessageManager.MacToIP[target.MacAddress], messageudp);
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

            if (target.MacAddress != null){
                ConnectionUDP.Send(MessageManager.MacToIP[target.MacAddress], messageudp);
            }


        }



        public static void RecieveKeyboardCommnad(Commands.Keyboard? command)
        {

            if (command == null) return;

            if (command.KeyPress != null){
                PressKey((KeyCode)command.KeyPress);
            }

            if (command.KeyRelease != null){
                ReleaseKey((KeyCode)command.KeyRelease);
            }

        }


    }
}
