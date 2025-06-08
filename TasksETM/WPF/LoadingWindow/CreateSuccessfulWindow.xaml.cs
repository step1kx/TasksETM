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

namespace TasksETM.WPF.LoadingWindow
{
    /// <summary>
    /// Логика взаимодействия для TaskCreatSuccessfulWindow.xaml
    /// </summary>
    public partial class CreateSuccessfulWindow : Window
    {
        private bool projectOrTask = false;

        public CreateSuccessfulWindow()
        {
            InitializeComponent();
            UpdateProgressBarAsync(projectOrTask);
        }


        public async Task UpdateProgressBarAsync(bool projectOrTask)
        {
            ProgressBar.Maximum = 100;

            if (projectOrTask)
            {

                ProgressBar.Value = 25;
                UpdateText("Создаем проект...");
                await Task.Delay(1500);
                ProgressBar.Value = 75;
                UpdateText("Обновляем базу данных...");
                await Task.Delay(1500);
                ProgressBar.Value = 100;
                UpdateText("Проект успешно создан!");
                await Task.Delay(2000);
            }
            else
            {
                ProgressBar.Value = 25;
                UpdateText("Создаем задание...");
                await Task.Delay(1500);
                ProgressBar.Value = 75;
                UpdateText("Обновляем базу данных...");
                await Task.Delay(1500);
                ProgressBar.Value = 100;
                UpdateText("Задание успешно создано! Отображаем...");
                await Task.Delay(2000);
            }
            
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
