using GenericParsing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Forms.DataVisualization;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;



namespace WPF_FormatWorker
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //симуляция нажатия кнопки - на время отладки
            Button_Click_LoadCSV(but1, null);
        }

        //кнопка ОТКРЫТЬ CSV
        private void Button_Click_LoadCSV(object sender, RoutedEventArgs e)
        {
            //объект диалога открытия файла
            var f = new OpenFileDialog();
            //фильтр диалога открытия файла
            f.Filter = "Файлы CSV| *.CSV";
            //переменная имени файла
            string file = "";


            //вызов диалога открытия файла
            //ЗАКРЫТО НА ОТЛАДКУ if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK) file = f.FileName;
            
            //выгрузка данных на форму
            file = @"C:\Users\VS\Downloads\TOP500_202011.csv"; //убрать на релизе!
            if (file != "")
            {
            using (GenericParserAdapter parser = new GenericParserAdapter(file))
              {
                // Разделитель CSV
                parser.ColumnDelimiter = ';';
                // Первая строка - заголовок
                parser.FirstRowHasHeader = true;
                // парсинг CSV в объект таблицы
                DataTable dtComputers = parser.GetDataTable();
                // выгрузка объекта таблицы в датагрид
                dataGrid.ItemsSource = dtComputers.DefaultView;
              }
            }
            

        }

        private void Button_Click_CountryDiag(object sender, RoutedEventArgs e)
        {

            Window1 ChartViewer = new Window1();
            ChartViewer.Show();

            

        }
    }
}
