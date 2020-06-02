using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.WahWah;
using SoundProcessing.View.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
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
        public FilterChartsViewModel FilterChartsViewModel { get; set; }

        public WavViewModel SelectedSound { get => selectedSound; set { selectedSound = value; IsButtonEnabled = selectedSound != null; } }
        public int FilterLength { get; set; } = 127;
        public int WindowLength { get; set; } = 2049;
        public int WindowHopSize { get; set; } = 1024;

        public int StartBandFrequency { get; set; } = 0;
        public int EndBandFrequency { get; set; } = 10000;
        public int BandSize { get; set; } = 300;
        public double LFO { get; set; } = 1;
        public double Gain { get; set; } = 4;

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
            FilterChartsViewModel = new FilterChartsViewModel(soundPlayerViewModel);
        }

        private void Apply()
        {
            var wahWahFilter = new WahWah(new HanningWindow(), WindowLength, WindowHopSize, FilterLength, StartBandFrequency, EndBandFrequency, BandSize, LFO, Gain);
            var frequencyFilter = new FrequencyFiltration(new HanningWindow(), WindowLength, WindowHopSize, IsCausal ? FilterType.Causal : FilterType.NotCausal, FilterLength, CutFrequency, N);

            FilterChartsViewModel.DrawPlots(SelectedSound.WavData.Samples, timeResult, timeElapsed, frequencyResult, frequencyElapsed);
        }
    }
}
