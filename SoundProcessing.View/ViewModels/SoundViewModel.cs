using SoundProcessing.Core.Model;
using SoundProcessing.View.ViewModels.Base;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class SoundViewModel : BaseViewModel
    {
        public int Time { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }

        public long Frequency { get; set; }
        public Sound Sound { get; set; }

        public ICommand RemoveFrequencyCommand { get; set; }
    }
}
