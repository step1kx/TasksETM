using System;
using System.ServiceProcess;
using System.Timers;
using System.Windows.Forms;
using TasksETM.Service.Tasks;
using TasksETM.Models;

namespace TaskNotificationService
{
    public partial class Service1 : ServiceBase
    {
        private NotifyIcon _notifyIcon;
        private Timer _timer;
        private readonly ITaskService _taskService;
        private readonly string _selectedProject = "YourProjectName"; // ������
        private readonly string _currentUserSection = "AR"; // ������

        public Service1()
        {
            InitializeComponent();
            _taskService = new TaskService("����_������_�����������"); // ������
        }

        protected override void OnStart(string[] args)
        {
            // ��������� ������ � ����
            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("TaskNotificationService.Icons.taskSoftwareIcon.ico")),
                Visible = true,
                Text = "����������� �����"
            };

            // ������ ��� ��������
            _timer = new Timer(10000); // ������ 10 ������
            _timer.Elapsed += async (s, e) => await CheckTasksAsync();
            _timer.Start();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }

        private async Task CheckTasksAsync()
        {
            try
            {
                var tasks = await _taskService.GetTasksByProjectAsync(_selectedProject);
                var userTasks = tasks.Where(t => GetSectionProperty(t, _currentUserSection)).ToList();

                foreach (var task in userTasks)
                {
                    if (!IsTaskAccepted(task, _currentUserSection))
                    {
                        _notifyIcon.ShowBalloonTip(5000, "�� �������", $"��, �� �� ������� ������� #{task.TaskNumber}!", ToolTipIcon.Info);
                    }
                    else if (IsTaskAccepted(task, _currentUserSection) &&
                             !IsTaskCompleted(task, _currentUserSection) &&
                             IsTaskOverdue(task.TaskDeadline))
                    {
                        _notifyIcon.ShowBalloonTip(5000, "����������", $"��, �� �� ��������� ������� #{task.TaskNumber}!", ToolTipIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                // ������� ������ (��������, � ����)
            }
        }

        private bool GetSectionProperty(TaskModel task, string section) => section switch
        {
            "AR" => task.IsAR ?? false,
            "VK" => task.IsVK ?? false,
            "OV" => task.IsOV ?? false,
            "SS" => task.IsSS ?? false,
            "ES" => task.IsES ?? false,
            "GIP" => task.IsGIP ?? false,
            _ => false
        };

        private bool IsTaskAccepted(TaskModel task, string section) => section switch
        {
            "AR" => task.IsAR == true,
            "VK" => task.IsVK == true,
            "OV" => task.IsOV == true,
            "SS" => task.IsSS == true,
            "ES" => task.IsES == true,
            "GIP" => task.IsGIP == true,
            _ => false
        };

        private bool IsTaskCompleted(TaskModel task, string section) => section switch
        {
            "AR" => task.IsARCompl ?? false,
            "VK" => task.IsVKCompl ?? false,
            "OV" => task.IsOVCompl ?? false,
            "SS" => task.IsSSCompl ?? false,
            "ES" => task.IsESCompl ?? false,
            "GIP" => task.IsGIPCompl ?? false,
            _ => false
        };

        private bool IsTaskOverdue(string deadline)
        {
            if (string.IsNullOrEmpty(deadline) || !DateTime.TryParse(deadline, out DateTime deadlineDate)) return false;
            return DateTime.Now > deadlineDate;
        }
    }
}