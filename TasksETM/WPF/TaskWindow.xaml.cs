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
using TasksETM.WPF;

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        private readonly string _selectedProject;

        public TaskWindow(string selectedProject)
        {
            InitializeComponent();
            _selectedProject = selectedProject;

            this.TitleBlock.Text = $"Задание смежным разделам по объекту: {selectedProject}";
        }

        private void CreateWindow_Click(object sender, RoutedEventArgs e)
        {

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
            Close();
        }

        private void tasksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
