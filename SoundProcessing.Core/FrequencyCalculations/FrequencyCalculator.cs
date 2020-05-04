using SoundProcessing.Core.Model;
using SoundProcessing.Core.Wav;
using System.Collections.Generic;

namespace SoundProcessing.Core.FrequencyCalculations
{
    public class FrequencyCalculator
    {
        public static List<Sound> GetFrequencies(WavData wavData, ICalculateFrequency processor)
        {
            return processor.Calculate(wavData);
        }
    }
}
