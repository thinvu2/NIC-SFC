using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using LabelManager2;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.Linq;

namespace MSL_PRINT
{
    class Comm
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

        //初始化CODESOFT文件的方法。
        public static void InitLabelRoom(ApplicationClass lb, string eLableName)
        {
            string filePath = string.Empty;
            if (lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            //modify by champion 
            //愛立信的機種中都有/，但是label文件名中不可以有/，所以把/替換成_，從而可以找到label檔
            filePath = path + eLableName.Replace('/', '_');


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePath))
            {
                string path2 = @"\\10.220.96.220:8080\Label\LabelRoom\";
                filePath = path2 + eLableName;
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Label檔不存在！" + filePath);
                    return;
                }
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
        public static string LabelRoomPrint(ApplicationClass lb, string eLableName)
        {
            string result = "OK";
            string filePath = string.Empty;
            if (lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            filePath = path + eLableName.Replace('/', '_');


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePath))
            {
                string path2 = @"\\10.220.96.220:8080\Label\LabelRoom\";
                filePath = path2 + eLableName;
                if (!File.Exists(filePath))
                {
                    result = "Label檔不存在！" + filePath;
                    return result;
                }
            }
            try
            {
                lb.Documents.Open(filePath, false);
                lb.EnableEvents = true;
                return result = "OK!";
            }
            catch
            {
                GC.Collect();
                return result = "读取Lab文件出错!";
            }
        }
        public static void InitLabelBOX(ApplicationClass lb, string eLableName)
        {
            string filePath = string.Empty;
            if (lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            filePath = path + eLableName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            WebClient webClient = new WebClient();
            if (!File.Exists(filePath))
            {
                webClient.DownloadFile("http://10.220.96.220:8080/FTP/Lot_Reprint_CS6-BOX/" + eLableName, filePath);
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Label file does not exist " + filePath);
                    return;
                }
            }
            else
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    MessageBox.Show("Error delete label file!!");
                    Process[] _proceses = null;
                    _proceses = Process.GetProcessesByName("lppa"); //lppa.exe
                    foreach (Process proces in _proceses)
                    {
                        proces.Kill();
                    }
                    Process[] process = Process.GetProcesses();
                    string s = "";
                    process.ToList().ForEach(item => s += item.ProcessName + "\n");
                    MessageBox.Show(s);
                    GC.Collect();
                   // lb.Quit();
                    File.Delete(filePath);
                }
                webClient.DownloadFile("http://10.220.96.220:8080/FTP/Lot_Reprint_CS6-BOX/" + eLableName, filePath);
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Label file does not exist " + filePath);
                    return;
                }
            }
            try
            {
                lb.Documents.Open(filePath, false);
                lb.EnableEvents = true;
            }
            catch
            {
                MessageBox.Show("Error reading Label file!");
                GC.Collect();
                lb.Quit();
            }
        }
        public static void InitLabelCTN(ApplicationClass lb, string eLableName)
        {
            string filePath = string.Empty;
            if (lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            filePath = path + eLableName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePath))
            {
                string path2 = @"\\10.220.96.220:8080\Packing2_CS6-CTN\";
                filePath = path2 + eLableName;
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("K CO LABEL！" + filePath);
                    return;
                }
            }
            try
            {
                lb.Documents.Open(filePath, false);
                lb.EnableEvents = true;
            }
            catch
            {
                MessageBox.Show("LOI!");
                GC.Collect();
            }
        }
        public static void InitLabelPALT(ApplicationClass lb, string eLableName)
        {
            string filePath = string.Empty;
            if (lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            filePath = path + eLableName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePath))
            {
                string path2 = @"\\10.220.96.220:8080\Packing3_PALT\";
                filePath = path2 + eLableName;
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Label檔不存在！" + filePath);
                    return;
                }
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
        public static void InitLabelRMACTN(ApplicationClass lb, string eLableName)
        {
            string filePath = string.Empty;
            if (lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            filePath = path + eLableName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePath))
            {
                string path2 = @"\\10.220.96.220:8080\PackingZ_CS6-CTN2\";
                filePath = path2 + eLableName;
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Label檔不存在！" + filePath);
                    return;
                }
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
        public static void InitLabelPackingList(ApplicationClass lb, string eLableName)
        {
            string filePath = string.Empty;
            if (lb.Documents != null && lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            filePath = path + eLableName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePath))
            {
                string path2 = @"\\10.220.96.220:8080\Packing3_PALT\NOKIA_Pallet\";
                filePath = path2 + eLableName;
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("文件加载失败,Labelfile not found！" + filePath);
                    return;
                }
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
        //如果增加一個參數:LabelFile路徑,就不用寫這麼多的Init了,
        //Label服務器設定好,Label Type設定好,每個類別一個文件夾
        //280代碼就變成35行了,如此隨時可以變更服務器,這裡還寫的固定值
        public static void InitLabelNetgearSNLabel(ApplicationClass lb, string eLableName)
        {
            string filePath = string.Empty;
            if (lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            filePath = path + eLableName;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePath))
            {
                string path2 = @"\\10.220.96.220:8080\Label\labelRoom(CPEII)\Netgear_SN_label\";
                filePath = path2 + eLableName;
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("LabelFile Not Found" + filePath);
                    return;
                }
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

        /// <summary>
        /// 此方法可涵蓋以上所有方法，只需多傳labelPath變量即可，以後可以只調用這個方法
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="eLableName"></param>
        /// <param name="sLabelPath"></param>
        /// <returns></returns>
        public static string InitLabelPrint(ApplicationClass lb, string eLableName, string sLabelPath)
        {
            string result = "OK";
            string filePath = string.Empty;
            if (lb.Documents.Count != 0)
            {
                lb.Documents.CloseAll(false);
            }
            string path = System.Windows.Forms.Application.StartupPath + @"\Lab\";
            filePath = path + eLableName.Replace('/', '_');

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(filePath))
            {
                filePath = sLabelPath + eLableName;
                if (!File.Exists(filePath))
                {
                    result = "Label檔不存在！" + filePath;
                    return result;
                }
            }
            try
            {
                lb.Documents.Open(filePath, false);
                lb.EnableEvents = true;
                return result = "OK!";
            }
            catch
            {
                GC.Collect();
                return result = "读取Label文件出错!";
            }
        }

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

        public static string PrintTest1(ApplicationClass lb)//因没有流水号所以不用   int length这个参数
        {
            try
            {
                int i2 = lb.ActiveDocument.PrintDocument(1);
                if (i2 != 1)
                {
                    return "PRINT Fail!";
                    lb.Quit();
                }
            }
            catch (Exception ex)
            {
                return "ERR:请确认打印机连接好或选择打印机然后在打印!" + ex.Message;
                lb.Quit();
            }
            return "PRINT OK!";
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

        public static void StreamWriter(string path, string[] content)
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
                content.SetValue(mySr.ReadLine(), i);
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
                    MessageBox.Show("Lab文件只读或被其它进程占用,本次设置只限本次使用!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
                short num = lb.Dialogs.Item(enumDialogType.lppxPageSetupDialog).Show();
                if (string.IsNullOrEmpty(lb.ActivePrinterName))
                {
                    MessageBox.Show("当前沒有打印机或选择的打印机出错!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
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
}
