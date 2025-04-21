using IssuingTasksETM.Interfaces;
using IssuingTasksETM.WPF;
using System.Windows;
using System.Windows.Controls;
using TasksETM.Interfaces;
using TasksETM.Service;


namespace TasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для ChooseProjectWindow.xaml
    /// </summary>
    public partial class ChooseProjectWindow : Window
    {
        private readonly IDatabaseConnection _dbConnection;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;

        public ChooseProjectWindow(IDatabaseConnection dbConnection, IDepartmentService departmentService, IProjectService projectService, IAuthService authService)
        {
            InitializeComponent();
            _dbConnection = dbConnection ?? new DatabaseConnection();
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);
            FillComboBoxAsync();
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

                var taskWindow = new TaskWindow(selectedProject, _dbConnection, _departmentService, _projectService, _authService);
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
            LoginWindow loginWindow = new LoginWindow(_dbConnection, _departmentService, _projectService, _authService);
            loginWindow.Show();
            Close();
        }

        

        private void ProjectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
