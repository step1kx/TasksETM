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
    /// Логика взаимодействия для ImageFullSize.xaml
    /// </summary>
    public partial class ImageFullSize : Window
    {
        public ImageFullSize()
        {
            InitializeComponent();
        }

        public void SetImageSource(BitmapImage imageSource)
        {
            DisplayedImage.Source = imageSource;
        }
    }
}
