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
        //публички для диаграммы
        public static int cores, power;
        public static string compName;

        public MainWindow()
        {
            InitializeComponent();
            //подсвечиваем первую строку для подстраховки
            //dataGrid.Focus();
            //симуляция нажатия кнопки - на время отладки
            Button_Click_LoadCSV(but1, null);
            // Button_Click_CountryDiag(but2, null);

            cores = 0;
            power = 0;
            compName = "";
        }

        //КНОПКА ОТКРЫТЬ CSV
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
            file = @"TOP500_202011.csv"; //убрать на релизе!
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

        

        //ОБРАБОТКА ВЫДЕЛЕННОЙ СТРОКИ
    
        private void dataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //int currentRowIndex = dataGrid.SelectedIndex;  //отладка
            // mainWin.Title = currentRowIndex.ToString();   //отладка
            //mainWin.Title = row["Name"].ToString();        //отладка


            //получаем текущую строку
            DataRowView row = (DataRowView)dataGrid.SelectedItems[0];            

            //получаем значение количества ядер
            cores = int.Parse(row["Total Cores"].ToString().Replace(" ", ""));

            //получаем значение общей производительности
            power = int.Parse(row["Nmax"].ToString().Replace(" ", ""));

            //получаем имя компьютера
            compName = row["Name"].ToString();
        }



        //КНОПКА ОТРИСОВКИ ДИАГРАММЫ
        private void Button_Click_CountryDiag(object sender, RoutedEventArgs e)
        {

            Window1 ChartViewer = new Window1();
            ChartViewer.Show();
   
        }

       
    }
}
