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

namespace IssuingTasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        public CreateTaskWindow()
        {
            InitializeComponent();
        }

        private void TaskInfoShowButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");

            fadeOut.Completed += (s, args) =>
            {
                BaseInfoGrid.Visibility = Visibility.Collapsed;
                TasksInfoGrid.Visibility = Visibility.Visible;
                fadeIn.Begin(TasksInfoGrid);
            };

            fadeOut.Begin(BaseInfoGrid);
        }

        private void BaseInfoShowButton_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");

            fadeOut.Completed += (s, args) =>
            {
                TasksInfoGrid.Visibility = Visibility.Collapsed;
                BaseInfoGrid.Visibility = Visibility.Visible;
                fadeIn.Begin(BaseInfoGrid);
            };

            fadeOut.Begin(TasksInfoGrid);
        }

        public void MovingWin(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
