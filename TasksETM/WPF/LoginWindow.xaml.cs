using IssuingTasksETM.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TasksETM.Models;
using TasksETM.WPF;
using TasksETM.Service;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IDatabaseConnection _dbConnection;
        public LoginWindow(IDatabaseConnection dbConnection = null)
        {
            InitializeComponent();
            _dbConnection = dbConnection ?? new DatabaseConnection();
            FillComboBox();
        }

        private void FillComboBox()
        {
            _dbConnection.FillDepartmentName(LoginComboBox);
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

        

        private void ToChooseProjectButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginComboBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Пожалуйста, введите логин.", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите пароль.", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var db = new DatabaseConnection();
            bool success = db.LoginDepartment(login, password);

            if (success)
            {
                UserSession.Login = login;

                ChooseProjectWindow chooseProjectWindow = new ChooseProjectWindow(_dbConnection);
                chooseProjectWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void MovingWin(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); 
            }
        }


    }
}
