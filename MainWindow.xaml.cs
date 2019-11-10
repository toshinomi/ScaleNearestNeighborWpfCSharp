using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ScaleNearestNeighborWpfCSharp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point m_mousePoint;
        private string m_strOpenFileName;
        private ScaleNearestNeighbor m_scaleImgProc;
        private CancellationTokenSource m_tokenSource;

        public MainWindow()
        {
            InitializeComponent();

            m_scaleImgProc = new ScaleNearestNeighbor(progressBar);

            labelValue.Content = "1";
            btnSaveImage.IsEnabled = false;
        }

        private void OnMouseDownLblTitle(object sender, MouseButtonEventArgs e)
        {
            if ((e.ChangedButton == MouseButton.Left) && e.ButtonState == MouseButtonState.Pressed)
            {
                m_mousePoint = e.GetPosition(this);
            }

            return;
        }

        private void OnMouseMoveLblTitle(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var point = e.GetPosition(this);
                this.Left += point.X - m_mousePoint.X;
                this.Top += point.Y - m_mousePoint.Y;
            }

            return;
        }

        public BitmapImage CreateImage(string _strFileName)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(_strFileName);
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        private void OnClickBtnFileSelect(object sender, RoutedEventArgs e)
        {
            ComOpenFileDialog openFileDlg = new ComOpenFileDialog();
            openFileDlg.Filter = "JPG|*.jpg|PNG|*.png";
            openFileDlg.Title = "Open the file";
            if (openFileDlg.ShowDialog() == true)
            {
                pictureBox.Source = null;
                m_strOpenFileName = openFileDlg.FileName;
                pictureBox.Source = CreateImage(m_strOpenFileName);
                lblSelectFileName.Content = m_strOpenFileName;
                m_scaleImgProc.Init();
            }

            return;
        }

        private void OnClickBtnSaveImage(object sender, RoutedEventArgs e)
        {
            ComSaveFileDialog saveDialog = new ComSaveFileDialog();
            saveDialog.Filter = "PNG|*.png";
            saveDialog.Title = "Save the file";
            if (saveDialog.ShowDialog() == true)
            {
                string strFileName = saveDialog.FileName;
                using (FileStream stream = new FileStream(strFileName, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    WriteableBitmap bitmap = m_scaleImgProc.bitmap;
                    if (bitmap != null)
                    {
                        try
                        {
                            encoder.Frames.Add(BitmapFrame.Create(bitmap));
                            encoder.Save(stream);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(this, "Save Image File Error", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void OnClickBtnInit(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(m_strOpenFileName))
            {
                pictureBox.Source = CreateImage(m_strOpenFileName);
            }
            m_scaleImgProc.Init();
            btnSaveImage.IsEnabled = false;
        }

        private void OnClickBtnClose(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(this, "Close the application ?", "Question", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.OK)
            {
                this.Close();
            }

            return;
        }

        private void OnValueChangedSliderScale(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (labelValue != null)
            {
                labelValue.Content = (sliderScale.Value * 0.1).ToString();
            }

            return;
        }

        private async void OnClickBtnGo(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(m_strOpenFileName))
            {
                return;
            }

            btnFileSelect.IsEnabled = false;
            btnSaveImage.IsEnabled = false;
            btnInit.IsEnabled = false;
            btnClose.IsEnabled = false;
            btnCloseIcon.IsEnabled = false;
            sliderScale.IsEnabled = false;
            btnGo.IsEnabled = false;

            progressBar.Value = 0;
            progressBar.Minimum = 0;
            float fScale = (float)(sliderScale.Value * 0.1);
            var bitmap = CreateImage(m_strOpenFileName);
            int nWidth = (int)(bitmap.Width * fScale);
            int nHeight = (int)(bitmap.Height * fScale);
            progressBar.Maximum = nWidth * nHeight;

            pictureBox.Source = null;

            bool bResult = await TaskWorkImageProcessing(bitmap);
            if (bResult)
            {
                pictureBox.Source = m_scaleImgProc.bitmap;
                btnSaveImage.IsEnabled = true;
            }
            else
            {
                pictureBox.Source = CreateImage(m_strOpenFileName);
            }

            btnFileSelect.IsEnabled = true;
            btnInit.IsEnabled = true;
            btnClose.IsEnabled = true;
            btnCloseIcon.IsEnabled = true;
            sliderScale.IsEnabled = true;
            btnGo.IsEnabled = true;

            return;
        }

        private async Task<bool> TaskWorkImageProcessing(BitmapImage _bitmap)
        {
            m_tokenSource = new CancellationTokenSource();
            CancellationToken token = m_tokenSource.Token;
            float fScale = (float)(sliderScale.Value * 0.1);

            PresentationSource source = PresentationSource.FromVisual(this);

            double dDpiX = 96;
            double dDpiY = 96;
            if (source != null)
            {
                dDpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dDpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            bool bRst = await Task.Run(() => m_scaleImgProc.GoImgProc(_bitmap, fScale, token, this, dDpiX, dDpiY));
            return bRst;
        }

        private void OnClickBtnMinimizedIcon(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

            return;
        }
    }
}