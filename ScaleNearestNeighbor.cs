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

/// <summary>
/// 最近傍補間法のロジック
/// </summary>
class ScaleNearestNeighbor
{
    private WriteableBitmap m_bitmap;
    private ProgressBar m_progressBar;

    /// <summary>
    /// Writeableなビットマップ
    /// </summary>
    public WriteableBitmap bitmap
    {
        get { return (WriteableBitmap)m_bitmap.Clone(); }
    }

    /// <summary>
    /// プログレスバー
    /// </summary>
    public ProgressBar progressBar
    {
        set { m_progressBar = value; }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="_progressBar">プログレスバー</param>
    public ScaleNearestNeighbor(ProgressBar _progressBar)
    {
        m_progressBar = _progressBar;
    }

    /// <summary>
    /// デスクトラクタ
    /// </summary>
    ~ScaleNearestNeighbor()
    {
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        m_bitmap = null;
    }

    /// <summary>
    /// 最近傍補間法の実行
    /// </summary>
    /// <param name="_bitmap">ビットマップ</param>
    /// <param name="_fScale">スケール</param>
    /// <param name="_token">キャンセルトークン</param>
    /// <param name="_window">ウィンドウ</param>
    /// <param name="_dDpiX">DPI X</param>
    /// <param name="_dDpiY">DPI Y</param>
    /// <returns>実行結果 成功/失敗</returns>
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

    /// <summary>
    /// プログレスバーの設定
    /// </summary>
    /// <param name="_nCount">カウント</param>
    private void SetProgressBar(int _nCount)
    {
        m_progressBar.Value = _nCount;
    }
}