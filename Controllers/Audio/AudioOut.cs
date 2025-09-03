using System.Runtime.InteropServices;
using InputConnect;
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



        public static void DetectAudioOutputDevices(){
            // this function main perpose is to list all the avaliable device that you can
            // stream out from and store  the data in the Shared.Device.AudioOutputDevices
            // which then can be used in other parts of the code

            // consider making this function return the list rather than  just storying it
            // in a public variable

            if (SharedData.Device.AudioOutputDevices == null)
                SharedData.Device.AudioOutputDevices = new System.Collections.Generic.List<string>();

            SharedData.Device.AudioOutputDevices.Clear();

            for (int i = 0; i < SDL.SDL_GetNumAudioDevices(0); i++){
                SharedData.Device.AudioOutputDevices.Add(SDL.SDL_GetAudioDeviceName(i, 0));
            }
        }


        public static void StartAudioOut(string device){
            // this needs to be started by you manually for the audio to come out

            if (OpenAudioDevice != null && 
                OpenAudioDevice != 0) 
            {
                SDL.SDL_PauseAudioDevice((uint)OpenAudioDevice, 0);
                SDL.SDL_CloseAudioDevice((uint)OpenAudioDevice);
            }


            if (SDL.SDL_Init(SDL.SDL_INIT_AUDIO) < 0){
                Console.WriteLine("SDL_Init failed: " + SDL.SDL_GetError());
                return;
            }

            // Desired audio spec
            SDL.SDL_AudioSpec desiredSpec = new SDL.SDL_AudioSpec{
                freq = InputConnect.Setting.Config.AudioFrequency,
                format = SDL.AUDIO_F32SYS,
                channels = Setting.Config.AudioChannals,
                samples = (ushort)(Setting.Config.AudioBufferSize / 10),
                callback = OnSDLAudioCallBack
            };


            

            //Console.WriteLine($"{device}");


            OpenAudioDevice = SDL.SDL_OpenAudioDevice(
                                  device,
                                  0,
                                  ref desiredSpec,
                                  out SDL.SDL_AudioSpec obtainedSpec,
                                  0);


            if (OpenAudioDevice == 0){
                Console.WriteLine("SDL_OpenAudioDevice failed: " + SDL.SDL_GetError());
                return;
            }

            SDL.SDL_PauseAudioDevice((uint)OpenAudioDevice, 0);

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

    }
}
