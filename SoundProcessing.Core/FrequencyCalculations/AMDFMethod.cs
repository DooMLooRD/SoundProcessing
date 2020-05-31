﻿using SoundProcessing.Core.Model;
using SoundProcessing.Core.Wav;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundProcessing.Core.FrequencyCalculations
{
    public class AMDFMethod : ICalculateFrequency
    {
        public List<Sound> Calculate(WavData wavData)
        {
            var frequencies = new List<Sound>();
            for (int i = 0; i < wavData.ChunkedSamples.Length; i++)
            {
                var samples = wavData.ChunkedSamples[i];

                var localMin = (Index: 0, Value: double.MaxValue);
                var lastValue = double.MinValue;
                var monotonicityChanged = 0;
                var isGrowing = true;
                var found = false;
                var result = new double[samples.Length - 1];

                for (int j = 1; j < samples.Length; j++)
                {
                    var correlated = 0.0;
                    for (int k = 0; k < samples.Length - j; k++)
                    {
                        correlated += Math.Abs(samples[k] - samples[k + j]);
                    }

                    result[j - 1] = correlated;

                    if (!found && isGrowing != lastValue < correlated)
                    {
                        isGrowing = lastValue < correlated;
                        monotonicityChanged++;

                        if (monotonicityChanged == 2)
                        {
                            localMin = (j - 1, lastValue);
                            found = true;
                        }
                    }

                    lastValue = correlated;
                }

                var frequency = wavData.FormatChunk.SampleRate / localMin.Index;
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
            return "AMDF Method";
        }
    }
}
