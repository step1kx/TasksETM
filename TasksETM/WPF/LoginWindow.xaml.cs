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
using TasksETMCommon.Models;
using TasksETMCommon.Helpers;
using Windows.Devices.Sensors;
using TasksETM.WPF.HelpingWindow;

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
        private bool _isHipLogin;
        public string loginForNotify = string.Empty;
        public string loginForDeaprtments = string.Empty;

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
            _isHipLogin = false;

            CheckSavedLoginAsync();

            SharedLoginStorage.SaveLogin(loginForNotify);

            UserSessionForNotify.Login = string.Empty;

            FillComboBoxAsync();

            
        }

        private async void CheckSavedLoginAsync()
        {
            try
            {
                if (_authService == null || _dbConnection == null || _departmentService == null ||
                    _projectService == null || _filterTasksService == null)
                {
                    MessageBox.Show("Одна или несколько служб не инициализированы. Перезапустите приложение.");
                    return;
                }

                if (TasksETM.Properties.Settings.Default.RememberMe && !string.IsNullOrEmpty(TasksETM.Properties.Settings.Default.SavedLogin))
                {
                    System.Diagnostics.Debug.WriteLine($"Проверка сохранённого логина: {TasksETM.Properties.Settings.Default.SavedLogin}");
                    bool isValid = await _authService.CheckSavedLoginAsync(TasksETM.Properties.Settings.Default.SavedLogin);
                    if (isValid)
                    {
                        UserSessionForNotify.Login = TasksETM.Properties.Settings.Default.SavedLogin;

                        UserSession.Login = TasksETM.Properties.Settings.Default.SavedDepartmentLogin;

                        SharedLoginStorage.SaveDepartmentLogin(UserSession.Login);

                        SharedLoginStorage.SaveLogin(UserSessionForNotify.Login);


                        System.Diagnostics.Debug.WriteLine($"Логин валиден, открываем ChooseProjectWindow для {UserSession.Login}");

                        Dispatcher.Invoke(() =>
                        {
                            var chooseProjectWindow = new ChooseProjectWindow(
                                _dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
                            chooseProjectWindow.Show();
                            Close();
                        });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Сохранённый логин невалиден, сбрасываем настройки");
                        TasksETM.Properties.Settings.Default.SavedDepartmentLogin = string.Empty;
                        TasksETM.Properties.Settings.Default.SavedLogin = string.Empty;
                        TasksETM.Properties.Settings.Default.RememberMe = false;
                        TasksETM.Properties.Settings.Default.Save();

                        UserSessionForNotify.Login = string.Empty;

                        SharedLoginStorage.SaveLogin(loginForNotify);

                        //SharedLoginStorage.SaveDepartmentLogin(null);

                        MessageBox.Show("Сохранённая сессия недействительна. Пожалуйста, войдите заново.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при проверке сохраненного логина: {ex.Message}");
                MessageBox.Show($"Ошибка при проверке сохраненного логина: {ex.Message}");
            }
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
                        if (dep == "Все отделы")
                        {
                            continue;
                        }
                        else if (dep == "GIP")
                        {
                            LoginHipComboBox.Items.Add(dep);
                            continue;
                        }
                        else
                        {
                            LoginComboBox.Items.Add(dep);
                        };
                    }

                    if (LoginComboBox.Items.Count > 0)
                    {
                        LoginComboBox.SelectedIndex = -1;
                        LoginEmployeeComboBox.SelectedIndex = -1;
                        LoginEmployeeComboBox.IsEnabled = false;
                    }
                    else if (LoginHipComboBox.Items.Count > 0) {
                        LoginHipComboBox.SelectedIndex = -1;
                        LoginHipSurnamesComboBox.SelectedIndex = -1;
                        LoginEmployeeComboBox.IsEnabled = false;

                    }

                        
                       
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке логинов: {ex.Message}");
            }
        }


        private void HipLoginButton_Click(object sender, RoutedEventArgs e)
        {
            _isHipLogin = true;
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
            _isHipLogin = false;
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
            string login;
            string surname;
            string password;
            CheckBox rememberMeCheckbox;

            if (_isHipLogin)
            {
                login = LoginHipComboBox.Text.Trim(); 
                surname = LoginHipSurnamesComboBox.Text.Trim();
                password = PasswordBoxHip.Password;
                rememberMeCheckbox = SaveCurrentHipCheckbox;
            }
            else
            {
                login = LoginComboBox.Text.Trim();
                surname = LoginEmployeeComboBox.Text.Trim();
                password = PasswordBox.Password;
                rememberMeCheckbox = SaveCurrentUserCheckbox; 
            }

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Пожалуйста, введите логин отдела.", "Пустое поле", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    UserSession.Login = surname;
                   

                    if (SaveCurrentUserCheckbox.IsChecked == true || SaveCurrentHipCheckbox.IsChecked == true)
                    {
                        TasksETM.Properties.Settings.Default.SavedDepartmentLogin = login;
                        TasksETM.Properties.Settings.Default.SavedLogin = surname;
                        TasksETM.Properties.Settings.Default.RememberMe = true;
                        TasksETM.Properties.Settings.Default.Save();

                        UserSessionForNotify.Login = surname;

                        UserSession.Login = login;

                        SharedLoginStorage.SaveLogin(surname);

                        SharedLoginStorage.SaveDepartmentLogin(login);
                    }
                    else
                    {
                        TasksETM.Properties.Settings.Default.SavedDepartmentLogin = string.Empty;
                        TasksETM.Properties.Settings.Default.SavedLogin = string.Empty;
                        TasksETM.Properties.Settings.Default.RememberMe = false;
                        TasksETM.Properties.Settings.Default.Save();

                        UserSessionForNotify.Login = string.Empty;

                        SharedLoginStorage.SaveLogin(loginForNotify);

                        SharedLoginStorage.SaveDepartmentLogin(loginForDeaprtments);
                    }

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

        private void HelpLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var helpLoginWindow = new HelpLoginWindow(_dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
            helpLoginWindow.Show();
            this.Close();
        }


        private async void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LoginComboBox.SelectedItem == null)
            {
                LoginEmployeeComboBox.Items.Clear();
                LoginEmployeeComboBox.IsEnabled = false;
                return;
            }

            string selectedDepartment = LoginComboBox.SelectedItem.ToString();

            // Загружаем пользователей выбранного отдела
            var users = await _departmentService.GetUsersSurnamesByDepartmentAsync(selectedDepartment);

            Dispatcher.Invoke(() =>
            {
                LoginEmployeeComboBox.Items.Clear();

                foreach (var user in users)
                {
                    LoginEmployeeComboBox.Items.Add(user);
                }

                LoginEmployeeComboBox.IsEnabled = users.Any();
                if (users.Any())
                    LoginEmployeeComboBox.SelectedIndex = 0;
            });
        }

        private async void StatusHipComboBox_SelectionChacnged(object sender, SelectionChangedEventArgs e)
        {

            if (LoginHipComboBox.SelectedItem == null)
            {
                LoginHipSurnamesComboBox.Items.Clear();
                LoginHipSurnamesComboBox.IsEnabled = false;
                return;
            }

            string selectedDepartment = LoginHipComboBox.SelectedItem.ToString();

            // Загружаем пользователей выбранного отдела
            var users = await _departmentService.GetUsersSurnamesByDepartmentAsync(selectedDepartment);

            Dispatcher.Invoke(() =>
            {
                LoginHipSurnamesComboBox.Items.Clear();

                foreach (var user in users)
                {
                    LoginHipSurnamesComboBox.Items.Add(user);
                }

                LoginHipSurnamesComboBox.IsEnabled = users.Any();
                if (users.Any())
                    LoginHipSurnamesComboBox.SelectedIndex = 0;
            });
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

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

    }
}
