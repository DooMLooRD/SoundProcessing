﻿using System;

namespace SoundProcessing.Core.Fourier.Windows
{
    public class HammingWindow : IFourierWindow
    {
        public double[] WindowFactors(int m)
        {
            var result = new double[m];

            for (int i = 0; i < m; i++)
            {
                result[i] = 0.53836 - 0.46164 * Math.Cos(2 * Math.PI * i / (m - 1));
            }

            return result;
        }

        public double[] Windowing(double[] data)
        {
            var n = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= 0.53836 - 0.46164 * Math.Cos(2 * Math.PI * i / (n - 1));
            }

            return data;
        }

        public override string ToString()
        {
            return "Hamming Window";
        }
    }
}
