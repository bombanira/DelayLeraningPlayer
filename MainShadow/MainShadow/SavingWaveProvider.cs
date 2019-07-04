using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Shadow_player_
{
    class SavingWaveProvider : IWaveProvider, IDisposable
    {
        private readonly IWaveProvider sourceWaveProvider;
        private readonly WaveFileWriter writer;
        private bool isWriterDisposed;
        public float Volume { get; set; }

        public SavingWaveProvider(IWaveProvider sourceWaveProvider, string wavFilePath)
        {
            this.sourceWaveProvider = sourceWaveProvider;
            writer = new WaveFileWriter(wavFilePath, sourceWaveProvider.WaveFormat);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var read = sourceWaveProvider.Read(buffer, offset, count);
            if (count > 0 && !isWriterDisposed)
            {
                writer.Write(buffer, offset, read);
            }
            if (count == 0)
            {
                Dispose();
            }
            if (Volume == 0.0f)
            {
                for (int n = 0; n < read; n++)
                {
                    buffer[offset++] = 0;
                }
            }
            else if (Volume != 1.0f)
            {
                for (int n = 0; n < read; n += 2)
                {
                    short sample = (short)((buffer[offset + 1] << 8) | buffer[offset]);
                    var newSample = sample * Volume;
                    sample = (short)newSample;
                    if (Volume > 1.0f)
                    {
                        if (newSample > Int16.MaxValue) sample = Int16.MaxValue;
                        else if (newSample < Int16.MinValue) sample = Int16.MinValue;
                    }

                    buffer[offset++] = (byte)(sample & 0xFF);
                    buffer[offset++] = (byte)(sample >> 8);
                }
            }

            return read;
        }

        public WaveFormat WaveFormat { get { return sourceWaveProvider.WaveFormat; } }

        public void Dispose()
        {
            if (!isWriterDisposed)
            {
                isWriterDisposed = true;
                writer.Dispose();
            }
        }
    }
}