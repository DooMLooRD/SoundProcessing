using SoundProcessing.Core.FrequencyCalculations;
using SoundProcessing.View.ViewModels.Base;
using System.Collections.Generic;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class FrequencyFinderViewModel : BaseViewModel
    {
        private WavViewModel selectedSound;

        public List<ICalculateFrequency> FrequencyFinders { get; set; }
        public ICalculateFrequency SelectedFrequencyFinder { get; set; }
        public ICommand FindFrequenciesCommand { get; set; }
        public SoundGeneratorViewModel SoundGeneratorViewModel { get; set; }
        public SoundPlayerViewModel SoundPlayerViewModel { get; set; }
        public WavViewModel SelectedSound { get => selectedSound; set { selectedSound = value; IsFindButtonEnabled = selectedSound != null; } }

        public bool IsFindButtonEnabled { get; set; }

        public FrequencyFinderViewModel(SoundGeneratorViewModel soundGeneratorViewModel, SoundPlayerViewModel soundPlayerViewModel)
        {
            SoundGeneratorViewModel = soundGeneratorViewModel;
            SoundPlayerViewModel = soundPlayerViewModel;

            FrequencyFinders = new List<ICalculateFrequency>
            {
                new Autocorrelation(),
                new AMDFMethod(),
            };

            SelectedFrequencyFinder = FrequencyFinders[0];
            FindFrequenciesCommand = new RelayCommand(FindFrequencies);
        }

        private void FindFrequencies()
        {
            var result = SelectedFrequencyFinder.Calculate(SelectedSound.WavData);
            SoundGeneratorViewModel.SetFrequencies(result);
        }
    }
}
