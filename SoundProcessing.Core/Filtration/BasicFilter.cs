using SoundProcessing.Core.Fourier;
using SoundProcessing.Core.Fourier.Windows;
using System;
using System.Linq;
using System.Numerics;

namespace SoundProcessing.Core.Filtration
{
    public static class BasicFilter
    {
        public static double[] LowPassFilterFactors(double fc, double fs, int l)
        {
            var result = new double[l];
            var half = (l - 1) / 2.0;
            for (int i = 0; i < l; i++)
            {
                if (i == half)
                {
                    result[i] = 2 * fc / fs;
                }
                else
                {
                    result[i] = Math.Sin(2 * Math.PI * fc / fs * (i - half)) / (Math.PI * (i - half));
                }
            }

            return result;
        }

        public static double[] HighPassFilterFactors(double fc, double fs, int l)
        {
            fc = fs / 2 - fc;
            var result = LowPassFilterFactors(fc, fs, l);
            for (int i = 1; i <= result.Length; i++)
            {
                result[i - 1] *= Math.Pow(-1.0, i);
            }

            return result;
        }


        public static Complex[] BandPassFilterFactors(double fl, double fh, double fs, int l, int n)
        {
            var window = new HanningWindow();
            var windowFilterFactors = window.WindowFactors(l);

            var low = LowPassFilterFactors(fl, fs, l);
            var high = HighPassFilterFactors(fh, fs, l);

            var lowWindowed = new double[n];
            var highWindowed = new double[n];

            for (int i = 0; i < l; i++)
            {
                lowWindowed[i] = low[i] * windowFilterFactors[i];
                highWindowed[i] = high[i] * windowFilterFactors[i];
            }

            for (int i = l; i < n; i++)
            {
                lowWindowed[i] = 0;
                highWindowed[i] = 0;
            }

            var shiftNumberFilter = (l - 1) / 2;

            var lowShiftedFilter = lowWindowed.Take(shiftNumberFilter);
            var lowFilteredTemp = lowWindowed.Skip(shiftNumberFilter).ToList();
            lowFilteredTemp.AddRange(lowShiftedFilter);
            lowWindowed = lowFilteredTemp.ToArray();

            var highShiftedFilter = highWindowed.Take(shiftNumberFilter);
            var highFilteredTemp = highWindowed.Skip(shiftNumberFilter).ToList();
            highFilteredTemp.AddRange(highShiftedFilter);
            highWindowed = highFilteredTemp.ToArray();

            var lowComplex = FourierTransform.FFT(lowWindowed);
            var highComplex = FourierTransform.FFT(highWindowed);

            var bandPass = new Complex[n];

            for (int i = 0; i < n; i++)
            {
                bandPass[i] = lowComplex[i] * highComplex[i];
            }

            return bandPass;
        }
    }
}