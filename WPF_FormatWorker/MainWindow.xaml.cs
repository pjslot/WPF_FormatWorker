using GenericParsing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Forms.DataVisualization;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Linq;

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
        public static DataTable dtCompPublic;
        //публичка для файла csv
        public string pubFile;

        public MainWindow()
        {
            InitializeComponent();
            //симуляция нажатия кнопки - на время отладки
            Button_Click_LoadCSV(but1, null);
            //Button_Click_CountryDiag(but2, null);

            cores = 0;
            power = 0;
            compName = "";
         //   sliderText.Text = slider.Value.ToString();
           
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
                pubFile = file;
            using (GenericParserAdapter parser = new GenericParserAdapter(file))
              {
                // Разделитель CSV
                parser.ColumnDelimiter = ';';
                // Первая строка - заголовок
                parser.FirstRowHasHeader = true;
                // парсинг CSV в объект таблицы
                DataTable dtComputers = parser.GetDataTable();
                //выкидываем копию наружу что б другие могли работать
                dtCompPublic = dtComputers;
                // выгрузка объекта таблицы в датагрид
                dataGrid.ItemsSource = dtComputers.DefaultView;
                    //активация кнопки отката
                    rollback.IsEnabled = true;
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
            //если что-то выбрано - рисуем
            if (compName != "")
            {
                Window1 ChartViewer = new Window1();
                ChartViewer.Show();
            } else
            //если ничего не выбрано то заставляем выбрать
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Вначале выберите строку с Суперкопьютером!",
                                          "Выбор не сделан",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Information);
            }
   
        }

        //реакция на прокрутку слайдера подрезки экспорта
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {    
            int sliderCount = (int)slider.Value;
            //прокидываю биндинг текстблока через резервное поле кнопки, напрямую почему то не получается, а биндинг слайдера через xaml даёт некрасивые double
            but1.Uid=sliderCount.ToString();    
           // sliderText.Text = "111";
        }


        //КНОПКА ВЫГРУЗКИ В XML
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //если табличка подгружена - выгружаем XML
            if (dtCompPublic != null)
            {    
                //подрезка таблицы перед экспортом
                int cutStart = (int)slider.Value;
                int cutStop = dtCompPublic.Rows.Count;
                int i = cutStart;              
                 while (dtCompPublic.Rows.Count > cutStart)
                {
                    DataRow dr = dtCompPublic.Rows[i];
                    dr.Delete();                
                }

                //выгружаем в XML
                dtCompPublic.TableName = "Supercomputers Top500";
                using (StreamWriter sw = new StreamWriter("data - XML EXPORT.xml"))
                {
                    dtCompPublic.WriteXml(sw);
                }
                MessageBoxResult result = System.Windows.MessageBox.Show("Файл XML экспортирован в корневую папку.",
                                         "Успешно!",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Information);
                //глушим слайдер 
                if (slider.Value < 500)
                {
                    slider.IsEnabled = false;
                    slider.Opacity = 0.5;
                }
            }
            else
            //если таблички нет то предлагаем подгрузить
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Вначале подгрузите CSV таблицу!",
                                          "CSV Таблица не подгружена",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Information);
            }
        }

        //КНОПКА ВЫГРУЗКИ В JSON
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //если табличка подгружена - выгружаем JSON
            if (dtCompPublic != null)
            {
                //подрезка таблицы перед экспортом
                int cutStart = (int)slider.Value;
                int cutStop = dtCompPublic.Rows.Count;
                int i = cutStart;
                while (dtCompPublic.Rows.Count > cutStart)
                {
                    DataRow dr = dtCompPublic.Rows[i];
                    dr.Delete();
                }

                //выгружаем в JSON
                string jsonString = JsonConvert.SerializeObject(dtCompPublic.AsEnumerable(),Formatting.Indented);

                    File.WriteAllText(@"data - JSON EXPORT.json", jsonString);

                MessageBoxResult result = System.Windows.MessageBox.Show("Файл JSON экспортирован в корневую папку.",
                                         "Успешно!",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Information);
                //глушим слайдер 
                if (slider.Value < 500)
                { 
                slider.IsEnabled = false;
                slider.Opacity = 0.5;
                }
            }
            else
            //если таблички нет то предлагаем подгрузить
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Вначале подгрузите CSV таблицу!",
                                          "CSV Таблица не подгружена",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Information);
            }
        }

        //КНОПКА ЭКСПОРТА ЭКСЕЛЬ
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //// создаём/открываем сеанс приложения с Excel
            //Excel.Application app = new Excel.Application();
            //// делаем окно Excel видимым
            //app.Visible = true;
            //// добавляем новую книгу
            //var wbk = app.Workbooks.Add();
            //// получаем доступ к первому листу книги
            //// нумерация от 1 (привет Visual Basic)
            //Excel.Worksheet sht = wbk.Worksheets[1];
            //// пишем в определенную ячейку
            //sht.Cells[1, 1].Value = "HEllo Excel";




            //var allCountry = (from super in dtCompPublic.AsEnumerable()
            //                  group super by super["Country"] into g
            //                  select new { Country = g.Key, Count = g.Count() }).ToList();
            //string[] selectedCountries = new string[] { "Russia", "Japan", "China",  "United States", "Germany", "India"  };
            //var selected = allCountry.Where(c =>
            //selectedCountries.Contains(c.Country)).ToList();

            //Excel.Application app = new Excel.Application();
            //app.Visible = true;
            //Excel.Workbook wbk = app.Workbooks.Add();
            //Excel.Worksheet sht = wbk.Worksheets[1];
            //sht.Name = "DataAndChart";
            //sht.Cells[1, 1].Value = "Страна";
            //sht.Cells[1, 2].Value = "Количество";
            //for (int i = 0; i < selected.Count; i++)
            //{
            //    sht.Cells[i + 2, 1].Value = selected[i].Country;
            //    sht.Cells[i + 2, 2].Value = selected[i].Count;
            //}
            //// Создаем гистограмму
            //Excel.Chart ch = app.Charts.Add();
            //ch.Location(Excel.XlChartLocation.xlLocationAsObject, "DataAndChart");
            //Excel.ChartObject chObj = sht.ChartObjects(1);
            //chObj.Chart.ChartTitle.Text = "Распределение суперкомпьютеров по странам";
            //// Тип гистограммы: круговая
            //chObj.Chart.ChartType = Excel.XlChartType.xlPie;




            // load excel, and create a new workbook
            var excelApp = new Excel.Application();
            excelApp.Visible = true;
            Excel.Workbook wbk = excelApp.Workbooks.Add();
            

            // single worksheet
            Excel.Worksheet workSheet = wbk.Worksheets[1];
            workSheet.Name = "Data Sheet";

            // column headings
            for (var i = 0; i < dtCompPublic.Columns.Count; i++)
            {
                workSheet.Cells[1, i + 1] = dtCompPublic.Columns[i].ColumnName;
            }

            // rows
            for (var i = 0; i < dtCompPublic.Rows.Count-490; i++)
            {
                // to do: format datetime values before printing
                for (var j = 0; j < dtCompPublic.Columns.Count; j++)
                {
                    workSheet.Cells[i + 2, j + 1] = dtCompPublic.Rows[i][j];
                }
            }

            //====================

            var allCountry = (from super in dtCompPublic.AsEnumerable()
                              group super by super["Country"] into g
                              select new { Country = g.Key, Count = g.Count() }).ToList();
            string[] selectedCountries = new string[] { "Russia", "Japan", "China", "United States", "Germany", "India" };
            var selected = allCountry.Where(c => selectedCountries.Contains(c.Country)).ToList();

        



        }

        //КНОПКА ОТКАТА ТАБЛИЦЫ
        private void rollback_Click(object sender, RoutedEventArgs e)
        {
          //заново подгружаем полную табличку
                using (GenericParserAdapter parser = new GenericParserAdapter(pubFile))
                {
                    // Разделитель CSV
                    parser.ColumnDelimiter = ';';
                    // Первая строка - заголовок
                    parser.FirstRowHasHeader = true;
                    // парсинг CSV в объект таблицы
                    DataTable dtComputers = parser.GetDataTable();
                    //выкидываем копию наружу что б другие могли работать
                    dtCompPublic = dtComputers;
                    // выгрузка объекта таблицы в датагрид
                    dataGrid.ItemsSource = dtComputers.DefaultView;
                    //активация кнопки отката
                    rollback.IsEnabled = true;
                }
            //активируем слайдер
            slider.IsEnabled = true;
            slider.Opacity = 1;
        }


    }
}


