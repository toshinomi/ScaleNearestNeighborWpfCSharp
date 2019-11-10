using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

class ScaleNearestNeighbor
{
    private WriteableBitmap m_bitmap;
    private ProgressBar m_progressBar;
    public WriteableBitmap bitmap
    {
        get { return (WriteableBitmap)m_bitmap.Clone(); }
    }
    public ProgressBar progressBar
    {
        set { m_progressBar = value; }
    }

    public ScaleNearestNeighbor(ProgressBar _progressBar)
    {
        m_progressBar = _progressBar;
    }

    ~ScaleNearestNeighbor()
    {
    }

    public void Init()
    {
        m_bitmap = null;
    }

    public bool GoImgProc(BitmapImage _bitmap, float _fScale, CancellationToken _token, Window _window, double _dDpiX, double _dDpiY)
    {
        bool bRst = true;

        int nWidthSize = (int)(_bitmap.PixelWidth * _fScale);
        int nHeightSize = (int)(_bitmap.PixelHeight * _fScale);

        var bitmapOrg = new WriteableBitmap(_bitmap);
        bitmapOrg.Lock();
        m_bitmap = new WriteableBitmap(nWidthSize, nHeightSize, _dDpiX, _dDpiY, PixelFormats.Pbgra32, null);
        m_bitmap.Lock();

        int nIdxWidth;
        int nIdxHeight;
        int nCount = 0;

        unsafe
        {
            for (nIdxHeight = 0; nIdxHeight < nHeightSize; nIdxHeight++)
            {
                if (_token.IsCancellationRequested)
                {
                    bRst = false;
                    break;
                }

                for (nIdxWidth = 0; nIdxWidth < nWidthSize; nIdxWidth++)
                {
                    if (_token.IsCancellationRequested)
                    {
                        bRst = false;
                        break;
                    }

                    int nWidth = (int)Math.Round(nIdxWidth / _fScale);
                    int nHeight = (int)Math.Round(nIdxHeight / _fScale);

                    if (nWidth < _bitmap.PixelWidth && nHeight < _bitmap.PixelHeight)
                    {
                        byte* pPixelOrg = (byte*)bitmapOrg.BackBuffer + nHeight * bitmapOrg.BackBufferStride + nWidth * 4;
                        byte* pPixelAfter = (byte*)m_bitmap.BackBuffer + nIdxHeight * m_bitmap.BackBufferStride + nIdxWidth * 4;

                        pPixelAfter[(int)ComInfo.Pixel.B] = pPixelOrg[(int)ComInfo.Pixel.B];
                        pPixelAfter[(int)ComInfo.Pixel.G] = pPixelOrg[(int)ComInfo.Pixel.G];
                        pPixelAfter[(int)ComInfo.Pixel.R] = pPixelOrg[(int)ComInfo.Pixel.R];
                        pPixelAfter[(int)ComInfo.Pixel.A] = pPixelOrg[(int)ComInfo.Pixel.A];

                        nCount++;
                    }
                }
                if (m_progressBar != null && _window != null)
                {
                    _window.Dispatcher.Invoke(new Action<int>(SetProgressBar), nCount);
                }
            }
            bitmapOrg.Unlock();
            bitmapOrg.Freeze();
            m_bitmap.AddDirtyRect(new Int32Rect(0, 0, nWidthSize, nHeightSize));
            m_bitmap.Unlock();
            m_bitmap.Freeze();
        }

        return bRst;
    }

    private void SetProgressBar(int _nCount)
    {
        m_progressBar.Value = _nCount;
    }
}