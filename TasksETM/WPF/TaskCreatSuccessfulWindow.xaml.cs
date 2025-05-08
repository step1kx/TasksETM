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
    /// Логика взаимодействия для TaskCreatSuccessfulWindow.xaml
    /// </summary>
    public partial class TaskCreatSuccessfulWindow : Window
    {
        public TaskCreatSuccessfulWindow()
        {
            InitializeComponent();
            UpdateProgressBarAsync();
        }


        public async Task UpdateProgressBarAsync()
        {
            ProgressBar.Maximum = 100;

            ProgressBar.Value = 25;
            UpdateText("Создаем задание...");
            await Task.Delay(1500);
            ProgressBar.Value = 75;
            UpdateText("Обновляем базу данных...");
            await Task.Delay(1500);
            ProgressBar.Value = 100;
            UpdateText("Задание успешно создано! Отображаем...");
            await Task.Delay(2000);
            
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
