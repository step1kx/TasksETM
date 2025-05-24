using IssuingTasksETM.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TasksETM.Interfaces;
using TasksETM.Interfaces.ITasks;
using TasksETM.Models;
using TasksETM.Service;
using TasksETM.Service.Tasks;
using TasksETM.WPF;
using TasksETM.WPF.HelpingWindow;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        private List<TaskModel> _tasks;
        private readonly string _selectedProject;
        private readonly ITaskService _taskService;
        private readonly IDatabaseConnection _dbConnection;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;
        private readonly IFilterTasksService _filterTasksService;
        private bool _isFiltered;
        //private System.Timers.Timer _notificationTimer;
        private string _currentUserSection;
        private readonly NotifyModel _notifyModel;


        public TaskWindow(string selectedProject, 
            IDatabaseConnection dbConnection, 
            IDepartmentService departmentService, 
            IProjectService projectService,
            IAuthService authService,
            IFilterTasksService filterTasksService
            )
        {
            InitializeComponent();
            _selectedProject = selectedProject;
            _dbConnection = dbConnection;
            _departmentService = departmentService ?? new DepartmentService(); 
            _projectService = projectService ?? new ProjectService();
            _authService = authService ?? new AuthService(DatabaseConnection.connString);
            _authService = new AuthService(DatabaseConnection.connString);
            _taskService = new TaskService(DatabaseConnection.connString);
            _filterTasksService = filterTasksService ?? new FilterTasksService();
            _isFiltered = false;
            _notifyModel = new NotifyModel();

            _currentUserSection = _authService.GetCurrentUserSection();
            tasksDataGrid.ItemsSource = _tasks;


            this.TitleBlock.Text = $"Задание по объекту: {selectedProject}";

            Loaded += TaskWindow_Loaded;
        }

        private async void TaskWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool isNotify = await _projectService.GetNotifyStatusByProjectNameAndUserLogin(_selectedProject, TasksETM.Properties.Settings.Default.SavedLogin);
            _notifyModel.IsNotify = isNotify;

            this.DataContext = _notifyModel;


            if (!_isFiltered)
            {
                await LoadAsync();
                //SetupNotificationTimer();
            }

        }

        //private void SetupNotificationTimer()
        //{
        //    _notificationTimer = new System.Timers.Timer(10000); // Проверка каждые 60 секунд
        //    _notificationTimer.Elapsed += async (s, e) => await CheckTasksForNotificationsAsync();
        //    _notificationTimer.Start();


        //}

        public async Task LoadAsync()
        {
            try
            {
                var tasks = await _taskService.GetTasksByProjectAsync(_selectedProject);
                if (tasks == null || !tasks.Any())
                {
                    
                    _tasks.Clear();
                }
                else
                {
                    _tasks = tasks;
                    tasksDataGrid.ItemsSource = _tasks;
                    tasksDataGrid.Items.Refresh();
                }
                _isFiltered = false; // Сбрасываем флаг фильтрации
            }
            catch (Exception ex)
            {
                MessageBox.Show("Пока что никто не добавил заданий в этот проект.");
            }
        }

        //static void ShowNotification(string title, string message)
        //{
        //    new ToastContentBuilder()
        //        .AddText(title)
        //        .AddText(message)
        //        .Show(); 
        //}

        //private async Task CheckTasksForNotificationsAsync()
        //{
        //    try
        //    {
        //        var tasks = await _taskService.GetTasksByProjectAsync(_selectedProject);
        //        var userTasks = tasks.Where(t => t.ToDepart == _currentUserSection).ToList();

        //        foreach (var task in userTasks)
        //        {
        //            // не принято
        //            if (!IsTaskAccepted(task, _currentUserSection))
        //            {
        //                Dispatcher.Invoke(() =>
        //                    ShowNotification($"Не принято. Объект: {_selectedProject}", $"Эй, вы не приняли задание №{task.TaskNumber}!"));
        //            }
        //            else if (IsTaskAccepted(task, _currentUserSection) &&
        //                     !IsTaskCompleted(task, _currentUserSection))
        //            {
        //                // Просрочено
        //                if (IsTaskOverdue(task.TaskDeadline))
        //                {
        //                    Dispatcher.Invoke(() =>
        //                        ShowNotification($"Просрочено. Объект: {_selectedProject}", $"Эй, вы не выполнили задание №{task.TaskNumber}! Дедлайн прошёл!"));
        //                }
        //                // Напоминание за 2 дня
        //                else if (IsDaysLeft(task.TaskDeadline, 2))
        //                {
        //                    Dispatcher.Invoke(() =>
        //                        ShowNotification($"Напоминание. Объект: {_selectedProject}", $"Осталось 2 дня до дедлайна для задания №{task.TaskNumber}!"));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Dispatcher.Invoke(() =>
        //            ShowNotification("Ошибка", $"Произошла ошибка при проверке заданий: {ex.Message}"));
        //    }
        //}


        //private bool IsTaskAccepted(TaskModel task, string section)
        //{
        //    return section switch
        //    {
        //        "AR" => task.IsAR == true,
        //        "VK" => task.IsVK == true,
        //        "OV" => task.IsOV == true,
        //        "SS" => task.IsSS == true,
        //        "ES" => task.IsES == true,
        //        "GIP" => task.IsGIP == true,
        //        _ => false
        //    };
        //}

        //private bool GetSectionProperty(TaskModel task, string section)
        //{
        //    return section switch
        //    {
        //        "AR" => task.IsAR ?? false,
        //        "VK" => task.IsVK ?? false,
        //        "OV" => task.IsOV ?? false,
        //        "SS" => task.IsSS ?? false,
        //        "ES" => task.IsES ?? false,
        //        "GIP" => task.IsGIP ?? false,
        //        _ => false
        //    };
        //}

        //private bool IsTaskCompleted(TaskModel task, string section)
        //{
        //    return section switch
        //    {
        //        "AR" => task.IsARCompl ?? false,
        //        "VK" => task.IsVKCompl ?? false,
        //        "OV" => task.IsOVCompl ?? false,
        //        "SS" => task.IsSSCompl ?? false,
        //        "ES" => task.IsESCompl ?? false,
        //        "GIP" => task.IsGIPCompl ?? false,
        //        _ => false
        //    };
        //}

        //private bool IsTaskOverdue(string deadline)
        //{
        //    if (string.IsNullOrEmpty(deadline)) return false;
        //    if (DateTime.TryParse(deadline, out DateTime deadlineDate))
        //    {
        //        return DateTime.Now > deadlineDate;
        //    }
        //    return false;
        //}

        //private bool IsDaysLeft(string deadline, int days)
        //{
        //    if (string.IsNullOrEmpty(deadline)) return false;
        //    if (DateTime.TryParse(deadline, out DateTime deadlineDate))
        //    {
        //        var daysLeft = (deadlineDate - DateTime.Now).Days;
        //        return daysLeft > 0 && daysLeft <= days;
        //    }
        //    return false;
        //}

        // Метод для обновления задач с учетом фильтров
        public void UpdateTasks(List<TaskModel> filteredTasks)
        {
            try
            {
                _tasks = filteredTasks ?? new List<TaskModel>();
                tasksDataGrid.ItemsSource = _tasks;
                tasksDataGrid.Items.Refresh();
                _isFiltered = true; // Устанавливаем флаг, что фильтры применены
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении задач: {ex.Message}");
            }
        }

        private async void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TaskModel task)
            {
                var taskNumber = task.TaskNumber;
                var bindingExpression = checkBox.GetBindingExpression(CheckBox.IsCheckedProperty);
                var bindingPath = bindingExpression?.ParentBinding?.Path?.Path;

                if (bindingPath?.Contains("Compl") == true) // "Готово" (IsARCompl и т.д.)
                {
                    await _taskService.UpdateTaskCompletedAsync(taskNumber,
                        task.IsARCompl ?? false, task.IsVKCompl ?? false, task.IsOVCompl ?? false,
                        task.IsSSCompl ?? false, task.IsESCompl ?? false, task.IsGIPCompl ?? false);
                }
                else // "Принял" (IsAR, IsVK и т.д.)
                {
                    await _taskService.UpdateTaskAssignmentsAsync(taskNumber,
                        task.IsAR ?? false, task.IsVK ?? false, task.IsOV ?? false,
                        task.IsSS ?? false, task.IsES ?? false, task.IsGIP ?? false);
                }
            }
        }

        private async void NotifyCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkbox)
            {
                bool newValue = checkbox.IsChecked == true;
                await _projectService.UpdateNotifyStatusForUserAndProject(_selectedProject, TasksETM.Properties.Settings.Default.SavedLogin, newValue);
            }
        }



        public void MovingWin(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        

        private void CreateTaskWindow_Click(object sender, RoutedEventArgs e)
        {
            var createTaskWindow = new CreateTaskWindow(_selectedProject, _dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
            createTaskWindow.Show();
            Close();
        }

        private void FilterWindow_Click(object sender, RoutedEventArgs e)
        {
            var filterTaskWindow = new FilterWindow(_selectedProject, _dbConnection, _departmentService, _projectService, _authService, _taskService, _filterTasksService);
            filterTaskWindow.Show();
            Close();
        }

       

        private void ToPrevWindow_Click(object sender, RoutedEventArgs e)
        {
            ChooseProjectWindow chooseProjectWindow = new ChooseProjectWindow(_dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
            chooseProjectWindow.Show();
            Close();
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var closeWindow = new CloseSoftwareWindow();
            closeWindow.Show();
            this.Close(); 

            await closeWindow.UpdateProgressBarAsync(); 
            Application.Current.Shutdown();

        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            if (image != null)
            {
                var imageSource = image.Source as BitmapImage;
                if (imageSource != null)
                {
                    ShowImageInNewWindow(imageSource);
                }
            }
        }
        private void ShowImageInNewWindow(BitmapImage imageSource)
        {
            var imageWindow = new ImageFullSize();
            imageWindow.SetImageSource(imageSource);
            imageWindow.ShowDialog();
        }

        private void HelpTasksButton_Click(object sender, RoutedEventArgs e)
        {
            HelpTasksWindow helpTasksWindow = new HelpTasksWindow(_selectedProject, _dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
            helpTasksWindow.Show();
            this.Close();

        }





        private void tasksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
