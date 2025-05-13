using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskNotificationService
{
    public class Worker : BackgroundService
    {
        private readonly ITaskService _taskService; // Инжектируй твой сервис
        private readonly string _currentUserSection; // Например, "AR", "VK" и т.д.
        private readonly ILogger<Worker> _logger;
        private NotifyIcon _notifyIcon;

        public Worker(ITaskService taskService, ILogger<Worker> logger)
        {
            _taskService = taskService;
            _currentUserSection = "AR"; // Укажи нужный раздел или передай через конфиг
            _logger = logger;

            // Настройка NotifyIcon
            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("path/to/icon.ico"),
                Visible = true,
                Text = "Task Notification Service"
            };
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => Environment.Exit(0));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckTasksForNotificationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при проверке задач");
                    ShowNotification("Ошибка", $"Не удалось проверить задачи: {ex.Message}", ToolTipIcon.Error);
                }
                await Task.Delay(60000, stoppingToken); // Проверка каждую минуту
            }
        }

        private async Task CheckTasksForNotificationsAsync()
        {
            var tasks = await _taskService.GetTasksByProjectAsync(/* ID проекта */);
            var userTasks = tasks.Where(t => t.ToDepart == _currentUserSection).ToList();

            foreach (var task in userTasks)
            {
                // 1. Не принятое задание
                if (!IsTaskAccepted(task, _currentUserSection))
                {
                    ShowNotification("Не принято", $"Эй, вы не приняли задание #{task.TaskNumber}!", ToolTipIcon.Warning);
                }
                // 2. Принято, но не выполнено, и дедлайн прошёл
                else if (IsTaskAccepted(task, _currentUserSection) &&
                         !IsTaskCompleted(task, _currentUserSection) &&
                         IsTaskOverdue(task.TaskDeadline))
                {
                    ShowNotification("Просрочено", $"Эй, вы не выполнили задание #{task.TaskNumber}! Дедлайн прошёл!", ToolTipIcon.Error);
                }
                // 3. Напоминание за 2 дня до дедлайна
                else if (IsTaskAccepted(task, _currentUserSection) &&
                         !IsTaskCompleted(task, _currentUserSection) &&
                         IsDaysLeft(task.TaskDeadline, 2))
                {
                    ShowNotification("Напоминание", $"Осталось 2 дня до дедлайна для задания #{task.TaskNumber}!", ToolTipIcon.Info);
                }
            }
        }

        private void ShowNotification(string title, string message, ToolTipIcon icon)
        {
            _notifyIcon.ShowBalloonTip(3000, title, message, icon);
        }

        // Перенеси методы IsTaskAccepted, IsTaskCompleted, IsTaskOverdue, IsDaysLeft из WPF
        private bool IsTaskAccepted(TaskModel task, string section) { /* ... */ }
        private bool IsTaskCompleted(TaskModel task, string section) { /* ... */ }
        private bool IsTaskOverdue(string deadline) { /* ... */ }
        private bool IsDaysLeft(string deadline, int days) { /* ... */ }

        public override void Dispose()
        {
            _notifyIcon.Dispose();
            base.Dispose();
        }
    }
}