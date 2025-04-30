using IssuingTasksETM.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        private List<TaskModel> _tasks;
        private readonly string _selectedProject;
        private readonly ITaskService _taskManager;
        private readonly IDatabaseConnection _dbConnection;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;
        private bool _isFiltered;


        public TaskWindow(string selectedProject, 
            IDatabaseConnection dbConnection, 
            IDepartmentService departmentService, 
            IProjectService projectService,
            IAuthService authService)
        {
            InitializeComponent();
            _selectedProject = selectedProject;
            _dbConnection = dbConnection;
            _departmentService = departmentService ?? new DepartmentService(); 
            _projectService = projectService ?? new ProjectService();
            _authService = authService ?? new AuthService(DatabaseConnection.connString);
            _authService = new AuthService(DatabaseConnection.connString);
            _taskManager = new TaskService(DatabaseConnection.connString);
            _isFiltered = false;

            tasksDataGrid.ItemsSource = _tasks;


            this.TitleBlock.Text = $"Задание смежным разделам по объекту: {selectedProject}";

            Loaded += TaskWindow_Loaded;
        }

        private async void TaskWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Загружаем данные только если фильтры не применены
            if (!_isFiltered)
            {
                await LoadAsync();
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                var tasks = await _taskManager.GetTasksByProjectAsync(_selectedProject);
                if (tasks == null || !tasks.Any())
                {
                    MessageBox.Show("Нет данных для отображения.");
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
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
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

                if (checkBox.IsChecked == task.TaskCompleted)
                {
                    var isCompleted = task.TaskCompleted ?? false;
                    await _taskManager.UpdateTaskCompletedAsync(taskNumber, isCompleted);
                }
                else
                {
                    var isAR = task.IsAR ?? false;
                    var isVK = task.IsVK ?? false;
                    var isOV = task.IsOV ?? false;
                    var isSS = task.IsSS ?? false;
                    var isES = task.IsES ?? false;

                    await _taskManager.UpdateTaskAssignmentsAsync(taskNumber, isAR, isVK, isOV, isSS, isES);
                }
               
            }
        }

        private void CreateTaskWindow_Click(object sender, RoutedEventArgs e)
        {
            var createTaskWindow = new CreateTaskWindow(_selectedProject, _dbConnection, _departmentService, _projectService, _authService);
            createTaskWindow.Show();
            Close();
        }

        private void FilterWindow_Click(object sender, RoutedEventArgs e)
        {
            var filterTaskWindow = new FilterWindow(_selectedProject, _dbConnection, _departmentService, _projectService, _authService, _taskManager);
            filterTaskWindow.Show();
            Close();
        }

        public void MovingWin(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ToPrevWindow_Click(object sender, RoutedEventArgs e)
        {
            ChooseProjectWindow chooseProjectWindow = new ChooseProjectWindow(_dbConnection, _departmentService, _projectService, _authService);
            chooseProjectWindow.Show();
            Close();
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var closeWindow = new CloseSoftwareWindow();
            closeWindow.Show();
            await closeWindow.UpdateProgressBarAsync();
            DialogResult = false;
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

        private void tasksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
