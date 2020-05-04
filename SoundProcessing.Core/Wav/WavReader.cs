using SoundProcessing.Core.Helpers;
using SoundProcessing.Core.Wav;
using System;
using System.IO;

namespace SoundProcessing.Core
{
    public static class WavReader
    {
        public static WavData ReadData(string fileName)
        {
            using (var fs = File.Open(fileName, FileMode.Open))
            using (var reader = new BinaryReader(fs))
            {
                var descriptorChunk = new WavData.Descriptor
                {
                    ChunkId = reader.ReadInt32(),
                    ChunkSize = reader.ReadInt32(),
                    Format = reader.ReadInt32()
                };

                var formatChunk = new WavData.Format
                {
                    ChunkId = reader.ReadInt32(),
                    ChunkSize = reader.ReadInt32(),
                    AudioFormat = reader.ReadInt16(),
                    NumChannels = reader.ReadInt16(),
                    SampleRate = reader.ReadInt32(),
                    ByteRate = reader.ReadInt32(),
                    BlockAlign = reader.ReadInt16(),
                    BitsPerSample = reader.ReadInt16()
                };

                if (formatChunk.ChunkSize == 18)
                {
                    var extraDataSize = reader.ReadInt16();
                    formatChunk.ExtraData = reader.ReadBytes(extraDataSize);
                }

                var dataChunk = new WavData.Data
                {
                    ChunkId = reader.ReadInt32(),
                    ChunkSize = reader.ReadInt32(),
                };

                dataChunk.RawData = reader.ReadBytes(dataChunk.ChunkSize);

                var samplesSize = dataChunk.ChunkSize / formatChunk.BytesPerSample;
                var samples = new double[samplesSize];

                switch (formatChunk.BitsPerSample)
                {
                    case 64:
                        var samples64 = new long[samplesSize];
                        Buffer.BlockCopy(dataChunk.RawData, 0, samples64, 0, dataChunk.ChunkSize);
                        samples = Array.ConvertAll(samples64, e => (double)e);
                        break;
                    case 32:
                        var samples32 = new int[samplesSize];
                        Buffer.BlockCopy(dataChunk.RawData, 0, samples32, 0, dataChunk.ChunkSize);
                        samples = Array.ConvertAll(samples32, e => (double)e);

                        break;
                    case 16:
                        var samples16 = new short[samplesSize];
                        Buffer.BlockCopy(dataChunk.RawData, 0, samples16, 0, dataChunk.ChunkSize);
                        samples = Array.ConvertAll(samples16, e => (double)e);
                        break;
                }

                var numberOfChunks = samplesSize / SoundHelper.GetWindowSize(formatChunk.SampleRate);
                var chunkedSamples = new double[numberOfChunks][];

                for (int i = 0; i < numberOfChunks; i++)
                {
                    chunkedSamples[i] = new double[SoundHelper.GetWindowSize(formatChunk.SampleRate)];
                    for (int j = 0; j < SoundHelper.GetWindowSize(formatChunk.SampleRate); j++)
                    {
                        chunkedSamples[i][j] = samples[SoundHelper.GetWindowSize(formatChunk.SampleRate) * i + j];
                    }
                }

                return new WavData
                {
                    DescriptorChunk = descriptorChunk,
                    FormatChunk = formatChunk,
                    DataChunk = dataChunk,
                    NumberOfChunks = numberOfChunks,
                    ChunkedSamples = chunkedSamples,
                    Samples = samples
                };
            }
        }
    }
}
