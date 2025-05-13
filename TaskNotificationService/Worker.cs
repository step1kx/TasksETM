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
        private readonly ITaskService _taskService; // ���������� ���� ������
        private readonly string _currentUserSection; // ��������, "AR", "VK" � �.�.
        private readonly ILogger<Worker> _logger;
        private NotifyIcon _notifyIcon;

        public Worker(ITaskService taskService, ILogger<Worker> logger)
        {
            _taskService = taskService;
            _currentUserSection = "AR"; // ����� ������ ������ ��� ������� ����� ������
            _logger = logger;

            // ��������� NotifyIcon
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
                    _logger.LogError(ex, "������ ��� �������� �����");
                    ShowNotification("������", $"�� ������� ��������� ������: {ex.Message}", ToolTipIcon.Error);
                }
                await Task.Delay(60000, stoppingToken); // �������� ������ ������
            }
        }

        private async Task CheckTasksForNotificationsAsync()
        {
            var tasks = await _taskService.GetTasksByProjectAsync(/* ID ������� */);
            var userTasks = tasks.Where(t => t.ToDepart == _currentUserSection).ToList();

            foreach (var task in userTasks)
            {
                // 1. �� �������� �������
                if (!IsTaskAccepted(task, _currentUserSection))
                {
                    ShowNotification("�� �������", $"��, �� �� ������� ������� #{task.TaskNumber}!", ToolTipIcon.Warning);
                }
                // 2. �������, �� �� ���������, � ������� ������
                else if (IsTaskAccepted(task, _currentUserSection) &&
                         !IsTaskCompleted(task, _currentUserSection) &&
                         IsTaskOverdue(task.TaskDeadline))
                {
                    ShowNotification("����������", $"��, �� �� ��������� ������� #{task.TaskNumber}! ������� ������!", ToolTipIcon.Error);
                }
                // 3. ����������� �� 2 ��� �� ��������
                else if (IsTaskAccepted(task, _currentUserSection) &&
                         !IsTaskCompleted(task, _currentUserSection) &&
                         IsDaysLeft(task.TaskDeadline, 2))
                {
                    ShowNotification("�����������", $"�������� 2 ��� �� �������� ��� ������� #{task.TaskNumber}!", ToolTipIcon.Info);
                }
            }
        }

        private void ShowNotification(string title, string message, ToolTipIcon icon)
        {
            _notifyIcon.ShowBalloonTip(3000, title, message, icon);
        }

        // �������� ������ IsTaskAccepted, IsTaskCompleted, IsTaskOverdue, IsDaysLeft �� WPF
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