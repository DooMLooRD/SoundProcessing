using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.Wav;
using System.Linq;

namespace SoundProcessing.Core.Filtration
{
    public class TimeFiltration
    {
        private readonly IFourierWindow _window;
        private readonly int _filterLength;
        private readonly double _cutFreq;

        public TimeFiltration(IFourierWindow window, int filterLength, double cutFreq)
        {
            _window = window;
            _filterLength = filterLength;
            _cutFreq = cutFreq;
        }

        public double[] FilterData(WavData wavData)
        {
            var result = new double[wavData.Samples.Length + _filterLength - 1];

            var filterFactors = BasicFilter.LowPassFilterFactors(_cutFreq, wavData.FormatChunk.SampleRate, _filterLength);
            var filtered = _window.Windowing(filterFactors);

            var data = wavData.Samples.ToList();
            var zeros = new double[_filterLength - 1];

            data.InsertRange(0, zeros);
            data.AddRange(zeros);

            for (int i = _filterLength - 1; i < data.Count; i++)
            {

                for (int j = 0; j < filtered.Length; j++)
                {
                    result[i - _filterLength + 1] += data[i - j] * filtered[j];
                }
            }

            return result;
        }
    }
}
