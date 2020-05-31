using SoundProcessing.Core.Fourier;
using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.FrequencyCalculations;
using SoundProcessing.Core.Helpers;
using SoundProcessing.Core.Model;
using SoundProcessing.Core.Wav;
using System;
using System.Linq;
using System.Numerics;

namespace SoundProcessing.Core.Equalizer
{
    public class Equalizer
    {
        private readonly IFourierWindow _window;
        private readonly FilterType _type;
        private readonly int _m;
        private readonly int _r;

        public Equalizer(IFourierWindow window, FilterType type, int m, int r)
        {
            _window = window;
            _type = type;
            _m = m;
            _r = r;
        }

        public double[] Equalize(double[] gains, WavData wavData)
        {
            var n = FourierHelper.GetExpandedPow2(_m);
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

                if (_type == FilterType.NotCausal)
                {
                    var shiftNumberWindow = (_m - 1) / 2;
                    var shiftedWindow = windows[i].Take(shiftNumberWindow).ToList();
                    var windowTemp = windows[i].Skip(shiftNumberWindow).ToList();
                    windowTemp.AddRange(shiftedWindow);
                    windows[i] = windowTemp.ToArray();
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
            var frequency = FourierMethod.Calculate(data, sampleRate);
            var gain = 0.0;
            if (frequency >= 20 && frequency < 40)
            {
                gain = gains[0];
            }
            else if (frequency >= 40 && frequency < 80)
            {
                gain = gains[1];
            }
            else if (frequency >= 80 && frequency < 160)
            {
                gain = gains[2];
            }
            else if (frequency >= 160 && frequency < 320)
            {
                gain = gains[3];
            }
            else if (frequency >= 320 && frequency < 640)
            {
                gain = gains[4];
            }
            else if (frequency >= 640 && frequency < 1280)
            {
                gain = gains[5];
            }
            else if (frequency >= 1280 && frequency < 2560)
            {
                gain = gains[6];
            }
            else if (frequency >= 2560 && frequency < 5120)
            {
                gain = gains[7];
            }
            else if (frequency >= 5120 && frequency < 10240)
            {
                gain = gains[8];
            }
            else if (frequency >= 10240 && frequency < 20480)
            {
                gain = gains[9];
            }

            if (gain < 0)
            {
                gain = 1 / Math.Abs(gain);
            }
            else if (gain == 0)
            {
                gain = 1;
            }

            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= gain;
            }

            return data;
        }
    }
}
