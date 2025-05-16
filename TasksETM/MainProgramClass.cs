using IssuingTasksETM.WPF;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using TasksETM.Service;
using TasksETM.Service.Tasks;

namespace TasksETM
{
    internal class MainProgramClass
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SetCurrentProcessExplicitAppUserModelID(string appID);

        private static void SetAppId()
        {
            SetCurrentProcessExplicitAppUserModelID("TasksETM.NotificationApp");
        }

        [STAThread]
        public static void Main()
        {
            SetAppId(); 

            var app = new Application();
            var dbConnection = new DatabaseConnection();
            var departmentService = new DepartmentService();
            var projectService = new ProjectService();
            var authService = new AuthService(DatabaseConnection.connString);
            var filterTasksService = new FilterTasksService();

            var window = new LoginWindow(dbConnection, departmentService, projectService, authService, filterTasksService);
            app.MainWindow = window;
            app.Run(window);
        }
    }
}
