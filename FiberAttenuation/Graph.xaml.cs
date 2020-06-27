using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FiberAttenuation {
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : Window {
        private ArrayList loss;
        private PlotView plot;
        private ArrayList wavelengths;
        private ArrayList markers;
        public Graph(ArrayList loss, ArrayList wavelengths, ArrayList markers) {
            InitializeComponent();
            this.DataContext = this;
            this.loss = loss;
            this.wavelengths = wavelengths;
            this.markers = markers;
            PlotGraph();
        }

        private void PlotGraph() {
            PlotData = new PlotModel();
            var yAxis = new OxyPlot.Axes.LinearAxis {
                Position = AxisPosition.Left,
                Title = "Fiber Attenuation",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            var xAxis = new OxyPlot.Axes.LinearAxis {
                Position = AxisPosition.Bottom,
                Title = "Wavelengths (nm)",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            PlotData.Axes.Add(xAxis);
            PlotData.Axes.Add(yAxis);
            DataPoint[] linePoints = new DataPoint[loss.Count];
            ScatterPoint[] markerPoints = new ScatterPoint[loss.Count];
            for (int i = 0; i < loss.Count; i++) {
                if (markers.Contains(wavelengths[i])) {
                    markerPoints[i] = new ScatterPoint((double)wavelengths[i], (double)loss[i]);
                } else {
                    markerPoints[i] = new ScatterPoint((double)wavelengths[i], 0);
                }
                linePoints[i] = new DataPoint((double)wavelengths[i], (double)loss[i]);
             
            }




        var lineSeries = new OxyPlot.Series.LineSeries {
            StrokeThickness = 2,
            ItemsSource = linePoints
        };
            var circleSeries = new OxyPlot.Series.ScatterSeries {
                MarkerSize = 3,
                MarkerType = MarkerType.Circle
            }; // W850 TO W1550 S10;
            circleSeries.Points.AddRange(markerPoints.Where(i => i.Y > 0));
            PlotData.Series.Add(lineSeries);
            PlotData.Series.Add(circleSeries);
            PlotData.InvalidatePlot(true);

        }
        public PlotModel PlotData { get; set; }
    }
}
