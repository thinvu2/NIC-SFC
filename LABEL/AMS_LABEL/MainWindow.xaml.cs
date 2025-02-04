using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SharpSvn;
using static AMSLabel.LoginWindow;
using System.Diagnostics;
using Sfc.Library.HttpClient;
using System.Deployment.Application;
using System.Reflection;
using SharpSvn.Security;
using Sfc.Core.Parameters;
using Newtonsoft.Json;
using Sfc.Library.HttpClient.Helpers;
using AMS_Label;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace AMSLabel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        string Label_Type = "";
        Grid momentGrid = new Grid();
        List<LabelInfo> listLabel = new List<LabelInfo>();
        FtpWebRequest ftpRequest = null;
        FtpWebResponse ftpResponse = null;
        Stream ftpStream = null;
        public string apType, apName, apPath, fileVersion, fileDesc, Owner, Size, fileName, fileTime, filePath, fileDir, fileMD5;
        public string sftpAdd = "";
        public string sftpSource = "";
        public int sftpPort = 4422;
        public string labelPath = "";
        public long bytes_total = 0;
        public string Dir, Permission, Filecode, Group, Filename, Datetime;
        public bool onProcess = false;
        public string empNo = "";
        public long FileSize = 0;
        public string lbSize = "";
        
        SfcHttpClient sfcClient;
        private byte[] downloadedData;
        public int labelIndex = 9999;
        List<LabelIndex> myList = null;
        public string UserName { get; set; }
        public string Password { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            momentGrid = Lotreprintcs6box;
            mainWindow.Title = "Label Management -- Version:" + getRunningVersion();
            
        }
        public MainWindow(string _info, SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            momentGrid = Lotreprintcs6box;
            sfcClient = _sfcClient;
            string[] digits = Regex.Split(_info, @";");
            txt_Database.Text = digits[0].ToString();
            txt_loginInfor.Text = digits[1].ToString();
            this.sftpAdd = digits[2].ToString();
            this.sftpPort = 4422;
            txt_labelServer.Text = "LABEL SERVER: " + this.sftpAdd;
            empNo = Regex.Split(txt_loginInfor.Text, @"-")[0].Trim();
            mainWindow.Title = "Label Management -- " + "Version: " + getRunningVersion().ToString();
            UserName = "amsdownload";
            Password = "getap168!";
            

            SftpDbInfo();
            GetPathIndex();
        }
        public async void SftpDbInfo()
        {
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = $"select vr_value,vr_desc from SFIS1.C_PARAMETER_INI where PRG_NAME='SFIS_AMS' and vr_class='SFTP' and rownum=1",
                SfcCommandType = SfcCommandType.Text,

            });
            if (result.Data != null)
            {
                UserName=result.Data["vr_value"]?.ToString();
                Password = result.Data["vr_desc"]?.ToString();
            }
            var result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = $"select vr_value from SFIS1.C_PARAMETER_INI where PRG_NAME='SFIS_AMS' and vr_class='SFTPPort' and rownum=1",
                SfcCommandType = SfcCommandType.Text,

            });
            if (result1.Data != null)
            {
                try
                {
                    sftpPort = Int32.Parse(result1.Data["vr_value"]?.ToString());
                }
                catch(Exception ex)
                {
                    MessageBox.Show("IT config sftp port error!");
                }
            }
        }
        public async void GetPathIndex()
        {
            var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT NO,UPPER(LABEL_PATH) LABEL_PATH,PATH_INDEX FROM SFIS1.C_LABEL_PATH_INDEX_T",
                SfcCommandType = SfcCommandType.Text,

            });
            if (result.Data != null)
            {
                myList = result.Data.ToListObject<LabelIndex>().ToList();
            }
            else
            {
                MessageBox.Show("Can not get label path_index\n" + "Table:SFIS1.C_LABEL_PATH_INDEX_T", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                this.Close();
            }

        }
        private Stream SvnStream(string svnFilePath)
        {
            Uri _Uri = new Uri(svnFilePath);
            MemoryStream stream = new MemoryStream();
            using (SvnClient client = new SvnClient())
            {
                client.Authentication.DefaultCredentials = new NetworkCredential("V0910853", "V0910853");
                client.Write(SvnTarget.FromUri(_Uri), stream);
            }
            return stream;
        }

        private void downloadFile(string ftpfilepath, string outputfilepath)
        {
            ProgressBar.Value = 0;
            downloadedData = new byte[0];
            DoEvents();
            try
            {
                string fullOutputfilepath = outputfilepath;
                string ftpfullpath = ftpfilepath.Replace("\\", "//");
                try
                {
                    using (SftpClient sftpClient = new SftpClient(new ConnectionInfo(sftpAdd, sftpPort, UserName, new AuthenticationMethod[]
                    {
                    new PasswordAuthenticationMethod(this.UserName, this.Password)
                    })
                    {
                        Encoding = Encoding.UTF8
                    }))

                    {
                        try
                        {
                            sftpClient.Connect();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("can not connect to SFTP " + sftpAdd);
                            return;
                        }
                        string text = fullOutputfilepath.Replace("/", "\\");
                        FileStream fileStream = new FileStream(text, FileMode.OpenOrCreate);
                        sftpClient.DownloadFile(ftpfullpath, fileStream, DownloadProgresBar);
                        fileStream.Close();
                        sftpClient.Disconnect();

                    }
                }
                catch (Exception ex2)
                {
                    MessageBox.Show("exception" + ex2);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("There was an error connecting to the FTP Server.");
            }
        }
        private void DownloadProgresBar(ulong Downloaded)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { ProgressBar.Value = (int)Downloaded; }));
        }
        private async void LogStore(string action, string action_desc)
        {
            var logInfo = new
            {
                TYPE = "SAVELOG",
                EMP_NO = empNo,
                PRG_NAME = PrgName,
                ACTION_TYPE = action,
                ACTION_DESC = action_desc

            };

            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "API_EXECUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="DATA",Value=logInfo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}

                }

            });
            dynamic ads = result.Data;
            string Ok = ads[0]["output"];
        }
        private bool UploadFile(string ftpfilepath, string Sourcefilepath)
        {
            bool isOK = true;
            string sftpfullpath = ftpfilepath.Replace("\\", "//");
            try
            {
                using (SftpClient sftpClient = new SftpClient(new ConnectionInfo(sftpAdd, sftpPort, UserName, new AuthenticationMethod[]
                {
                    new PasswordAuthenticationMethod(this.UserName, this.Password)
                })
                {
                    Encoding = Encoding.UTF8
                }))

                {
                    try
                    {
                        sftpClient.Connect();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return false;
                    }
                    FileStream fs = File.OpenRead(Sourcefilepath);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    FileSize = new FileInfo(Sourcefilepath).Length;
                    sftpClient.ChangeDirectory(this.labelPath);

                    FileStream fileStream = new FileStream(Sourcefilepath, FileMode.Open);
                    sftpClient.UploadFile(fileStream, System.IO.Path.GetFileName(Sourcefilepath), DownloadProgresBar);

                    sftpClient.Disconnect();

                    return true;
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.ToString());
                return false;
            }
        }

        private bool RenameFileName(string currentFilename, string path, string newFilename)
        {
            try
            {
                using (SftpClient sftpClient = new SftpClient(new ConnectionInfo(sftpAdd, sftpPort, UserName, new AuthenticationMethod[]
                {
                    new PasswordAuthenticationMethod(this.UserName, this.Password)
                })
                {
                    Encoding = Encoding.UTF8
                }))

                {
                    string oldpath = "\\" + path + "\\" + currentFilename;
                    string newpath = "\\" + path + "\\" + newFilename;
                    try
                    {
                        sftpClient.Connect();
                        sftpClient.RenameFile(oldpath, newpath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return false;
                    }


                    sftpClient.Disconnect();

                    return true;
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.ToString());
                return false;
            }
        }
        public bool CheckFileExistOrNot(string sftpURL)
        {
            bool IsExists = false;
            try
            {
                sftpURL = sftpURL.Replace("\\", "//");
                using (SftpClient sftpClient = new SftpClient(new ConnectionInfo(sftpAdd, sftpPort, UserName, new AuthenticationMethod[]
                {
                    new PasswordAuthenticationMethod(this.UserName, this.Password)
                })
                {
                    Encoding = Encoding.UTF8
                }))

                {
                    try
                    {
                        sftpClient.Connect();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("can not connect to SFTP " + sftpAdd);
                        return false;
                    }
                    IsExists = sftpClient.Exists(sftpURL);
                    sftpClient.Disconnect();

                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show("exception" + ex2);
            }
            return IsExists;
        }

        public bool DeleteLabelFile(string Path)
        {
            bool isOk = false;
            try
            {
                string fullPath = "\\" + Path;
                try
                {
                    using (SftpClient sftpClient = new SftpClient(new ConnectionInfo(sftpAdd, sftpPort, UserName, new AuthenticationMethod[]
                    {
                    new PasswordAuthenticationMethod(this.UserName, this.Password)
                    })
                    {
                        Encoding = Encoding.UTF8
                    }))
                    {
                        try
                        {
                            sftpClient.Connect();
                            sftpClient.DeleteFile(fullPath);

                            sftpClient.Disconnect();
                        }
                        catch (Exception er)
                        {
                            Console.WriteLine("An exception has been caught " + er.ToString());
                        }
                    }
                    isOk = true;
                }
                catch
                {
                    isOk = false;
                }
            }
            catch
            {
                isOk = false;
            }
            return isOk;

        }
        private Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            StackPanel stack = grid.Children.Cast<StackPanel>().ToList()[0];
            var childStack = stack.Children.Cast<Control>().ToList();
            PackIcon icon = childStack[0] as PackIcon;
            icon.Height = 30;
            icon.Width = 30;
            Label label = childStack[1] as Label;
            label.FontSize = 14;

            grid.Cursor = Cursors.Hand;
        }
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));

        }
        [DllImport("user32.dll")]
        static extern int ShowCursor(bool bShow);

        public async Task<string> getMD5(string fileName)
        {
            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = $"SELECT MD5 FROM SFIS1.C_LABEL_WIP_T WHERE LOWER(LABEL_NAME) = '{fileName.ToLower()}' and rownum=1",
                SfcCommandType = SfcCommandType.Text,

            });
            return result.Data["md5"]?.ToString();

        }
        public DateTime getdate(string filePath)
        {
            DateTime lastModifiedDate = new DateTime();
            try
            {
                using (SftpClient sftpClient = new SftpClient(new ConnectionInfo(sftpAdd, sftpPort, UserName, new AuthenticationMethod[]
                {
                        new PasswordAuthenticationMethod(this.UserName, this.Password)
                })
                {
                    Encoding = Encoding.UTF8
                }))

                {
                    try
                    {
                        sftpClient.Connect();
                        lastModifiedDate = sftpClient.GetLastAccessTime(filePath);
                        sftpClient.Disconnect();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("can not connect to SFTP " + sftpAdd);
                    }

                }
            }
            catch (Exception ex)
            {
            }
            return lastModifiedDate;
        }

        public async void getFileList(string filePath)
        {
            ProgressBar.Value = 0;
            DoEvents();
            var result = new StringBuilder();
            try
            {
                int labelCount = 0;
                try
                {
                    using (SftpClient sftpClient = new SftpClient(new ConnectionInfo(sftpAdd, sftpPort, UserName, new AuthenticationMethod[]
                    {
                        new PasswordAuthenticationMethod(this.UserName, this.Password)
                    })
                    {
                        Encoding = Encoding.UTF8
                    }))
                        
                    {
                        try
                        {
                            sftpClient.Connect();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("can not connect to SFTP " + sftpAdd);
                            return;
                        }
                        List<string> list = new List<string>();
                        var files = sftpClient.ListDirectory("\\" + filePath);
                        ProgressBar.Maximum = 100;
                        int ic = 0;
                        foreach (SftpFile file in files)
                        {
                            if (file.Name != "." && file.Name != "..")
                            {
                                if (file.Name.ToUpper().Contains(".LAB"))
                                {
                                    ic++;
                                    if (!file.IsDirectory)
                                        Dir = "File";
                                    else
                                        Dir = "Dir";
                                    listLabel.Add(new LabelInfo(file.Name, file.Length.ToString(), Dir, file.LastAccessTime.ToString("yyyy/MM/dd HH:mm:ss"), "SFTP", "SFTP", "LABELROOM"));

                                    ProgressBar.Value = (int)Math.Ceiling((double)((double)ic / (double)labelCount) * 100);
                                    tbl_total.Text = "Total: " + ic;
                                    DoEvents();
                                }
                            }

                        }

                        sftpClient.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("exception " + ex);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (onProcess)
            {
                return;
            }
            onProcess = true;
            ProgressbarCicle.Visibility = Visibility.Visible;
            ProgressBar.Value = 0;
            ProgressBar.Maximum = 100;

            Grid grid = sender as Grid;
            StackPanel stack = grid.Children.Cast<StackPanel>().ToList()[0];
            var childStack = stack.Children.Cast<Control>().ToList();
            Label label = childStack[1] as Label;

            //Grid moment            
            StackPanel stackMoment = momentGrid.Children.Cast<StackPanel>().ToList()[0];
            var childStackMoment = stackMoment.Children.Cast<Control>().ToList();
            Label labelMoment = childStackMoment[1] as Label;
            labelMoment.FontWeight = FontWeights.Regular;
            momentGrid.Background = (Brush)new BrushConverter().ConvertFromString("#008082");

            //Change moment grid;
            momentGrid = grid;
            grid.Background = (Brush)new BrushConverter().ConvertFromString("#ab47b0");
            label.FontWeight = FontWeights.Bold;

            Label_Type = label.Content.ToString();
            txt_path.Text = Label_Type;
            grid_label.ItemsSource = null;
            listLabel.Clear();
            switch (label.Content.ToString())
            {
                case "Lot-Reprint-CS6-BOX":
                    tbl_labelPath.Text = "Label path: " + "/Lot_Reprint_CS6-BOX";
                    labelPath = "Lot_Reprint_CS6-BOX";
                    this.labelIndex = Int32.Parse(myList.Where(x => x.LABEL_PATH == "LOT_REPRINT_CS6-BOX").ToList().SingleOrDefault<LabelIndex>().PATH_INDEX);
                    getFileList(labelPath);
                    grid_label.ItemsSource = listLabel;
                    break;
                case "Packing2-CS6-CTN":
                    tbl_labelPath.Text = "Label path: " + "/packing2_cs6-ctn";
                    labelPath = "packing2_cs6-ctn";
                    this.labelIndex = Int32.Parse(myList.Where(x => x.LABEL_PATH == "PACKING2_CS6-CTN").ToList().SingleOrDefault<LabelIndex>().PATH_INDEX);
                    getFileList(labelPath);
                    grid_label.ItemsSource = listLabel;
                    break;
                case "Packing2-CS6--MAC":
                    tbl_labelPath.Text = "Label path: " + "/packing2_cs6--mac";
                    labelPath = "packing2_cs6--mac";
                    this.labelIndex = Int32.Parse(myList.Where(x => x.LABEL_PATH == "PACKING2_CS6--MAC").ToList().SingleOrDefault<LabelIndex>().PATH_INDEX);
                    getFileList(labelPath);
                    grid_label.ItemsSource = listLabel;
                    break;
                case "PackingZ-CS6-CTN2":
                    tbl_labelPath.Text = "Label path: " + "/packingz_cs6-ctn2";
                    labelPath = "packingz_cs6-ctn2";
                    this.labelIndex = Int32.Parse(myList.Where(x => x.LABEL_PATH == "PACKINGZ_CS6-CTN2").ToList().SingleOrDefault<LabelIndex>().PATH_INDEX);
                    getFileList(labelPath);
                    grid_label.ItemsSource = listLabel;
                    break;
                case "Packing3-PALT":
                    tbl_labelPath.Text = "Label path: " + "/packing3_palt";
                    labelPath = "packing3_palt";
                    this.labelIndex = Int32.Parse(myList.Where(x => x.LABEL_PATH == "PACKING3_PALT").ToList().SingleOrDefault<LabelIndex>().PATH_INDEX);
                    getFileList(labelPath);
                    grid_label.ItemsSource = listLabel;
                    break;
                case "FG-Label-CTN":
                    tbl_labelPath.Text = "Label path: " + "/fg_label/pnd_label/carton_label";
                    labelPath = "fg_label/pnd_label/carton_label";
                    this.labelIndex = Int32.Parse(myList.Where(x => x.LABEL_PATH == "FG_LABEL/PND_LABEL/CARTON_LABEL").ToList().SingleOrDefault<LabelIndex>().PATH_INDEX);
                    getFileList(labelPath);
                    grid_label.ItemsSource = listLabel;
                    break;
                case "FG-Label-Pallet":
                    tbl_labelPath.Text = "Label path: " + "/fg_label/pnd_label/pallet_label";
                    labelPath = "fg_label/pnd_label/pallet_label";
                    this.labelIndex = Int32.Parse(myList.Where(x => x.LABEL_PATH == "FG_LABEL/PND_LABEL/PALLET_LABEL").ToList().SingleOrDefault<LabelIndex>().PATH_INDEX);
                    getFileList(labelPath);
                    grid_label.ItemsSource = listLabel;
                    break;
                case "LabelRoom":
                    tbl_labelPath.Text = "Label path: " + "/labelroom";
                    labelPath = "labelroom";
                    this.labelIndex = Int32.Parse(myList.Where(x => x.LABEL_PATH == "LABELROOM").ToList().SingleOrDefault<LabelIndex>().PATH_INDEX);
                    getFileList(labelPath);
                    grid_label.ItemsSource = listLabel;
                    break;
                default:
                    this.labelIndex = 9999;
                    tbl_labelPath.Text = "Label path: " + "/lot_reprint_cs6-box";

                    break;
            }
            onProcess = false;
            ProgressbarCicle.Visibility = Visibility.Hidden;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            StackPanel stack = grid.Children.Cast<StackPanel>().ToList()[0];
            var childStack = stack.Children.Cast<Control>().ToList();
            PackIcon icon = childStack[0] as PackIcon;
            icon.Height = 25;
            icon.Width = 25;
            Label label = childStack[1] as Label;
            label.FontSize = 13;
        }


        private async void grid_label_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid x = sender as DataGrid;
                dynamic obj = x.CurrentItem;
                var axs = x.RowBackground;
                txt_labelname.Text = obj.Name;
                txt_Modified.Text = obj.Modified;
                this.lbSize = obj.Size;
                txt_Md5.Text = await getMD5(obj.Name);
            }
            catch
            {

            }
        }

        private async void btn_Download_click(object sender, RoutedEventArgs e)
        {
            if (onProcess)
            {
                return;
            }
            try
            {
                if (txt_labelname.Text == "")
                {
                    MessageBox.Show("Please choose label you need download", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    return;
                }
                SaveFileDialog saveLabel = new SaveFileDialog();

                saveLabel.FileName = txt_labelname.Text;
                if (saveLabel.ShowDialog() == true)
                {

                    downloadFile("\\" + this.labelPath + "/" + txt_labelname.Text, saveLabel.FileName);

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please select label.", "Message");
                return;

            }
        }

        private async void btn_Upload_click(object sender, RoutedEventArgs e)
        {
            if (onProcess)
            {
                return;
            }
            if (this.labelIndex == 9999)
            {
                MessageBox.Show("Please choose the ftp label folder!", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                ProgressBar.Value = 0;
                ProgressBar.Maximum = 100;
                OpenFileDialog openLabel = new OpenFileDialog();
                openLabel.Filter = "Lanel files (*.Lab)|*.Lab|All files (*.*)|*.*";

                if (openLabel.ShowDialog() == true)
                {
                    string labelCurrenPath = openLabel.FileName;
                    string labelname = System.IO.Path.GetFileName(labelCurrenPath);
                    if (System.IO.Path.GetExtension(labelname).ToUpper() != ".LAB")
                    {
                        MessageBox.Show("The name of the file is not .lab", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string path = "\\" + this.labelPath + "\\" + labelname;
                    LabelAnalyse lbl = new LabelAnalyse();
                    string crc322 = lbl.CRC322(labelCurrenPath);
                    if (crc322.Length == 0)
                    {
                        MessageBox.Show("Label checksum error Pls kill\nall lbpa.exe process and upload again", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SELECT * FROM SFIS1.C_LABEL_WIP_T WHERE LABEL_NAME='" + labelname + "' AND MD5='" + crc322 + "'",
                        SfcCommandType = SfcCommandType.Text,

                    });
                    if (result.Data != null)
                    {
                        MessageBox.Show("This label dup md5 with other label\nPleas modify and upload again", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string sqlLabel = lbl.GetLabelSql(crc322, labelCurrenPath, labelname.ToLower(), labelIndex);
                    
                    var _result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sqlLabel,
                    });
                    if (_result.Result.ToString() != "OK")
                    {
                        MessageBox.Show("Insert new label error\n" + _result.Result.ToString(), "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (CheckFileExistOrNot(path))
                    {
                        if (MessageBox.Show("The label  '" + labelname + "' already exist in '" + this.labelPath + "',\ndo you want replace ?", "Warring", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            string newName = System.IO.Path.GetFileNameWithoutExtension(labelname) + "." + empNo + DateTime.Now.ToString("yyMMddHHmmss") + ".bak";
                            if (!RenameFileName(labelname, this.labelPath, newName))
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    if (UploadFile(this.labelPath + "/" + labelname, labelCurrenPath))
                    {
                        var actionDesc = "IP:" + GetIPAddress().ToString() + "," + this.labelPath + "/" + labelname;
                        LogStore("UPLOAD", actionDesc);
                        listLabel = listLabel.Where(x => x.Name != labelname).ToList();
                        listLabel.Add(new LabelInfo(labelname, FileSize.ToString(), "File", getdate("/" + this.labelPath + "/" + labelname).ToString(), "SFTP", "SFTP", "LABELROOM"));
                        grid_label.ItemsSource = null;
                        grid_label.ItemsSource = listLabel;
                    }
                    else
                    {
                        MessageBox.Show("Upload label fail", "Message");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private async void btn_Delete_click(object sender, RoutedEventArgs e)
        {
            if (onProcess)
            {
                return;
            }
            try
            {
                if (txt_labelname.Text == "")
                {
                    MessageBox.Show("Please choose label want delete", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (MessageBox.Show("Do you want delete label '" + txt_labelname.Text + "'?", "Warring", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    this.DeleteLabelFile(this.labelPath + "\\" + txt_labelname.Text);
                    var actionDesc = "IP:" + GetIPAddress().ToString() + ", " + this.labelPath + "/" + txt_labelname.Text;
                    LogStore("DELETE", actionDesc);

                    var logInfo = new
                    {
                        TYPE = "DELETELABEL",
                        LABEL_INDEX = this.labelIndex.ToString(),
                        LABEL_NAME = txt_labelname.Text
                    };
                    string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.API_EXECUTE ",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}

                        }

                    });
                    dynamic ads = result.Data;
                    string Ok = ads[0]["output"];
                    listLabel = listLabel.Where(x => x.Name != txt_labelname.Text).ToList();
                    grid_label.ItemsSource = null;
                    grid_label.ItemsSource = listLabel;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private async void btn_Rename_click(object sender, RoutedEventArgs e)
        {
            if (onProcess)
            {
                return;
            }
            try
            {
                InputBox inputBox = new InputBox();
                inputBox.txtAnswer.Text = txt_labelname.Text;
                inputBox.ShowDialog();
                if (inputBox.DialogResult == true)
                {
                    string newName = inputBox.txtAnswer.Text;
                    if (newName == txt_labelname.Text)
                    {
                        return;
                    }
                    if (System.IO.Path.GetExtension(newName).ToUpper() != ".LAB")
                    {
                        MessageBox.Show("The new ext name of the file is not .lab", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (CheckFileExistOrNot("\\" + this.labelPath + "\\" + newName))
                    {
                        MessageBox.Show("The label  '" + newName + "' already exist in '" + this.labelPath + "'", "Warring", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                        return;

                    }
                    if (!RenameFileName(txt_labelname.Text, this.labelPath, newName))
                    {
                        return;
                    }
                    var actionDesc = "IP:" + GetIPAddress().ToString() + ", " + this.labelPath + "/" + txt_labelname.Text + ", Newname:" + newName;
                    LogStore("RENAME", actionDesc);
                    listLabel = listLabel.Where(x => x.Name != txt_labelname.Text).ToList();
                    listLabel.Add(new LabelInfo(newName, this.lbSize, "File", getdate("/" + this.labelPath + "/" + newName).ToString(), "SFTP", "SFTP", "LABELROOM"));
                    txt_labelname.Text = newName;
                    grid_label.ItemsSource = null;
                    grid_label.ItemsSource = listLabel;
                }
                else
                    return;
            }
            catch (Exception)
            {
                MessageBox.Show("Please select label.", "Message");
                return;
            }
        }
        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                List<LabelInfo> listSearch = listLabel.Where(p => p.Name.ToUpper().Contains(txt_search.Text.Trim().ToUpper())).ToList();
                grid_label.ItemsSource = null;
                grid_label.ItemsSource = listSearch;
            }
            catch (Exception ex)
            {
            }
        }

    }
}
