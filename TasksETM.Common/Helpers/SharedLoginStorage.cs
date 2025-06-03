using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TasksETMCommon.Helpers
{
    public static class SharedLoginStorage
    {
        private static readonly string _path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TasksETM", "saved_login.txt");

        private static readonly string _path2 = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TasksETM", "saved_department_login.txt");

        /// <summary>
        /// Сохраняет логин в файл (перезаписывает).
        /// </summary>
        public static void SaveLogin(string login)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            File.WriteAllText(_path, login);
        }

        public static void SaveDepartmentLogin(string departmentName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path2));
            File.WriteAllText(_path2, departmentName);
        }

        /// <summary>
        /// Загружает логин из файла, если он существует.
        /// </summary>
        public static string LoadLogin()
        {
            if (File.Exists(_path))
                return File.ReadAllText(_path).Trim();

            return null;
        }


        public static string LoadDepartmentLogin()
        {
            if(File.Exists(_path2))
                return File.ReadAllText(_path2).Trim();

            return null;
        }

        /// <summary>
        /// Проверяет, существует ли файл с логином.
        /// </summary>


    }
}
