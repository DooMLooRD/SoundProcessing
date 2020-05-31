using Microsoft.Win32;
using SoundProcessing.Core.Wav;
using SoundProcessing.View.ViewModels.Base;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class WavViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public int Duration { get; set; }

        public WavData WavData { get; set; }
        public byte[] WavByteData { get; set; }

        public ICommand PlaySoundCommand { get; set; }
        public ICommand RemoveSoundCommand { get; set; }
        public ICommand SaveSoundCommand { get; set; }

        public WavViewModel(string name, WavData wavData)
        {
            Name = name;
            Duration = wavData.NumberOfChunks * 50;
            WavData = wavData;
            WavByteData = wavData.GetByteData();

            PlaySoundCommand = new RelayCommand(PlaySound);
            SaveSoundCommand = new RelayCommand(SaveSound);
        }

        private void SaveSound()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Sound file ( *.wav)| *.wav",
                AddExtension = true,
                OverwritePrompt = true,
                RestoreDirectory = true,
                FileName = Name
            };

            if (sfd.ShowDialog() == true)
            {
                WavWriter.WriteData(sfd.FileName, WavData);
            }
        }

        private async void PlaySound()
        {
            await Task.Run(() =>
            {
                using (MemoryStream ms = new MemoryStream(WavByteData))
                {
                    SoundPlayer player = new SoundPlayer(ms);
                    player.Play();
                }
            });
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
