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
        private readonly string _selectedProject;
        private readonly TaskService _taskManager;
        private readonly IDatabaseConnection _dbConnection;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService;


        public TaskWindow(string selectedProject, 
            IDatabaseConnection dbConnection, 
            IDepartmentService departmentService, 
            IProjectService projectService,
            IAuthService authService)
        {
            InitializeComponent();
            _selectedProject = selectedProject;
            _dbConnection = dbConnection;
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);
            _taskManager = new TaskService(DatabaseConnection.connString);
            

            this.TitleBlock.Text = $"Задание смежным разделам по объекту: {selectedProject}";

            Loaded += TaskWindow_Loaded;
        }

        private void TaskWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAsync(); 
        }

        public async void LoadAsync()
        {
            try
            {
                var tasks = await _taskManager.GetTasksByProjectAsync(_selectedProject);

                if (tasks == null || !tasks.Any())
                {
                    MessageBox.Show("Нет данных для отображения.");
                }
                else
                {
                    tasksDataGrid.ItemsSource = tasks;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private async void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TaskModel task)
            {
                var taskNumber = task.TaskNumber;

                var isAR = task.IsAR ?? false;
                var isVK = task.IsVK ?? false;
                var isOV = task.IsOV ?? false;
                var isSS = task.IsSS ?? false;
                var isES = task.IsES ?? false;

                await _taskManager.UpdateTaskAssignmentsAsync(taskNumber, isAR, isVK, isOV, isSS, isES);

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
            var filterTaskWindow = new FilterWindow();
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
            var loadingWindow = new Window
            {
                Width = 300,
                Height = 100,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new StackPanel
                {
                    Children =
            {
                new TextBlock { Text = "Завершаем работу..." },
                new ProgressBar { IsIndeterminate = true }
            }
                }
            };

            loadingWindow.Show();

            await Task.Delay(2000);

            DialogResult = false;
            loadingWindow.Close();
            Close();
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
