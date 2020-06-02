using SoundProcessing.Core.Equalizer;
using SoundProcessing.Core.Fourier.Windows;
using SoundProcessing.Core.Model;
using SoundProcessing.View.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class EqualizerViewModel : BaseViewModel
    {
        private WavViewModel selectedSound;

        public SoundPlayerViewModel SoundPlayerViewModel { get; set; }

        public WavViewModel SelectedSound { get => selectedSound; set { selectedSound = value; IsButtonEnabled = selectedSound != null; } }

        public bool IsButtonEnabled { get; set; }

        public EqualizerSliderViewModel[] Gains { get; set; }
        public EqualizerChartsViewModel ChartsViewModel { get; set; }

        public ICommand EqualizeCommand { get; set; }

        public EqualizerViewModel(SoundPlayerViewModel soundPlayerViewModel)
        {
            SoundPlayerViewModel = soundPlayerViewModel;
            EqualizeCommand = new RelayCommand(Equalize);
            ChartsViewModel = new EqualizerChartsViewModel(soundPlayerViewModel);
            Gains = new[]
            {
                new EqualizerSliderViewModel
                {
                    Text="20 - 40Hz",
                    Value = 0,
                },
                new EqualizerSliderViewModel
                {
                    Text="40 - 80Hz",
                    Value = 0,
                },
                new EqualizerSliderViewModel
                {
                    Text="80 - 160Hz",
                    Value = 0,
                },
                new EqualizerSliderViewModel
                {
                    Text="160 - 320Hz",
                    Value = 0,
                },
                new EqualizerSliderViewModel
                {
                    Text="320 - 640Hz",
                    Value = 0,
                },
                new EqualizerSliderViewModel
                {
                    Text="0.6 - 1.2kHz",
                    Value = 0,
                },
                new EqualizerSliderViewModel
                {
                    Text="1.2 - 2.5kHz",
                    Value = 0,
                },
                new EqualizerSliderViewModel
                {
                    Text="2.5 - 5kHz",
                    Value = 0,
                },
                new EqualizerSliderViewModel
                {
                    Text="5 - 10kHz",
                    Value = 0,
                },
                new EqualizerSliderViewModel
                {
                    Text="10 - 20kHz",
                    Value = 0,
                },
            };
        }

        private void Equalize()
        {
            var eqaulizer = new Equalizer(new HanningWindow(), 2049, 1024, 2049);

            var result = eqaulizer.Equalize(Gains.Select(c => c.Value).ToArray(), SelectedSound.WavData);
            ChartsViewModel.DrawPlots(SelectedSound.WavData.Samples, result);
        }
    }
}
