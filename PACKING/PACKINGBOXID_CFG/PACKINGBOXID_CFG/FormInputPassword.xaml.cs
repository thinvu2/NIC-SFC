using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
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

namespace PACKINGBOXID_CFG
{
    /// <summary>
    /// Interaction logic for FormInputPassword.xaml
    /// </summary>
    public partial class FormInputPassword : Window
    {
        SfcHttpClient sfcClient;
        public MainWindow formPackBox_Cfg;
        public string EMP_NO;
        public string thisType, ID_NO ;
        public FormInputPassword(MainWindow main, SfcHttpClient _sfcClient,string type)
        {
            InitializeComponent();
            sfcClient = _sfcClient;
            this.formPackBox_Cfg = main;
            thisType = type;
            this.formPackBox_Cfg.CheckPri = "NG";
            Edt_Pass.Focus();
        }

        private async void Input_Pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ID_NO = (Edt_Pass.Password).ToUpper();
                if (thisType=="") await Old_Check_Pass();
                if(thisType=="REPRINT") await CheckPass();
            }
        }
        private async Task CheckPass()
        {
            string indata = "PCMAC:" + formPackBox_Cfg.PCMAC.Replace(":", "").Replace("-", "").Trim() + "|PCIP:" + formPackBox_Cfg.PCIP.Trim() + "|PWD:" + ID_NO;
            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.SP_PackingBoxID",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="AP_VER",Value=formPackBox_Cfg.ap_version,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="MYGROUP",Value=formPackBox_Cfg.M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="LINE",Value=formPackBox_Cfg.M_sThisLineName,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_EMP",Value=formPackBox_Cfg.empNo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_FUNC",Value="CHKPWD",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_DATA",Value=indata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
            });
            dynamic _ads = result.Data;
            string _RES = _ads[1]["res"];
            if (_RES == "OK")
            {
                this.formPackBox_Cfg.CheckPri = "OK";
                this.formPackBox_Cfg.lbError.Text = "PASS CORRECT!!";
                this.Close();
            }
            else
            {
                this.formPackBox_Cfg.item_Pilotrun.IsChecked = true;
                this.formPackBox_Cfg.item_Controlrun.IsChecked = true;
                this.formPackBox_Cfg.lbError.Text = _RES.Split('|')[1].ToString();
                Edt_Pass.Focus();
                Edt_Pass.SelectAll();
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = _RES.Split('|')[0].ToString();
                _er.MessageVietNam = _RES.Split('|')[1].ToString();
                _er.ShowDialog();
                return;
            }
        }
        private async Task Old_Check_Pass()
        {
            this.formPackBox_Cfg.item_Pilotrun.IsChecked = true;
            this.formPackBox_Cfg.item_Controlrun.IsChecked = true;

            if ((this.formPackBox_Cfg.item_Pilotrun.IsChecked == true) || (this.formPackBox_Cfg.item_Controlrun.IsChecked == true))
            {
                string strGetEMP = "SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC='" + ID_NO + "' OR EMP_PASS='" + ID_NO + "'";
                var qry_Emp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetEMP,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Emp.Data != null)
                {
                    EMP_NO = qry_Emp.Data["emp_no"].ToString();

                    string strGetPrivilege = "select * from sfis1.c_privilege where emp='" + EMP_NO + "' AND FUN='CHECK_PILOT_RUN' AND PRG_NAME='PACKING'";
                    var qry_Privilege = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetPrivilege,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Privilege.Data != null)
                    {
                        this.formPackBox_Cfg.CheckPri = "OK";
                        this.formPackBox_Cfg.lbError.Text = "PASS CORRECT!!";
                        this.Close();
                    }
                    else
                    {
                        this.formPackBox_Cfg.item_Pilotrun.IsChecked = true;
                        this.formPackBox_Cfg.item_Controlrun.IsChecked = true;
                        this.formPackBox_Cfg.lbError.Text = "80138 - " + await this.formPackBox_Cfg.GetPubMessage("80138");
                        Edt_Pass.Focus();
                        Edt_Pass.SelectAll();
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "80138 - " + await this.formPackBox_Cfg.GetPubMessage("80138");
                        _er.MessageVietNam = "80138 - " + await this.formPackBox_Cfg.GetPubMessage("80138");
                        _er.ShowDialog();
                        return;
                    }
                }
            }
            else
            {
                string strGetEMPNO = "SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO='LOCALLABEL'";
                var qry_EMPNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetEMPNO,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_EMPNO.Data != null)
                {
                    if (Edt_Pass.Password == qry_EMPNO.Data["emp_pass"].ToString())
                    {
                        this.formPackBox_Cfg.item_Label_local.IsChecked = true;
                        return;
                    }
                    else
                    {
                        this.formPackBox_Cfg.lbError.Text = "00087 - " + await this.formPackBox_Cfg.GetPubMessage("00087");
                        Edt_Pass.Focus();
                        Edt_Pass.SelectAll();
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        this.formPackBox_Cfg.item_Label_local.IsChecked = false;
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "00087 - " + await this.formPackBox_Cfg.GetPubMessage("00087");
                        _er.MessageVietNam = "00087 - " + await this.formPackBox_Cfg.GetPubMessage("00087");
                        _er.ShowDialog();
                        return;
                    }
                }
                else
                {
                    this.formPackBox_Cfg.lbError.Text = "00200 - " + await this.formPackBox_Cfg.GetPubMessage("00200");
                    Edt_Pass.Focus();
                    Edt_Pass.SelectAll();
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = "00200 - " + await this.formPackBox_Cfg.GetPubMessage("00200");
                    _er.MessageVietNam = "00200 - " + await this.formPackBox_Cfg.GetPubMessage("00200");
                    _er.ShowDialog();
                    return;
                }
            }
        }
    }
}
