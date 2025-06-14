﻿using System;
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
using System.Runtime.InteropServices;
using System.IO;
using IWshRuntimeLibrary;
using Windows.UI.Notifications;

namespace Notify
{
    public partial class App : Application
    {
        private System.Timers.Timer _notificationTimer; 
        private ITaskService _taskService;
        private CancellationTokenSource _cts = new CancellationTokenSource();


        private static int _notificationCounter = 0;

        private const string AppId = "com.tasks.notify";
        private const string ShortcutName = "Notify.lnk";

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SetCurrentProcessExplicitAppUserModelID(string appID);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _taskService = new TaskService();


            SetCurrentProcessExplicitAppUserModelID(AppId);

            SetupNotificationTimer();
            
        }

        private void CreateStartMenuShortcut()
        {
            string shortcutPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                "Programs",
                ShortcutName);

            if (System.IO.File.Exists(shortcutPath))
                return;

            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            var shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = exePath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
            shortcut.Arguments = "";
            shortcut.WindowStyle = 1;
            shortcut.Description = "Уведомления от TasksETM";
            shortcut.IconLocation = exePath;
            shortcut.Save();

            try
            {
                dynamic shortcutObject = shell.CreateShortcut(shortcutPath);
                shortcutObject.AppUserModelID = AppId;
                shortcutObject.Save();
            }
            catch
            {
            }
        }

        private void SetupNotificationTimer()
        {
            _notificationTimer = new System.Timers.Timer(600000);
            //_notificationTimer = new System.Timers.Timer(12000);
            _notificationTimer.Elapsed += async (s, e) => await CheckTasksForNotificationsAsync();
            _notificationTimer.AutoReset = true;
            _notificationTimer.Start();
        }


        private async Task CheckTasksForNotificationsAsync()
        {
            try
            {
                string savedLogin = SharedLoginStorage.LoadLogin();

                string departmentLogin = SharedLoginStorage.LoadDepartmentLogin();

                //MessageBox.Show($"лОГИН Отдела: {departmentLogin}");

                if (string.IsNullOrWhiteSpace(savedLogin))
                {
                    //MessageBox.Show($"Логина неть: {savedLogin}");
                    return;
                }

                var notifyProjects = await _taskService.GetNotifyStatusFromProjectsAsync(savedLogin);

                //MessageBox.Show($"Логин хранимый в памяти: {savedLogin}");



                var tasks = await _taskService.GetTasksByUserAsync(departmentLogin);
                if (tasks == null || !tasks.Any())
                {
                    //MessageBox.Show("Какашке");
                    return;
                    
                }
                    

                foreach (var task in tasks)
                {
                    // Проверка: включены ли уведомления по проекту
                    if (!notifyProjects.TryGetValue(task.ProjectName, out bool isNotify) || !isNotify)
                        continue;

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
                        if (IsTaskOverdue(task.TaskDeadline))
                        {
                            ShowNotification(
                                $"Сотрудник отдела {task.ToDepart}. Объект {task.ProjectName}",
                                $"Вы не выполнили задание №{task.TaskNumber} от раздела {task.FromDepart}" +
                                $"\nКрайний срок сдачи задания прошёл - {task.TaskDeadline}");
                        }
                        else if (IsDaysLeft(task.TaskDeadline, 2))
                        {
                            ShowNotification(
                                $"Напоминание! Объект {task.ProjectName}",
                                $"Осталось 2 дня до дедлайна для задания №{task.TaskNumber} от раздела {task.FromDepart}" +
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


        private static void ShowNotification(string title, string message)
        {
            string tag = $"taskNotification{_notificationCounter++}";
            var toastContent = new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .GetToastContent();
            var toast = new ToastNotification(toastContent.GetXml()) { Tag = tag, Group = "tasksGroup" };
            ToastNotificationManager.CreateToastNotifier(AppId).Show(toast);
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