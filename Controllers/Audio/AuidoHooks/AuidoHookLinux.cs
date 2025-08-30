using System.Diagnostics;
using System.Threading;
using System;
using System.Linq;
using System.Text.RegularExpressions;




namespace InputConnect.Controllers.Audio
{
    public class AuidoHookLinux : AudioHookInterface
    {
        // this will only work with PulseAudio

        public static PortAudioSharp.Stream? Stream;

        private Thread? audioThread;
        private bool isRunning = false;

        public AuidoHookLinux()
        {
            StartRecording();
        }

        private void StartRecording()
        {
            isRunning = true;
            audioThread = new Thread(AudioLoop){
                IsBackground = true
            };
            audioThread.Start();
        }

        private void AudioLoop()
        {
            int sampleSize = 4; // float32 = 4 bytes per sample
            int frameSize = Setting.Config.AudioChannals * sampleSize;

            byte[] byteBuffer = new byte[Setting.Config.AudioBufferSize];
            float[] floatBuffer = new float[Setting.Config.AudioBufferSize / sampleSize];

            // Get the default monitor source dynamically
            string? monitorSource = GetDefaultMonitorSource();
            if (string.IsNullOrEmpty(monitorSource))
            {
                Console.WriteLine("No monitor source found.");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "parec",
                Arguments = $"-d {monitorSource} " +
                            $"--format=float32le " +
                            $"--channels={Setting.Config.AudioChannals} " +
                            $"--rate={Setting.Config.AudioFrequency} " +
                            $"--latency-msec=50",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            using var stream = process.StandardOutput.BaseStream;

            while (isRunning){
                int bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length);
                if (bytesRead == 0)
                    break;

                int floatsRead = bytesRead / sampleSize;
                for (int i = 0; i < floatsRead; i++)
                {
                    floatBuffer[i] = BitConverter.ToSingle(byteBuffer, i * sampleSize);
                }

                // Convert float array back to byte array
                byte[] byteBufferToSend = new byte[floatsRead * sampleSize];
                Buffer.BlockCopy(floatBuffer, 0, byteBufferToSend, 0, byteBufferToSend.Length);

                Audio.TransmitAudio(byteBufferToSend, byteBufferToSend.Length);
            }

            process.Kill();
        }




        private string? GetDefaultMonitorSource(){
            try{
                var startInfo = new ProcessStartInfo
                {
                    FileName = "pactl",
                    Arguments = "list short sources",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = startInfo };
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Match the exact source name ending with .monitor
                var match = Regex.Match(output, @"\balsa_output\..+?\.monitor\b");
                if (match.Success){
                    return match.Value; // This is the full source name
                }

                return null;
            }
            catch
            {
                return null;
            }
        }


        public void StopRecording()
        {
            isRunning = false;
            audioThread?.Join();
        }
    }
}
