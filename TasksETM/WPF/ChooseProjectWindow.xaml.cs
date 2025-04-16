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

namespace TasksETM.WPF
{
    /// <summary>
    /// Логика взаимодействия для ChooseProjectWindow.xaml
    /// </summary>
    public partial class ChooseProjectWindow : Window
    {
        public ChooseProjectWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ZaglushkaButton_Click(Object sender, RoutedEventArgs e)
        {
            MessageBox.Show("круто!");
        }
    }
}
