using Microsoft.Win32;
using SoundProcessing.Core;
using SoundProcessing.View.ViewModels.Base;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public ICommand Open { get; set; }

        public SoundPlayerViewModel SoundPlayerViewModel { get; set; }
        public SoundGeneratorViewModel SoundGeneratorViewModel { get; set; }
        public FrequencyFinderViewModel FrequencyFinderViewModel { get; set; }
        public MainWindowViewModel()
        {
            SoundPlayerViewModel = new SoundPlayerViewModel();
            SoundGeneratorViewModel = new SoundGeneratorViewModel(SoundPlayerViewModel);
            FrequencyFinderViewModel = new FrequencyFinderViewModel(SoundGeneratorViewModel, SoundPlayerViewModel);
            Open = new RelayCommand(Load);
        }

        private void Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Sound files (*.wav)|*.wav",
                RestoreDirectory = true,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    var data = WavReader.ReadData(openFileDialog.FileNames[i]);
                    SoundPlayerViewModel.AddSound(openFileDialog.SafeFileNames[i], data);
                }
            }
        }
    }
}
