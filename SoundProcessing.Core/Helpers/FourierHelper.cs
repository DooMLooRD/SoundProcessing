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
    }
}
