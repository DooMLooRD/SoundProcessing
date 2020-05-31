using SoundProcessing.Core.Helpers;
using System;
using System.Linq;
using System.Numerics;

namespace SoundProcessing.Core.Fourier
{
    public class FourierTransform
    {

        public static Complex[] DFT(double[] points)
        {
            var transformed = new Complex[points.Length];
            int N = points.Length;
            for (int i = 0; i < N; i++)
            {
                Complex transformedValue = Complex.Zero;
                for (int j = 0; j < N; j++)
                {
                    transformedValue += points[j] * Complex.Exp(new Complex(0, -2 * Math.PI * i * j / N));
                }
                transformed[i] = transformedValue / N;
            }
            return transformed;
        }
        public static double[] IDFT(Complex[] points)
        {
            var transformed = new double[points.Length];

            int N = points.Length;
            for (int i = 0; i < N; i++)
            {
                double transformedValue = 0;
                for (int j = 0; j < N; j++)
                {
                    transformedValue += (points[j] * Complex.Exp(new Complex(0, 2 * Math.PI * i * j / N))).Real;
                }
                transformed[i] = transformedValue;
            }
            return transformed;
        }

        public static Complex[] FFT(double[] data)
        {
            var complexData = data.Select(c => (Complex)c).ToArray();

            FFT1D(complexData);

            return complexData;
        }

        public static double[] IFFT(Complex[] complexData)
        {
            FourierHelper.Conjugate(complexData);
            FFT1D(complexData);
            FourierHelper.Conjugate(complexData);

            return complexData.Select(c => (c / complexData.Length).Real).ToArray();
        }

        private static void FFT1D(Complex[] buffer)
        {
            int bits = (int)Math.Log(buffer.Length, 2);

            for (int j = 1; j < buffer.Length; j++)
            {
                int swapPos = BitReverse(j, bits);
                if (swapPos <= j)
                {
                    continue;
                }
                var temp = buffer[j];
                buffer[j] = buffer[swapPos];
                buffer[swapPos] = temp;
            }


            for (int N = 2; N <= buffer.Length; N <<= 1)
            {
                for (int i = 0; i < buffer.Length; i += N)
                {
                    for (int k = 0; k < N / 2; k++)
                    {
                        int evenIndex = i + k;
                        int oddIndex = i + k + (N / 2);
                        var even = buffer[evenIndex];
                        var odd = buffer[oddIndex];

                        double term = -2 * Math.PI * k / N;
                        Complex exp = new Complex(Math.Cos(term), Math.Sin(term)) * odd;

                        buffer[evenIndex] = even + exp;
                        buffer[oddIndex] = even - exp;
                    }
                }
            }
        }

        private static int BitReverse(int n, int bits)
        {
            int reversedN = n;
            int count = bits - 1;

            n >>= 1;
            while (n > 0)
            {
                reversedN = (reversedN << 1) | (n & 1);
                count--;
                n >>= 1;
            }

            return (reversedN << count) & ((1 << bits) - 1);
        }
    }
}
