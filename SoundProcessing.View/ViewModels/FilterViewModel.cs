using SoundProcessing.Core.Filtration;
using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.Model;
using SoundProcessing.View.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class FilterViewModel : BaseViewModel
    {
        private WavViewModel selectedSound;

        public IFourierWindow[] WindowTypes { get; set; }
        public IFourierWindow SelectedWindowType { get; set; }
        public ICommand FilterCommand { get; set; }
        public SoundPlayerViewModel SoundPlayerViewModel { get; set; }
        public FilterChartsViewModel FilterChartsViewModel { get; set; }

        public WavViewModel SelectedSound { get => selectedSound; set { selectedSound = value; IsButtonEnabled = selectedSound != null; } }
        public double CutFrequency { get; set; } = 550;
        public int FilterLength { get; set; } = 1025;
        public int WindowLength { get; set; } = 2049;
        public int WindowHopSize { get; set; } = 1024;
        public int? N { get; set; }
        public bool IsCausal { get; set; } = true;

        public bool IsButtonEnabled { get; set; }

        public FilterViewModel(SoundPlayerViewModel soundPlayerViewModel)
        {
            WindowTypes = new IFourierWindow[]
            {
                new RectangularWindow(),
                new HanningWindow(),
                new HammingWindow(),
            };

            SelectedWindowType = WindowTypes[0];

            FilterCommand = new RelayCommand(Filter);
            SoundPlayerViewModel = soundPlayerViewModel;
            FilterChartsViewModel = new FilterChartsViewModel(soundPlayerViewModel);
        }

        private void Filter()
        {
            var timeFilter = new TimeFiltration(new HanningWindow(), FilterLength, CutFrequency);
            var frequencyFilter = new FrequencyFiltration(new HanningWindow(), WindowLength, WindowHopSize, IsCausal ? FilterType.Causal : FilterType.NotCausal, FilterLength, CutFrequency, N);

            var timer = new Stopwatch();
            timer.Start();
            var timeResult = timeFilter.FilterData(SelectedSound.WavData);
            timer.Stop();
            var timeElapsed = timer.ElapsedMilliseconds;
            timer.Restart();
            var frequencyResult = frequencyFilter.FilterData(SelectedSound.WavData);
            timer.Stop();
            var frequencyElapsed = timer.ElapsedMilliseconds;

            FilterChartsViewModel.DrawPlots(SelectedSound.WavData.Samples, timeResult, timeElapsed, frequencyResult, frequencyElapsed);
        }
    }
}
