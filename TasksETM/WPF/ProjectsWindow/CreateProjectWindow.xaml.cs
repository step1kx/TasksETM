using IssuingTasksETM.Interfaces;
using IssuingTasksETM.WPF;
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
using TasksETMCommon.Helpers;
using TasksETM.Service.Tasks;
using TasksETM.Service;
using TasksETM.WPF.ProjectsWindow;
using TasksETM.WPF.LoadingWindow;

namespace TasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        private readonly IDatabaseConnection _dbConnection;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;
        private readonly IFilterTasksService _filterTasksService;

        public CreateProjectWindow(IDatabaseConnection dbConnection,
            IDepartmentService departmentService,
            IProjectService projectService,
            IAuthService authService,
            IFilterTasksService filterTasksService)
        {
            _dbConnection = dbConnection ?? new DatabaseConnection();
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);
            _filterTasksService = new FilterTasksService();

            InitializeComponent();
        }


        private void ToPrevWindow_Click(object sender, RoutedEventArgs e)
        {
            ChooseProjectGIPWindow chooseProjectGIPWindow = new ChooseProjectGIPWindow(_dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
            chooseProjectGIPWindow.Show();
            Close();
        }

        private async void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string projectName = CreateProjectTextBox.Text.Trim();
                bool notifyStatus = true;
                bool projectOrTask = true;

                if (string.IsNullOrEmpty(projectName))
                {
                    MessageBox.Show("Пожалуйста, напишите название проекта");
                    return;
                }

                await _projectService.CreateProjectAsync(projectName, notifyStatus);
                await _projectService.UpdateNotifyStatusForCreatedProjectAsync(projectName, notifyStatus);

                var projectCreatedSucceful = new CreateSuccessfulWindow();
                projectCreatedSucceful.Show();
                await projectCreatedSucceful.UpdateProgressBarAsync(projectOrTask);

                ChooseProjectGIPWindow chooseProjectGIPWindow = new ChooseProjectGIPWindow(_dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
                chooseProjectGIPWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"При создании проекта что-то пошло не так: {ex.Message}");
            }
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
