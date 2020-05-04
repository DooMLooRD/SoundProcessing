using SoundProcessing.Core.Model;
using SoundProcessing.Core.Wav;
using SoundProcessing.View.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class SoundGeneratorViewModel : BaseViewModel
    {
        private int sampleRate = 44100;
        private string name;

        public int Time { get; set; } = 1000;
        public long Frequency { get; set; } = 250;

        public ICommand AddFrequencyCommand { get; set; }

        public ObservableCollection<SoundViewModel> Frequencies { get; set; }

        public string Name { get => name; set { name = value; SetGenerateButton(); } }
        public int SampleRate { get => sampleRate; set { sampleRate = value; SetGenerateButton(); } }

        public bool IsGenerateButtonEnabled { get; set; }

        public ICommand GenerateSoundCommand { get; set; }
        public SoundPlayerViewModel SoundPlayerViewModel { get; }

        public SoundGeneratorViewModel(SoundPlayerViewModel soundPlayerViewModel)
        {
            SoundPlayerViewModel = soundPlayerViewModel;
            AddFrequencyCommand = new RelayCommand(AddFrequency);
            GenerateSoundCommand = new RelayCommand(GenerateSound);

            Frequencies = new ObservableCollection<SoundViewModel>();
            Frequencies.CollectionChanged += Frequencies_CollectionChanged;
        }

        public void SetGenerateButton()
        {
            IsGenerateButtonEnabled = Frequencies.Any() && !string.IsNullOrEmpty(Name) && SampleRate > 0;
        }

        private void Frequencies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateFrequencies();
            SetGenerateButton();
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateFrequencies();
        }

        private void UpdateFrequencies()
        {
            var currentTime = 0;
            foreach (var frequency in Frequencies)
            {
                frequency.StartTime = currentTime;
                currentTime += frequency.Time;
                frequency.EndTime = currentTime;
            }
        }

        private void GenerateSound()
        {
            var frequencies = Frequencies.Select(c => new Sound
            {
                Frequency = c.Frequency,
                StartTime = c.StartTime,
                EndTime = c.EndTime
            }).ToList();

            var wavData = new WavData(SampleRate, frequencies);
            SoundPlayerViewModel.AddSound(Name, wavData);
        }

        public void SetFrequencies(List<Sound> frequencies)
        {
            Frequencies = new ObservableCollection<SoundViewModel>();
            foreach (var frequency in frequencies)
            {
                var startTime = Frequencies.Any() ? Frequencies.Last().EndTime : 0;
                var sound = new SoundViewModel
                {
                    Time = frequency.EndTime - frequency.StartTime,
                    Frequency = frequency.Frequency,
                    StartTime = frequency.StartTime,
                    EndTime = frequency.EndTime
                };

                Frequencies.Add(sound);
                sound.RemoveFrequencyCommand = new RelayCommand(() => Frequencies.Remove(sound));
            }
        }

        private void AddFrequency()
        {
            if (Frequencies.Any() && Frequencies.Last().Frequency == Frequency)
            {
                Frequencies.Last().Time += Time;
                Frequencies.Last().EndTime += Time;
            }
            else
            {
                var startTime = Frequencies.Any() ? Frequencies.Last().EndTime : 0;
                var sound = new SoundViewModel
                {
                    Time = Time,
                    Frequency = Frequency,
                    StartTime = startTime,
                    EndTime = startTime + Time
                };

                Frequencies.Add(sound);
                sound.RemoveFrequencyCommand = new RelayCommand(() => Frequencies.Remove(sound));
            }
        }
    }
}
