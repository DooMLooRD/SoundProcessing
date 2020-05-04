using SoundProcessing.Core.Wav;
using SoundProcessing.View.ViewModels.Base;
using System.Collections.ObjectModel;

namespace SoundProcessing.View.ViewModels
{
    public class SoundPlayerViewModel
    {
        public ObservableCollection<WavViewModel> Sounds { get; set; }

        public SoundPlayerViewModel()
        {
            Sounds = new ObservableCollection<WavViewModel>();
        }

        public void AddSound(string name, WavData wavData)
        {
            var sound = new WavViewModel(name, wavData);
            Sounds.Add(sound);
            sound.RemoveSoundCommand = new RelayCommand(() => Sounds.Remove(sound));
        }
    }
}
