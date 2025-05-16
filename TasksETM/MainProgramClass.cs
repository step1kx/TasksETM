using IssuingTasksETM.WPF;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Win32;
using TasksETM.Service;
using TasksETM.Service.Tasks;
using System.Reflection;
using System.IO;

namespace TasksETM
{
    internal class MainProgramClass
    {
        private const string pathToAutoLoading = @"Software\Microsoft\Windows\CurrentVersion\Run";

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SetCurrentProcessExplicitAppUserModelID(string appID);

        private static void AddToStartup()
        {
            try
            {
                // Получаем путь к ярлыку приложения в меню "Пуск"
                string appName = "TasksETM"; // Имя вашего приложения
                string shortcutPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                    "TasksETM", 
                    $"{appName}.appref-ms"); 

                if (File.Exists(shortcutPath))
                {
                    // Открываем раздел реестра для автозагрузки
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                        pathToAutoLoading, true))
                    {
                        // Добавляем приложение в автозагрузку
                        key.SetValue(appName, $"\"{shortcutPath}\"");
                    }
                }
                else
                {
                    MessageBox.Show("Ярлык приложения не найден. Убедитесь, что приложение установлено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении в автозагрузку: {ex.Message}");
            }
        }

        //private static void RemoveFromStartup()
        //{
        //    try
        //    {
        //        string appName = "TasksETM"; // Имя вашего приложения
        //        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
        //            @"Software\Microsoft\Windows\CurrentVersion\Run", true))
        //        {
        //            // Удаляем приложение из автозагрузки, если оно там есть
        //            if (key.GetValue(appName) != null)
        //            {
        //                key.DeleteValue(appName);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка при удалении из автозагрузки: {ex.Message}");
        //    }
        //}




        [STAThread]
        public static void Main()
        {
            AddToStartup();


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
