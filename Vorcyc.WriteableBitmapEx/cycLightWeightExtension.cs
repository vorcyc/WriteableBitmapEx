#region Header
//
//   Project:           WriteableBitmapEx - WriteableBitmap extensions
//   Description:       Collection of extension methods for the WriteableBitmap class.
//
//   Author:            duanlinli aka cyclone_dll
//
#endregion

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if NETFX_CORE
using Windows.Foundation;

namespace Windows.UI.Xaml.Media.Imaging
#else
namespace Vorcyc.WriteableBitmapEx
#endif
{
    //轻量级扩展

    static partial class WriteableBitmapExtensions
    {
        public static void DrawLinesAa(this WriteableBitmap bitmap, int[] points, Color color)
        {
            if (points.Length < 2)
            {
                return;
            }

            // Draw all points
            for (int i = 0; i < points.Length - 3; i += 2)
            {
                var startX = points[i];
                var startY = points[i + 1];
                var endX = points[i + 2];
                var endY = points[i + 3];

                bitmap.DrawLineAa(startX, startY, endX, endY, color);
            }
        }

        public static void DrawLines(this WriteableBitmap bitmap, int[] points, Color color)
        {
            if (points.Length < 2)
            {
                return;
            }

            // Draw all points
            for (int i = 0; i < points.Length - 3; i += 2)
            {
                var startX = points[i];
                var startY = points[i + 1];
                var endX = points[i + 2];
                var endY = points[i + 3];

                bitmap.DrawLineDDA(startX, startY, endX, endY, color);
            }
        }


        public unsafe static void ForEach(this WriteableBitmap bitmap,
            Func<int, int, int> func)
        {
            using (var context = bitmap.GetBitmapContext())
            {
                var pixels = context.Pixels;
                int w = context.Width;
                int h = context.Height;
                int index = 0;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        pixels[index++] = func(x, y);
                    }
                }
            }
        }


        public unsafe static void ForEach(this WriteableBitmap bitmap,
            Func<int, int, Color, int> func)
        {
            using (var context = bitmap.GetBitmapContext())
            {
                var pixels = context.Pixels;
                int w = context.Width;
                int h = context.Height;
                int index = 0;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var c = pixels[index];

                        // Premultiplied Alpha!
                        var a = (byte)(c >> 24);
                        // Prevent division by zero
                        int ai = a;
                        if (ai == 0)
                        {
                            ai = 1;
                        }
                        // Scale inverse alpha to use cheap integer mul bit shift
                        ai = ((255 << 8) / ai);
                        var srcColor = Color.FromArgb(a,
                                                      (byte)((((c >> 16) & 0xFF) * ai) >> 8),
                                                      (byte)((((c >> 8) & 0xFF) * ai) >> 8),
                                                      (byte)((((c & 0xFF) * ai) >> 8)));

                        var color = func(x, y, srcColor);
                        pixels[index++] = color;
                    }
                }
            }
        }



        public static void DrawQuad(this WriteableBitmap bitmap, IntPoint p1, IntPoint p2, IntPoint p3, IntPoint p4, Color color)
        {
            using (var context = bitmap.GetBitmapContext())
            {
                bitmap.DrawQuad(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, color);
            }
        }


        public static void FillQuad(this WriteableBitmap bitmap, IntPoint p1, IntPoint p2, IntPoint p3, IntPoint p4, Color color)
        {
            using (var context = bitmap.GetBitmapContext())
            {
                bitmap.FillQuad(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, color);
            }
        }



        /// <summary>
        /// draw a dot (cyclone).if radius =0 ,draw a single pixel dot.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawDot(this WriteableBitmap bitmap, int x, int y, int radius, Color color, DotStyle dotStyle = DotStyle.Circle)
        {
            using (var context = bitmap.GetBitmapContext())
            {

                if (dotStyle == DotStyle.Quad)
                {
                    //左上，右上，右下，左下
                    if (radius > 0)
                    {
                        bitmap.DrawQuad(x - radius, y - radius, x + radius, y - radius, x + radius, y + radius, x - radius, y + radius, color);
                    }
                    else if (radius == 0)
                    {
                        bitmap.FillRectangle(x, y, x + 1, y + 1, color);
                    }
                }
                else if (dotStyle == DotStyle.Circle)
                {
                    if (radius > 0)
                    {
                        bitmap.DrawEllipseCentered(x, y, radius, radius, color);
                    }
                    else if (radius == 0)
                    {
                        //用FillEllipseCentered就错了
                        bitmap.FillRectangle(x, y, x + 1, y + 1, color);
                    }
                }
            }
        }

        /// <summary>
        /// Draw a dot with fill.(cyclone)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="radius"></param>
        /// <param name="strokeColor"></param>
        /// <param name="fillColor"></param>
        /// <param name="dotStyle"></param>
        public static void DrawDot(this WriteableBitmap bitmap, int x, int y, int radius, Color strokeColor, Color fillColor, DotStyle dotStyle = DotStyle.Circle)
        {
            using (var context = bitmap.GetBitmapContext())
            {

                if (dotStyle == DotStyle.Quad)
                {

                    if (radius > 0)
                    {
                        //左上，右上，右下，左下
                        var p1 = new IntPoint(x - radius, y - radius);
                        var p2 = new IntPoint(x + radius, y - radius);
                        var p3 = new IntPoint(x + radius, y + radius);
                        var p4 = new IntPoint(x - radius, y + radius);

                        bitmap.FillQuad(p1, p2, p3, p4, fillColor);
                        bitmap.DrawQuad(p1, p2, p3, p4, strokeColor);
                    }
                    else if (radius == 0)
                    {
                        bitmap.FillRectangle(x, y, x + 1, y + 1, strokeColor);
                    }

                }
                else if (dotStyle == DotStyle.Circle)
                {

                    if (radius > 0)
                    {
                        bitmap.FillEllipseCentered(x, y, radius, radius, fillColor);
                        bitmap.DrawEllipseCentered(x, y, radius, radius, strokeColor);
                    }
                    else if (radius == 0)
                    {
                        //用FillEllipseCentered就错了。而且相比这个时候效果好
                        bitmap.FillRectangle(x, y, x + 1, y + 1, strokeColor);
                    }

                }

            }
        }

        public enum DotStyle
        {
            Quad,
            Circle,
        }



        public static void DrawDots(this WriteableBitmap bitmap, int[] points, int radius, Color strokeColor, DotStyle dotStyle = DotStyle.Circle)
        {
            using (var context = bitmap.GetBitmapContext())
            {

                // Draw all points
                for (int i = 0; i < points.Length; i += 2)
                {
                    var startX = points[i];
                    var startY = points[i + 1];

                    bitmap.DrawDot(startX, startY, radius, strokeColor, dotStyle);
                }

            }
        }

        public static void DrawDots(this WriteableBitmap bitmap, int[] points, int radius, Color strokeColor, Color fillColor, DotStyle dotStyle = DotStyle.Circle)
        {
            using (var context = bitmap.GetBitmapContext())
            {

                // Draw all points
                for (int i = 0; i < points.Length; i += 2)
                {
                    var startX = points[i];
                    var startY = points[i + 1];

                    bitmap.DrawDot(startX, startY, radius, strokeColor, fillColor, dotStyle);
                }

            }
        }

        /// <summary>
        /// 用指定格式保存<see cref="WriteableBitmap"/>至文件，自动加扩展名。(Emf,Exif,Icon）未实现
        /// </summary>
        /// <param name="bitmap">一个<see cref="WriteableBitmap"/>对象</param>
        /// <param name="filename">保存的文件名</param>
        /// <param name="format">图片格式</param>
        /// <param name="addExtensionName">是否依照格式自动追加扩展名</param>
        public static void Save(this WriteableBitmap bitmap, string filename, ImageFormat format, bool addExtensionName = false)
        {
            if (bitmap == null)
                throw new InvalidOperationException(nameof(bitmap));

            RenderTargetBitmap rtbitmap = new RenderTargetBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, PixelFormats.Default);
            DrawingVisual drawingVisual = new DrawingVisual();

            using (var dc = drawingVisual.RenderOpen())
            {
                dc.DrawImage(bitmap, new Rect(0, 0, bitmap.Width, bitmap.Height));
            }

            rtbitmap.Render(drawingVisual);

            BitmapEncoder encoder = null;
            string finnalFilename = filename;

            switch (format)
            {
                case ImageFormat.Bmp:
                    encoder = new BmpBitmapEncoder();
                    finnalFilename = $"{filename}.bmp";
                    break;
                case ImageFormat.Emf:
                    break;
                case ImageFormat.Wmf:
                    encoder = new WmpBitmapEncoder();
                    finnalFilename = $"{filename}.wmf";
                    break;
                case ImageFormat.Gif:
                    encoder = new GifBitmapEncoder();
                    finnalFilename = $"{filename}.gif";
                    break;
                case ImageFormat.Jpeg:
                    encoder = new JpegBitmapEncoder();
                    finnalFilename = $"{filename}.jpg";
                    break;
                case ImageFormat.Png:
                    encoder = new PngBitmapEncoder();
                    finnalFilename = $"{filename}.png";
                    break;
                case ImageFormat.Tiff:
                    encoder = new TiffBitmapEncoder();
                    finnalFilename = $"{filename}.tiff";
                    break;
                case ImageFormat.Exif:
                    break;
                case ImageFormat.Icon:
                    break;
                default:
                    break;
            }

            encoder.Frames.Add(BitmapFrame.Create(rtbitmap));
            if (addExtensionName)
                encoder.Save(File.OpenWrite(finnalFilename));
            else
                encoder.Save(File.OpenWrite(filename));


        }



    }

    //参考 System.Drawing.Imaging
    /// <summary>
    /// 保存的图片格式
    /// </summary>
    public enum ImageFormat
    {
        Bmp,
        Emf,
        Wmf,
        Gif,
        Jpeg,
        Png,
        Tiff,
        Exif,
        Icon,
    }
}
