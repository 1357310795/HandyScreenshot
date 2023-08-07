using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HandyScreenshot.Core.Common;
using HandyScreenshot.Core.Helpers;
using HandyScreenshot.Core.Models;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HandyScreenshot.Core.Views
{
    /// <summary>
    /// Interaction logic for ClipWindow.xaml
    /// </summary>
    [INotifyPropertyChanged]
    public partial class ClipWindow : Window
    {
        #region Constructors
        public ClipWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public ClipWindow(System.Drawing.Bitmap backgroundimage, MonitorInfo monitorInfo)
        {
            InitializeComponent();
            State = new ScreenshotState();
            Image = backgroundimage;
            MonitorInfo = monitorInfo;
        }
        #endregion

        #region Fields
        private static readonly byte[] SampleBytes = new byte[4];
        public Func<int, int, Color> ColorGetter { get; set; }
        public BitmapImage BackImage { get; set; }
        public BitmapSource AreaImage { get; set; }
        public MonitorInfo MonitorInfo { get; }
        public System.Drawing.Bitmap Image { get; set; }
        public MemoryStream ImageStream { get; set; }
        public ICommand CloseCommand { get; set; }
        public ScreenshotState State { get; }
        #endregion

        #region Main Functions
        private void ExitCapture()
        {
            CaptureOKWindow();
        }
        
        public void Initialize()
        {
            var initPoint = Win32Helper.GetPhysicalMousePosition();
            State.ScreenshotRect.Set(0, 0, 0, 0);
            State.PushState(MouseMessage.MouseMove, initPoint.X, initPoint.Y);
            State.CaptureOK += (e) => {
                this.CaptureOKWindow();
            };
            ImageStream = Image.ToMemoryStream();
            BackImage = ImageStream.ToBitmapImage();
            ColorGetter = GetColorByCoordinate;
            CloseCommand = new RelayCommand(ExitCapture);
            this.DataContext = this;
        }

        private Color GetColorByCoordinate(int x, int y)
        {
            if (x < 0 || x >= BackImage.PixelWidth ||
                y < 0 || y >= BackImage.PixelHeight) return Colors.Transparent;

            BackImage.CopyPixels(new Int32Rect(x, y, 1, 1), SampleBytes, 4, 0);

            return Color.FromArgb(SampleBytes[3], SampleBytes[2], SampleBytes[1], SampleBytes[0]);
        }

        #endregion

        #region AfterShot
        public delegate void CaptureOKDelegate(bool ok, ClipWindow sender);
        public event CaptureOKDelegate CaptureOK;
        private void CaptureOKWindow()
        {
            if (State.Mode != ScreenshotMode.Fixed)
            {
                CaptureOK?.Invoke(false, this);
                return;
            }
            //MessageBox.Show(State.ScreenshotRect.ToString());

            ProcessAreaImage();
            CaptureOK?.Invoke(true, this);

            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    this.Content = new AreaScanUC();
            //    ScreenshotHelper.SetWindowRect(this, State.ScreenshotRect.ToReadOnlyRect());
            //});

            //var func = this.ThreadWithReturn<CommonResult>(() =>
            //{
            //    return PicDecode();
            //});
            //var res = PicDecode();
            //if (res.success)
            //{
            //    Result = res.result;
            //    Clipboard.SetText(Result);
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        this.Content = new AreaScanSuccUC();
            //    });
            //}
            //else
            //{
            //    Result = res.result;
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        this.Content = new AreaScanFailUC();
            //    });
            //}
            //this.MouseLeftButtonDown += delegate { this.DragMove(); };
            //this.Topmost = false;
            //this.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;

            //if (GlobalSettings.fastCaptureMode)
            //{
            //    var da = AnimationHelper.CubicBezierDoubleAnimation(
            //        TimeSpan.FromSeconds(2),
            //        TimeSpan.FromSeconds(0.8),
            //        1, 0, ".38,.02,.6,.99");
            //    da.Completed += CloseAnimation_Completed;
            //    this.BeginAnimation(Window.OpacityProperty, da);
            //}
        }

        private void CloseAnimation_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ProcessAreaImage()
        {            
            AreaImage = new CroppedBitmap(BackImage, new Int32Rect(State.ScreenshotRect.X - MonitorInfo.PhysicalScreenRect.X, State.ScreenshotRect.Y - MonitorInfo.PhysicalScreenRect.Y, State.ScreenshotRect.Width, State.ScreenshotRect.Height));
        }

        //private CommonResult PicDecode()
        //{
        //    var t1 = DateTime.Now.Ticks;

        //    var res = MyScanner.ScanCode(AreaImage);
        //    var t2 = DateTime.Now.Ticks;
        //    //Console.WriteLine("识别用时：" + ((t2 - t1) / 10000).ToString() + "ms");

        //    switch (res.status)
        //    {
        //        case result_status.error:
        //            return new CommonResult(false, res.data[0].data);
        //        case result_status.ok:
        //            return new CommonResult(true, stringhelper.ResListToString(res.data));
        //        case result_status.nocode:
        //            return new CommonResult(false, LangHelper.GetStr("NoCode"));
        //    }
        //    return new CommonResult(false, LangHelper.GetStr("SystemError"));
        //}
        #endregion

        #region Mouse Events
        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = Win32Helper.GetPhysicalMousePosition();
            State.PushState(MouseMessage.LeftButtonDown, p.X, p.Y);
        }

        private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = Win32Helper.GetPhysicalMousePosition();
            State.PushState(MouseMessage.LeftButtonUp, p.X, p.Y);
        }

        private void Window_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = Win32Helper.GetPhysicalMousePosition();
            State.PushState(MouseMessage.RightButtonDown, p.X, p.Y);
        }

        private void Window_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var p = Win32Helper.GetPhysicalMousePosition();
            State.PushState(MouseMessage.RightButtonUp, p.X, p.Y);
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var p = Win32Helper.GetPhysicalMousePosition();
            State.PushState(MouseMessage.MouseMove, p.X, p.Y);
        }
        #endregion
    }
}
