
using System;

namespace SoundProcessing.Core.Fourier.Windows
{
    public class HanningWindow : IFourierWindow
    {
        public double[] Windowing(double[] data)
        {
            var n = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= 0.5 * (1 - Math.Cos(2 * Math.PI * i / (n - 1)));
            }

            return data;
        }
    }
}
