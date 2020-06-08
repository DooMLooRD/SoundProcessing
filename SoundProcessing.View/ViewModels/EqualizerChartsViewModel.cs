using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using SoundProcessing.Core.Wav;
using SoundProcessing.View.ViewModels.Base;
using System.Linq;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class EqualizerChartsViewModel : BaseViewModel
    {
        public PlotModel OriginalPlotModel { get; set; }
        public PlotModel EqualizedPlotModel { get; set; }
        public double[] Original { get; set; }
        public double[] Equalized { get; set; }

        public SoundPlayerViewModel SoundPlayerViewModel { get; set; }

        public ICommand GenerateOriginalCommand { get; set; }
        public ICommand SaveOriginalCommand { get; set; }

        public ICommand GenerateEqualizedCommand { get; set; }
        public ICommand SaveEqualizedCommand { get; set; }

        public EqualizerChartsViewModel(SoundPlayerViewModel soundPlayerViewModel)
        {
            SoundPlayerViewModel = soundPlayerViewModel;
            GenerateOriginalCommand = new RelayCommand(GenerateOriginal);
            SaveOriginalCommand = new RelayCommand(() => SavePlot("original", Original));

            GenerateEqualizedCommand = new RelayCommand(GenerateEqualized);
            SaveEqualizedCommand = new RelayCommand(() => SavePlot("equalized", Equalized));
        }

        public void DrawPlots(double[] original, double[] equalized)
        {
            DrawPlot(original, true);
            DrawPlot(equalized, false);
            Original = original;
            Equalized = equalized;
        }

        public void GenerateOriginal()
        {
            var wavData = new WavData(44100, Original);
            SoundPlayerViewModel.AddSound("Original", wavData);
        }

        public void GenerateEqualized()
        {
            var wavData = new WavData(44100, Equalized);
            SoundPlayerViewModel.AddSound("Equalized", wavData);
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


        public void DrawPlot(double[] data, bool isOriginal)
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
                Title = isOriginal ? "Original" : "Equalized",
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

            if (isOriginal)
            {
                OriginalPlotModel = model;
            }
            else
            {
                EqualizedPlotModel = model;
            }
        }
    }
}
