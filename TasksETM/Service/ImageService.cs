using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TasksETM.Interfaces;

namespace TasksETM.Service
{
    public class ImageService : IImageService
    {
        public byte[] ConvertImageToBytes(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return null;

            return System.IO.File.ReadAllBytes(imagePath);
        }

        public string SaveImageToTempFile(BitmapSource image)
        {
            string tempFilePath = System.IO.Path.GetTempFileName();
            tempFilePath = System.IO.Path.ChangeExtension(tempFilePath, ".png");

            using (var fileStream = new System.IO.FileStream(tempFilePath, System.IO.FileMode.Create))
            {
                var pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(image));
                pngEncoder.Save(fileStream);
            }

            return tempFilePath;
        }
    }
}
