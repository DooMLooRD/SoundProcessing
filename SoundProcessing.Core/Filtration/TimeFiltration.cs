using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.Wav;
using System.Linq;

namespace SoundProcessing.Core.Filtration
{
    public class TimeFiltration
    {
        private readonly IFourierWindow _window;
        private readonly int _l;
        private readonly double _fc;

        public TimeFiltration(IFourierWindow window, int l, double fc)
        {
            _window = window;
            _l = l;
            _fc = fc;
        }

        public double[] FilterData(WavData wavData)
        {
            var result = new double[wavData.Samples.Length + _l - 1];

            var filterFactors = BasicFilter.FilterFactors(_fc, wavData.FormatChunk.SampleRate, _l);
            var filtered = _window.Windowing(filterFactors);

            var data = wavData.Samples.ToList();
            var zeros = new double[_l - 1];

            data.InsertRange(0, zeros);
            data.AddRange(zeros);

            for (int i = _l - 1; i < data.Count; i++)
            {

                for (int j = 0; j < filtered.Length; j++)
                {
                    result[i - _l + 1] += data[i - j] * filtered[j];
                }
            }

            return result;
        }
    }
}
