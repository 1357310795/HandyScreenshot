﻿using System.Windows;

namespace HandyScreenshot.Interop
{
    public static class GeometryExtensions
    {
        public static Point ToPoint(this NativeMethods.POINT point, double scale = 1)
        {
            return new Point(point.X * scale, point.Y * scale);
        }

        public static Rect Scale(this Rect rect, double scale)
        {
            return new Rect(rect.Left * scale, rect.Top * scale, rect.Width * scale, rect.Height * scale);
        }
    }
}