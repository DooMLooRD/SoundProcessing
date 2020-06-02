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
        private readonly int _windowLength;
        private readonly int _windowHopSize;
        private readonly int _filterLength;

        public Equalizer(IFourierWindow window, int windowLength, int windowHopSize, int filterLength)
        {
            _window = window;
            _windowLength = windowLength;
            _windowHopSize = windowHopSize;
            _filterLength = filterLength;
        }

        public double[] Equalize(double[] gains, WavData wavData)
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

            var windowFactors = _window.WindowFactors(_windowLength);
            var gainsComplex = GenerateGains(gains, wavData.FormatChunk.SampleRate, n);
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

                windowsComplex[i] = FourierTransform.FFT(windows[i]);
                windowsComplex[i] = AdjustGain(gainsComplex, windowsComplex[i]);
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


        public Complex[] AdjustGain(Complex[][] gainsComplex, Complex[] data)
        {
            var n = data.Length;

            var equalized = new Complex[n];

            for (int j = 0; j < data.Length; j++)
            {
                equalized[j] = 0;
            }

            for (int i = 0; i < gainsComplex.Length; i++)
            {
                AddGain(data, gainsComplex[i], equalized);
            }

            return equalized;
        }

        public void AddGain(Complex[] data, Complex[] gainsComplex, Complex[] equalized)
        {
            for (int j = 0; j < data.Length; j++)
            {
                equalized[j] += data[j] * gainsComplex[j];
            }
        }

        private Complex[][] GenerateGains(double[] gains, int sampleRate, int n)
        {
            var gainsComplex = new Complex[10][];
            var low = 40;
            var high = 20;

            for (int i = 0; i < 10; i++)
            {
                if (gains[i] == 0)
                {
                    gains[i] += 1;
                }
                else if (gains[i] < 0)
                {
                    gains[i] = 1.0 / Math.Abs(gains[i]);
                }

                var bandFilterFactors = BasicFilter.BandPassFilterFactors(low, high, sampleRate, _filterLength, n);

                for (int j = 0; j < bandFilterFactors.Length; j++)
                {
                    bandFilterFactors[j] *= gains[i];
                }

                gainsComplex[i] = bandFilterFactors;

                low *= 2;
                high *= 2;
            }

            return gainsComplex;
        }
    }
}
