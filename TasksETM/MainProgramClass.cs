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
            var window = new LoginWindow(dbConnection);
            app.MainWindow = window;
            app.Run(window);
        }
    }
}
