using IssuingTasksETM.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IssuingTasksETM.Models
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
