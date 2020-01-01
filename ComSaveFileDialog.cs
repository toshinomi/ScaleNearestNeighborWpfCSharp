using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// ファイルセーブのロジック
/// </summary>
class ComSaveFileDialog
{
    protected SaveFileDialog m_saveFileDialog;

    /// <summary>
    /// ファイル名称
    /// </summary>
    public String FileName
    {
        set { m_saveFileDialog.FileName = value; }
        get { return m_saveFileDialog.FileName; }
    }

    /// <summary>
    /// ファイルダイアログに表示される初期ディレクトリ
    /// </summary>
    public String InitialDirectory
    {
        set { m_saveFileDialog.InitialDirectory = value; }
        get { return m_saveFileDialog.InitialDirectory; }
    }

    /// <summary>
    /// ファイルの種類のフィルタ
    /// </summary>
    public String Filter
    {
        set { m_saveFileDialog.Filter = value; }
        get { return m_saveFileDialog.Filter; }
    }

    /// <summary>
    /// 現在選択中のフィルタのインデックス
    /// </summary>
    public int FilterIndex
    {
        set { m_saveFileDialog.FilterIndex = value; }
        get { return m_saveFileDialog.FilterIndex; }
    }

    /// <summary>
    /// ファイルダイアログに表示されるタイトル
    /// </summary>
    public String Title
    {
        set { m_saveFileDialog.Title = value; }
        get { return m_saveFileDialog.Title; }
    }

    /// <summary>
    /// 存在しないファイルを指定した場合に警告を表示するかどうかの値
    /// </summary>
    public bool CheckFileExists
    {
        set { m_saveFileDialog.CheckFileExists = value; }
        get { return m_saveFileDialog.CheckFileExists; }
    }

    /// <summary>
    /// 無効なパスとファイルを入力した場合に警告を表示するかどうかの値
    /// </summary>
    public bool CheckPathExists
    {
        set { m_saveFileDialog.CheckPathExists = value; }
        get { return m_saveFileDialog.CheckPathExists; }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ComSaveFileDialog() : base()
    {
        m_saveFileDialog = new SaveFileDialog();
    }

    /// <summary>
    /// デスクトラクタ
    /// </summary>
    ~ComSaveFileDialog()
    {
    }

    /// <summary>
    /// ダイアログの表示
    /// </summary>
    /// <returns>結果 成功/失敗</returns>
    public bool ShowDialog()
    {
        bool bRst = false;

        if (m_saveFileDialog.ShowDialog() == true)
        {
            bRst = true;
        }

        return bRst;
    }

    /// <summary>
    /// ストリームの書込み
    /// </summary>
    /// <param name="_str">ファイル名称</param>
    /// <returns>実行結果 成功/失敗</returns>
    public bool StreamWrite(string _str)
    {
        Stream stream;
        bool bRst = true;
        try
        {
            stream = m_saveFileDialog.OpenFile();
        }
        catch (Exception)
        {
            bRst = false;
            return bRst;
        }
        StreamWriter streamWriter = new StreamWriter(stream, Encoding.GetEncoding("UTF-8"));
        streamWriter.Write(_str);
        streamWriter.Close();
        stream.Close();

        return bRst;
    }
}