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
                if (_dbConnection.Connected())
                {
                    MessageBox.Show("Все хорошо. Подключаемся...");
                    await Task.Delay(5000);
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

        
    }
}
