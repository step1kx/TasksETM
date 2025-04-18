using IssuingTasksETM.Interfaces;
using IssuingTasksETM.Models;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TasksETM.Models;
using TasksETM.WPF;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        private readonly TaskWindow _taskWindow;
        private readonly IDatabaseConnection _dbConnection;

        public static string loggedInUser = UserSession.Login;
        public CreateTaskWindow(string selectedProject, IDatabaseConnection dbConnection)
        {
            InitializeComponent();
            _taskWindow = new TaskWindow(selectedProject, dbConnection);
            _dbConnection = new DatabaseConnection();
            FillComboBox();
        }

        private void FillComboBox()
        {
            try
            {
                if (string.IsNullOrEmpty(loggedInUser))
                {
                    MessageBox.Show("Логин не был инициализирован.");
                    return;
                }

                ToDepartComboBox.Text = loggedInUser;  // Устанавливаем логин в TextBox

                if (_dbConnection == null)
                {
                    MessageBox.Show("Соединение с базой данных не инициализировано.");
                    return;
                }

                _dbConnection.FillDepartmentName(ToDepartComboBox);  // Заполняем ComboBox с данными
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Чето не пошло: {ex.Message}");
            }
        }


        private void TaskInfoShowButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");

            fadeOut.Completed += (s, args) =>
            {
                BaseInfoGrid.Visibility = Visibility.Collapsed;
                TasksInfoGrid.Visibility = Visibility.Visible;
                fadeIn.Begin(TasksInfoGrid);
            };

            fadeOut.Begin(BaseInfoGrid);
        }

        private void BaseInfoShowButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");

            fadeOut.Completed += (s, args) =>
            {
                TasksInfoGrid.Visibility = Visibility.Collapsed;
                BaseInfoGrid.Visibility = Visibility.Visible;
                fadeIn.Begin(BaseInfoGrid);
            };

            fadeOut.Begin(TasksInfoGrid);
        }

        public void MovingWin(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void LoadScreenshotButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateTaskButton_Click(object sender, RoutedEventArgs e) 
        {

        }

        private void ToPrevWindow_Click(object sender, RoutedEventArgs e)
        {
            _taskWindow.Show();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
