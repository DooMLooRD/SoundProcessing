using SoundProcessing.Core.Fourier;
using SoundProcessing.Core.Fourier.Windows;
using System;
using System.Linq;
using System.Numerics;

namespace SoundProcessing.Core.Filtration
{
    public static class BasicFilter
    {
        public static double[] LowPassFilterFactors(double cutFreq, double sampleFreq, int filterLength)
        {
            var result = new double[filterLength];
            var half = (filterLength - 1) / 2.0;
            for (int i = 0; i < filterLength; i++)
            {
                if (i == half)
                {
                    result[i] = 2 * cutFreq / sampleFreq;
                }
                else
                {
                    result[i] = Math.Sin(2 * Math.PI * cutFreq / sampleFreq * (i - half)) / (Math.PI * (i - half));
                }
            }

            return result;
        }

        public static double[] HighPassFilterFactors(double cutFreq, double sampleFreq, int filterLength)
        {
            cutFreq = sampleFreq / 2 - cutFreq;
            var result = LowPassFilterFactors(cutFreq, sampleFreq, filterLength);

            for (int i = 1; i <= result.Length; i++)
            {
                result[i - 1] *= Math.Pow(-1.0, i);
            }

            return result;
        }

        public static Complex[] BandPassFilterFactors(double lowPassFreq, double highPassFreq, double sampleFreq, int filterLength, int n)
        {
            var window = new HanningWindow();
            var windowFilterFactors = window.WindowFactors(filterLength);

            var low = LowPassFilterFactors(lowPassFreq, sampleFreq, filterLength);
            var high = HighPassFilterFactors(highPassFreq, sampleFreq, filterLength);

            var lowWindowed = new double[n];
            var highWindowed = new double[n];

            for (int i = 0; i < filterLength; i++)
            {
                lowWindowed[i] = low[i] * windowFilterFactors[i];
                highWindowed[i] = high[i] * windowFilterFactors[i];
            }

            for (int i = filterLength; i < n; i++)
            {
                lowWindowed[i] = 0;
                highWindowed[i] = 0;
            }

            var shiftNumberFilter = (filterLength - 1) / 2;

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