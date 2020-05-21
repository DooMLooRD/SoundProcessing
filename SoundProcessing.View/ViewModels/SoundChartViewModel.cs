using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Wpf;
using SoundProcessing.Core.Model;
using SoundProcessing.View.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SoundProcessing.View.ViewModels
{
    public class SoundChartViewModel : BaseViewModel
    {
        private Sound selectedSound;

        public PlotModel PlotModel { get; set; }

        public ObservableCollection<Sound> Sounds { get; set; }
        public Sound SelectedSound { get => selectedSound; set { selectedSound = value; IsDrawButtonEnabled = selectedSound != null; } }

        public bool IsDrawButtonEnabled { get; set; }

        public ICommand DrawCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DockWindowCommand { get; set; }

        public SoundChartViewModel()
        {
            DrawCommand = new RelayCommand(DrawPlot);
            SaveCommand = new RelayCommand(SavePlot);
            DockWindowCommand = new RelayCommand(DockWindow);
            PlotModel = new PlotModel();
        }

        public void DockWindow()
        {
            SoundChartViewModel vm = new SoundChartViewModel();
            vm.AddSounds(Sounds);
            SoundChartWindow window = new SoundChartWindow()
            {
                DataContext = vm
            };

            window.Show();
            vm.DrawPlot();
        }

        public void AddSounds(List<Sound> sounds, double[] original)
        {
            Sounds = new ObservableCollection<Sound>();
            Sounds.Add(new Sound { Result = original, StartTime = -1 });
            sounds.ForEach(c => Sounds.Add(c));
            SelectedSound = Sounds[0];
            DrawPlot();
        }

        public void AddSounds(IEnumerable<Sound> sounds)
        {
            Sounds = new ObservableCollection<Sound>(sounds);
            SelectedSound = Sounds[0];
        }

        public void SavePlot()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Image file ( *.png)| *.png",
                AddExtension = true,
                OverwritePrompt = true,
                RestoreDirectory = true,
                FileName = SelectedSound.ToString()
            };

            if (sfd.ShowDialog() == true)
            {
                var i = 0;
                var chartPoints = SelectedSound.Result.Select(c => new DataPoint(i++, c)).ToList();
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

                var pngExporter = new PngExporter { Width = 1920, Height = 1080, Background = OxyColors.White };
                pngExporter.ExportToFile(model, sfd.FileName);
            }
        }

        public void DrawPlot()
        {
            var i = 0;
            var chartPoints = SelectedSound.Result.Select(c => new DataPoint(i++, c)).ToList();
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

            PlotModel = model;
        }
    }
}
