using IssuingTasksETM.Interfaces;
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
using TasksETM.Interfaces.ITasks;
using TasksETM.Interfaces;
using TasksETM.Service.Tasks;
using TasksETM.Service;
using IssuingTasksETM.WPF;
using TasksETMCommon.Helpers;
using TasksETMCommon.Models;

namespace TasksETM.WPF.ProjectsWindow
{
    /// <summary>
    /// Логика взаимодействия для ChooseProjectGIPWindow.xaml
    /// </summary>
    public partial class ChooseProjectGIPWindow : Window
    {

        private readonly IDatabaseConnection _dbConnection;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;
        private readonly IFilterTasksService _filterTasksService;

        public ChooseProjectGIPWindow(IDatabaseConnection dbConnection,
            IDepartmentService departmentService,
            IProjectService projectService,
            IAuthService authService,
            IFilterTasksService filterTasksService)
        {
            InitializeComponent();
            _dbConnection = dbConnection ?? new DatabaseConnection();
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);
            _filterTasksService = new FilterTasksService();

            UpdateWelcomeMessage();

            FillComboBoxAsync();
        }

        private void UpdateWelcomeMessage()
        {
            WelcomeTextBlock.Text = "Добро пожаловать, ГИП";
        }


        private async void FillComboBoxAsync()
        {
            try
            {
                if (_projectService == null)
                {
                    MessageBox.Show("Сервис проектов не инициализирован.");
                    return;
                }

                var projectNames = await _projectService.GetAllProjectNamesAsync();

                Dispatcher.Invoke(() =>
                {
                    ProjectsComboBox.Items.Clear();

                    foreach (var name in projectNames)
                    {
                        ProjectsComboBox.Items.Add(name);
                    }

                    if (ProjectsComboBox.Items.Count > 0)
                        ProjectsComboBox.SelectedIndex = 0;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке проектов: {ex.Message}");
            }
        }

        private void ToChoosenProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsComboBox.SelectedItem != null)
            {
                string selectedProject = ProjectsComboBox.SelectedItem.ToString();

                var taskWindow = new TaskWindow(selectedProject, _dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
                taskWindow.Show();
                Close();
            }
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ToPrevWindow_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SavedLogin = string.Empty;
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();
            SharedLoginStorage.SaveLogin(Properties.Settings.Default.SavedLogin);
            LoginWindow loginWindow = new LoginWindow(_dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
            loginWindow.Show();
            Close();
        }

        private void ToCreateProjectWindow_Click( object sender, RoutedEventArgs e)
        {

        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
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
