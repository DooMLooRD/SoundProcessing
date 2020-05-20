using System;
using System.Collections.Generic;
using System.Text;

namespace SoundProcessing.Core.Fourier.Windows
{
    public class HammingWindow : IFourierWindow
    {
        public double[] Windowing(double[] data)
        {
            var n = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= 0.53836 - 0.46164 * Math.Cos(2 * Math.PI * i / (n - 1));
            }

            return data;
        }
    }
}
