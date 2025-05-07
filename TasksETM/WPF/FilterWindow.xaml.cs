using IssuingTasksETM.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using TasksETM.Service.Tasks;

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
        private readonly IFilterTasksService _filterTasksService;

        private List<TaskModel> _tasks;
        private List<TaskModel> _filteredTasks;

        private Grid[] _screens;
        private Grid _currentScreen;


        public FilterWindow(string selectedProject,
            IDatabaseConnection dbConnection,
            IDepartmentService departmentService,
            IProjectService projectService,
            IAuthService authService,
            ITaskService taskService,
            IFilterTasksService filterTasksService
            )
        {
            InitializeComponent();
            _selectedProject = selectedProject;
            _taskWindow = new TaskWindow(selectedProject, dbConnection, departmentService, projectService, authService, filterTasksService);
            _dbConnection = new DatabaseConnection();
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);
            _taskService = taskService;
            _filterTasksService = new FilterTasksService();

            _tasks = new List<TaskModel>();

            _screens = new[] { DepartInfoGrid, CompletedInfoGrid};
            _currentScreen = DepartInfoGrid;
            LoadTasksAsync();
            FillComboBoxAsync();
            InitializeFiltersAsync();
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

                var taskStatus = await _departmentService.GetTaskStatusAsync();

                if (departmentNames == null || !departmentNames.Any())
                {
                    MessageBox.Show("Не удалось загрузить список отделов.");
                    return;
                }

                if (taskStatus == null || !taskStatus.Any())
                {
                    MessageBox.Show("Не удалось загрузить список статусов.");
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    ToDepartComboBox.ItemsSource = departmentNames;

                    FromDepartComboBox.ItemsSource = departmentNames;

                    SectionComboBox.ItemsSource = departmentNames;

                    StatusComboBox.ItemsSource = taskStatus;

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

        private async void ApplyFilters()
        {
            _filteredTasks = _tasks.ToList();
            try
            {
                // FromDepart
                if (FromDepartComboBox.SelectedItem != null)
                {
                    string fromDepart = FromDepartComboBox.SelectedItem.ToString();
                    _filteredTasks = _filteredTasks.Where(t => t.FromDepart == fromDepart).ToList();
                    TasksETM.Properties.Settings.Default.FromDepartFilter = fromDepart;
                }

                // ToDepart
                if (ToDepartComboBox.SelectedItem != null)
                {
                    string toDepart = ToDepartComboBox.SelectedItem.ToString();
                    _filteredTasks = _filteredTasks.Where(t => t.ToDepart == toDepart).ToList();
                    TasksETM.Properties.Settings.Default.ToDepartFilter = toDepart;
                }

                // Готово/Не готово
                if (StatusComboBox.SelectedItem != null)
                {
                    string status = StatusComboBox.SelectedItem.ToString();
                    if (status == "Готов")
                        _filteredTasks = _filteredTasks.Where(t => t.IsARCompl == true || t.IsVKCompl == true ||
                            t.IsOVCompl == true || t.IsSSCompl == true || t.IsESCompl == true || t.IsGIPCompl == true).ToList();
                    else if (status == "Не готов")
                        _filteredTasks = _filteredTasks.Where(t => t.IsARCompl == false || t.IsVKCompl == false ||
                            t.IsOVCompl == false || t.IsSSCompl == false || t.IsESCompl == false || t.IsGIPCompl == false ||
                            t.IsARCompl == null || t.IsVKCompl == null || t.IsOVCompl == null || t.IsSSCompl == null ||
                            t.IsESCompl == null || t.IsGIPCompl == null).ToList();
                    TasksETM.Properties.Settings.Default.StatusFilter = status; // Исправлено на StatusFilter
                }
                else
                {
                    TasksETM.Properties.Settings.Default.StatusFilter = null;
                }

                // Раздел (работает только если статус выбран)
                if (SectionComboBox.IsEnabled && SectionComboBox.SelectedItem != null)
                {
                    string section = SectionComboBox.SelectedItem.ToString();
                    _filteredTasks = _filteredTasks.Where(t =>
                        (section == "AR" && t.IsAR == true) ||
                        (section == "VK" && t.IsVK == true) ||
                        (section == "OV" && t.IsOV == true) ||
                        (section == "SS" && t.IsSS == true) ||
                        (section == "ES" && t.IsES == true) ||
                        (section == "GIP" && t.IsGIP == true)).ToList();
                    TasksETM.Properties.Settings.Default.SectionFilter = section; // Исправлено на SectionFilter
                }
                else
                {
                    TasksETM.Properties.Settings.Default.SectionFilter = null;
                }

                // TaskDate и TaskDeadline
                if (!string.IsNullOrEmpty(TaskDateTextBox.Text))
                {
                    _filteredTasks = _filteredTasks.Where(t => t.TaskDate == TaskDateTextBox.Text).ToList();
                    TasksETM.Properties.Settings.Default.TaskDateFilter = TaskDateTextBox.Text;
                }
                if (!string.IsNullOrEmpty(TaskDeadLineTextBox.Text))
                {
                    _filteredTasks = _filteredTasks.Where(t => t.TaskDeadline == TaskDeadLineTextBox.Text).ToList();
                    TasksETM.Properties.Settings.Default.TaskDeadlineFilter = TaskDeadLineTextBox.Text;
                }

                TasksETM.Properties.Settings.Default.Save();
                await _filterTasksService.SaveFilterSettingsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"АХТУНГ: {ex.Message}");
            }
        }
        //private async void ApplyFilters()
        //{
        //    _filteredTasks = _tasks.ToList();
        //    try
        //    {
        //        // FromDepart
        //        if (FromDepartComboBox.SelectedItem != null)
        //        {
        //            string fromDepart = FromDepartComboBox.SelectedItem.ToString();
        //            _filteredTasks = _filteredTasks
        //                .Where(t => t.FromDepart == fromDepart)
        //                .ToList();
        //            TasksETM.Properties.Settings.Default.FromDepartFilter = fromDepart;
        //        }

        //        // ToDepart
        //        if (ToDepartComboBox.SelectedItem != null)
        //        {
        //            string toDepart = ToDepartComboBox.SelectedItem.ToString();
        //            _filteredTasks = _filteredTasks
        //                .Where(t => t.ToDepart == toDepart)
        //                .ToList();
        //            TasksETM.Properties.Settings.Default.ToDepartFilter = toDepart;
        //        }

        //        //TaskCompleted


        //        // TaskDate
        //        if (!string.IsNullOrEmpty(TaskDateTextBox.Text))
        //        {
        //            _filteredTasks = _filteredTasks
        //                .Where(t => t.TaskDate == TaskDateTextBox.Text)
        //                .ToList();
        //            TasksETM.Properties.Settings.Default.TaskDateFilter = TaskDateTextBox.Text;
        //        }

        //        if (!string.IsNullOrEmpty(TaskDeadLineTextBox.Text))
        //        {
        //            _filteredTasks = _filteredTasks
        //                .Where(t => t.TaskDeadline == TaskDeadLineTextBox.Text)
        //                .ToList();
        //            TasksETM.Properties.Settings.Default.TaskDeadlineFilter = TaskDeadLineTextBox.Text;
        //        }

        //        await _filterTasksService.SaveFilterSettingsAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"АХТУНГ: {ex.Message}");
        //    }


        //}

        private async void FilterSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                ApplyFilters();
                _taskWindow.UpdateTasks(_filteredTasks);
                _taskWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при применении фильтров: {ex.Message}");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusComboBox.SelectedIndex > -1) // "Готово" или "Не готово"
            {
                SectionComboBox.IsEnabled = true;
            }
            else
            {
                SectionComboBox.IsEnabled = false;
                SectionComboBox.SelectedIndex = -1; // Сбрасываем выбор раздела
                TasksETM.Properties.Settings.Default.SectionFilter = null;
            }
            ApplyFilters();
        }


        //private async Task InitializeFiltersAsync()
        //{
        //    await _filterTasksService.LoadFilterSettingsAsync();

        //    // Восстанавливаем ComboBox
        //    if (!string.IsNullOrEmpty(TasksETM.Properties.Settings.Default.FromDepartFilter))
        //    {
        //        FromDepartComboBox.SelectedItem = TasksETM.Properties.Settings.Default.FromDepartFilter;
        //    }

        //    if (!string.IsNullOrEmpty(TasksETM.Properties.Settings.Default.ToDepartFilter))
        //    {
        //        ToDepartComboBox.SelectedItem = TasksETM.Properties.Settings.Default.ToDepartFilter;
        //    }

        //    if (!string.IsNullOrEmpty(TasksETM.Properties.Settings.Default.TaskCompletedFilter))
        //    {
        //        TaskCompletedComboBox.SelectedItem = TasksETM.Properties.Settings.Default.TaskCompletedFilter;
        //    }

        //    // Восстанавливаем TextBox
        //    TaskDateTextBox.Text = TasksETM.Properties.Settings.Default.TaskDateFilter;
        //    TaskDeadLineTextBox.Text = TasksETM.Properties.Settings.Default.TaskDeadlineFilter;

        //}

        private async Task InitializeFiltersAsync()
        {
            await _filterTasksService.LoadFilterSettingsAsync();
            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(TasksETM.Properties.Settings.Default.FromDepartFilter))
                    FromDepartComboBox.SelectedItem = TasksETM.Properties.Settings.Default.FromDepartFilter;
                if (!string.IsNullOrEmpty(TasksETM.Properties.Settings.Default.ToDepartFilter))
                    ToDepartComboBox.SelectedItem = TasksETM.Properties.Settings.Default.ToDepartFilter;
                if (!string.IsNullOrEmpty(TasksETM.Properties.Settings.Default.StatusFilter))
                    StatusComboBox.SelectedItem = TasksETM.Properties.Settings.Default.StatusFilter;
                if (!string.IsNullOrEmpty(TasksETM.Properties.Settings.Default.SectionFilter))
                    SectionComboBox.SelectedItem = TasksETM.Properties.Settings.Default.SectionFilter;
                TaskDateTextBox.Text = TasksETM.Properties.Settings.Default.TaskDateFilter;
                TaskDeadLineTextBox.Text = TasksETM.Properties.Settings.Default.TaskDeadlineFilter;
                SectionComboBox.IsEnabled = StatusComboBox.SelectedIndex > 0;
            });
        }

        private void ClearFromDepartComboBox_Click(object sender, RoutedEventArgs e)
        {
            FromDepartComboBox.SelectedIndex = -1;
            TasksETM.Properties.Settings.Default.FromDepartFilter = null;
        }

        private void ClearToDepartComboBox_Click(object sender, RoutedEventArgs e)
        {
            ToDepartComboBox.SelectedIndex = -1;
            TasksETM.Properties.Settings.Default.ToDepartFilter = null;
        }


        private void ClearSectionComboBox_Click(object sender, RoutedEventArgs e)
        {
            SectionComboBox.SelectedIndex = -1;
            TasksETM.Properties.Settings.Default.TaskCompletedFilter = null;
        }

        

        private async void ClearFilterSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FromDepartComboBox.SelectedItem = null;
                ToDepartComboBox.SelectedItem = null;
                StatusComboBox.SelectedItem = null;
                SectionComboBox.SelectedItem = null;
                TaskDateTextBox.Text = string.Empty;
                TaskDeadLineTextBox.Text = string.Empty;

                // Сбрасываем настройки
                TasksETM.Properties.Settings.Default.FromDepartFilter = null;
                TasksETM.Properties.Settings.Default.ToDepartFilter = null;
                TasksETM.Properties.Settings.Default.StatusFilter = null; // Исправлено
                TasksETM.Properties.Settings.Default.SectionFilter = null;
                TasksETM.Properties.Settings.Default.TaskDateFilter = null;
                TasksETM.Properties.Settings.Default.TaskDeadlineFilter = null;

                // Сохраняем сброшенные настройки
                await _filterTasksService.SaveFilterSettingsAsync();


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
