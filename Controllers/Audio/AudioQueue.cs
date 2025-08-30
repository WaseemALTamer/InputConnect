using System;
using System.Collections.Generic;

namespace InputConnect.Controllers.Audio
{


    // this class is more like a structure class to keep track of the Audio Buffer Queue
    // Thread safe as more than one  thread can  be using  this, this  class can be used
    // across the code as a structure class


    // this file will not support converting the quido frequency rather you should do it
    // before you add it to the queue

    // removing the lock part  which scraps the thread  safe part  made the audio output
    // faster as the callback function from the sdl  for  the audio  was getting  locked
    // this is not the best apporch thats why  you should  either make  a something that
    // pushes a read buffer for the audio callback function  or track  it  other way for
    // assume this filse is not thread  safe for  the most  part but it  still works and
    // the error rate is very minimal could arrise later on though 


    // the lock is used again interduced


    public class AudioQueue
    {
        private readonly byte[] buffer;
        private readonly int capacity;

        private int readIndex = 0;
        private int writeIndex = 0;
        private int bytesAvailable = 0;

        private readonly object lockObj = new object();

        /// <summary>
        /// Initializes the AudioQueue with a fixed buffer capacity (default 104000 bytes)
        /// </summary>
        public AudioQueue(int capacity = 104000)
        {
            this.capacity = capacity;
            buffer = new byte[capacity];
        }

        /// <summary>
        /// Writes bytes into the buffer (network thread)
        /// Drops bytes if the buffer is full
        /// </summary>
        public void Write(byte[] data, int offset = 0, int count = -1)
        {
            if (data == null) return;
            if (count == -1) count = data.Length - offset;

            lock (lockObj)
            {
                int freeSpace = capacity - bytesAvailable;
                int toWrite = Math.Min(count, freeSpace);

                for (int i = 0; i < toWrite; i++)
                {
                    buffer[writeIndex] = data[offset + i];
                    writeIndex = (writeIndex + 1) % capacity;
                }
                bytesAvailable += toWrite;
            }
        }

        /// <summary>
        /// Reads exactly output.Length bytes from the buffer (callback thread)
        /// Fills remaining bytes with 0 (silence) if not enough data
        /// </summary>
        public int Read(byte[] output)
        {
            if (output == null || output.Length == 0) return 0;

            int bytesRead = 0;

            lock (lockObj)
            {
                int toRead = Math.Min(output.Length, bytesAvailable);

                for (int i = 0; i < toRead; i++)
                {
                    output[i] = buffer[readIndex];
                    readIndex = (readIndex + 1) % capacity;
                }

                bytesAvailable -= toRead;
                bytesRead = toRead;
            }

            // Fill remaining bytes with silence if underflow
            for (int i = bytesRead; i < output.Length; i++)
                output[i] = 0;

            return bytesRead;
        }

        /// <summary>
        /// Returns the number of bytes currently available in the buffer
        /// </summary>
        public int BytesAvailable
        {
            get { lock (lockObj) return bytesAvailable; }
        }
    }

}
