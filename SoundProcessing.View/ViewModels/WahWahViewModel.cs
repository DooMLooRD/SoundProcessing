using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.WahWah;
using SoundProcessing.View.ViewModels.Base;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class WahWahViewModel : BaseViewModel
    {
        private WavViewModel selectedSound;

        public IFourierWindow[] WindowTypes { get; set; }
        public IFourierWindow SelectedWindowType { get; set; }
        public ICommand ApplyCommand { get; set; }

        public SoundPlayerViewModel SoundPlayerViewModel { get; set; }
        public WahWahChartsViewModel WahWahChartsViewModel { get; set; }

        public WavViewModel SelectedSound { get => selectedSound; set { selectedSound = value; IsButtonEnabled = selectedSound != null; } }
        public int FilterLength { get; set; } = 259;
        public int WindowLength { get; set; } = 1025;
        public int WindowHopSize { get; set; } = 513;

        public int StartBandFrequency { get; set; } = 0;
        public int EndBandFrequency { get; set; } = 2000;
        public int BandSize { get; set; } = 200;
        public double LFO { get; set; } = 1;
        public double Gain { get; set; } = 6;

        public bool IsButtonEnabled { get; set; }

        public WahWahViewModel(SoundPlayerViewModel soundPlayerViewModel)
        {
            WindowTypes = new IFourierWindow[]
            {
                new HanningWindow(),
                new RectangularWindow(),
                new HammingWindow(),
            };

            SelectedWindowType = WindowTypes[0];

            ApplyCommand = new RelayCommand(Apply);
            SoundPlayerViewModel = soundPlayerViewModel;
            WahWahChartsViewModel = new WahWahChartsViewModel(soundPlayerViewModel);
        }

        private void Apply()
        {
            var wahWahFilter = new WahWah(new HanningWindow(), WindowLength, WindowHopSize, FilterLength, StartBandFrequency, EndBandFrequency, BandSize, LFO, Gain);

            var result = wahWahFilter.Process(SelectedSound.WavData);

            WahWahChartsViewModel.DrawPlots(SelectedSound.WavData.Samples, result);
        }
    }
}
