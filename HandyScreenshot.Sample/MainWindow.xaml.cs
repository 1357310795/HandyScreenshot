using HandyScreenshot.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace HandyScreenshot.Sample
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScreenshotHelper.CaptureOK -= ScreenshotHelper_CaptureOK;
            ScreenshotHelper.CaptureOK += ScreenshotHelper_CaptureOK;
            ScreenshotHelper.StartScreenshot();
        }

        private void ScreenshotHelper_CaptureOK(bool ok, Core.Views.ClipWindow window)
        {
            this.Dispatcher.Invoke(() => { window.Close(); });
            Debug.WriteLine(ok);
            if (ok)
            {
                Debug.WriteLine(window.AreaImage.PixelWidth);
                Debug.WriteLine(window.AreaImage.PixelHeight);
            }
            
        }
    }
}
