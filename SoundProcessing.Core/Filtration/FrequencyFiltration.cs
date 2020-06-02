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
        private readonly int _windowLength;
        private readonly int _windowHopSize;
        private readonly FilterType _type;
        private readonly int _filterLength;
        private readonly double _cutFreq;
        private readonly int? _n;

        public FrequencyFiltration(IFourierWindow window, int windowLength, int windowHopSize, FilterType type, int filterLength, double cutFreq, int? n = null)
        {
            _window = window;
            _windowLength = windowLength;
            _windowHopSize = windowHopSize;
            _type = type;
            _filterLength = filterLength;
            _cutFreq = cutFreq;
            _n = n;
        }

        public double[] FilterData(WavData wavData)
        {
            var n = _n ?? FourierHelper.GetExpandedPow2(_windowLength + _filterLength - 1);
            var size = wavData.Samples.Length + n - _windowLength;
            var result = new double[size];

            var windows = new double[size / _windowHopSize][];
            var windowsComplex = new Complex[size / _windowHopSize][];

            for (int i = 0; i < windows.Length; i++)
            {
                windows[i] = new double[n];
                windowsComplex[i] = new Complex[n];
            }

            var windowFactors = _window.WindowFactors(_windowLength);
            for (int i = 0; i < windows.Length; i++)
            {
                for (int j = 0; j < _windowLength; j++)
                {
                    if (i * _windowHopSize + j < wavData.Samples.Length)
                    {
                        windows[i][j] = windowFactors[j] * wavData.Samples[i * _windowHopSize + j];
                    }
                    else
                    {
                        windows[i][j] = 0;
                    }
                }
                for (int j = _windowLength; j < n; j++)
                {
                    windows[i][j] = 0;
                }
            }

            var windowFilterFactors = _window.WindowFactors(_filterLength);
            var filterFactors = BasicFilter.LowPassFilterFactors(_cutFreq, wavData.FormatChunk.SampleRate, _filterLength);
            var filtered = new double[n];
            for (int i = 0; i < _filterLength; i++)
            {
                filtered[i] = windowFilterFactors[i] * filterFactors[i];
            }

            for (int i = _filterLength; i < n; i++)
            {
                filtered[i] = 0;
            }


            if (_type == FilterType.NotCausal)
            {
                var shiftNumberFilter = (_filterLength - 1) / 2;

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
                    if (i * _windowHopSize + j < wavData.Samples.Length)
                    {
                        result[i * _windowHopSize + j] += windows[i][j];
                    }
                }
            }

            return result;
        }
    }
}
