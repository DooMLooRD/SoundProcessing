using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using SoundProcessing.Core.Wav;
using SoundProcessing.View.ViewModels.Base;
using System.Linq;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class FilterChartsViewModel : BaseViewModel
    {
        public PlotModel OriginalPlotModel { get; set; }
        public PlotModel TimePlotModel { get; set; }
        public PlotModel FrequencyPlotModel { get; set; }

        public double[] Original { get; set; }
        public double[] Time { get; set; }
        public double[] Frequency { get; set; }

        public string TimeCalculationTime { get; set; }
        public string FrequencyCalculationTime { get; set; }

        public SoundPlayerViewModel SoundPlayerViewModel { get; set; }

        public ICommand GenerateOriginalCommand { get; set; }
        public ICommand SaveOriginalCommand { get; set; }

        public ICommand GenerateTimeCommand { get; set; }
        public ICommand SaveTimeCommand { get; set; }

        public ICommand GenerateFrequencyCommand { get; set; }
        public ICommand SaveFrequencyCommand { get; set; }

        public FilterChartsViewModel(SoundPlayerViewModel soundPlayerViewModel)
        {
            SoundPlayerViewModel = soundPlayerViewModel;
            GenerateOriginalCommand = new RelayCommand(GenerateOriginal);
            SaveOriginalCommand = new RelayCommand(() => SavePlot("original", Original));

            GenerateTimeCommand = new RelayCommand(GenerateTime);
            SaveTimeCommand = new RelayCommand(() => SavePlot("time", Time));

            GenerateFrequencyCommand = new RelayCommand(GenerateFrequency);
            SaveFrequencyCommand = new RelayCommand(() => SavePlot("frequency", Frequency));
        }

        public void DrawPlots(double[] original, double[] time, double timeTime, double[] frequency, double frequencyTime)
        {
            DrawPlot(original, 0);
            DrawPlot(time, 1);
            DrawPlot(frequency, 2);

            TimeCalculationTime = $"Calculation time: {timeTime}";
            FrequencyCalculationTime = $"Calculation time: {frequencyTime}";

            Original = original;
            Time = time;
            Frequency = frequency;
        }

        public void GenerateOriginal()
        {
            var wavData = new WavData(44100, Original);
            SoundPlayerViewModel.AddSound("Original", wavData);
        }

        public void GenerateTime()
        {
            var wavData = new WavData(44100, Time);
            SoundPlayerViewModel.AddSound("TimeFiltered", wavData);
        }

        public void GenerateFrequency()
        {
            var wavData = new WavData(44100, Frequency);
            SoundPlayerViewModel.AddSound("FrequencyFiltered", wavData);
        }

        public void SavePlot(string title, double[] data)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Image file ( *.png)| *.png",
                AddExtension = true,
                OverwritePrompt = true,
                RestoreDirectory = true,
                FileName = title
            };

            if (sfd.ShowDialog() == true)
            {
                var i = 0;
                var chartPoints = data.Select(c => new DataPoint(i++, c)).ToList();
                var series = new OxyPlot.Series.LineSeries()
                {
                    Color = OxyColor.Parse("#673ab7"),
                };
                var xAxis = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Bottom,
                };

                var yAxis = new OxyPlot.Axes.LinearAxis()
                {
                    Position = OxyPlot.Axes.AxisPosition.Left,
                };

                series.Points.AddRange(chartPoints);
                var model = new PlotModel();

                model.Series.Add(series);
                model.Axes.Add(xAxis);
                model.Axes.Add(yAxis);

                var pngExporter = new PngExporter { Width = 1920, Height = 270, Background = OxyColors.White };
                pngExporter.ExportToFile(model, sfd.FileName);
            }
        }

        public void DrawPlot(double[] data, int chart)
        {
            var i = 0;
            var chartPoints = data.Select(c => new DataPoint(i++, c)).ToList();
            var series = new OxyPlot.Series.LineSeries()
            {
                Color = OxyColor.Parse("#673ab7"),
            };
            var xAxis = new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                TextColor = OxyColor.Parse("#bdbdbd"),
                AxislineColor = OxyColor.Parse("#bdbdbd"),
                TicklineColor = OxyColor.Parse("#bdbdbd"),
            };

            var yAxis = new OxyPlot.Axes.LinearAxis()
            {
                TitleColor = OxyColor.Parse("#bdbdbd"),
                Position = OxyPlot.Axes.AxisPosition.Left,
                TextColor = OxyColor.Parse("#bdbdbd"),
                AxislineColor = OxyColor.Parse("#bdbdbd"),
                TicklineColor = OxyColor.Parse("#bdbdbd"),
            };

            series.Points.AddRange(chartPoints);
            var model = new PlotModel()
            {
                PlotAreaBorderColor = OxyColor.Parse("#bdbdbd"),
            };

            model.Series.Add(series);
            model.Axes.Add(xAxis);
            model.Axes.Add(yAxis);

            if (chart == 0)
            {
                OriginalPlotModel = model;
            }
            else if (chart == 1)
            {
                TimePlotModel = model;
            }
            else
            {
                FrequencyPlotModel = model;
            }
        }
    }
}