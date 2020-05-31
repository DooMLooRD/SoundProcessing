using SoundProcessing.Core.Equalizer;
using SoundProcessing.Core.Filtration;
using SoundProcessing.Core.Fourier.Windows;
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
        public SoundChartViewModel SoundChartViewModel { get; }

        public WavViewModel SelectedSound { get => selectedSound; set { selectedSound = value; IsFindButtonEnabled = selectedSound != null; } }

        public bool IsFindButtonEnabled { get; set; }

        public FrequencyFinderViewModel(SoundGeneratorViewModel soundGeneratorViewModel, SoundPlayerViewModel soundPlayerViewModel, SoundChartViewModel soundChartViewModel)
        {
            SoundGeneratorViewModel = soundGeneratorViewModel;
            SoundPlayerViewModel = soundPlayerViewModel;
            SoundChartViewModel = soundChartViewModel;
            FrequencyFinders = new List<ICalculateFrequency>
            {
                new Autocorrelation(),
                new AMDFMethod(),
                new FourierMethod(new HanningWindow()),
            };

            SelectedFrequencyFinder = FrequencyFinders[0];
            FindFrequenciesCommand = new RelayCommand(FindFrequencies);
        }

        private void FindFrequencies()
        {
            //var result = SelectedFrequencyFinder.Calculate(SelectedSound.WavData);
            int l = 1025;
            int fc = 550;
            var filter1 = new TimeFiltration(new HanningWindow(), l, fc);
            var filter2 = new FrequencyFiltration(new HanningWindow(), 1800, 900, Core.Model.FilterType.Causal, l, fc, 4096);
            
            var result1 = filter1.FilterData(SelectedSound.WavData);
            var result2 = filter2.FilterData(SelectedSound.WavData);

            //SoundGeneratorViewModel.SetFrequencies(result);
            SoundChartViewModel.AddSounds(new List<Core.Model.Sound>
            {
                new Core.Model.Sound
                {

                    StartTime=0,
                    EndTime=1,
                    Frequency=1,
                    Result=result1
                },
                new Core.Model.Sound
                {

                    StartTime=1,
                    EndTime=2,
                    Frequency=2,
                    Result=result2
                }
            }, SelectedSound.WavData.Samples);
        }
    }
}
