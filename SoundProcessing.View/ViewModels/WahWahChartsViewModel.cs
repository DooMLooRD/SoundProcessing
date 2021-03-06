﻿using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using SoundProcessing.Core.Wav;
using SoundProcessing.View.ViewModels.Base;
using System.Linq;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class WahWahChartsViewModel : BaseViewModel
    {
        public PlotModel OriginalPlotModel { get; set; }
        public PlotModel WahWahPlotModel { get; set; }

        public double[] Original { get; set; }
        public double[] WahWah { get; set; }

        public SoundPlayerViewModel SoundPlayerViewModel { get; set; }

        public ICommand GenerateOriginalCommand { get; set; }
        public ICommand SaveOriginalCommand { get; set; }

        public ICommand GenerateWahWahCommand { get; set; }
        public ICommand SaveWahWahCommand { get; set; }

        public WahWahChartsViewModel(SoundPlayerViewModel soundPlayerViewModel)
        {
            SoundPlayerViewModel = soundPlayerViewModel;
            GenerateOriginalCommand = new RelayCommand(GenerateOriginal);
            SaveOriginalCommand = new RelayCommand(() => SavePlot("original", Original));

            GenerateWahWahCommand = new RelayCommand(GenerateWahWah);
            SaveWahWahCommand = new RelayCommand(() => SavePlot("wahwah", WahWah));
        }

        public void DrawPlots(double[] original, double[] wahWah)
        {
            DrawPlot(original, 0);
            DrawPlot(wahWah, 1);

            Original = original;
            WahWah = wahWah;
        }

        public void GenerateOriginal()
        {
            var wavData = new WavData(44100, Original);
            SoundPlayerViewModel.AddSound("Original", wavData);
        }

        public void GenerateWahWah()
        {
            var wavData = new WavData(44100, WahWah);
            SoundPlayerViewModel.AddSound("TimeFiltered", wavData);
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
                WahWahPlotModel = model;
            }
        }
    }
}
