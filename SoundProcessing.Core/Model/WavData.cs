using SoundProcessing.Core.Helpers;
using SoundProcessing.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoundProcessing.Core.Wav
{
    public class WavData
    {
        public Descriptor DescriptorChunk { get; set; }
        public Format FormatChunk { get; set; }
        public Data DataChunk { get; set; }

        public double[] Samples { get; set; }

        public int NumberOfChunks { get; set; }
        public double[][] ChunkedSamples { get; set; }

        public WavData() { }

        public WavData(int sampleRate, List<Sound> sounds)
        {
            var bitsPerSample = SoundHelper.GetBitsPerSample(sounds);

            FormatChunk = new Format
            {
                ChunkId = 0x20746D66, //'fmt' in ASCII
                ChunkSize = 16,
                AudioFormat = 1,
                NumChannels = 1,
                SampleRate = sampleRate,
                ByteRate = sampleRate + bitsPerSample / 8,
                BlockAlign = (short)(bitsPerSample / 8),
                BitsPerSample = bitsPerSample
            };

            DataChunk = new Data
            {
                ChunkId = 0x61746164 //'data' in ASCII
            };

            var chunkedSamples = SoundHelper.GenerateSound(sounds, sampleRate);
            NumberOfChunks = chunkedSamples.Length;
            ChangeSamples(chunkedSamples);

            DescriptorChunk = new Descriptor
            {
                ChunkId = 0x46464952, //'RIFF' in ASCII
                ChunkSize = 36 + DataChunk.ChunkSize,
                Format = 0x45564157 //'WAVE' in ASCII
            };
        }

        public WavData(int sampleRate, double[] samples)
        {
            short bitsPerSample = 16;

            FormatChunk = new Format
            {
                ChunkId = 0x20746D66, //'fmt' in ASCII
                ChunkSize = 16,
                AudioFormat = 1,
                NumChannels = 1,
                SampleRate = sampleRate,
                ByteRate = sampleRate + bitsPerSample / 8,
                BlockAlign = (short)(bitsPerSample / 8),
                BitsPerSample = bitsPerSample
            };

            DataChunk = new Data
            {
                ChunkId = 0x61746164 //'data' in ASCII
            };

            Samples = samples;
            AdjustDataChunk();

            DescriptorChunk = new Descriptor
            {
                ChunkId = 0x46464952, //'RIFF' in ASCII
                ChunkSize = 36 + DataChunk.ChunkSize,
                Format = 0x45564157 //'WAVE' in ASCII
            };
        }

        public void ChangeSamples(double[] samples)
        {
            Samples = samples;
            ChunkedSamples = new double[NumberOfChunks][];

            for (int i = 0; i < NumberOfChunks; i++)
            {
                ChunkedSamples[i] = new double[FormatChunk.SampleRate];
                for (int j = 0; j < FormatChunk.SampleRate; j++)
                {
                    ChunkedSamples[i][j] = samples[FormatChunk.SampleRate * i + j];
                }
            }

            AdjustDataChunk();
        }

        public void ChangeSamples(double[][] chunkedSamples)
        {
            Samples = chunkedSamples.SelectMany(c => c).ToArray();
            ChunkedSamples = chunkedSamples;
            AdjustDataChunk();
        }

        private void AdjustDataChunk()
        {
            switch (FormatChunk.BitsPerSample)
            {
                case 64:
                    var samples64 = Array.ConvertAll(Samples, e => (long)e);
                    var result64 = new byte[samples64.Length * sizeof(long)];
                    Buffer.BlockCopy(samples64, 0, result64, 0, result64.Length);
                    DataChunk.ChunkSize = result64.Length;
                    DataChunk.RawData = result64;
                    break;
                case 32:
                    var samples32 = Array.ConvertAll(Samples, e => (int)e);
                    var result32 = new byte[samples32.Length * sizeof(int)];
                    Buffer.BlockCopy(samples32, 0, result32, 0, result32.Length);
                    DataChunk.ChunkSize = result32.Length;
                    DataChunk.RawData = result32;
                    break;
                case 16:
                    var samples16 = Array.ConvertAll(Samples, e => (short)e);
                    var result16 = new byte[samples16.Length * sizeof(short)];
                    Buffer.BlockCopy(samples16, 0, result16, 0, result16.Length);
                    DataChunk.ChunkSize = result16.Length;
                    DataChunk.RawData = result16;
                    break;
            }
        }

        public class Descriptor
        {
            public int ChunkId { get; set; }
            public int ChunkSize { get; set; }
            public int Format { get; set; }
        }

        public class Format
        {
            public int ChunkId { get; set; }
            public int ChunkSize { get; set; }
            public short AudioFormat { get; set; }
            public short NumChannels { get; set; }
            public int SampleRate { get; set; }
            public int ByteRate { get; set; }
            public short BlockAlign { get; set; }
            public short BitsPerSample { get; set; }
            public byte[] ExtraData { get; set; }
            public short BytesPerSample { get => (short)(BitsPerSample / 8); }
        }

        public class Data
        {
            public int ChunkId { get; set; }
            public int ChunkSize { get; set; }
            public byte[] RawData { get; set; }
        }

        public byte[] GetByteData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(DescriptorChunk.ChunkId);
                writer.Write(DescriptorChunk.ChunkSize);
                writer.Write(DescriptorChunk.Format);

                writer.Write(FormatChunk.ChunkId);
                writer.Write(FormatChunk.ChunkSize);
                writer.Write(FormatChunk.AudioFormat);
                writer.Write(FormatChunk.NumChannels);
                writer.Write(FormatChunk.SampleRate);
                writer.Write(FormatChunk.ByteRate);
                writer.Write(FormatChunk.BlockAlign);
                writer.Write(FormatChunk.BitsPerSample);

                if (FormatChunk.ChunkSize == 18)
                {
                    writer.Write(FormatChunk.ExtraData);
                }

                writer.Write(DataChunk.ChunkId);
                writer.Write(DataChunk.ChunkSize);
                writer.Write(DataChunk.RawData);

                return stream.ToArray();
            }
        }

        public static WavData Clone(WavData wavData)
        {
            return new WavData
            {
                DescriptorChunk = new Descriptor
                {
                    ChunkId = wavData.DescriptorChunk.ChunkId,
                    ChunkSize = wavData.DescriptorChunk.ChunkSize,
                    Format = wavData.DescriptorChunk.Format,
                },
                FormatChunk = new Format
                {
                    ChunkId = wavData.FormatChunk.ChunkId,
                    ChunkSize = wavData.FormatChunk.ChunkSize,
                    AudioFormat = wavData.FormatChunk.AudioFormat,
                    NumChannels = wavData.FormatChunk.NumChannels,
                    SampleRate = wavData.FormatChunk.SampleRate,
                    ByteRate = wavData.FormatChunk.ByteRate,
                    BlockAlign = wavData.FormatChunk.BlockAlign,
                    BitsPerSample = wavData.FormatChunk.BitsPerSample
                },
                DataChunk = new Data
                {
                    ChunkId = wavData.DataChunk.ChunkId,
                    ChunkSize = wavData.DataChunk.ChunkSize,
                    RawData = wavData.DataChunk.RawData.ToArray()
                },
                NumberOfChunks = wavData.NumberOfChunks,
                ChunkedSamples = wavData.ChunkedSamples.Select(s => s.ToArray()).ToArray(),
                Samples = wavData.Samples.ToArray()
            };
        }
    }
}
