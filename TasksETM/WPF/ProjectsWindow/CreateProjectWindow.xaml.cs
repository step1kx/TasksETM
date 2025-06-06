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
            Properties.Settings.Default.SavedLogin = string.Empty;
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.Save();
            SharedLoginStorage.SaveLogin(Properties.Settings.Default.SavedLogin);
            LoginWindow loginWindow = new LoginWindow(_dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
            loginWindow.Show();
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
