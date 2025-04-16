using IssuingTasksETM.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IDatabaseConnection _dbConnection;
        public LoginWindow(IDatabaseConnection dbConnection)
        {
            InitializeComponent();
            _dbConnection = dbConnection;
        }

        public async void CheckConnection(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Попытка подключения...");
                await Task.Delay(2000);
                if (_dbConnection.Connected())
                {
                    MessageBox.Show("Все хорошо. Подключаемся...");
                    await Task.Delay(3000);
                    MessageBox.Show("Подключение установлено!");
                }
                else
                {
                    MessageBox.Show("Не удалось установить подключение к базе данных. Повторите попытку.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void HipLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");

            fadeOut.Completed += (s, args) =>
            {
                DeptLoginGrid.Visibility = Visibility.Collapsed;
                HipLoginGrid.Visibility = Visibility.Visible;
                fadeIn.Begin(HipLoginGrid);
            };

            fadeOut.Begin(DeptLoginGrid);
        }

        private void DeptLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");

            fadeOut.Completed += (s, args) =>
            {
                HipLoginGrid.Visibility = Visibility.Collapsed;
                DeptLoginGrid.Visibility = Visibility.Visible;
                fadeIn.Begin(DeptLoginGrid);
            };

            fadeOut.Begin(HipLoginGrid);
        }

        private void ToTaskLoginButton_Click(Object sender, RoutedEventArgs e)
        {
            MessageBox.Show("КРУТО!");
            return;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
