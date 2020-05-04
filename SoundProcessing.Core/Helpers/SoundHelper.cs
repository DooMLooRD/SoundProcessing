using SoundProcessing.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundProcessing.Core.Helpers
{
    public class SoundHelper
    {
        public static short GetBitsPerSample(List<Sound> sounds)
        {
            var max = sounds.Max(c => c.Frequency);
            var min = sounds.Min(c => c.Frequency);

            if (max < short.MaxValue && min > short.MinValue)
            {
                return 16;
            }
            if (max < int.MaxValue && min > int.MinValue)
            {
                return 32;
            }
            if (max < long.MaxValue && min > long.MinValue)
            {
                return 64;
            }

            return 0;
        }


        public static int GetWindowSize(int sampleRate)
        {
            return sampleRate / 20;
        }

        public static double[][] GenerateSound(List<Sound> frequencies, int sampleRate)
        {
            var splittedFreq = new List<long>();

            var bitsPerSample = GetBitsPerSample(frequencies);
            var doubleScale = bitsPerSample == 64 ? long.MaxValue : bitsPerSample == 32 ? int.MaxValue : short.MaxValue;

            foreach (var freq in frequencies)
            {
                for (int i = 0; i < freq.EndTime / 50 - freq.StartTime / 50; i++)
                {
                    splittedFreq.Add(freq.Frequency);
                }
            }

            var result = new double[splittedFreq.Count][];
            for (int i = 0; i < splittedFreq.Count; i++)
            {
                result[i] = new double[GetWindowSize(sampleRate)];
                var freq = (double)sampleRate / splittedFreq[i];

                for (int j = 0; j < GetWindowSize(sampleRate); j++)
                {
                    result[i][j] = doubleScale * Math.Sin(2 * Math.PI * (i * GetWindowSize(sampleRate) + j) / freq) / 10;
                }
            }

            return result;
        }
    }
}
