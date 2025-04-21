using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksETM.Interfaces
{
    public interface IImageManager
    {
        public string SaveImageToTempFile(System.Windows.Media.Imaging.BitmapSource image);
        public byte[] ConvertImageToBytes(string imagePath);

    }
}
