using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// ファイルオープンのロジック
/// </summary>
public class ComOpenFileDialog
{
    protected OpenFileDialog m_openFileDialog;

    /// <summary>
    /// ファイル名称
    /// </summary>
    public String FileName
    {
        set { m_openFileDialog.FileName = value; }
        get { return m_openFileDialog.FileName; }
    }

    /// <summary>
    /// ファイルダイアログに表示される初期ディレクトリ
    /// </summary>
    public String InitialDirectory
    {
        set { m_openFileDialog.InitialDirectory = value; }
        get { return m_openFileDialog.InitialDirectory; }
    }

    /// <summary>
    /// ファイルの種類のフィルタ
    /// </summary>
    public String Filter
    {
        set { m_openFileDialog.Filter = value; }
        get { return m_openFileDialog.Filter; }
    }

    /// <summary>
    /// 現在選択中のフィルタのインデックス
    /// </summary>
    public int FilterIndex
    {
        set { m_openFileDialog.FilterIndex = value; }
        get { return m_openFileDialog.FilterIndex; }
    }

    /// <summary>
    /// ファイルダイアログに表示されるタイトル
    /// </summary>
    public String Title
    {
        set { m_openFileDialog.Title = value; }
        get { return m_openFileDialog.Title; }
    }

    /// <summary>
    /// 存在しないファイルを指定した場合に警告を表示するかどうかの値
    /// </summary>
    public bool CheckFileExists
    {
        set { m_openFileDialog.CheckFileExists = value; }
        get { return m_openFileDialog.CheckFileExists; }
    }

    /// <summary>
    /// 無効なパスとファイルを入力した場合に警告を表示するかどうかの値
    /// </summary>
    public bool CheckPathExists
    {
        set { m_openFileDialog.CheckPathExists = value; }
        get { return m_openFileDialog.CheckPathExists; }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ComOpenFileDialog()
    {
        m_openFileDialog = new OpenFileDialog();
    }

    /// <summary>
    /// デスクトラクタ
    /// </summary>
    ~ComOpenFileDialog()
    {
    }

    /// <summary>
    /// ダイアログの表示
    /// </summary>
    /// <returns>結果 成功/失敗</returns>
    public bool ShowDialog()
    {
        bool bRst = false;

        if (m_openFileDialog.ShowDialog() == true)
        {
            bRst = true;
        }

        return bRst;
    }
}