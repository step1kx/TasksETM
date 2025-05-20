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
using Windows.Services.TargetedContent;
using Windows.ApplicationModel.UserDataTasks;

namespace Notify
{
    public partial class App : Application
    {
        private System.Timers.Timer _notificationTimer; 
        private ITaskService _taskService;

        private static int _notificationCounter = 0;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _taskService = new TaskService(); 

           

            SetupNotificationTimer();
        }

        private void SetupNotificationTimer()
        {
            _notificationTimer = new System.Timers.Timer(60000); 
            _notificationTimer.Elapsed += async (s, e) => await CheckTasksForNotificationsAsync();
            _notificationTimer.AutoReset = true;
            _notificationTimer.Start();
            //MessageBox.Show("Таймер уведомлений запущен.", "Отладка");
        }

        private async Task CheckTasksForNotificationsAsync()
        {
            try
            {

                string savedLogin = SharedLoginStorage.LoadLogin();

                if (!string.IsNullOrWhiteSpace(savedLogin))
                {
                    UserSessionForNotify.Login = savedLogin;
                }
                else
                {
                    return;
                }

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
                            $"Сотрудник отдела {task.ToDepart}. Объект {task.ProjectName}",
                            $"Вы не приняли задание №{task.TaskNumber} от раздела {task.FromDepart}" +
                            $"\nКрайний срок сдачи задания - {task.TaskDeadline}");
                    }
                    // Принято, но не завершено
                    else if (IsTaskAccepted(task, userSection) && !IsTaskCompleted(task, userSection))
                    {
                        // Просрочено
                        if (IsTaskOverdue(task.TaskDeadline))
                        {
                            ShowNotification(
                                $"Сотрудник отдела {task.ToDepart}. Объект {task.ProjectName}",
                                $"Вы не выполнили задание №{task.TaskNumber} от раздела {task.FromDepart}" + 
                                $"\nКрайний срок сдачи задания - {task.TaskDeadline}");
                        }
                        // Напоминание за 2 дня
                        else if (IsDaysLeft(task.TaskDeadline, 2))
                        {
                            ShowNotification(
                                $"Напоминание! Объект {task.ProjectName}",
                                $"Осталось 2 дня до дедлайна для задания №{task.TaskNumber} от раздела  {task.FromDepart}" +
                                $"\nКрайний срок сдачи задания - {task.TaskDeadline}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке заданий: {ex.Message}", "Отладка: Ошибка");
            }
        }

        //private static void ShowNotification(string title, string message)
        //{
        //    new ToastContentBuilder()
        //        .AddText(title)
        //        .AddText(message)
        //        .Show();
        //}

        private static void ShowNotification(string title, string message)
        {
            string tag = $"taskNotification{_notificationCounter++}"; 

            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show(toast =>
                {
                    toast.Tag = tag;
                    toast.Group = "tasksGroup";
                });
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