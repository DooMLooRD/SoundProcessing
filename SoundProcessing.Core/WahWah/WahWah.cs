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
        private readonly int _m;
        private readonly int _r;
        private readonly int _l;
        private readonly int _fl;
        private readonly int _fh;
        private readonly int _bandSize;
        private readonly double _lfo;
        private int _gain;

        public WahWah(IFourierWindow window, int m, int r, int l, int fl, int fh, int bandSize, double lfo, int gain)
        {
            _window = window;
            _m = m;
            _r = r;
            _l = l;
            _fl = fl;
            _fh = fh;
            _bandSize = bandSize;
            _lfo = lfo;
            _gain = gain;
        }

        public double[] Process(WavData wavData)
        {
            var n = FourierHelper.GetExpandedPow2(_m + _l - 1);
            var size = wavData.Samples.Length + n - _m;
            var result = new double[size];

            var windows = new double[size / _r][];
            var windowsComplex = new Complex[size / _r][];

            for (int i = 0; i < windows.Length; i++)
            {
                windows[i] = new double[n];
                windowsComplex[i] = new Complex[n];
            }

            if (_gain < 0)
            {
                _gain = 1 / Math.Abs(_gain);
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

            var freq = (double)wavData.FormatChunk.SampleRate / _lfo;
            var filterSize = _fh - _bandSize - _fl;
            for (int i = 0; i < windows.Length; i++)
            {
                var highPass = (Math.Sin(2 * Math.PI * i * _r / freq) * 0.5 + 0.5) * filterSize + _fl;
                var lowPass = highPass + _bandSize;

                var bandFilterFactors = BasicFilter.BandPassFilterFactors(lowPass, highPass, wavData.FormatChunk.SampleRate, _l, n);

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
