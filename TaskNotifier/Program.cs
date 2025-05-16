using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Npgsql;

namespace TaskNotificationApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Запуск проверки с таймером, чтобы проверка выполнялась каждую минуту
            var timer = new System.Timers.Timer(60000); // 60000 миллисекунд = 1 минута
            timer.Elapsed += async (sender, e) => await CheckTasksForNotificationsAsync();
            timer.Start();

            Console.WriteLine("Программа запущена. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        // Метод для отправки уведомлений
        static void ShowNotification(string title, string message)
        {
            var toastContent = new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .GetToastContent();

            var toast = new Windows.UI.Notifications.ToastNotification(toastContent.GetXml());
            Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        // Метод для получения задач из базы данных PostgreSQL по проекту
        public static async Task<List<TaskModel>> GetTasksByProjectAsync(string projectName)
        {
            var tasks = new List<TaskModel>();

            string connString = "Server=192.168.0.171;Port=5432;User Id=User;Password=123;Database=postgres";
            using (var conn = new NpgsqlConnection(connString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(@"
                SELECT 
                    t.""TaskNumber"", 
                    t.""FromDepart"", 
                    t.""ToDepart"", 
                    t.""AcceptedDepart"",
                    t.""TaskCompleted"",
                    t.""ScreenShot"", 
                    t.""TaskView"", 
                    t.""TaskDescription"", 
                    t.""TaskDate"", 
                    t.""TaskDeadLine"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'AR') AS ""IsAR"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'VK') AS ""IsVK"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'OV') AS ""IsOV"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'SS') AS ""IsSS"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'ES') AS ""IsES"",
                    (SELECT ""IsAssigned"" FROM public.""TaskAssignments"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'GIP') As ""IsGIP"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'AR') AS ""IsARCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'VK') AS ""IsVKCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'OV') AS ""IsOVCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'SS') AS ""IsSSCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'ES') AS ""IsESCompl"",
                    (SELECT ""IsCompleted"" FROM public.""TaskCompleted"" WHERE ""TaskNumber"" = t.""TaskNumber"" AND ""Section"" = 'GIP') As ""IsGIPCompl""
                FROM public.""Tasks"" t
                WHERE t.""PK_ProjectNumber"" = (
                    SELECT ""ProjectNumber"" FROM public.""Projects"" WHERE ""ProjectName"" = @projectname
                )
                AND t.""FromDepart"" IS NOT NULL 
                AND t.""ToDepart"" IS NOT NULL 
                AND t.""TaskDescription"" IS NOT NULL
                ", conn))
                {
                    cmd.Parameters.AddWithValue("projectname", projectName);
                    try
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var task = new TaskModel
                                {
                                    TaskNumber = reader.GetInt32(reader.GetOrdinal("TaskNumber")),
                                    FromDepart = reader["FromDepart"] is DBNull ? string.Empty : reader["FromDepart"]!.ToString()!,
                                    ToDepart = reader["ToDepart"] is DBNull ? string.Empty : reader["ToDepart"]!.ToString()!,
                                    Accepted = reader["AcceptedDepart"] is DBNull ? null : (int)reader["AcceptedDepart"] == 1,
                                    Completed = reader["TaskCompleted"] is DBNull ? null : (int)reader["TaskCompleted"] == 1,
                                    ScreenshotPath = reader["ScreenShot"] is DBNull ? new byte[0] : (byte[])reader["ScreenShot"],
                                    TaskView = reader["TaskView"] is DBNull ? string.Empty : reader["TaskView"]!.ToString()!,
                                    TaskDescription = reader["TaskDescription"] is DBNull ? string.Empty : reader["TaskDescription"]!.ToString()!,
                                    TaskDate = reader["TaskDate"] is DBNull ? string.Empty : reader["TaskDate"]!.ToString()!,
                                    TaskDeadline = reader["TaskDeadLine"] is DBNull ? string.Empty : reader["TaskDeadLine"]!.ToString()!,
                                    IsAR = reader["IsAR"] is DBNull ? null : (bool)reader["IsAR"],
                                    IsVK = reader["IsVK"] is DBNull ? null : (bool)reader["IsVK"],
                                    IsOV = reader["IsOV"] is DBNull ? null : (bool)reader["IsOV"],
                                    IsSS = reader["IsSS"] is DBNull ? null : (bool)reader["IsSS"],
                                    IsES = reader["IsES"] is DBNull ? null : (bool)reader["IsES"],
                                    IsGIP = reader["IsGIP"] is DBNull ? null : (bool)reader["IsGIP"],
                                    IsARCompl = reader["IsARCompl"] is DBNull ? null : (bool)reader["IsARCompl"],
                                    IsVKCompl = reader["IsVKCompl"] is DBNull ? null : (bool)reader["IsVKCompl"],
                                    IsOVCompl = reader["IsOVCompl"] is DBNull ? null : (bool)reader["IsOVCompl"],
                                    IsSSCompl = reader["IsSSCompl"] is DBNull ? null : (bool)reader["IsSSCompl"],
                                    IsESCompl = reader["IsESCompl"] is DBNull ? null : (bool)reader["IsESCompl"],
                                    IsGIPCompl = reader["IsGIPCompl"] is DBNull ? null : (bool)reader["IsGIPCompl"],
                                };

                                tasks.Add(task);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                    }
                }
            }

            return tasks;
        }

        // Метод для проверки задач и отправки уведомлений
        public static async Task CheckTasksForNotificationsAsync()
        {
            // Для примера, используем проект с названием "ProjectName"
            var tasks = await GetTasksByProjectAsync(projectName);

            foreach (var task in tasks)
            {
                // 1. Не принятое задание
                if (task.IsAR == true && !task.Accepted)
                {
                    ShowNotification($"Не принято задание #{task.TaskNumber}", "Пожалуйста, примите задание.");
                }
                // 2. Принято, но не выполнено, и дедлайн прошёл
                else if (task.IsAR == true && task.Accepted == true && !task.Completed && DateTime.Parse(task.TaskDeadline) < DateTime.Now)
                {
                    ShowNotification($"Задание #{task.TaskNumber} просрочено", "Дедлайн прошёл!");
                }
                // 3. Напоминание за 2 дня до дедлайна
                else if (task.IsAR == true && task.Accepted == true && !task.Completed && IsDaysLeft(task.TaskDeadline, 2))
                {
                    ShowNotification($"Осталось 2 дня до дедлайна для задания #{task.TaskNumber}", "Напоминание!");
                }
            }
        }

        // Метод для проверки оставшихся дней до дедлайна
        public static bool IsDaysLeft(string deadline, int daysLeft)
        {
            DateTime taskDeadline;
            if (DateTime.TryParse(deadline, out taskDeadline))
            {
                return (taskDeadline - DateTime.Now).Days <= daysLeft;
            }

            return false;
        }
    }

    // Модель задачи
    public class TaskModel
    {
        public int TaskNumber { get; set; }
        public string FromDepart { get; set; }
        public string ToDepart { get; set; }
        public bool? Accepted { get; set; }
        public bool? Completed { get; set; }
        public byte[] ScreenshotPath { get; set; }
        public string TaskView { get; set; }
        public string TaskDescription { get; set; }
        public string TaskDate { get; set; }
        public
