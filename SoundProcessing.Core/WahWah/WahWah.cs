using SoundProcessing.Core.Filtration;
using SoundProcessing.Core.Fourier;
using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.Helpers;
using SoundProcessing.Core.Wav;
using System;
using System.Numerics;

namespace SoundProcessing.Core.WahWah
{
    public class WahWah
    {
        private readonly IFourierWindow _window;
        private readonly int _windowLength;
        private readonly int _windowHopSize;
        private readonly int _filterLength;
        private readonly int _startBandFreq;
        private readonly int _endBandFreq;
        private readonly int _bandSize;
        private readonly double _lfo;
        private double _gain;

        public WahWah(IFourierWindow window, int windowLength, int windowHopSize, int filterLength, int startBandFreq, int endBandFreq, int bandSize, double lfo, double gain)
        {
            _window = window;
            _windowLength = windowLength;
            _windowHopSize = windowHopSize;
            _filterLength = filterLength;
            _startBandFreq = startBandFreq;
            _endBandFreq = endBandFreq;
            _bandSize = bandSize;
            _lfo = lfo;
            _gain = gain;
        }

        public double[] Process(WavData wavData)
        {
            var n = FourierHelper.GetExpandedPow2(_windowLength + _filterLength - 1);
            var size = wavData.Samples.Length + n - _windowLength;
            var result = new double[size];

            var windows = new double[size / _windowHopSize][];
            var windowsComplex = new Complex[size / _windowHopSize][];

            for (int i = 0; i < windows.Length; i++)
            {
                windows[i] = new double[n];
                windowsComplex[i] = new Complex[n];
            }

            if (_gain < 0)
            {
                _gain = 1 / Math.Abs(_gain) - 1;
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

            var freq = wavData.FormatChunk.SampleRate / _lfo;
            var filterSize = _endBandFreq - _bandSize - _startBandFreq;
            for (int i = 0; i < windows.Length; i++)
            {
                var highPass = (Math.Sin(2 * Math.PI * i * _windowHopSize / freq) * 0.5 + 0.5) * filterSize + _startBandFreq;
                var lowPass = highPass + _bandSize;

                var bandFilterFactors = BasicFilter.BandPassFilterFactors(lowPass, highPass, wavData.FormatChunk.SampleRate, _filterLength, n);

                windowsComplex[i] = FourierTransform.FFT(windows[i]);
                for (int j = 0; j < windowsComplex[i].Length; j++)
                {
                    windowsComplex[i][j] += windowsComplex[i][j] * bandFilterFactors[j] * _gain;
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
