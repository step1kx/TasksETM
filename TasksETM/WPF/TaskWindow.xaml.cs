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
using TasksETM.WPF.ProjectsWindow;

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
            }

        }

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
                _isFiltered = false; 
            }
            catch (Exception ex)
            {
            }
        }

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
            string login = TasksETM.Properties.Settings.Default.SavedDepartmentLogin;

            if (login.Equals("GIP", StringComparison.OrdinalIgnoreCase))
            {
                ChooseProjectGIPWindow chooseProjectGIPWindow = new ChooseProjectGIPWindow(_dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
                chooseProjectGIPWindow.Show();
                Close();
            }
            else
            {
                ChooseProjectWindow chooseProjectWindow = new ChooseProjectWindow(_dbConnection, _departmentService, _projectService, _authService, _filterTasksService);
                chooseProjectWindow.Show();
                Close();
            }

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

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private async void RefreshGrid()
        {
            tasksDataGrid.Items.Refresh(); 
        }


        private async void TasksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Commit && e.Column.Header.ToString() == "Комментарий")
                {
                    var task = e.Row.Item as TaskModel;
                    if (task != null)
                    {
                        await _taskService.UpdateTaskCommentAsync(task.TaskNumber, task.TaskComment); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении комментария: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void TasksDataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            tasksDataGrid.UnselectAllCells();
        }

    }
}
