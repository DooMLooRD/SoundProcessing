using System;

namespace SoundProcessing.Core.Filtration
{
    public static class BasicFilter
    {
        public static double[] LowPassFilterFactors(double fc, double fs, int l)
        {
            var result = new double[l];

            for (int i = 0; i < l; i++)
            {
                if (i == (l - 1) / 2.0)
                {
                    result[i] = 2 * fc / fs;
                }
                else
                {
                    result[i] = Math.Sin(2 * Math.PI * fc / fs * (i - ((l - 1) / 2))) / (Math.PI * (i - ((l - 1) / 2)));
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
    }
}