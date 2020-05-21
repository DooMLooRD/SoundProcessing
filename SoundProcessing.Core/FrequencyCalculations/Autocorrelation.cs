﻿using SoundProcessing.Core.Model;
using SoundProcessing.Core.Wav;
using System.Collections.Generic;
using System.Linq;

namespace SoundProcessing.Core.FrequencyCalculations
{
    public class Autocorrelation : ICalculateFrequency
    {
        public List<Sound> Calculate(WavData wavData)
        {
            var frequencies = new List<Sound>();
            for (int i = 0; i < wavData.ChunkedSamples.Length; i++)
            {
                var samples = wavData.ChunkedSamples[i];

                var localMax = (Index: 0, Value: double.MinValue);
                var lastValue = double.MaxValue;
                var monotonicityChanged = 0;
                var isFalling = true;
                var found = false;
                var result = new double[samples.Length - 1];

                for (int j = 1; j < samples.Length; j++)
                {
                    var correlated = 0.0;
                    for (int k = 0; k < samples.Length - j; k++)
                    {
                        correlated += samples[k] * samples[k + j];
                    }

                    result[j - 1] = correlated;

                    if (!found && isFalling != lastValue > correlated)
                    {
                        isFalling = lastValue > correlated;
                        monotonicityChanged++;

                        if (monotonicityChanged == 2)
                        {
                            localMax = (j - 1, lastValue);
                            found = true;
                        }
                    }

                    lastValue = correlated;
                }

                if (localMax.Index == 0)
                {
                    continue;
                }

                var frequency = wavData.FormatChunk.SampleRate / localMax.Index;
                if (frequencies.Any() && frequencies.Last().Frequency == frequency)
                {
                    frequencies.Last().EndTime += 50;
                }
                else
                {
                    frequencies.Add(new Sound
                    {
                        StartTime = i * 50,
                        EndTime = i * 50 + 50,
                        Frequency = frequency,
                        Result = result
                    });
                }
            }

            return frequencies;
        }

        public override string ToString()
        {
            return "Autocorrelation";
        }
    }
}
