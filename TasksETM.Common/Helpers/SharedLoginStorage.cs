using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETMCommon.Helpers
{
    public static class SharedLoginStorage
    {
        private static readonly string _path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TasksETM", "saved_login.txt");

        /// <summary>
        /// Сохраняет логин в файл (перезаписывает).
        /// </summary>
        public static void SaveLogin(string login)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            File.WriteAllText(_path, login);
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

        /// <summary>
        /// Проверяет, существует ли файл с логином.
        /// </summary>
        public static bool Exists()
        {
            return File.Exists(_path);
        }
    }
}
