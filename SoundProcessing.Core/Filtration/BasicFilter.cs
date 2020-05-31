using System;

namespace SoundProcessing.Core.Filtration
{
    public static class BasicFilter
    {
        public static double[] FilterFactors(double fc, double fs, int l)
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
    }
}
