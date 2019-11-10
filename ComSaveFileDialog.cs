using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ComSaveFileDialog
{
    protected SaveFileDialog m_saveFileDialog;

    public String FileName
    {
        set { m_saveFileDialog.FileName = value; }
        get { return m_saveFileDialog.FileName; }
    }
    public String InitialDirectory
    {
        set { m_saveFileDialog.InitialDirectory = value; }
        get { return m_saveFileDialog.InitialDirectory; }
    }

    public String Filter
    {
        set { m_saveFileDialog.Filter = value; }
        get { return m_saveFileDialog.Filter; }
    }

    public int FilterIndex
    {
        set { m_saveFileDialog.FilterIndex = value; }
        get { return m_saveFileDialog.FilterIndex; }
    }

    public String Title
    {
        set { m_saveFileDialog.Title = value; }
        get { return m_saveFileDialog.Title; }
    }

    public bool CheckFileExists
    {
        set { m_saveFileDialog.CheckFileExists = value; }
        get { return m_saveFileDialog.CheckFileExists; }
    }

    public bool CheckPathExists
    {
        set { m_saveFileDialog.CheckPathExists = value; }
        get { return m_saveFileDialog.CheckPathExists; }
    }

    public ComSaveFileDialog() : base()
    {
        m_saveFileDialog = new SaveFileDialog();
    }

    ~ComSaveFileDialog()
    {
    }

    public bool ShowDialog()
    {
        bool bRst = false;

        if (m_saveFileDialog.ShowDialog() == true)
        {
            bRst = true;
        }

        return bRst;
    }

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