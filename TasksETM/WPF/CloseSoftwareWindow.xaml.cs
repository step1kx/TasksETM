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

namespace TasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для CloseSoftwareWindow.xaml
    /// </summary>
    public partial class CloseSoftwareWindow : Window
    {
        public CloseSoftwareWindow()
        {
            InitializeComponent();
            UpdateProgressBarAsync();
        }

        public async Task UpdateProgressBarAsync()
        {
            ProgressBar.Maximum = 100;

            ProgressBar.Value = 25;
            UpdateText("Отключаемся от базы");
            await Task.Delay(1000);
            ProgressBar.Value = 50;
            UpdateText("Сохраняем изменения");
            await Task.Delay(1000);
            ProgressBar.Value = 75;
            UpdateText("Анализируем данные");
            await Task.Delay(1000);
            ProgressBar.Value = 100;
            UpdateText("Завершаем работу");
            await Task.Delay(500);

            Close();
        }

        private void UpdateText(string text)
        {
            try
            {
                var textBlock = (TextBlock)FindName("TextBlock");
                textBlock.Text = text;
            }
            catch (Exception ex)  
            {
                MessageBox.Show(ex.Message);    
            }
            
        }
    }
}
