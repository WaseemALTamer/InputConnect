using InputConnect.Network;
using InputConnect.Structures;
using System;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace InputConnect.Controllers.Audio
{
    public static class Audio{


        public static AudioHookInterface? AudioHook;


        public static void Start(){
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
                AudioHook = new AudioHookWindows();
            }

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
                AudioHook = new AuidoHookLinux();
            }

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)){
                Console.WriteLine("macOS");
            }

            else{
                Console.WriteLine("Unknown OS");
            }


            AudioOut.Start(); // start the AudioStart to play audio on the run
                              // this approch needs  to be  changed to fit the
                              // change of output audio device


            MessageManager.OnCommandAudio += OnReceiveCommand;
        }


        public static void TransmitAudio(byte[] buffer, int bytesRecorded){
            // this function is going to transmit a text to the other devices to store it in
            // the clipboard

            // int offset -> still havent been implemented look through it

            // run this function for all the auido that you are trying to transmit this function
            // will filter out the ones that doent need to be sent and  the ones that need to be
            // sent


            foreach (var connection in Connections.Devices.ConnectionList){

                if (connection.AudioState != Connections.Constants.Transmit) continue;


                // Skip sending if audio is effectively silent
                if (IsSilent(buffer, bytesRecorded))
                    return;


                var _command = new Commands.Audio{
                    Buffer = buffer,
                    BytesRecorded = bytesRecorded,
                };

                var _commandMessage = new MessageCommand{
                    Type = Commands.Constants.CommandTypes.Audio,
                    SequenceNumber = connection.SequenceNumber + 1,
                    Command = JsonSerializer.Serialize(_command)
                };
                connection.SequenceNumber += 1;

                var messageudp = new MessageUDP{
                    MessageType = Network.Constants.MessageTypes.Command,
                    Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.PasswordKey),
                    IsEncrypted = true
                };

                if (connection.MacAddress != null){
                    ConnectionUDP.Send(MessageManager.MacToIP[connection.MacAddress], messageudp);
                }
            }
        }



        public static void OnReceiveCommand(Commands.Audio command, Connection connection){
            if (command.Buffer == null) return;
            if (command.BytesRecorded == null) return;

            //Console.WriteLine(command.BytesRecorded);


            if (connection.AudioQueue == null)
            {
                connection.AudioQueue = new AudioQueue();
            }
            //Console.WriteLine(connection.MacAddress);

            //Console.WriteLine(command.Buffer.Length);

            connection.AudioQueue.Write(command.Buffer);
        }



        private static bool IsSilent(byte[] buffer, int bytesRecorded, float threshold = 0.0001f){
            // this function checks if the buffer is filled with zeros and is silent if so we return true
            
            // Each float is 4 bytes
            int floatCount = bytesRecorded / 4;

            for (int i = 0; i < floatCount; i++){
                float sample = BitConverter.ToSingle(buffer, i * 4);
                if (Math.Abs(sample) > threshold)
                    return false; // Found a non-silent sample
            }

            return true; // All samples are below threshold → silent
        }


    }

}

