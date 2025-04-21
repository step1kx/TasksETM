using IssuingTasksETM.Interfaces;
using IssuingTasksETM.WPF;
using System.Windows;
using System.Windows.Controls;
using TasksETM.Service;


namespace TasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для ChooseProjectWindow.xaml
    /// </summary>
    public partial class ChooseProjectWindow : Window
    {
        private readonly IDatabaseConnection _dbConnection;

        public ChooseProjectWindow(IDatabaseConnection dbConnection = null)
        {
            InitializeComponent();
            _dbConnection = dbConnection ?? new DatabaseConnection();
            FillComboBox();
        }

        private void FillComboBox()
        {
            _dbConnection.FillProjects(ProjectsComboBox);
        }

        private void ToChoosenProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsComboBox.SelectedItem != null)
            {
                string selectedProject = ProjectsComboBox.SelectedItem.ToString();

                var taskWindow = new TaskWindow(selectedProject, _dbConnection);
                taskWindow.Show();
                Close();
            }
        }



        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ToPrevWindow_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow(_dbConnection);
            loginWindow.Show();
            Close();
        }

        

        private void ProjectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
