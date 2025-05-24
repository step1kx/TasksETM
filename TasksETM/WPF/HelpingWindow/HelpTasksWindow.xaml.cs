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

namespace TasksETM.WPF.HelpingWindow
{
    /// <summary>
    /// Логика взаимодействия для HelpTasksWindow.xaml
    /// </summary>
    public partial class HelpTasksWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly IDatabaseConnection _dbConnection;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;
        private readonly IFilterTasksService _filterTasksService;

        private readonly string _selectedProject;


        public HelpTasksWindow(string selectedProject,
            IDatabaseConnection dbConnection,
            IDepartmentService departmentService,
            IProjectService projectService,
            IAuthService authService,
            IFilterTasksService filterTasksService
            )
        {
            InitializeComponent();
            _dbConnection = dbConnection ?? new DatabaseConnection();
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);
            _filterTasksService = new FilterTasksService();

            _selectedProject = selectedProject;
        }


        private void ToPrevWindow_Click(object sender, RoutedEventArgs e)
        {
            TaskWindow taskWindow = new TaskWindow(_selectedProject, _dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
            taskWindow.Show();
            Close();
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
