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
using TasksETM.Interfaces;
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

        private Grid[] _screens;
        private Grid _currentScreen;


        public FilterWindow(string selectedProject,
            IDatabaseConnection dbConnection,
            IDepartmentService departmentService,
            IProjectService projectService,
            IAuthService authService
            )
        {
            InitializeComponent();
            _selectedProject = selectedProject;
            _taskWindow = new TaskWindow(selectedProject, dbConnection, departmentService, projectService, authService);
            _dbConnection = new DatabaseConnection();
            _departmentService = new DepartmentService();
            _projectService = new ProjectService();
            _authService = new AuthService(DatabaseConnection.connString);

            _screens = new[] { DepartInfoGrid, AcceptedInfoGrid, CompletedInfoGrid};
            _currentScreen = DepartInfoGrid;
            FillComboBoxAsync();
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
                    if (ToDepartComboBox.Items.Count > 0)
                    {
                        ToDepartComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("ToDepartComboBox не заполнен: список отделов пуст.");
                    }

                    FromDepartComboBox.ItemsSource = departmentNames;
                    if (FromDepartComboBox.Items.Count > 0)
                    {
                        FromDepartComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        MessageBox.Show("FromDepartComboBox не заполнен: список отделов пуст.");
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Что-то пошло не так: {ex.Message}");
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

        private void AcceptedShowButton_Click(object sender, RoutedEventArgs e)
        {
            ShowScreen(AcceptedInfoGrid);
        }

        private void CompletedShowButton_Click(object sender, RoutedEventArgs e)
        {
            ShowScreen(CompletedInfoGrid);
        }


        private void FilterSettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearFilterSettingsButton_Click(object sender, RoutedEventArgs e)
        {

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
