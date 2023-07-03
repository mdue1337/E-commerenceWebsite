using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace ImageBytes
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Path to the image
            string path = "C:\\Users\\marti\\OneDrive - TEC\\Documents\\Martins Coding Projects\\ImageToBytesAndViceVersa\\ImageBytes\\ImageBytes\\50cent.jpg";

            // Create image
            Image Photo = Image.FromFile(path);

            // convert to byte[]
            byte[] PhotoBytes = ConvertByte(Photo);

            // Send to string & Save in a file called 'text.txt'
            string byteString = BitConverter.ToString(PhotoBytes);
            StreamWriter txt = new StreamWriter("C:\\Users\\marti\\OneDrive - TEC\\Documents\\Martins Coding Projects\\ImageToBytesAndViceVersa\\ImageBytes\\ImageBytes\\text.txt");
            txt.Write(byteString);
            txt.Close();

            /// Convert back to jpeg
            Image jpegImage = ConvertBytesToImage(PhotoBytes);
            var jpeg = new Bitmap(jpegImage);
            jpeg.Save("C:\\Users\\marti\\OneDrive - TEC\\Documents\\Martins Coding Projects\\ImageToBytesAndViceVersa\\ImageBytes\\ImageBytes\\50centConvert.jpeg", ImageFormat.Jpeg);

            // QOL
            Console.WriteLine("Succes!");
            Console.ReadKey();
        }

        public static byte[] ConvertByte(Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }

        public static Image ConvertBytesToImage(byte[] byteData)
        {
            using (MemoryStream ms = new MemoryStream(byteData))
            {
                Image image = Image.FromStream(ms);
                return image;
            }
        }
    }
}
