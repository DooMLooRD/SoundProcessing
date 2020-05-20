using SoundProcessing.Core.Fourier;
using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.Helpers;
using SoundProcessing.Core.Model;
using SoundProcessing.Core.Wav;
using System.Collections.Generic;
using System.Linq;

namespace SoundProcessing.Core.FrequencyCalculations
{
    public class FourierMethod : ICalculateFrequency
    {
        private readonly IFourierWindow _fourierWindow;

        public FourierMethod(IFourierWindow fourierWindow)
        {
            _fourierWindow = fourierWindow;
        }

        public List<Sound> Calculate(WavData wavData)
        {
            var frequencies = new List<Sound>();
            for (int i = 0; i < wavData.ChunkedSamples.Length; i++)
            {
                var samples = wavData.ChunkedSamples[i];

                var reducedSamples = FourierHelper.ReduceToPow2(samples);
                //reducedSamples = FourierHelper.PreEmphasis(reducedSamples);
                reducedSamples = _fourierWindow.Windowing(reducedSamples);

                var complexSamples = FastFourierTransform.FFT(reducedSamples);
                complexSamples = complexSamples.Take(complexSamples.Length / 2).ToArray();

                var threshold = complexSamples.Max(c => c.Magnitude) / 10;
                var localMax = 0;

                for (int j = 1; j < complexSamples.Length; j++)
                {
                    if (complexSamples[j].Magnitude > threshold && complexSamples[j].Magnitude > complexSamples[j - 1].Magnitude && complexSamples[j].Magnitude > complexSamples[j + 1].Magnitude)
                    {
                        localMax = j;
                        break;
                    }
                }

                var frequency = (wavData.FormatChunk.SampleRate / (complexSamples.Length * 2)) * localMax;
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
                    });
                }
            }

            return frequencies;
        }

        public override string ToString()
        {
            return "Fourier Method";
        }
    }
}
