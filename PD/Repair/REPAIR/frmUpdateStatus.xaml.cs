using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using REPAIR.Models;
using Newtonsoft.Json;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using Path = System.IO.Path;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for frmUpdateStatus.xaml
    /// </summary>
    public partial class frmUpdateStatus : Window
    {
        public SfcHttpClient sfcHttpClient;
        public string[] RESArray = { "NULL" };

        public string strSql;
        public char charSub;
        public frmUpdateStatus()
        {
            InitializeComponent();
        }

        private async void UpdateStatus_loaded(object sender, RoutedEventArgs e)
        {
            strSql = "SELECT VR_ITEM FROM SFIS1.C_PARAMETER_INI WHERE prg_name ='REPAIR' and VR_CLASS ='NVIDIA_TYPE' ";
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strSql,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0)
            {
                foreach (var row in result.Data)
                {
                    cbbType.Items.Add(row["vr_item"].ToString());
                }
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
             InputSN(tbData.Text, cbbType.Text);
        }

        public async void InputSN (string data , string type)
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "UPDATE_STATUS",
                    DATA = tbData.Text,
                    TYPE = cbbType.Text
                };
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REPAIR_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter {Name = "DATA" ,Value = jsonData, SfcParameterDataType = SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter {Name = "OUTPUT" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                }
                });
                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split(charSub);
                    if (RESArray[0] == "OK")
                    {
                        lstSN.Items.Add(tbData.Text + " - " + cbbType.Text + " , time: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                        tbData.SelectAll();
                        return;
                    }
                    else
                    {
                        lstError.Items.Add(RESArray[1].ToString() + " - " + cbbType.Text + " , error: " + RESArray[2].ToString());
                        tbData.SelectAll();
                        return;
                    }
                }
                else
                {
                    lstError.Items.Add(RESArray[1].ToString() + " - " + cbbType.Text + " , error: SFIS1.REPAIR_API_EXECUTE/UPDATE_STATUS result data null ");
                    tbData.SelectAll();
                    return;
                }
            }
            catch (Exception ex)
            {
                lstError.Items.Add(RESArray[1].ToString() + " - " + cbbType.Text + " , error: " + ex.ToString());
                tbData.SelectAll();
                return;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                    txtSN.Content = Path.GetFileName(openFileDialog.FileNames.ToString());
                string[] dataSN = File.ReadAllLines(openFileDialog.FileName.ToString());
                if (dataSN != null)
                {
                    foreach (string sn in dataSN)
                    {
                        InputSN(sn, cbbType.Text);
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "File data not found !";
                    frmMessage.MessageVietNam = "File không có dữ liệu !";
                    frmMessage.ShowDialog();
                    return;
                }
            }
        }

        private void tbData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(sender,e);
            }
        }

        private void showError(string strVN, string strEng, bool Flag)
        {
            MessageError FrmMessage = new MessageError();
            FrmMessage.sfcHttpClient = sfcHttpClient;
            if (Flag)
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.MessageVietNam = strVN;
                FrmMessage.MessageEnglish = strEng;
            }
            else
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.errorcode = strEng;
            }

            FrmMessage.ShowDialog();
        }

    }
}
