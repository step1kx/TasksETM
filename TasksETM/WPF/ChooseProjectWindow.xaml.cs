using IssuingTasksETM.Interfaces;
using IssuingTasksETM.Models;
using IssuingTasksETM.WPF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
            if (_dbConnection.FillProjects(ProjectsComboBox))
            {
                MessageBox.Show("ComboBox успешно заполнился");           
            }
        }

        private void ChooseProjects(object sender, RoutedEventArgs e)
        {
            if (ProjectsComboBox.SelectedItem != null)
            {
                string selectedProject = ProjectsComboBox.SelectedItem.ToString();
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

        private void ZaglushkaButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
