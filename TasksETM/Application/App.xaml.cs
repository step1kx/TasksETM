using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace TasksETM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SetCurrentProcessExplicitAppUserModelID(string AppID);

        protected override void OnStartup(StartupEventArgs e)
        {
            SetCurrentProcessExplicitAppUserModelID("ETM.123");
            base.OnStartup(e);
        }
    }

}
