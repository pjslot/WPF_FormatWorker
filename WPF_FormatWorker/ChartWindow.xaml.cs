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
            string[] axisXData = new string[] { $"Количество ядер {MainWindow.cores}", $"Общая производительность {MainWindow.power}" };
            int[] axisYData = new int[] { MainWindow.cores, MainWindow.power};           
            chart.Series["Series1"].Points.DataBindXY(axisXData, axisYData);

            //заголовок диаграммы
            chart.Titles.Add($"Отношение количества ядер суперкомпьютера {MainWindow.compName} к его общей производительности.");

            //создание легенды
            chart.Legends.Add(new Legend("Legend2"));

            //подвязка легенды
            chart.Series["Series1"].Legend = "Legend2";
            chart.Series["Series1"].IsVisibleInLegend = true;
        }

    }
}
