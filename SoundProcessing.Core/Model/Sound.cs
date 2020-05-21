using System.Collections.Generic;

namespace SoundProcessing.Core.Model
{
    public class Sound
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public long Frequency { get; set; }
        public double[] Result { get; set; }

        public override string ToString()
        {
            if (StartTime == -1)
            {
                return $"Original";

            }
            return $"Window {StartTime} - {EndTime}";
        }
    }
}
