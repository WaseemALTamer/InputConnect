using System.Runtime.InteropServices;
using InputConnect.Structures;
using InputConnect.Network;
using System;
using SDL2;







namespace InputConnect.Controllers.Audio
{
    public static class AudioOut
    {

        // this class should be  changed later to be the master class
        // that handels the crossplatform desktop  audio  capture and
        // the AudioOutput should be moved to another file for better
        // readablity



        // this uses the sdl library to play the sound on your  Audio output device
        // though this will not detect the defult output device because this is not
        // the latest version of sdl2 Consider Upgrading




        public static uint? OpenAudioDevice;


        public static void Start()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_AUDIO) < 0){
                Console.WriteLine("SDL_Init failed: " + SDL.SDL_GetError());
                return;
            }

            // Desired audio spec
            SDL.SDL_AudioSpec desiredSpec = new SDL.SDL_AudioSpec{
                freq = Setting.Config.AudioFrequency,
                format = SDL.AUDIO_F32SYS,
                channels = Setting.Config.AudioChannals,
                samples = (ushort)(Setting.Config.AudioBufferSize / 10),
                callback = OnSDLAudioCallBack
            };


            string device = SDL.SDL_GetAudioDeviceName(0, 0);

            //Console.WriteLine($"{device}");


            OpenAudioDevice = SDL.SDL_OpenAudioDevice(
                                  device,
                                  0,
                                  ref desiredSpec,
                                  out SDL.SDL_AudioSpec obtainedSpec,
                                  0);


            if (OpenAudioDevice == 0){
                Console.WriteLine("SDL_OpenAudioDevice failed: " + SDL.SDL_GetError());
                SDL.SDL_Quit();
                return;
            }

            SDL.SDL_PauseAudioDevice((uint)OpenAudioDevice, 0);


            MessageManager.OnCommandAudio += OnReceiveCommand;
        }


        private static void OnSDLAudioCallBack(nint userdata, nint stream, int len)
        {
            int samples = len / 4; // 4 bytes per float sample
            float[] mixBuffer = new float[samples];


            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection.AudioQueue == null || 
                    connection.AudioState != Connections.Constants.Receive)
                    continue;

                // Read as many bytes as are available, up to len
                byte[] buffer = new byte[len];
                int bytesRead = connection.AudioQueue.Read(buffer);

                int framesRead = bytesRead / 4; // number of float samples
                float[] input = new float[framesRead];
                Buffer.BlockCopy(buffer, 0, input, 0, bytesRead);

                // Mix (add) into mixBuffer
                for (int i = 0; i < framesRead && i < samples; i++){
                    mixBuffer[i] += input[i];
                }
            }

            // Write result to SDL stream
            Marshal.Copy(mixBuffer, 0, stream, samples);
        }


        public static void OnReceiveCommand(Commands.Audio command, Connection connection){
            if (command.Buffer == null) return;
            if (command.BytesRecorded == null) return;


            if (connection.AudioQueue == null) {
                connection.AudioQueue = new AudioQueue();
            }
            //Console.WriteLine(connection.MacAddress);

            connection.AudioQueue.Write(command.Buffer);
        }

    }
}

