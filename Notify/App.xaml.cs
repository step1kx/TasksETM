using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Microsoft.Toolkit.Uwp.Notifications;
using Notify.Interfaces;
using Notify.NotificationService;
using Notify.Models;
using TasksETM.Models;
using TasksETMCommon.Helpers;
using TasksETMCommon.Models;

namespace Notify
{
    public partial class App : Application
    {
        private System.Timers.Timer _notificationTimer; 
        private ITaskService _taskService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _taskService = new TaskService(); // Убедитесь, что TaskService инициализируется правильно

           

            SetupNotificationTimer();
        }

        private void SetupNotificationTimer()
        {
            _notificationTimer = new System.Timers.Timer(3000); // Проверка каждые 60 секунд
            _notificationTimer.Elapsed += async (s, e) => await CheckTasksForNotificationsAsync();
            _notificationTimer.AutoReset = true;
            _notificationTimer.Start();
            MessageBox.Show("Таймер уведомлений запущен.", "Отладка");
        }

        private async Task CheckTasksForNotificationsAsync()
        {
            try
            {

                string savedLogin = SharedLoginStorage.LoadLogin();

                if (!string.IsNullOrEmpty(savedLogin))
                {
                    UserSessionForNotify.Login = savedLogin;
                }
                else
                {
                    MessageBox.Show("Файл с логином не найден или пуст.", "Отладка");
                }

                // Получаем все задания для пользователя
                var tasks = await _taskService.GetTasksByUserAsync(savedLogin);
                if (tasks == null || !tasks.Any())
                {
                    MessageBox.Show("Задания не найдены или список пуст.", "Отладка");
                    return;
                }

                foreach (var task in tasks)
                {
                    string userSection = task.ToDepart;

                    // Непринято
                    if (!IsTaskAccepted(task, userSection))
                    {
                        ShowNotification(
                            $"Не принято: Объект №{task.TaskNumber}",
                            $"Эй, вы не приняли задание №{task.TaskNumber}!");
                    }
                    // Принято, но не завершено
                    else if (IsTaskAccepted(task, userSection) && !IsTaskCompleted(task, userSection))
                    {
                        // Просрочено
                        if (IsTaskOverdue(task.TaskDeadline))
                        {
                            ShowNotification(
                                $"Просрочено: Задание №{task.TaskNumber}",
                                $"Эй, вы не выполнили задание №{task.TaskNumber}! Дедлайн прошёл!");
                        }
                        // Напоминание за 2 дня
                        else if (IsDaysLeft(task.TaskDeadline, 2))
                        {
                            ShowNotification(
                                $"Напоминание: Задание №{task.TaskNumber}",
                                $"Осталось 2 дня до дедлайна для задания №{task.TaskNumber}!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке заданий: {ex.Message}", "Отладка: Ошибка");
            }
        }

        private static void ShowNotification(string title, string message)
        {
            MessageBox.Show($"Отправка уведомления: {title}\n{message}", "Отладка");
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }

        private bool IsTaskAccepted(TaskModel task, string section)
        {
            return section switch
            {
                "AR" => task.IsAR == true,
                "VK" => task.IsVK == true,
                "OV" => task.IsOV == true,
                "SS" => task.IsSS == true,
                "ES" => task.IsES == true,
                "GIP" => task.IsGIP == true,
                _ => false
            };
        }

        private bool IsTaskCompleted(TaskModel task, string section)
        {
            return section switch
            {
                "AR" => task.IsARCompl ?? false,
                "VK" => task.IsVKCompl ?? false,
                "OV" => task.IsOVCompl ?? false,
                "SS" => task.IsSSCompl ?? false,
                "ES" => task.IsESCompl ?? false,
                "GIP" => task.IsGIPCompl ?? false,
                _ => false
            };
        }

        private bool IsTaskOverdue(string deadline)
        {
            if (string.IsNullOrEmpty(deadline)) return false;
            if (DateTime.TryParse(deadline, out DateTime deadlineDate))
            {
                return DateTime.Now > deadlineDate;
            }
            return false;
        }

        private bool IsDaysLeft(string deadline, int days)
        {
            if (string.IsNullOrEmpty(deadline)) return false;
            if (DateTime.TryParse(deadline, out DateTime deadlineDate))
            {
                var daysLeft = (deadlineDate - DateTime.Now).Days;
                return daysLeft > 0 && daysLeft <= days;
            }
            return false;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _notificationTimer?.Stop();
            _notificationTimer?.Dispose();
            base.OnExit(e);
        }
    }

}