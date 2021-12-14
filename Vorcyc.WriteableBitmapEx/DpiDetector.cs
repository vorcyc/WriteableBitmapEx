#region Header
//
//   Project:           WriteableBitmapEx - WriteableBitmap extensions
//   Description:       Collection of extension methods for the WriteableBitmap class.
//
//   Author:            duanlinli aka cyclone_dll
//
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Vorcyc.WriteableBitmapEx
{
    public static class DpiDetector
    {
        private static double? _dpiXKoef;

        public static double DpiXKoef
        {
            get
            {
                if (_dpiXKoef == null)
                {
                    using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        _dpiXKoef = graphics.DpiX / 96.0;
                    }
                }
                return _dpiXKoef ?? 1;
            }
        }

        private static double? _dpiYKoef;

        public static double DpiYKoef
        {
            get
            {
                if (_dpiYKoef==null)
                {
                    using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        _dpiYKoef= graphics.DpiY / 96.0;
                    }
                }
                return _dpiYKoef ?? 1;

                //return Screen.PrimaryScreen.WorkingArea.Width / SystemParameters.WorkArea.Width;
                //WantGlobalTransformMatrix();
                //if (_globalTransformPatrix.HasValue) return _globalTransformPatrix.Value.M22;
                //return 1;
            }
        }

        //private static void WantGlobalTransformMatrix()
        //{
        //    if (_globalTransformPatrix != null) return;
        //    try
        //    {
        //        _globalTransformPatrix =
        //            PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
        //    }
        //    catch
        //    {
        //        _globalTransformPatrix = null;
        //    }
        //}
    }
}
