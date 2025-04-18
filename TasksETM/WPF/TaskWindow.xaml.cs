using IssuingTasksETM.Interfaces;
using IssuingTasksETM.Models;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TasksETM.Interfaces;
using TasksETM.Service;
using TasksETM.WPF;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        private readonly string _selectedProject;
        private readonly TaskManager _taskManager;
        private readonly IDatabaseConnection _dbConnection;


        public TaskWindow(string selectedProject, IDatabaseConnection dbConnection)
        {
            InitializeComponent();
            _selectedProject = selectedProject;
            _dbConnection = dbConnection;
            _taskManager = new TaskManager(DatabaseConnection.connString);
            

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
                var table = await _taskManager.GetTasksByProjectAsync(_selectedProject);
                tasksDataGrid.ItemsSource = table.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }



        private void CreateTaskWindow_Click(object sender, RoutedEventArgs e)
        {
            var createTaskWindow = new CreateTaskWindow(_selectedProject, _dbConnection);
            createTaskWindow.Show();
            Close();
        }

        private void FilterWindow_Click(object sender, RoutedEventArgs e)
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
            ChooseProjectWindow chooseProjectWindow = new ChooseProjectWindow();
            chooseProjectWindow.Show();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void tasksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
