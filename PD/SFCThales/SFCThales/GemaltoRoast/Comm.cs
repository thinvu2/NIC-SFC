using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using LabelManager2;
using Microsoft.VisualBasic;
using System.Threading;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using Sfc.Library.HttpClient;

namespace GemaltoRoast
{
    public class Comm
    {
        public static Dictionary<string, ApplicationClass> userLB = new Dictionary<string, ApplicationClass>();
        public static string StartPath = System.Windows.Forms.Application.StartupPath + "\\System.ini";
        public static string strLook = ReadString("setSystem", "Look", "true", StartPath);
        public static string Number = ReadString("setSystem", "Number", "100", StartPath);
        public static bool Look = Convert.ToBoolean(strLook);
        public static int iNumber = Convert.ToInt32(Number);
        public static int SaoMiaoNum = Convert.ToInt32(ReadString("setSystem", "SaoMiaoNum", "3", StartPath));
        public static bool isEqual = Convert.ToBoolean(ReadString("setSystem", "isEqual", "true", StartPath));
        public static bool isDate = Convert.ToBoolean(ReadString("setSystem", "isDate", "false", StartPath));

        #region 初始化CODESOFT文件的方法。
        public static void InitLabel(ApplicationClass lb, string eLableName)
        {
            string filePath = string.Empty;
            if (lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            //string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            string path =  @"\\10.64.32.62\Lot_Reprint_CS6-BOX\";
            filePath = path + eLableName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePath))
            {
                MessageBox.Show("文件加载失败！");
                return;
            }
            try
            {
                lb.Documents.Open(filePath, false);
                lb.EnableEvents = true;
            }
            catch
            {
                MessageBox.Show("读取Lab文件出错!");
                GC.Collect();
            }
        }
        #endregion

        #region 操作ini文件
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

        public static string ReadString(string Section, string Ident, string Default, string FileName)
        {
            byte[] retVal = new byte[0xffff];
            int length = GetPrivateProfileString(Section, Ident, Default, retVal, retVal.GetUpperBound(0), FileName);
            return Encoding.GetEncoding(0).GetString(retVal).Substring(0, length).Trim();
        }

        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
        public static bool WriteString(string Section, string Ident, string Value, string FileName)
        {
            if (WritePrivateProfileString(Section, Ident, Value, FileName))
                return true;
            else
                return false;
        }
        #endregion

         #region 打印公共方法
        public static void PrintTest(ApplicationClass lb)//因没有流水号所以不用   int length这个参数
        {
            try
            {
                int i2 = lb.ActiveDocument.PrintDocument(1);
                if (i2 != 1) return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("请确认打印机连接好或选择打印机然后在打印\n" + ex.Message, "故障", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Print(int num, ApplicationClass lb, string label, string bicker)
        {
            try
            {
                int SysNum = Comm.iNumber;
                int multiple = (num + SysNum - 1) / SysNum;
                for (int i = 1; i <= multiple; i++)
                {
                    if (i == multiple)
                    {
                        SysNum = num - (SysNum * (i - 1));
                    }
                    int i2 = lb.ActiveDocument.PrintDocument(SysNum);
                    if (i2 != 1)
                    {
                        return;
                    }
                    
                    if (Comm.Look)
                    {
                        Thread.Sleep(10000);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("请确认打印机连接好或选择打印机然后在打印\n" + ex.Message, "故障", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 文件读写

        public static void StreamWriter(string path,string[] content)
        {
            //创建文件流
            FileStream myFs = new FileStream(path, FileMode.Create);
            //创建写入器
            StreamWriter mySw = new StreamWriter(myFs);
            for (int i = 0; i < content.Length; i++)
            {
                //将录入的内容写入文件
                mySw.WriteLine(content[i]);
            }
            //关闭写入器
            mySw.Close();
            //关闭文件流
            myFs.Close();
        }

        public static string[] StreamReader(string path)
        {
            string[] content = new string[4];
            //创建文件流
            FileStream myFs = new FileStream(path, FileMode.Open);
            //创建读取器
            StreamReader mySr = new StreamReader(myFs);
            int i = 0;
            while (!mySr.EndOfStream)
            {
                //读取文件所有内容
                content.SetValue(mySr.ReadLine(),i);
                i++;
            }
            //关闭读取器
            mySr.Close();
            //关闭文件流
            myFs.Close();
            return content;
        }

        #endregion

        #region 长久保存数据方法
        /// <summary>
        /// 写入App.config方法
        /// </summary>
        /// <param name="AppKey"></param>
        /// <param name="AppValue"></param>
        public static void SetConfigValue(string AppKey, string AppValue)
        {
            XmlDocument xDoc = new XmlDocument();
            string strFileName = System.Windows.Forms.Application.ExecutablePath + ".config";
            xDoc.Load(strFileName);

            XmlNode xNode;
            XmlElement xElem1;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null) xElem1.SetAttribute("value", AppValue);

            xDoc.Save(strFileName);
        }
        /// <summary>
        /// 读取App.config方法
        /// </summary>
        /// <param name="AppKey"></param>
        /// <param name="AppValue"></param>
        public static string GetConfigValue(string AppKey)
        {
            XmlDocument xDoc = new XmlDocument();
            string strFileName = System.Windows.Forms.Application.ExecutablePath + ".config";
            string AppValue = null;
            xDoc.Load(strFileName);

            XmlNode xNode;
            XmlElement xElem1;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null)
            {
                AppValue = xElem1.Attributes.Item(1).Value;
            }
            return AppValue;
        }

        public static bool WriteXml(string strKey, string strValue)
        {
            try
            {
                string strFileName = System.Windows.Forms.Application.ExecutablePath + ".config";
                XmlDocument doc = new XmlDocument();
                doc.Load(strFileName);
                XmlNode xAS = doc.SelectSingleNode("/configuration/applicationSettings");
                XmlNode xNode = xAS.SelectSingleNode(String.Format("//setting[@name='{0}']", strKey));
                foreach (XmlElement xEle in xNode)
                {
                    if (xEle.Name == "value")
                    {
                        xEle.InnerText = strValue;
                        break;
                    }
                }
                doc.Save(strFileName);
                doc = null;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 打印详情，打印机选择
        public static void btnSelectPrinter_Click(ApplicationClass lb)
        {
            try
            {
                lb.Dialogs.Item(enumDialogType.lppxPrinterSelectDialog).Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show("错误:--> " + exception.Message);
            }
        }
        public static void btnSetupPrt_Click(ApplicationClass lb)
        {
            try
            {
                if (lb.ActiveDocument.ReadOnly)
                {
                    MessageBox.Show("Lab文件只读或被其它进程占用,本次设置只限本次使用!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
                short num = lb.Dialogs.Item(enumDialogType.lppxPageSetupDialog).Show();
                if (string.IsNullOrEmpty(lb.ActivePrinterName))
                {
                    MessageBox.Show("当前沒有打印机或选择的打印机出错!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
                else
                {
                    lb.ActiveDocument.Save();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("错误:--> " + exception.Message);
            }
        }

        public static void btnPrintSet_Click(ApplicationClass lb)
        {
            try
            {
                lb.Dialogs.Item(enumDialogType.lppxPrinterSetupDialog).Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show("错误:--> " + exception.Message);
            }
        }
        #endregion
    }
    class DBApi
    {
        public static SfcHttpClient _sfcHttpClient;
        public static SfcHttpClient sfcClient(string db_name, string api_link)
        {
            return _sfcHttpClient = new SfcHttpClient(api_link, db_name, "LOT_Reprint", "1234567");
        }
    }
}
