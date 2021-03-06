using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vorcyc.WriteableBitmapEx;

namespace WriteableBitmapExBlitAlphaRepro.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var unmodifiedBmp = LoadFromFile("Assets/Overlays/19.jpg");
            var sticker = LoadFromFile("Assets/Overlays/nEW.png");
            ImgOrg.Source = unmodifiedBmp;
            ImgOrgOverlay.Source = sticker;
            
            ImgMod.Source = Overlay(unmodifiedBmp, sticker, new Point(10, 10));
            ImgModPrgba.Source = Overlay(BitmapFactory.ConvertToPbgra32Format(unmodifiedBmp), BitmapFactory.ConvertToPbgra32Format(sticker), new Point(10, 10));
        }

        public static WriteableBitmap Overlay(WriteableBitmap bmp, WriteableBitmap overlay, Point location)
        {
            var result = bmp.Clone();
            var size = new Size(overlay.PixelWidth, overlay.PixelHeight);
            result.Blit(new Rect(location, size), overlay, new Rect(new Point(0, 0), size), WriteableBitmapExtensions.BlendMode.Alpha);
            return result;
        }

        public static WriteableBitmap LoadFromFile(string fileName)
        {
            using (var fileStream = File.OpenRead(fileName))
            {
                var wb = BitmapFactory.FromStream(fileStream);
                return wb;
            }
        }
    }
}
