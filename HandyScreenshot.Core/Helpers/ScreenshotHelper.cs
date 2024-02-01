using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using HandyScreenshot.Core.Common;
using HandyScreenshot.Core.Views;
using static HandyScreenshot.Core.Interop.NativeMethods;

namespace HandyScreenshot.Core.Helpers
{
    public static class ScreenshotHelper
    {
        private static ClipWindow mw;
        public delegate void CaptureOKDelegate(bool ok, ClipWindow window);
        public static event CaptureOKDelegate CaptureOK;
        public static void StartScreenshot()
        {
            var monitorInfos = MonitorHelper.GetMonitorInfos();

            foreach (var monitorInfo in monitorInfos)
            {
                mw = new ClipWindow(
                    CaptureScreen(monitorInfo.PhysicalScreenRect),
                    monitorInfo);
                mw.CaptureOK += Mw_CaptureOK;
                SetWindowRect(mw, monitorInfo.PhysicalScreenRect);
                mw.Initialize();
                mw.Show();
            }
        }

        private static void Mw_CaptureOK(bool ok, ClipWindow sender)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window w in Application.Current.Windows)
                    if (w is ClipWindow cw && w != sender)
                        if (cw.State.Mode != ScreenshotMode.Done)
                            w.Close();
            });
            CaptureOK(ok, sender);
        }

        private static void SetWindowRect(Window window, ReadOnlyRect rect)
        {
            SetWindowPos(
                window.GetHandle(),
                (IntPtr)HWND_TOPMOST,
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height,
                SWP_NOZORDER);
        }

        private static Bitmap CaptureScreen(ReadOnlyRect rect)
        {
            var hdcSrc = GetAllMonitorsDC();

            var width = rect.Width;
            var height = rect.Height;
            var hdcDest = CreateCompatibleDC(hdcSrc);
            var hBitmap = CreateCompatibleBitmap(hdcSrc, width, height);
            _ = SelectObject(hdcDest, hBitmap);

            BitBlt(hdcDest, 0, 0, width, height, hdcSrc, rect.X, rect.Y,
                TernaryRasterOperations.SRCCOPY | TernaryRasterOperations.CAPTUREBLT);

            var image = System.Drawing.Image.FromHbitmap(hBitmap);

            DeleteObject(hBitmap);
            DeleteDC(hdcDest);
            DeleteDC(hdcSrc);

            return image;
        }

        public static MemoryStream ToMemoryStream(this System.Drawing.Image image)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        public static BitmapImage ToBitmapImage(this MemoryStream memoryStream)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        private static IntPtr GetAllMonitorsDC()
        {
            return CreateDC("DISPLAY", null, null, IntPtr.Zero);
        }
    }
}