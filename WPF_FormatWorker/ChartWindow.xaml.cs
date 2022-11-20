using System;
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
using System.Windows.Forms.DataVisualization.Charting;

namespace WPF_FormatWorker
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            // Создаем область построения ChartArea
            chart.ChartAreas.Add(new ChartArea("defaultArea"));
            // Добавляем ряд данных "defaultArea"
            chart.Series.Add(new Series("Series1"));
            chart.Series["Series1"].ChartArea = "defaultArea";
            chart.Series["Series1"].ChartType = SeriesChartType.Pie;
            // добавляем данные
            int[] axisXData = new int[] { 1, 2, 3, 4, 5 };
            int[] axisYData = new int[] { 56, 34, 15, 11, 5 };
            chart.Series["Series1"].Points.DataBindXY(axisXData, axisYData);


        }


    }
}
