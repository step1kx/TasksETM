using IssuingTasksETM.WPF;
using System.Windows;
using TasksETM.Service;

namespace TasksETM
{
    internal class MainProgramClass
    {
        [STAThread]
        public static void Main()
        {
            var app = new Application();
            var dbConnection = new DatabaseConnection();
            var departmentService = new DepartmentService();
            var projectService = new ProjectService();
            var authService = new AuthService(DatabaseConnection.connString);

            var window = new LoginWindow(dbConnection, departmentService, projectService, authService);
            app.MainWindow = window;
            app.Run(window);
        }
    }
}
