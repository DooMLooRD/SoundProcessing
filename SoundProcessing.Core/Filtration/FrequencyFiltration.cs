using SoundProcessing.Core.Fourier;
using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.Helpers;
using SoundProcessing.Core.Model;
using SoundProcessing.Core.Wav;
using System.Linq;
using System.Numerics;

namespace SoundProcessing.Core.Filtration
{
    public class FrequencyFiltration
    {
        private readonly IFourierWindow _window;
        private readonly int _m;
        private readonly int _r;
        private readonly FilterType _type;
        private readonly int _l;
        private readonly double _fc;
        private readonly int? _n;

        public FrequencyFiltration(IFourierWindow window, int m, int r, FilterType type, int l, double fc, int? n = null)
        {
            _window = window;
            _m = m;
            _r = r;
            _type = type;
            _l = l;
            _fc = fc;
            _n = n;
        }

        public double[] FilterData(WavData wavData)
        {
            var n = _n ?? FourierHelper.GetExpandedPow2(_m + _l - 1);
            var size = wavData.Samples.Length + n - _m;
            var result = new double[size];

            var windows = new double[size / _r][];
            var windowsComplex = new Complex[size / _r][];

            for (int i = 0; i < windows.Length; i++)
            {
                windows[i] = new double[n];
                windowsComplex[i] = new Complex[n];
            }

            var windowFactors = _window.WindowFactors(_m);
            for (int i = 0; i < windows.Length; i++)
            {
                for (int j = 0; j < _m; j++)
                {
                    if (i * _r + j < wavData.Samples.Length)
                    {
                        windows[i][j] = windowFactors[j] * wavData.Samples[i * _r + j];
                    }
                    else
                    {
                        windows[i][j] = 0;
                    }
                }
                for (int j = _m; j < n; j++)
                {
                    windows[i][j] = 0;
                }
            }

            var windowFilterFactors = _window.WindowFactors(_l);
            var filterFactors = BasicFilter.LowPassFilterFactors(_fc, wavData.FormatChunk.SampleRate, _l);
            var filtered = new double[n];
            for (int i = 0; i < _l; i++)
            {
                filtered[i] = windowFilterFactors[i] * filterFactors[i];
            }

            for (int i = _l; i < n; i++)
            {
                filtered[i] = 0;
            }


            if (_type == FilterType.NotCausal)
            {
                var shiftNumberFilter = (_l - 1) / 2;

                var shiftedFilter = filtered.Take(shiftNumberFilter);
                var filteredTemp = filtered.Skip(shiftNumberFilter).ToList();
                filteredTemp.AddRange(shiftedFilter);
                filtered = filteredTemp.ToArray();
            }

            var filteredComplex = FourierTransform.FFT(filtered);

            for (int i = 0; i < windows.Length; i++)
            {
                windowsComplex[i] = FourierTransform.FFT(windows[i]);
                for (int j = 0; j < windowsComplex[i].Length; j++)
                {
                    windowsComplex[i][j] *= filteredComplex[j];
                }
                windows[i] = FourierTransform.IFFT(windowsComplex[i]);
            }

            for (int i = 0; i < windows.Length; i++)
            {
                for (int j = 0; j < windows[i].Length; j++)
                {
                    if (i * _r + j < wavData.Samples.Length)
                    {
                        result[i * _r + j] += windows[i][j];
                    }
                }
            }

            return result;
        }
    }
}
