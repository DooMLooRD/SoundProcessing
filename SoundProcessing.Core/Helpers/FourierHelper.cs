using System;
using System.Linq;
using System.Numerics;

namespace SoundProcessing.Core.Helpers
{
    public static class FourierHelper
    {
        public static void Conjugate(Complex[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Complex.Conjugate(data[i]);
            }
        }

        public static double[] ReduceToPow2(double[] data)
        {
            var length = (int)Math.Log2(data.Length);
            return data.Take((int)Math.Pow(2, length)).ToArray();
        }

        public static int GetExpandedPow2(int length)
        {
            return (int)Math.Pow(2, (int)Math.Log2(length) + 1);
        }

        public static double[] PreEmphasis(double[] data)
        {
            var result = new double[data.Length];

            result[0] = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                result[i] = data[i] - 0.9 * data[i - 1];
            }

            return result;
        }
    }
}
