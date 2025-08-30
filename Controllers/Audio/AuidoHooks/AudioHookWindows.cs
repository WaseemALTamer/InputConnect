using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;

namespace InputConnect.Controllers.Audio
{
    public class AudioHookWindows : AudioHookInterface
    {


        public static WaveFormat? WaveFormat;
        public static WasapiLoopbackCapture? Capture;

        public AudioHookWindows()
        {


            var enumerator = new MMDeviceEnumerator();
            var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

            WaveFormat = defaultDevice.AudioClient.MixFormat;
            Capture = new WasapiLoopbackCapture()
            {
                ShareMode = AudioClientShareMode.Shared,
                WaveFormat = WaveFormat,
            };
            WaveFormat = Capture.WaveFormat;

            Capture.DataAvailable += OnDataAvailable;
            Capture.StartRecording();

        }



        private static void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            int bytesPerSlice = Setting.Config.AudioBufferSize;


            // we slice the data before transmition if we need to do so
            for (int offset = 0; offset < e.BytesRecorded; offset += bytesPerSlice)
            {
                int sliceLength = Math.Min(bytesPerSlice, e.BytesRecorded - offset);
                byte[] slice = new byte[sliceLength];
                Array.Copy(e.Buffer, offset, slice, 0, sliceLength);

                Audio.TransmitAudio(slice, slice.Length);
            }


        }


        public static void Stop()
        {
            if (Capture == null) return;

            Capture.StopRecording();
            Capture.Dispose();
        }

    }
}
