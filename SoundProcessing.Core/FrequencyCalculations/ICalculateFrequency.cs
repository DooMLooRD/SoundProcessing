using SoundProcessing.Core.Model;
using SoundProcessing.Core.Wav;
using System.Collections.Generic;

namespace SoundProcessing.Core.FrequencyCalculations
{
    public interface ICalculateFrequency
    {
        List<Sound> Calculate(WavData wavData);
    }
}
