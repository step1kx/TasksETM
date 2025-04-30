using IssuingTasksETM.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TasksETM.Converters;
using TasksETM.Interfaces;
using TasksETM.Interfaces.ITasks;
using TasksETM.Models;
using TasksETM.Service;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для FilterWindow.xaml
    /// </summary>
    public partial class FilterWindow : Window
    {
        private readonly TaskWindow _taskWindow;
        private readonly IDatabaseConnection _dbConnection;
        private readonly string _selectedProject;
        private readonly IDepartmentService _departmentService;
        private readonly IProjectService _projectService;
        private readonly IAuthService _authService; 
        private readonly ITaskService _taskService;

        private List<TaskModel> _tasks;
        private List<TaskModel> _filteredTasks;

        private Grid[] _screens;
        private Grid _currentScreen;


        public FilterWindow(string selectedProject,
            IDatabaseConnection dbConnection,
            IDepartmentService departmentService,
            IProjectService projectService,
            IAuthService authService,
            ITaskService taskService
            )
        {
            InitializeComponent();
            _selectedProject = selectedProject;
            _taskWindow = new TaskWindow(selectedProject, dbConnection, departmentService, projectService, authService);
            _dbConnection = new DatabaseConnection();
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);
            _taskService = taskService;

            _tasks = new List<TaskModel>();

            _screens = new[] { DepartInfoGrid, CompletedInfoGrid};
            _currentScreen = DepartInfoGrid;
            LoadTasksAsync();
            FillComboBoxAsync();
        }

        private async Task LoadTasksAsync()
        {
            try
            {
                _tasks = await _taskService.GetTasksByProjectAsync(_selectedProject );
                _filteredTasks = new List<TaskModel>(_tasks);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}");
            }
        }

        private async Task FillComboBoxAsync()
        {
            try
            {
                if (_departmentService == null)
                {
                    MessageBox.Show("Сервис отделов не инициализирован.");
                    return;
                }

                var departmentNames = await _departmentService.GetDepartmentNamesAsync();

                if (departmentNames == null || !departmentNames.Any())
                {
                    MessageBox.Show("Не удалось загрузить список отделов.");
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    ToDepartComboBox.ItemsSource = departmentNames;

                    FromDepartComboBox.ItemsSource = departmentNames;

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отделов: {ex.Message}");
            }
        }

        private void ShowScreen(Grid screenToShow)
        {
            if (_currentScreen == screenToShow) return; 

            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");

            int completedAnimations = 0;
            int totalAnimations = 0;

            foreach (var screen in _screens)
            {
                if (screen != screenToShow && screen.Visibility == Visibility.Visible)
                {
                    totalAnimations++;
                    var fadeOutClone = fadeOut.Clone(); 
                    fadeOutClone.Completed += (s, args) =>
                    {
                        screen.Visibility = Visibility.Collapsed;
                        completedAnimations++;

                        
                        if (completedAnimations == totalAnimations)
                        {
                            screenToShow.Visibility = Visibility.Visible;
                            fadeIn.Begin(screenToShow);
                            _currentScreen = screenToShow; 
                        }
                    };
                    fadeOutClone.Begin(screen);
                }
            }

            if (totalAnimations == 0)
            {
                screenToShow.Visibility = Visibility.Visible;
                fadeIn.Begin(screenToShow);
                _currentScreen = screenToShow;
            }
        }



        private void DepartShowButton_Click(object sender, RoutedEventArgs e)
        {
            ShowScreen(DepartInfoGrid);
        }

        //private void AcceptedShowButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ShowScreen(AcceptedInfoGrid);
        //}

        private void CompletedShowButton_Click(object sender, RoutedEventArgs e)
        {
            ShowScreen(CompletedInfoGrid);
        }


        private void FilterSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _filteredTasks = _tasks.ToList(); 

                if (FromDepartComboBox.SelectedItem != null)
                {
                    string selectedFrom = FromDepartComboBox.SelectedItem.ToString();
                    _filteredTasks = _filteredTasks.Where(t => t.FromDepart == selectedFrom).ToList();
                }
                if (ToDepartComboBox.SelectedItem != null)
                {
                    string selectedTo = ToDepartComboBox.SelectedItem.ToString();
                    _filteredTasks = _filteredTasks.Where(t => t.ToDepart == selectedTo).ToList();
                }

                if (TaskCompletedComboBox.SelectedItem != null)
                {
                    var selectedItem = TaskCompletedComboBox.SelectedItem as ComboBoxItem;
                    bool isCompleted = selectedItem?.Content.ToString() == "Готово";
                    _filteredTasks = _filteredTasks
                        .Where(t => t.TaskCompleted.HasValue && t.TaskCompleted.Value == isCompleted)
                        .ToList();
                }

                if (!string.IsNullOrEmpty(TaskDateTextBox.Text) && DateTime.TryParse(TaskDateTextBox.Text, out DateTime issueDate))
                {
                    _filteredTasks = _filteredTasks.Where(t => DateTime.TryParse(t.TaskDate, out DateTime taskDate) && taskDate.Date == issueDate.Date).ToList();
                }
                if (!string.IsNullOrEmpty(TaskDeadLineTextBox.Text) && DateTime.TryParse(TaskDeadLineTextBox.Text, out DateTime deadline))
                {
                    _filteredTasks = _filteredTasks.Where(t => !string.IsNullOrEmpty(t.TaskDeadline) && DateTime.TryParse(t.TaskDeadline, out DateTime taskDeadline) && taskDeadline.Date == deadline.Date).ToList();
                }

                //if (WhoTakenComboBox.SelectedItem != null)
                //{
                //    string selectedDepartment = WhoTakenComboBox.SelectedItem.ToString();
                //    _filteredTasks = _filteredTasks.Where(t =>
                //        (selectedDepartment == "AR" && t.IsAR == true) ||
                //        (t.IsVK == true) ||
                //        (t.IsOV == true) ||
                //        (t.IsSS == true) ||
                //        (t.IsES == true)
                //    ).ToList();
                //}

                _taskWindow.UpdateTasks(_filteredTasks);
                _taskWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при применении фильтров: {ex.Message}");
            }
        }

        private void ClearFromDepartComboBox_Click(object sender, RoutedEventArgs e)
        {
            FromDepartComboBox.SelectedIndex = -1; 
        }

        private void ClearToDepartComboBox_Click(object sender, RoutedEventArgs e)
        {
            ToDepartComboBox.SelectedIndex = -1;
        }

        //private void ClearWhoTakenComboBox_Click(object sender, RoutedEventArgs e)
        //{
        //    WhoTakenComboBox.SelectedIndex = -1;
        //}

        private void ClearTaskCompletedComboBox_Click(object sender, RoutedEventArgs e)
        {
            TaskCompletedComboBox.SelectedIndex = -1; 
        }

        //private void ClearIsAcceptedComboBox_Click(object sender, RoutedEventArgs e)
        //{
        //    IsAcceptedComboBox.SelectedIndex = -1;
        //}

        

        private void ClearFilterSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FromDepartComboBox.SelectedIndex = -1;
                ToDepartComboBox.SelectedIndex = -1;
                //WhoTakenComboBox.SelectedIndex = -1;
                TaskCompletedComboBox.SelectedIndex = -1;
                //IsAcceptedComboBox.SelectedIndex = -1;
                TaskDateTextBox.Text = null;
                TaskDeadLineTextBox.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сбросе фильтров: {ex.Message}");
            }
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
            _taskWindow.Show();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _taskWindow.Show();
            Close();
        }
    }


}
