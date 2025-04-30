using IssuingTasksETM.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TasksETM.Models;
using TasksETM.WPF;
using TasksETM.Service;
using TasksETM.Interfaces;
using TasksETM.Interfaces.ITasks;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IAuthService _authService;
        private readonly IProjectService _projectService;
        private readonly IDepartmentService _departmentService;
        private readonly IDatabaseConnection _dbConnection;
        private readonly IFilterTasksService _filterTasksService;

        public LoginWindow(
            IDatabaseConnection dbConnection,
            IDepartmentService departmentService,
            IProjectService projectService,
            IAuthService authService,
            IFilterTasksService filterTasksService)
        {
            InitializeComponent();

            _dbConnection = dbConnection ?? new DatabaseConnection();
            _departmentService = departmentService;
            _projectService = projectService;
            _authService = authService;
            _filterTasksService = filterTasksService;
            FillComboBoxAsync();
        }

        private async void FillComboBoxAsync()
        {
            try
            {
                var departments = await _departmentService.GetDepartmentNamesAsync();

                Dispatcher.Invoke(() =>
                {
                    LoginComboBox.Items.Clear();
                    foreach (var dep in departments)
                    {
                        LoginComboBox.Items.Add(dep);
                    }

                    if (LoginComboBox.Items.Count > 0)
                        LoginComboBox.SelectedIndex = 0;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке логинов: {ex.Message}");
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



        private async void ToChooseProjectButton_Click(object sender, RoutedEventArgs e)
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

            try
            {
                bool success = await _authService.LoginAsync(login, password);

                if (success)
                {
                    UserSession.Login = login;

                    var chooseProjectWindow = new ChooseProjectWindow(_dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
                    chooseProjectWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
