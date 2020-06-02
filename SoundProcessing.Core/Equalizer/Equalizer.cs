using SoundProcessing.Core.Filtration;
using SoundProcessing.Core.Fourier;
using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.Helpers;
using SoundProcessing.Core.Wav;
using System;
using System.Numerics;

namespace SoundProcessing.Core.Equalizer
{
    public class Equalizer
    {
        private readonly IFourierWindow _window;
        private readonly int _m;
        private readonly int _r;
        private readonly int _l;

        public Equalizer(IFourierWindow window, int m, int r, int l)
        {
            _window = window;
            _m = m;
            _r = r;
            _l = l;
        }

        public double[] Equalize(double[] gains, WavData wavData)
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

                windowsComplex[i] = FourierTransform.FFT(windows[i]);
                windowsComplex[i] = AdjustGain(gains, windowsComplex[i], wavData.FormatChunk.SampleRate);
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


        public Complex[] AdjustGain(double[] gains, Complex[] data, int sampleRate)
        {
            var n = data.Length;

            var gain = new Complex[n];

            for (int j = 0; j < data.Length; j++)
            {
                gain[j] = 0;
            }

            var low = 40;
            var high = 20;
            for (int i = 0; i < 10; i++)
            {
                if (gains[i] == 0)
                {
                    gains[i] += 1;
                }

                AddGain(data, gain, sampleRate, n, low, high, gains[i]);

                low *= 2;
                high *= 2;
            }

            return gain;
        }

        public void AddGain(Complex[] data, Complex[] gains, int sampleRate, int n, int low, int high, double gain)
        {
            var bandFilterFactors = BasicFilter.BandPassFilterFactors(low, high, sampleRate, _l, n);

            if (gain < 0)
            {
                gain = 1.0 / Math.Abs(gain);
            }

            for (int j = 0; j < data.Length; j++)
            {
                gains[j] += data[j] * bandFilterFactors[j] * gain;
            }
        }
    }
}
