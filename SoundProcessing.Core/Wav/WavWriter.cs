using System.IO;

namespace SoundProcessing.Core.Wav
{
    public static class WavWriter
    {
        public static void WriteData(string fileName, WavData data)
        {
            using (var fs = File.Create(fileName))
            using (var writer = new BinaryWriter(fs))
            {
                writer.Write(data.DescriptorChunk.ChunkId);
                writer.Write(data.DescriptorChunk.ChunkSize);
                writer.Write(data.DescriptorChunk.Format);

                writer.Write(data.FormatChunk.ChunkId);
                writer.Write(data.FormatChunk.ChunkSize);
                writer.Write(data.FormatChunk.AudioFormat);
                writer.Write(data.FormatChunk.NumChannels);
                writer.Write(data.FormatChunk.SampleRate);
                writer.Write(data.FormatChunk.ByteRate);
                writer.Write(data.FormatChunk.BlockAlign);
                writer.Write(data.FormatChunk.BitsPerSample);

                if (data.FormatChunk.ChunkSize == 18)
                {
                    writer.Write(data.FormatChunk.ExtraData);
                }

                writer.Write(data.DataChunk.ChunkId);
                writer.Write(data.DataChunk.ChunkSize);
                writer.Write(data.DataChunk.RawData);
            }
        }
    }
}
