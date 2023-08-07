using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using HandyScreenshot.Core.Common;

namespace HandyScreenshot.Core.Converters
{
    public class PointOrientationToCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is PointOrientation orientation)) return Binding.DoNothing;

            switch(orientation)
            {
                case PointOrientation.Left: return Cursors.SizeWE;
                case PointOrientation.Right: return Cursors.SizeWE;
                case PointOrientation.Top: return Cursors.SizeNS;
                case PointOrientation.Bottom: return Cursors.SizeNS;
                case PointOrientation.Left | PointOrientation.Top: return Cursors.SizeNWSE;
                case PointOrientation.Right | PointOrientation.Bottom: return Cursors.SizeNWSE;
                case PointOrientation.Right | PointOrientation.Top: return Cursors.SizeNESW;
                case PointOrientation.Left | PointOrientation.Bottom: return Cursors.SizeNESW;
                default: return Cursors.Cross;
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
