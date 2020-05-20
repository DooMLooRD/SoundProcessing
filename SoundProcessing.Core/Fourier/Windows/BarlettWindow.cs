using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundProcessing.Core.Fourier.Windows
{
    public class BarlettWindow : IFourierWindow
    {
        public double[] Windowing(double[] data)
        {
            var n = data.Length;

            for (int i = 0; i < data.Length; i++)
            {
                data[i] *= 2 / (n - 1) * ((n - 1) / 2 - Math.Abs(i - (n - 1) / 2));
            }

            return data;
        }
    }
}
