using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.OracleClient;
using System.Globalization;
//using SFC_Library;
using Sfc.Library.HttpClient;
using System.Text.RegularExpressions;
using Sfc.Core.Parameters;
using HOLD.LogInfo;
using Sfc.Library.HttpClient.Helpers;
using Newtonsoft.Json;
using Microsoft.Office.Interop.Excel;
using Application = System.Windows.Forms.Application;
using System.IO;

namespace HOLD
{
    public partial class HandHold : Form
    {
        private FormMain objfrmMain;
        const string ERRORSTRING = "Message";
        public int linecount;

        public SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string empNo = "";
        public bool zFlag = false;
        public string tab107 = "sfism4.r107",notWip= "AND WIP_GROUP <> 'FG' AND SUBSTR(WIP_GROUP,1,2) <> 'SC' ";
        List<zFGcolumn> zoutput;

        public HandHold(FormMain frmMain)
        {
            InitializeComponent();
            objfrmMain = frmMain;
            loginInfor = frmMain.loginInfor;
            string[] digits = Regex.Split(loginInfor, @";");
            connectDB.Text = digits[0].ToString();
            tssEmp.Text = digits[1].ToString();
            empNo = Regex.Split(tssEmp.Text, @"-")[0].Trim();

        }
        
        private async void getGroup()
        {
            cbbGroup.Items.Clear();
            cbbStation.Items.Clear();
            try
            {
                var resultGroup = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = " SELECT DISTINCT GROUP_NAME FROM SFIS1.C_GROUP_CONFIG_T " +
                             " WHERE SUBSTR (GROUP_NAME, 1, 2) <> 'R_' " +
                             " AND SUBSTR (GROUP_NAME, 1, 2) <> 'SC'  " +
                             " AND SUBSTR (GROUP_NAME, 1, 2) <> 'BC' " +
                             " ORDER BY GROUP_NAME ",
                    SfcCommandType = SfcCommandType.Text,
                });
                if (resultGroup.Data != null)
                {
                    var a = resultGroup.Data.ToListObject<infGroup>();
                    List<infGroup> listGroup = a.Cast<infGroup>().ToList();
                    for (int i = 0; i < listGroup.Count; i++)
                    {
                        cbbGroup.Items.Add(listGroup[i].GROUP_NAME.ToString());
                        cbbStation.Items.Add(listGroup[i].GROUP_NAME.ToString());
                    }
                    cbbGroup.SelectedIndex = 0;
                    cbbStation.SelectedIndex = 0;
                }
            }
            catch(Exception ex)
            {
                showMessage(ex.Message.ToString());
            }                       
        }

        private async void getModel()
        {
            cbbModel.Items.Clear();
            try
            {
                var resultModelName = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = "SELECT DISTINCT MODEL_NAME FROM SFIS1.C_MODEL_DESC_T ORDER BY MODEL_NAME",
                    SfcCommandType = SfcCommandType.Text,
                });
                if (resultModelName.Data != null)
                {
                    var a = resultModelName.Data.ToListObject<infGroup>();
                    List<infGroup> listModel = a.Cast<infGroup>().ToList();
                    for (int i = 0; i < listModel.Count; i++)
                    {
                        cbbModel.Items.Add(listModel[i].MODEL_NAME.ToString());
                    }
                    cbbModel.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                showMessage(ex.Message.ToString());
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            cbbCondition.Text = "";
            txtCondition.Clear();
            cbxAteStation.Checked = false;
            cbxFG.Checked = false;
            cbxGroup.Checked = false;
            cbxHoldByStation.Checked = false;
            cbxHoldStockin.Checked = false;
            txtAteStation.Text = "";
            cbbGroup.Text = "";
            cbbStation.Text = "";
            lblTotalQty.Text = "0";
            lblSuccessQty.Text = "0";
            lblFailQty.Text = "0";
            cbxUnLineMo.Checked = false;
            cbxTestTime.Checked = false;
            txtReason.Clear();
            txtErrorSN.Text = "Error Message:";
        }

        private void HandHold_Shown(object sender, EventArgs e)
        {
            getGroup();
            getModel();
            cbbTimeStart.SelectedIndex = 0;
            cbbTimeEnd.SelectedIndex = 0;
            cbbCondition.SelectedIndex = 0;
        }

        private bool ChkInput_data_valid(string str_input)
        {
            if (str_input.Trim().ToUpper() == "N/A" || str_input.Trim().ToUpper() == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void btnQuery_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();
            string  s_sql = "", c_sql = "", temp = "",sql = "";
            string begintime = "", endtime = "";
            lblTotalQty.Text = "0";
            lblSuccessQty.Text = "0";
            lblFailQty.Text = "0";
            linecount = Regex.Matches(txtCondition.Text, "\r\n").Count + 1;
            for (int i = 0; i < linecount; i++)
            {
                if (ChkInput_data_valid(txtCondition.Lines[i].ToString()))
                {
                    showMessage("Invalid input data"+ txtCondition.Lines[i].ToString().Trim()+"|Dữ liệu nhập vào không hợp lệ-->" + txtCondition.Lines[i].ToString().Trim());                    
                    txtCondition.Focus();
                    return;
                }
            }
            #region newFunc
            if (chkTest.Checked)
            {
                zFlag = false;
                if (cbbCondition.Text != "IMEI")
                {
                    showMessage("This function apply only for mPallet(IMEI)|Chức năng chỉ áp dụng cho mPallet(IMEI)");
                    return;
                }
                string ztemp = "";
                for (int i = 0; i < linecount; i++)
                {
                    if (i == 0)
                    {
                        ztemp = ztemp + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                    }
                    else
                    {
                        ztemp = ztemp + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                    }
                }
                sql = "select model_name,version_code,pallet_no,imei,group_name,wip_group,count(*) QTY from  "+tab107;
                sql += " where IMEI IN(" + ztemp + ")  "+notWip;
                sql += " group by model_name,version_code,pallet_no,imei,group_name,wip_group";
                var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text,
                });
                var z = result.Data.ToListObject<zFGcolumn>();
                zoutput = null;
                zoutput = z.Cast<zFGcolumn>().ToList();
                dataGridView1.DataSource = null;
                if (zoutput.Count == 0)
                {
                    zoutput = null;
                    showMessage("No data found|Không tìm thấy dữ liệu");
                    return;
                }
                dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 13);
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 16);
                dataGridView1.DataSource = zoutput.Select(p => new {
                    p.MODEL_NAME,
                    p.VERSION_CODE,
                    p.PALLET_NO,
                    p.IMEI,
                    p.GROUP_NAME,
                    p.WIP_GROUP,
                    p.QTY,
                    p.RESULT
                }).ToList();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.Refresh();
                dataGridView1.Columns[dataGridView1.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                zFlag = true;
                btnHold.Enabled = true;
                btnUnHold.Enabled = true;
                return;
            }
            #endregion
            if (cbxGroup.Checked)
            {
                if (cbbGroup.Text.Trim() == "")
                {
                    showMessage("Group is null!");
                    cbbGroup.Focus();
                    return;
                }
            }

            if (cbxTestTime.Checked)
            {
                if (cbbTimeStart.Text.Trim() == "")
                {
                    showMessage("Start time is null!");
                    cbbTimeStart.Focus();
                    return;
                }

                if (cbbTimeEnd.Text.Trim() == "")
                {
                    showMessage("End time is null!");
                    cbbTimeEnd.Focus();
                    return;
                }
            }

            if (cbxAteStation.Checked)
            {
                if (txtAteStation.Text.Trim() == "")
                {
                    showMessage("Ate station is null!");
                    txtAteStation.Focus();
                    return;
                }
            }

            if (cbxHoldStockin.Checked || cbxUnLineMo.Checked)
            {
                if (cbbCondition.Text.Trim() != "MO_NUMBER")
                {
                    cbbCondition.Text = "MO_NUMBER";
                }
            }

            if (cbxHoldByStation.Checked)
            {
                cbxTestTime.Checked = false;
                cbxGroup.Checked = false;
                cbxAteStation.Checked = false;
                cbxFG.Checked = false;
                if (cbbStation.Text.Trim() == "")
                {
                    showMessage("Station name is null!");
                    cbbStation.Focus();
                    return;
                }
            }

            //HOLD CAM IN R_CAMERA_T
            if (cbxHoldMaterial.Checked)
            {
                if (cbbCondition.Text.Trim() != "SERIAL_NUMBER")
                {
                    showMessage("Condition must is SERIAL_NUMBER / SHIPPING_SN!");                    
                    cbbCondition.SelectAll();
                    return;
                }
                else
                {
                    sql = "SELECT * FROM SFISM4.R_CAMERA_T WHERE " + cbbCondition.Text + " IN ( ";
                    for (int i = 0; i < linecount; i++)
                    {
                        if (i == 0)
                        {
                            sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                        }
                        else
                        {
                            sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                        }
                    }
                    sql = sql + " ) ";
                    if (cbxTestTime.Checked)
                    {
                        begintime = dateTimeStart.Value.ToString("yyyyMMdd") + " " + cbbTimeStart.Text.Trim(); ;
                        endtime = dateTimeEnd.Value.ToString("yyyyMMdd") + " " + cbbTimeEnd.Text.Trim(); ;
                        sql = sql + " AND MANUAFACTURE_TIME BETWEEN TO_DATE('" + begintime + "','YYYYMMDD HH24:MI') " +
                            " AND TO_DATE('" + endtime + "','YYYYMMDD HH24:MI')";
                    }

                    var resultCamera = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text,                        
                    });
                    var listCamera = resultCamera.Data.ToListObject<infCamera>();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = listCamera;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dataGridView1.Refresh();
                    lblTotalQty.Text = listCamera.Count().ToString();
                }
            }//END TO SEARCH MATERIAL
            else if (cbxUnHoldMaterial.Checked) //UNHOLD CAM IN R_CAMERA_T
            {
                if (cbbCondition.Text.Trim() != "SERIAL_NUMBER")
                {
                    showMessage("Condition must is SERIAL_NUMBER / SHIPPING_SN!");
                    cbbCondition.SelectAll();
                    return;
                }
                else
                {
                    sql = "SELECT * FROM SFISM4.R_CAMERA_T WHERE REPLACE(SERIAL_NUMBER,'HOLD-','') IN(";
                    for (int i = 0; i < linecount; i++)
                    {                        
                        if (i == 0)
                        {
                            sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                        }
                        else
                        {
                            sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                        }
                    }
                    sql = sql + " ) ";
                    if (cbxTestTime.Checked)
                    {
                        begintime = dateTimeStart.Value.ToString("yyyyMMdd") + " " + cbbTimeStart.Text.Trim(); ;
                        endtime = dateTimeEnd.Value.ToString("yyyyMMdd") + " " + cbbTimeEnd.Text.Trim(); ;
                        sql = sql + " AND MANUAFACTURE_TIME BETWEEN TO_DATE('" + begintime + "','YYYYMMDD HH24:MI') " +
                            " AND TO_DATE('" + endtime + "','YYYYMMDD HH24:MI')";
                    }

                    var resultUnholdCamera = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    var listUnholdCamera = resultUnholdCamera.Data.ToListObject<infCamera>();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = listUnholdCamera;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dataGridView1.Refresh();
                    lblTotalQty.Text = listUnholdCamera.Count().ToString();
                }
            }//END TO SEARCH MATERIAL
            else if (cbxHoldStockin.Checked)
            {
                sql = "SELECT MO_NUMBER, MO_TYPE, MODEL_NAME, VERSION_CODE, TARGET_QTY, MO_CREATE_DATE, ROUTE_CODE, " +
                    " INPUT_QTY, OUTPUT_QTY, CLOSE_FLAG, UPC_CO FROM SFISM4.R_MO_BASE_T WHERE " + cbbCondition.Text + " IN(";
                for (int i = 0; i < linecount; i++)
                {
                    if (i == 0)
                    {
                        sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                    }
                    else
                    {
                        sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                    }
                }
                sql = sql + ") ";

                var resultMo = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text,
                });
                var a = resultMo.Data.ToListObject<infMo>();
                List<infMo> listMo = a.Cast<infMo>().ToList();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = listMo.Select(p => new { p.MO_NUMBER, p.MO_TYPE, p.MODEL_NAME, p.VERSION_CODE, p.TARGET_QTY, p.MO_CREATE_DATE, p.ROUTE_CODE, p.INPUT_QTY, p.OUTPUT_QTY, p.CLOSE_FLAG, p.UPC_CO }).ToList();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.Refresh();
                lblTotalQty.Text = listMo.Count().ToString();
            }
            else if (cbxUnLineMo.Checked)
            {
                for (int i = 0; i < linecount; i++)
                {
                    s_sql = "SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + txtCondition.Lines[i].ToString() + "'";

                    var resultMo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = s_sql,
                        SfcCommandType = SfcCommandType.Text,
                    });

                    if (resultMo.Data != null)
                    {
                        showMessage("MO_NUMBER: " + txtCondition.Lines[i].ToString() + " Has been onlined !!");
                        return;
                    }
                    else
                    {
                        s_sql = "SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER = '" + txtCondition.Lines[i].ToString() + "' ";

                        var resultWo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = s_sql,
                            SfcCommandType = SfcCommandType.Text,
                        });

                        if (resultWo.Data == null)
                        {
                            showMessage("MO_NUMBER: " + txtCondition.Lines[i].ToString() + " not download from SAP !!");
                            return;
                        }
                        else
                        {
                            temp = temp + "'" + txtCondition.Lines[i].ToString() + "',";
                        }
                    }
                }

                var resultMoSAP = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = "SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER IN ("+ temp.Substring(0,temp.Length-1) + ")",
                    SfcCommandType = SfcCommandType.Text,
                });
                var listMoSAP = resultMoSAP.Data.ToListObject<infMoSAP>();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = listMoSAP;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.Refresh();
                lblTotalQty.Text = listMoSAP.Count().ToString();
            }
            else
            {
                if (cbxFG.Checked && cbbCondition.Text != "HOLD_NO")
                {
                    if (cbbCondition.Text == "SSN1")
                    {
                        sql = " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, " +
                         " LINE_NAME, GROUP_NAME, IN_STATION_TIME, SHIPPING_SN, PO_NO, REWORK_NO, " +
                         " SPECIAL_ROUTE, PALLET_NO, NEXT_STATION, " +
                         " CARTON_NO, PMCC, ATE_STATION_NO, IMEI, MCARTON_NO, " +
                         " STOCK_NO, TRAY_NO, WIP_GROUP, SHIPPING_SN2 FROM SFISM4.Z_WIP_TRACKING_T WHERE SERIAL_NUMBER IN (" +
                          "SELECT SERIAL_NUMBER FROM SFISM4.R_CUSTSN_T WHERE " + cbbCondition.Text + " IN (";

                        for (int i = 0; i < linecount; i++)
                        {
                            if (i == 0)
                            {
                                sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                            }
                            else
                            {
                                sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                            }
                        }
                        sql = sql + ") ";
                        if (cbxGroup.Checked)
                        {
                            sql = sql + " AND WIP_GROUP = '" + cbbGroup.Text + "'";
                        }
                        sql = sql + ") AND WIP_GROUP <> 'SHIPPING' AND WIP_GROUP <> 'CHECK_OUT'";
                    }
                    else
                    {
                        
                        sql = " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, " +
                         " LINE_NAME, GROUP_NAME, IN_STATION_TIME, SHIPPING_SN, PO_NO, REWORK_NO, " +
                         " SPECIAL_ROUTE, PALLET_NO, NEXT_STATION, " +
                         " CARTON_NO, PMCC, ATE_STATION_NO, IMEI, MCARTON_NO, " +
                         " STOCK_NO, TRAY_NO, WIP_GROUP, SHIPPING_SN2 FROM SFISM4.Z_WIP_TRACKING_T WHERE " + cbbCondition.Text + " IN(";
                        for (int i = 0; i < linecount; i++)
                        {
                            if (i == 0)
                            {
                                sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                            }
                            else
                            {
                                sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                            }
                        }
                        sql = sql + ") ";
                        if (cbxGroup.Checked)
                        {
                            sql = sql + " AND WIP_GROUP = '" + cbbGroup.Text + "'";
                        }
                        sql = sql + " AND WIP_GROUP <> 'SHIPPING' AND WIP_GROUP <> 'CHECK_OUT'";
                    }
                    var resultcbxFG = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    var a = resultcbxFG.Data.ToListObject<infcbxFG>();
                    List<infcbxFG> listcbxFG = a.Cast<infcbxFG>().ToList();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = listcbxFG.Select(p => new { p.SERIAL_NUMBER, p.MO_NUMBER, p.MODEL_NAME, p.VERSION_CODE, p.LINE_NAME, p.GROUP_NAME, p.IN_STATION_TIME,
                        p.SHIPPING_SN, p.PO_NO, p.REWORK_NO, p.SPECIAL_ROUTE, p.PALLET_NO, p.NEXT_STATION, p.CARTON_NO, p.PMCC, p.ATE_STATION_NO, p.IMEI, p.MCARTON_NO, p.STOCK_NO,
                        p.TRAY_NO, p.WIP_GROUP, p.SHIPPING_SN2
                    }).ToList();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dataGridView1.Refresh();
                    lblTotalQty.Text = listcbxFG.Count().ToString();
                }
                else if (cbbCondition.Text == "HOLD_NO")
                {
                    sql = " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, " +
                         " LINE_NAME, GROUP_NAME, IN_STATION_TIME, SHIPPING_SN, PO_NO, REWORK_NO, " +
                         " SPECIAL_ROUTE, PALLET_NO, NEXT_STATION, " +
                         " CARTON_NO, PMCC, ATE_STATION_NO, IMEI, MCARTON_NO, " +
                         " STOCK_NO, TRAY_NO, WIP_GROUP, SHIPPING_SN2 FROM SFISM4.Z_WIP_TRACKING_T WHERE PMCC IN (";
                    for (int i = 0; i < linecount; i++)
                    {
                        if (i == 0)
                        {
                            sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                        }
                        else
                        {
                            sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                        }
                    }
                    sql = sql + ") ";
                    sql = sql + " AND WIP_GROUP <> 'SHIPPING' AND WIP_GROUP <> 'CHECK_OUT'";

                    var resultcbbCondition = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    var a = resultcbbCondition.Data.ToListObject<infcbxFG>();
                    List<infcbxFG> listcbbCondition = a.Cast<infcbxFG>().ToList();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = listcbbCondition.Select(p => new { p.SERIAL_NUMBER, p.MO_NUMBER, p.MODEL_NAME, p.VERSION_CODE, p.LINE_NAME, p.GROUP_NAME, p.IN_STATION_TIME, p.SHIPPING_SN,
                        p.PO_NO, p.REWORK_NO, p.SPECIAL_ROUTE, p.PALLET_NO, p.NEXT_STATION, p.CARTON_NO, p.PMCC, p.ATE_STATION_NO, p.IMEI, p.MCARTON_NO, p.STOCK_NO, p.TRAY_NO, p.WIP_GROUP,
                        p.SHIPPING_SN2
                    }).ToList();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dataGridView1.Refresh();
                    lblTotalQty.Text = listcbbCondition.Count().ToString();
                }
                else if(cbbCondition.Text == "SSN1")
                {
                    sql = " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, " +
                         " LINE_NAME, GROUP_NAME, IN_STATION_TIME, SHIPPING_SN, PO_NO, REWORK_NO, " +
                         " SPECIAL_ROUTE, PALLET_NO, NEXT_STATION, " +
                         " CARTON_NO, PMCC, ATE_STATION_NO, IMEI, MCARTON_NO, " +
                         " STOCK_NO, TRAY_NO, WIP_GROUP, SHIPPING_SN2 FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER IN (" +
                          "SELECT SERIAL_NUMBER FROM SFISM4.R_CUSTSN_T WHERE " + cbbCondition.Text + " IN( ";

                    c_sql = " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, " +
                         " LINE_NAME, GROUP_NAME, IN_STATION_TIME, SHIPPING_SN, PO_NO, REWORK_NO, " +
                         " SPECIAL_ROUTE, PALLET_NO, NEXT_STATION, " +
                         " CARTON_NO, PMCC, ATE_STATION_NO, IMEI, MCARTON_NO, " +
                         " STOCK_NO, TRAY_NO, WIP_GROUP, SHIPPING_SN2 FROM SFISM4.Z_WIP_TRACKING_T WHERE SERIAL_NUMBER IN (" +
                          "SELECT SERIAL_NUMBER FROM SFISM4.R_CUSTSN_T WHERE " + cbbCondition.Text + " IN( ";

                    for (int i = 0; i < linecount; i++)
                    {
                        if (i == 0)
                        {
                            sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                            c_sql = c_sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                        }
                        else
                        {
                            sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                            c_sql = c_sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                        }
                    }

                    sql = sql + ") ";
                    c_sql = c_sql + ")) ";

                    sql = sql + ") AND WIP_GROUP <> 'FG' AND LENGTH(SHIP_NO) < 4  AND SUBSTR(WIP_GROUP,1,2) <> 'SC'";

                    if (!cbxFG.Checked && !cbxHoldMaterial.Checked && !cbxUnHoldMaterial.Checked)
                    {
                        if (cbbCondition.Text != "HOLD_NO")
                        {
                            c_sql = c_sql + " AND WIP_GROUP <> 'FG' AND SUBSTR(WIP_GROUP,1,2) <> 'SC' AND ROWNUM = 1";

                            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = c_sql,
                                SfcCommandType = SfcCommandType.Text,
                            });
                            if (result.Data != null)
                            {
                                showMessage("DATA include the products in FG or NO data HOLD");
                                return;
                            }
                        }
                    }

                    var resultcbbCondition = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    var a = resultcbbCondition.Data.ToListObject<infR107>();
                    List<infR107> listcbbCondition = a.Cast<infR107>().ToList();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = listcbbCondition.Select(p => new {
                        p.SERIAL_NUMBER,
                        p.MO_NUMBER,
                        p.MODEL_NAME,
                        p.VERSION_CODE,
                        p.LINE_NAME,
                        p.GROUP_NAME,
                        p.IN_STATION_TIME,
                        p.SHIPPING_SN,
                        p.SPECIAL_ROUTE,
                        p.PALLET_NO,
                        p.NEXT_STATION,
                        p.CARTON_NO,
                        p.PMCC,
                        p.ATE_STATION_NO,
                        p.IMEI,
                        p.MCARTON_NO,
                        p.STOCK_NO,
                        p.TRAY_NO,
                        p.WIP_GROUP,
                        p.SHIPPING_SN2
                    }).ToList();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dataGridView1.Refresh();
                    lblTotalQty.Text = listcbbCondition.Count().ToString();
                }
                else
                {
                    sql = " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, " +
                            " LINE_NAME, GROUP_NAME, IN_STATION_TIME, SHIPPING_SN, " +
                            " SPECIAL_ROUTE, PALLET_NO, NEXT_STATION, " +
                            " CARTON_NO, PMCC, ATE_STATION_NO, IMEI, MCARTON_NO, " +
                            " STOCK_NO, TRAY_NO, WIP_GROUP, SHIPPING_SN2 FROM " +
                            " SFISM4.R_WIP_TRACKING_T WHERE " +
                            cbbCondition.Text + " IN( ";

                    c_sql = " SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, " +
                            " LINE_NAME, GROUP_NAME, IN_STATION_TIME, SHIPPING_SN, " +
                            " SPECIAL_ROUTE, PALLET_NO, NEXT_STATION, " +
                            " CARTON_NO, PMCC, ATE_STATION_NO, IMEI, MCARTON_NO, " +
                            " STOCK_NO, TRAY_NO, WIP_GROUP, SHIPPING_SN2 FROM " +
                            " SFISM4.Z_WIP_TRACKING_T WHERE " +
                            cbbCondition.Text + " IN (";

                    for (int i = 0; i < linecount; i++)
                    {
                        if (i == 0)
                        {
                            sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                            c_sql = c_sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                        }
                        else
                        {
                            sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                            c_sql = c_sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                        }
                    }
                    sql = sql + ") ";
                    c_sql = c_sql + ") ";
                    
                    sql = sql + " AND WIP_GROUP <> 'FG' AND LENGTH(SHIP_NO) < 4  AND SUBSTR(WIP_GROUP,1,2) <> 'SC'";

                    if (!cbxFG.Checked && !cbxHoldMaterial.Checked && !cbxUnHoldMaterial.Checked)
                    {
                        if (cbbCondition.Text != "HOLD_NO")
                        {
                            c_sql = c_sql + " AND WIP_GROUP <> 'FG' AND SUBSTR(WIP_GROUP,1,2) <> 'SC' AND ROWNUM = 1";

                            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = c_sql,
                                SfcCommandType = SfcCommandType.Text,
                            });
                            if (result.Data != null)
                            {
                                showMessage("DATA include the products in FG or NO data HOLD");
                                return;
                            }
                        }
                    }

                    if (cbxHoldByStation.Checked)
                    {
                        sql = sql + " AND WIP_GROUP ='" + cbbStation.Text.Trim() + "' ";
                    }
                    if(cbxTestTime.Checked || cbxAteStation.Checked)
                    {
                        if (cbxTestTime.Checked)
                        {
                            begintime = dateTimeStart.Value.ToString("yyyyMMdd") + " " + cbbTimeStart.Text.Trim(); ;
                            endtime = dateTimeEnd.Value.ToString("yyyyMMdd") + " " + cbbTimeEnd.Text.Trim(); ;
                        }
                        sql = sql + " AND SERIAL_NUMBER IN ( ";
                        sql = sql + " SELECT SERIAL_NUMBER FROM SFISM4.R_SN_DETAIL_T WHERE  ";

                        if (cbxTestTime.Checked && cbxAteStation.Checked)
                        {
                            sql = sql + " IN_STATION_TIME BETWEEN TO_DATE('" + begintime + "','YYYYMMDD HH24:MI')  " +
                                            " AND TO_DATE('" + endtime + "','YYYYMMDD HH24:MI')  " +
                                            " AND GROUP_NAME='" + cbbGroup.Text.Trim() + "' " +
                                            " AND ATE_STATION_NO='" + txtAteStation.Text.Trim() + "' ";
                        }
                        else if (cbxTestTime.Checked && !cbxAteStation.Checked)
                        {
                            sql = sql + " IN_STATION_TIME BETWEEN TO_DATE('" + begintime + "','YYYYMMDD HH24:MI')  " +
                                            " AND TO_DATE('" + endtime + "','YYYYMMDD HH24:MI')  " +
                                            " AND GROUP_NAME='" + cbbGroup.Text.Trim() + "' ";
                        }
                        else if (!cbxTestTime.Checked && cbxAteStation.Checked)
                        {
                            sql = sql + " ATE_STATION_NO='" + txtAteStation.Text.Trim() + "' ";
                            if (cbxGroup.Checked)
                            {
                                sql = sql + " AND GROUP_NAME= '" + cbbGroup.Text.Trim() + "'";
                            }
                        }

                        sql = sql + " AND  " + cbbCondition.Text + " IN ( ";

                        for (int i = 0; i < linecount; i++)
                        {                            
                            if (i == 0)
                            {
                                sql = sql + " '" + txtCondition.Lines[i].ToString().Trim() + "' ";
                            }
                            else
                            {
                                sql = sql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                            }
                        }

                        sql = sql + " ) ";
                        sql = sql + " ) ";
                    }
                    else
                    {
                        if (cbxGroup.Checked)
                        {
                            sql = sql + " AND WIP_GROUP ='" + cbbGroup.Text + "'";
                        }
                    }

                    var resultcbbCondition = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    var a = resultcbbCondition.Data.ToListObject<infR107>();
                    List<infR107> listcbbCondition = a.Cast<infR107>().ToList();
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = listcbbCondition.Select(p => new { p.SERIAL_NUMBER, p.MO_NUMBER, p.MODEL_NAME, p.VERSION_CODE, p.LINE_NAME, p.GROUP_NAME, p.IN_STATION_TIME,
                        p.SHIPPING_SN, p.SPECIAL_ROUTE, p.PALLET_NO, p.NEXT_STATION, p.CARTON_NO, p.PMCC, p.ATE_STATION_NO, p.IMEI, p.MCARTON_NO, p.STOCK_NO,
                        p.TRAY_NO, p.WIP_GROUP, p.SHIPPING_SN2
                    }).ToList();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dataGridView1.Refresh();
                    lblTotalQty.Text = listcbbCondition.Count().ToString();
                }
            }

            if(dataGridView1.RowCount > 0)
            {
                btnHold.Enabled = true;
                btnUnHold.Enabled = true;
            }
        }

        private async void btnHold_Click(object sender, EventArgs e)
        {
            if (txtReason.Text.Trim() == "")
            {
                showMessage("Please input hold reasons");
                return;
            }
            if (chkTest.Checked)
            {
                if(!zFlag)
                {
                    showMessage("Please search first|Tìm kiếm dữ liệu trước");
                    return;
                }
                for(int i=0;i< zoutput.Count;i++)
                {
                    string inFunction = "HOLDBYIMEI", inData = "TABLE:" + tab107;
                    if (cbxFG.Checked) inData = "TABLE:" + tab107;
                    inData += "|IMEI:"+zoutput[i].IMEI+"|GROUP_NAME:"+zoutput[i].GROUP_NAME+"|WIP_GROUP:"+zoutput[i].WIP_GROUP+"|QTY:"+zoutput[i].QTY+"|REASON:"+txtReason.Text.Trim();
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.SP_HOLD",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="AP_VER",Value=FormMain.appVer,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_EMP",Value=FormMain.empNo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="PCMAC",Value=FormMain.MACAddress,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="PCIP",Value=FormMain.IP,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_FUNC",Value=inFunction,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_DATA",Value=inData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                    });
                    dynamic _ads = result.Data;
                    string _RES = _ads[1]["res"];
                    string _outData = _ads[0]["out_data"];
                    if(_RES.IndexOf("|")!=-1)
                    {
                        if (FormMain.lang == "EN") _RES=_RES.Split('|')[0];
                        else _RES = _RES.Split('|')[1];
                    }
                    zoutput[i].RESULT = _RES;
                    dataGridView1.DataSource = zoutput;
                    dataGridView1.Refresh();
                }
                zFlag = false;
                showMessage("Completed");
                return;
            }
            string newroutecode = "", serial_number = "", logstr = "", tmplogstr = "", routecode = "";
            int successQty = 0, failQty = 0, totalQty = 0;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = 100;
            linecount = Regex.Matches(txtCondition.Text, "\r\n").Count + 1;

            for (int i = 0; i < linecount; i++)
            {
                if (ChkInput_data_valid(txtCondition.Lines[i].ToString()))
                {
                    showMessage("Du lieu nhap vao khong hop le-->" + txtCondition.Lines[i].ToString().Trim());
                    txtCondition.Focus();
                    return;
                }
            }

            if (dataGridView1.RowCount == 0)
            {
                showMessage("There is no data by HOLD");
                return;
            }

            if (cbxUnHoldMaterial.Checked)
            {
                showMessage("Please click unhold");
                return;
            }

            if (cbxHoldMaterial.Checked)
            {
                totalQty = linecount;
                for (int i = 0; i < linecount; i++)
                {
                    if (await HoldBySerialMaterial(txtCondition.Lines[i].ToString().Trim()))
                    {
                        successQty = successQty + 1;
                        lblSuccessQty.Text = successQty.ToString();
                        lblSuccessQty.Refresh();
                    }
                    else
                    {
                        failQty = failQty + 1;
                        lblFailQty.Text = failQty.ToString();
                        lblFailQty.Refresh();
                    }
                    if (i == (totalQty - 1))
                    {
                        toolStripProgressBar1.Value = 100;
                    }
                    else
                    {
                        toolStripProgressBar1.Value = (int)((((double)i) / (totalQty - 1)) * 100);
                    }
                    statusStrip1.Refresh();
                }
            }
            else if (cbxHoldStockin.Checked)
            {
                totalQty = linecount;
                for (int i = 0; i < linecount; i++)
                {
                    if (await HoldStockinByMo(txtCondition.Lines[i].ToString().Trim(), "Y"))
                    {
                        successQty = successQty + 1;
                        lblSuccessQty.Text = successQty.ToString();
                        lblSuccessQty.Refresh();
                    }
                    else
                    {
                        failQty = failQty + 1;
                        lblFailQty.Text = failQty.ToString();
                        lblFailQty.Refresh();
                    }

                    if (i == (totalQty - 1))
                    {
                        toolStripProgressBar1.Value = 100;
                    }
                    else
                    {
                        toolStripProgressBar1.Value = (int)((((double)i) / (totalQty - 1)) * 100);
                    }
                    statusStrip1.Refresh();
                }
            }
            else if (cbxUnLineMo.Checked)
            {
                totalQty = linecount;
                for (int i = 0; i < linecount; i++)
                {
                    if (await HoldUnLineMo(txtCondition.Lines[i].ToString().Trim()))
                    {
                        successQty = successQty + 1;
                        lblSuccessQty.Text = successQty.ToString();
                        lblSuccessQty.Refresh();
                    }
                    else
                    {
                        failQty = failQty + 1;
                        lblFailQty.Text = failQty.ToString();
                        lblFailQty.Refresh();
                    }
                    if (i == (totalQty - 1))
                    {
                        toolStripProgressBar1.Value = 100;
                    }
                    else
                    {
                        toolStripProgressBar1.Value = (int)((((double)i) / (totalQty - 1)) * 100);
                    }
                    statusStrip1.Refresh();
                }
            }
            else
            {
                if (cbbCondition.Text == "HOLD_NO")
                {
                    showMessage("Please click unhold");
                    return;
                }
                else if (cbbCondition.Text == "MO_NUMBER" && cbxHoldByStation.Checked)
                {
                    for (int i = 0; i < linecount; i++)
                    {
                        routecode = await getroutecode(txtCondition.Lines[i].ToString().Trim());
                        newroutecode = await getNewRouteCode(routecode);
                        if (routecode.Trim() == "")
                        {
                            showMessage("Mo number:" + txtCondition.Lines[i].ToString().Trim() + " not found !!");
                            return;
                        }
                        if (await updateRouteCode(txtCondition.Lines[i].ToString().Trim(), newroutecode))
                        {
                            await holdRouteNext(newroutecode, cbbStation.Text);
                        }
                        else
                        {
                            showMessage("Update ROUTE_CODE error");
                            return;
                        }
                    }
                }
                else
                {
                    totalQty = dataGridView1.RowCount;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        serial_number = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        if (await holdBySerial(serial_number))
                        {
                            successQty = successQty + 1;
                            lblSuccessQty.Text = successQty.ToString();
                            lblSuccessQty.Refresh();
                        }
                        else
                        {
                            failQty = failQty + 1;
                            lblFailQty.Text = failQty.ToString();
                            lblFailQty.Refresh();
                            txtErrorSN.AppendText(serial_number + "have been hold" + "\n");
                        }
                        if (i == (totalQty - 1))
                        {
                            toolStripProgressBar1.Value = 100;
                        }
                        else
                        {
                            toolStripProgressBar1.Value = (int)((((double)i) / (totalQty - 1)) * 100);
                        }
                        statusStrip1.Refresh();
                    }
                }
            }
            tmplogstr = "";
            logstr = getChooseItem();
                for (int i = 0; i < linecount; i++)
                {
                    tmplogstr = tmplogstr + txtCondition.Lines[i].ToString().Trim();
                    if (tmplogstr.Length >= 800)
                    {
                        tmplogstr = tmplogstr + "; Qty:" + lblTotalQty.Text;
                        if (await SaveLog("HOLD", logstr + cbbCondition.Text + ":" + tmplogstr + "; Reason:" + txtReason.Text.Trim()))
                        {
                            tmplogstr = "";
                        }
                    }
                }
                tmplogstr = tmplogstr + "; Qty:" + lblTotalQty.Text;
                if (await SaveLog("HOLD", logstr + cbbCondition.Text + ":" + tmplogstr + "; Reason:" + txtReason.Text.Trim()))
                {
                    MessageBox.Show("Hold success!", ERRORSTRING);
                }
            Application.DoEvents();
            btnHold.Enabled = false;
            btnUnHold.Enabled = false;
        }

        private async Task<bool> holdBySerial(string serial_number)
        {
            string ssql = "";
            await InsertLogHold(serial_number, "", empNo, txtReason.Text.Trim(), "HOLD-PROGRAM", "0");

            if (cbxFG.Checked)
            {  
                ssql = " UPDATE SFISM4.Z_WIP_TRACKING_T SET GROUP_NAME = DECODE (SUBSTR (GROUP_NAME, 1, 4), " +
                       " 'HOLD', GROUP_NAME,'HOLD-' || GROUP_NAME ),NEXT_STATION =  " +
                       " DECODE (SUBSTR (NEXT_STATION, 1, 4),'HOLD', NEXT_STATION, " +
                       " 'HOLD-' || NEXT_STATION  ),WIP_GROUP=DECODE (SUBSTR (WIP_GROUP, 1, 4), " +
                       " 'HOLD', WIP_GROUP,'HOLD-'||WIP_GROUP) " +
                       " WHERE SERIAL_NUMBER = '" + serial_number + "' ";
            }
            else
            {
                ssql = " UPDATE SFISM4.R_WIP_TRACKING_T SET GROUP_NAME = DECODE (SUBSTR (GROUP_NAME, 1, 4), " +
                       " 'HOLD', GROUP_NAME,'HOLD-' || GROUP_NAME ),NEXT_STATION =  " +
                       " DECODE (SUBSTR (NEXT_STATION, 1, 4),'HOLD', NEXT_STATION, " +
                       " 'HOLD-' || NEXT_STATION  ),WIP_GROUP=DECODE (SUBSTR (WIP_GROUP, 1, 4), " +
                       " 'HOLD', WIP_GROUP,'HOLD-'||WIP_GROUP) " +
                       " WHERE SERIAL_NUMBER = '" + serial_number + "' ";
            }
            try
            {  
                var sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> SaveLog(string action, string action_desc)
        {
            var logInfo = new
            {
                TYPE = "SAVELOG",
                EMP_NO = empNo,
                PRG_NAME = "HOLD",
                ACTION_TYPE = action,
                ACTION_DESC = action_desc

            };

            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
            try
            {
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
                if (Ok.Substring(0, 2) == "OK")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            
        }

        private async Task<bool> holdRouteNext(string route_code, string next_station)
        {
            string ssql = "";
            await SaveLog("HOLD", empNo + " HOLD " + next_station + " IN MO_NUMBER " + txtCondition.Text + " ROUTE_CODE " + route_code);

            try
            {
                ssql = " UPDATE SFIS1.C_ROUTE_CONTROL_T SET GROUP_NEXT = DECODE (SUBSTR (GROUP_NEXT, 1, 5), 'HOLD-', GROUP_NEXT,'HOLD-' || GROUP_NEXT) " +
                    " WHERE STATE_FLAG = '0' AND ROUTE_CODE ='" + route_code + "' AND GROUP_NEXT = '" + next_station + "' ";
                //dbsfis.ExecuteNonQuery(ssql);
                var sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()

                });

                return true;
            }
            catch (Exception e)
            {
                showMessage(e.Message);
                //MessageBox.Show(e.Message, errorstring);
                return false;
                throw;
            } 
        }

        private async Task<bool> updateRouteCode(string mo_number, string route_code)
        {
            string ssql = "";
            try
            {
                ssql = " UPDATE SFISM4.R_MO_BASE_T SET ROUTE_CODE='" + route_code + "' WHERE MO_NUMBER='" + mo_number + "' ";
                //dbsfis.ExecuteNonQuery(ssql);
                var sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });

                ssql = " UPDATE SFISM4.R_WIP_TRACKING_T SET SPECIAL_ROUTE='" + route_code + "' WHERE MO_NUMBER='" + mo_number + "' ";
                //dbsfis.ExecuteNonQuery(ssql);
                sbUpdate.Append(ssql);
                result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });

                return true;
            }
            catch (Exception e)
            {
                showMessage(e.Message);
                //MessageBox.Show(e.Message, errorstring);
                return false;
                throw;
            }
        }

        private async Task<string> getNewRouteCode(string route_code)
        {
            string ssql, routename, newroutecode = "", newroutename = "";
            int i = 1;
            System.Data.DataTable dt = new System.Data.DataTable();

            ssql = "SELECT * FROM SFIS1.C_ROUTE_NAME_T WHERE ROUTE_CODE='" + route_code + "'";

            //dt = dbsfis.DoSelectQuery(ssql);

            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                var ROUTE = result.Data.ToObject<LogInformation>();
                //routename = dt.Rows[0]["ROUTE_NAME"].ToString();
                routename = ROUTE.ROUTE_NAME;
            }
            else
            {
                showMessage("Route code:" + route_code + "  not found !!");
                //MessageBox.Show("Route code:" + route_code + "  not found !!", errorstring);
                return "";
            }

            if (routename.IndexOf("HOLD") > 0)
            {
                newroutecode = route_code;
                return newroutecode;
            }
            else
            {
                try
                {
                    newroutename = routename + "HOLD";
                    while (true)
                    {
                        if ( await findroutename(newroutename + i.ToString()))
                        {
                            newroutename = newroutename + i.ToString();
                            break;
                        }
                        i = i + 1;
                    }

                    ssql = "SELECT MAX(ROUTE_CODE)+1 ROUTECODE FROM SFIS1.C_ROUTE_NAME_T";

                    //dt = dbsfis.DoSelectQuery(ssql);

                    var resultMaxRoute = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });

                    if (resultMaxRoute.Data != null)
                    {
                        var MAX_ROUTE = resultMaxRoute.Data.ToObject<LogInformation>();
                        //newroutecode = dt.Rows[0]["ROUTECODE"].ToString();
                        newroutecode = MAX_ROUTE.ROUTECODE;
                    }

                    ssql = " INSERT INTO SFIS1.C_ROUTE_NAME_T " +
                           " (ROUTE_CODE, ROUTE_NAME, ROUTE_DESC) SELECT " +
                           " '" + newroutecode + "','" + newroutename + "',ROUTE_DESC FROM   " +
                           " SFIS1.C_ROUTE_NAME_T WHERE ROUTE_CODE='" + route_code + "' ";
                    //dbsfis.ExecuteNonQuery(ssql);
                    var sbInsertRouteName = new StringBuilder();
                    sbInsertRouteName.Append(ssql);
                    var resultInsertRouteName = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbInsertRouteName.ToString()
                    });

                    ssql = " INSERT INTO  SFIS1.C_ROUTE_CONTROL_T " +
                           " (ROUTE_CODE, GROUP_NAME, GROUP_NEXT, STATE_FLAG, STEP_SEQUENCE, ROUTE_DESC) " +
                           " SELECT '" + newroutecode + "', GROUP_NAME, GROUP_NEXT, STATE_FLAG, " +
                           " STEP_SEQUENCE, ROUTE_DESC FROM SFIS1.C_ROUTE_CONTROL_T " +
                           " WHERE ROUTE_CODE='" + route_code + "' ";
                    //dbsfis.ExecuteNonQuery(ssql);
                    var sbInsertRouteCode = new StringBuilder();
                    sbInsertRouteCode.Append(ssql);
                    var resultInsertRouteCode = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbInsertRouteCode.ToString()
                    });

                    return newroutecode;

                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        private async Task<bool> findroutename(string routename)
        {
            string ssql = "";
            ssql = "SELECT * FROM SFIS1.C_ROUTE_NAME_T WHERE ROUTE_NAME='" + routename + "'";
            //DataTable dt = new DataTable();
            //dt = dbsfis.DoSelectQuery(ssql);

            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });

            if (result.Data == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> HoldUnLineMo(string mo_number)
        {
            string ssql = "";
            try
            {
                ssql = "SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER='" + mo_number + "'";

                var resultHoldUnLineMo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (resultHoldUnLineMo.Data != null)
                {
                    showMessage("Mo_number:" + mo_number + " have online");
                    return false;
                }

                ssql = "SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER='" + mo_number + "'";

                var resultMoOnSAP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (resultMoOnSAP.Data != null)
                {
                    ssql = "UPDATE SFISM4.R_BPCS_MOPLAN_T SET LOC='HOLD' WHERE MO_NUMBER='" + mo_number + "' ";

                    var sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    var resultUpdate = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });
                    return true;
                }
                else
                {
                    showMessage("Mo_number:" + mo_number + " not download from sap");
                    return false;
                }
            }
            catch (Exception e)
            {
                showMessage(e.Message.ToString());
                return false;
            }
        }

        private string getChooseItem()
        {
            string tmpstr = "";
            if (cbxHoldStockin.Checked)
            {
                tmpstr = tmpstr + "HoldStockinByMo:true;";
            }
            if (cbxUnLineMo.Checked)
            {
                tmpstr = tmpstr + "HoldUnLineMo:true;";
            }
            if (cbxFG.Checked)
            {
                tmpstr = tmpstr + "HoldFG:true;";
            }
            if (cbxGroup.Checked)
            {
                tmpstr = tmpstr + "HoldGroup:true->" + cbbGroup.Text + ";";
            }
            if (cbxAteStation.Checked)
            {
                tmpstr = tmpstr + "HoldAteStation:true->" + txtAteStation.Text + ";";
            }
            if (cbxHoldByStation.Checked)
            {
                tmpstr = tmpstr + "HoldByStation:true->" + cbbStation.Text + ";";
            }
            if (cbxTestTime.Checked)
            {
                tmpstr = tmpstr + "HoldTest:true->" + dateTimeStart.ToString() + cbbTimeStart.Text +
                             "~" + dateTimeEnd.ToString() + cbbTimeEnd.Text + ";";
            }

            return tmpstr;

        }

        private async Task<bool> HoldStockinByMo(string mo_number, string flag)
        {
            string ssql = "";
            try
            {
                ssql = "SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER ='" + mo_number + "' ";

                var resultHoldStockinByMo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (resultHoldStockinByMo.Data == null)
                {
                    ssql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = 'HOLD' AND VR_CLASS ='" + mo_number + "' ";

                    var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (result.Data == null)
                    {
                        if (flag == "Y")
                        {
                            ssql = "INSERT INTO SFIS1.C_PARAMETER_INI (PRG_NAME, VR_CLASS, VR_ITEM, VR_NAME, VR_VALUE) VALUES " +
                                  " ('HOLD', '" + mo_number + "', 'HOLD', 'HOLD', 'HOLD')";

                            var sbInsert = new StringBuilder();
                            sbInsert.Append(ssql);
                            var resultInsert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbInsert.ToString()
                            });
                        }
                        else
                        {
                            showMessage("Mo_number:" + mo_number + " are not HOLD");
                            return false;
                        }
                    }
                    else
                    {
                        if (flag == "Y")
                        {
                            showMessage("Mo_number:" + mo_number + " had already HOLD");
                            return false;
                        }
                        else
                        {
                            ssql = "DELETE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='HOLD' AND VR_CLASS='" + mo_number + "' ";

                            var sbDelete = new StringBuilder();
                            sbDelete.Append(ssql);
                            var resultDelete = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbDelete.ToString()
                            });
                        }
                    }
                }
                else
                {
                    ssql = "UPDATE SFISM4.R_MO_BASE_T SET UPC_CO = '" + flag + "' WHERE MO_NUMBER = '" + mo_number + "' ";

                    var sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    var resultUpdate = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });

                    ssql = "DELETE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = 'HOLD' AND VR_CLASS ='" + mo_number + "' ";

                    var sbDelete = new StringBuilder();
                    sbDelete.Append(ssql);
                    var resultDelete = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbDelete.ToString()
                    });
                }
                return true;
            }
            catch (Exception e)
            {
                showMessage(e.Message.ToString());
                return false;
            }
        }

        private async Task<bool> InsertLogHold(string serial_number, string main_desc, string emp_no, string hold_reason, string hold_prg, string finish_flag)
        {
            try
            {
                var resultExProcedure = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.SYSTEM_HOLD",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{ Name ="SN", Value = serial_number, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter{ Name ="MAIN_DESC", Value = main_desc,SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter{ Name ="EMP_NO", Value = emp_no, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter{ Name ="REASON", Value = hold_reason,SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter{ Name ="PROGRAM_NAME", Value = hold_prg, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter{ Name ="FLAG", Value = finish_flag,SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },                       
                        new SfcParameter{ Name ="RES",SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                    }
                });
                return true;
            }
            catch(Exception)
            {
                return false;
                throw;
            }
            
            //txtResult.Text = listParams["RES"].Value.ToString();
        }

        private async Task<bool> HoldBySerialMaterial(string serial_number)
        {
            string ssql = "";
            try
            {
                await InsertLogHold(serial_number, "", empNo, txtReason.Text.Trim(), "HOLD-PROGRAM", "0");

                //ssql = "INSERT INTO SFISM4.R_SYSTEM_HOLD_T(SERIAL_NUMBER, MAIN_DESC, HOLD_EMP_NO, HOLD_TIME, HOLD_REASON, HOLD_PROGRAM, FINISH_FLAG)" +
                //       " VALUES('" + serial_number + "', '', '" + empNo + "',SYSDATE, '" + txtReason.Text.Trim() + "', 'HOLD-PROGRAM', '0')";

                ssql = "SELECT * FROM SFISM4.R_CAMERA_T WHERE SERIAL_NUMBER = '" + serial_number + "' AND MACID IS NULL";

                var resultCamera = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                //DataTable dt = dbsfis.DoSelectQuery(ssql);

                if (resultCamera.Data != null)
                {
                    ssql = "UPDATE SFISM4.R_CAMERA_T SET SERIAL_NUMBER = DECODE (SUBSTR (SERIAL_NUMBER, 1, 4),'HOLD', SERIAL_NUMBER,'HOLD-' || SERIAL_NUMBER )" +
                        " WHERE SERIAL_NUMBER = '" + serial_number + "'";

                    //dbsfis.ExecuteNonQuery(ssql);
                    var sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });
                }
                else
                {
                    ssql = "UPDATE SFISM4.R_CAMERA_T SET SERIAL_NUMBER = DECODE(SUBSTR(SERIAL_NUMBER, 1, 4),'HOLD', SERIAL_NUMBER,'HOLD-' || SERIAL_NUMBER)," +
                        " MACID = DECODE(SUBSTR(MACID, 1, 4), 'HOLD', MACID, 'HOLD-' || MACID)" +
                        " WHERE SERIAL_NUMBER = '" + serial_number + "'";

                    //dbsfis.ExecuteNonQuery(ssql);
                    var sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });
                }

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async void btnUnHold_Click(object sender, EventArgs e)
        {
            if (txtReason.Text.Trim() == "")
            {
                showMessage("Please input unhold reasons|Nhập nguyên nhân unhold");
                return;
            }
            if (chkTest.Checked)
            {
                if (!zFlag)
                {
                    showMessage("Please search first|Tìm kiếm dữ liệu trước");
                    return;
                }
                for (int i = 0; i < zoutput.Count; i++)
                {
                    string inFunction = "UNHOLDBYIMEI", inData = "TABLE:" + tab107;
                    if (cbxFG.Checked) inData = "TABLE:" + tab107;
                    inData += "|IMEI:" + zoutput[i].IMEI + "|GROUP_NAME:" + zoutput[i].GROUP_NAME + "|WIP_GROUP:" + zoutput[i].WIP_GROUP + "|QTY:" + zoutput[i].QTY + "|REASON:" + txtReason.Text.Trim();
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.SP_HOLD",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="AP_VER",Value=FormMain.appVer,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_EMP",Value=FormMain.empNo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="PCMAC",Value=FormMain.MACAddress,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="PCIP",Value=FormMain.IP,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_FUNC",Value=inFunction,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_DATA",Value=inData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                    });
                    dynamic _ads = result.Data;
                    string _RES = _ads[1]["res"];
                    string _outData = _ads[0]["out_data"];
                    if (_RES.IndexOf("|") != -1)
                    {
                        if (FormMain.lang == "EN") _RES = _RES.Split('|')[0];
                        else _RES = _RES.Split('|')[1];
                    }
                    zoutput[i].RESULT = _RES;
                    dataGridView1.DataSource = zoutput;
                    dataGridView1.Refresh();
                }
                zFlag = false;
                showMessage("Completed");
                return;
            }
            string routecode = "", c_mo = "", c_model = "", ssql = "";
            int successQty = 0, failQty = 0, totalQty = 0;
            string logstr = "", tmplogstr = "";
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = 100;
            linecount = Regex.Matches(txtCondition.Text, "\r\n").Count + 1;
            string _productname = Application.ProductName;

            for (int i = 0; i < linecount; i++)
            {
                if (ChkInput_data_valid(txtCondition.Lines[i].ToString()))
                {
                    showMessage("Du lieu nhap vao khong hop le-->" + txtCondition.Lines[i].ToString().Trim());
                    txtCondition.Focus();
                    return;
                }
            }
            if(chkTest.Checked)
            {

                return;
            }
            if (linecount == 0)
            {
                showMessage("There is no data by UNHOLD");
                return;
            }
            if(cbbCondition.Text != "HOLD_NO" && cbbCondition.Text != "MO_NUMBER")
            {
                if (await CheckFlagHold())
                {
                    ssql = "SELECT SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, LINE_NAME, GROUP_NAME, IN_STATION_TIME, " +
                            "SHIPPING_SN, PO_NO, REWORK_NO, SPECIAL_ROUTE, PALLET_NO, NEXT_STATION, CARTON_NO, PMCC, ATE_STATION_NO, IMEI, MCARTON_NO, " +
                            "STOCK_NO, TRAY_NO, WIP_GROUP, SHIPPING_SN2 FROM SFISM4.R107 WHERE " + cbbCondition.Text + " IN(";
                    for (int i = 0; i < linecount; i++)
                    {
                        if (i == 0)
                        {
                            ssql = ssql + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                        }
                        else
                        {
                            ssql = ssql + "," + " '" + txtCondition.Lines[i].ToString().Trim() + "' "; ;
                        }
                    }
                    ssql = ssql + ") ";
                    ssql = ssql + " AND WIP_GROUP NOT LIKE '%FG%' AND SUBSTR(WIP_GROUP,1,2)<>'SC' AND (LENGTH(PALLET_NO)>'3' OR LENGTH(CARTON_NO)>'3') ";

                    var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (result.Data != null)
                    {
                        var resultQuery = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SELECT * FROM SFIS1.C_PRIVILEGE WHERE EMP=:EMP AND FUN='UNHOLD_FOR_SATION' AND PRG_NAME ='HOLD'",
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>
                            {
                                new SfcParameter{ Name = "EMP", Value = empNo }
                            }
                        });
                        if (resultQuery.Data == null)
                        {
                            ReportPromptCode("00158");
                            return;
                        }
                    }
                }
            }

            if (cbxHoldStockin.Checked)
            {
                totalQty = linecount;
                for(int i = 0; i < linecount; i++)
                {
                    if (await HoldStockinByMo(txtCondition.Lines[i].ToString().Trim(), "N"))
                    {
                        successQty = successQty + 1;
                        lblSuccessQty.Text = successQty.ToString();
                        lblSuccessQty.Refresh();
                    }
                    else
                    {
                        failQty = failQty + 1;
                        lblFailQty.Text = failQty.ToString();
                        lblFailQty.Refresh();
                    }

                    if (i == (totalQty - 1))
                    {
                        toolStripProgressBar1.Value = 100;
                    }
                    else
                    {
                        toolStripProgressBar1.Value = (int)((((double)i) / (totalQty - 1)) * 100);
                    }
                    statusStrip1.Refresh();
                }
            }
            else if (cbxUnLineMo.Checked)
            {
                totalQty = linecount;
                for(int i = 0; i < linecount; i++)
                {
                    if (await UnHoldUnLineMo(txtCondition.Lines[i].ToString().Trim()))
                    {
                        successQty = successQty + 1;
                        lblSuccessQty.Text = successQty.ToString();
                        lblSuccessQty.Refresh();
                    }
                    else
                    {
                        failQty = failQty + 1;
                        lblFailQty.Text = failQty.ToString();
                        lblFailQty.Refresh();
                    }

                    if (i == (totalQty - 1))
                    {
                        toolStripProgressBar1.Value = 100;
                    }
                    else
                    {
                        toolStripProgressBar1.Value = (int)((((double)i) / (totalQty - 1)) * 100);
                    }
                    statusStrip1.Refresh();
                }
            }
            else
            {
                if (cbbCondition.Text == "HOLD_NO")
                {
                    c_mo = dataGridView1.Rows[0].Cells[1].Value.ToString();
                    c_model = dataGridView1.Rows[0].Cells[2].Value.ToString();
                    if (MessageBox.Show("Please confirm this " + c_mo + " had STOCKIN. Do you want to UNHOLD?", _productname, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        await UPDATEHOLDZ107(txtCondition.Text.Trim());
                        await UPDATER105(c_mo);
                        await SENDMAIL(c_mo, c_model, lblTotalQty.Text);
                    }
                    else
                    {
                        await UPDATEHOLDZ107(txtCondition.Text.Trim());
                        await SENDMAIL(c_mo, c_model, lblTotalQty.Text);
                    }
                }
                else if (cbbCondition.Text == "SERIAL_NUMBER" && cbxUnHoldMaterial.Checked)
                {
                    for (int i = 0; i < linecount; i++)
                    {
                        routecode = await getroutecode(txtCondition.Lines[i].ToString().Trim());
                        if (await unholdroute(txtCondition.Lines[i].ToString().Trim(), routecode))
                        {
                            await SaveLog("UNHOLD", empNo + " UNHOLD MO_NUMBER " + txtCondition.Text + " ROUTE_CODE " + routecode);
                        }
                    }

                    totalQty = linecount;
                    for (int i = 0; i < linecount; i++)
                    {
                        if (await unholdbycamserial(txtCondition.Lines[i].ToString().Trim()))
                        {
                            successQty = successQty + 1;
                            lblSuccessQty.Text = successQty.ToString();
                            lblSuccessQty.Refresh();
                        }
                        else
                        {
                            failQty = failQty + 1;
                            lblFailQty.Text = failQty.ToString();
                            lblFailQty.Refresh();
                        }
                        if (i == (totalQty - 1))
                        {
                            toolStripProgressBar1.Value = 100;
                        }
                        else
                        {
                            toolStripProgressBar1.Value = (int)((((double)i) / (totalQty - 1)) * 100);
                        }
                        statusStrip1.Refresh();
                    }
                }
                else if (cbbCondition.Text == "MO_NUMBER" && cbxHoldByStation.Checked)
                {
                    for (int i = 0; i < linecount; i++)
                    {
                        routecode = await getroutecode(txtCondition.Lines[i].ToString().Trim());
                        if (await unholdroute(txtCondition.Lines[i].ToString().Trim(), routecode))
                        {
                            await SaveLog("UNHOLD", empNo + " UNHOLD MO_NUMBER " + txtCondition.Text + " ROUTE_CODE " + routecode);
                        }
                    }
                }
                else
                {
                    totalQty = dataGridView1.RowCount;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        string serial_number = dataGridView1.Rows[i].Cells[0].Value.ToString();
                        //if (await unholdbyserial(txtCondition.Lines[i].ToString().Trim()))
                        if (await unholdbyserial(serial_number))
                        {
                            successQty = successQty + 1;
                            lblSuccessQty.Text = successQty.ToString();
                            lblSuccessQty.Refresh();
                        }
                        else
                        {
                            failQty = failQty + 1;
                            lblFailQty.Text = failQty.ToString();
                            lblFailQty.Refresh();
                            txtErrorSN.AppendText(serial_number + "\n");
                        }

                        if (i == (totalQty - 1))
                        {
                            toolStripProgressBar1.Value = 100;
                        }
                        else
                        {
                            toolStripProgressBar1.Value = (int)((((double)i) / (totalQty - 1)) * 100);
                        }
                        statusStrip1.Refresh();
                    }
                }
            }
            tmplogstr = "";
            logstr = getChooseItem();
            for (int i = 0; i < linecount; i++)
            {
                tmplogstr = tmplogstr + txtCondition.Lines[i].ToString().Trim();
                if (tmplogstr.Length >= 800)
                {
                    tmplogstr = tmplogstr + "; Qty:" + lblTotalQty.Text;
                    if (await SaveLog("UNHOLD",
                    logstr + cbbCondition.Text + ":" + tmplogstr + "; Reason:" + txtReason.Text.Trim()))
                    {
                        tmplogstr = "";
                    }
                }
            }
            tmplogstr = tmplogstr + "; Qty:" + lblTotalQty.Text;
            if (await SaveLog("UNHOLD",
                    logstr + cbbCondition.Text + ":" + tmplogstr + "; Reason:" + txtReason.Text.Trim()))
            {
                MessageBox.Show("Unhold success!", ERRORSTRING);
            }
            System.Windows.Forms.Application.DoEvents();
            btnHold.Enabled = false;
            btnUnHold.Enabled = false;
        }

        private async void ReportPromptCode(string promptcode)
        {
            var resultQuery = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = " SELECT PROMPT_CHINESE FROM SFIS1.C_PROMPT_CODE_T WHERE PROMPT_CODE=:PROMPT_CODE ",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                            {
                                new SfcParameter{ Name = "PROMPT_CODE", Value = promptcode }
                            }
            });
            if (resultQuery.Data != null)
            {
                var PROMPT = resultQuery.Data.ToObject<LogInformation>();
                StackFrame CallStack = new StackFrame(1, true);
                MessageBox.Show("Error: " + PROMPT.PROMPT_CHINESE + ", File: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
            else
            {
                StackFrame CallStack = new StackFrame(1, true);
                MessageBox.Show("Error: Promt code not found. Please call IT check!, Promt code:" + promptcode + ", File: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }

        private async Task<bool> unholdbyserial(string serial_number)
        {
            string ssql = "";
            #region sua cho b5
            ssql = "select * from SFISM4.R_SYSTEM_HOLD_T where SERIAL_NUMBER='" + serial_number + "' and HOLD_REASON like '%SCAN WRONG%' AND UNHOLD_EMP_NO is null and FINISH_FLAG=0";
            var sbUpdate = new StringBuilder();
            sbUpdate.Append(ssql);
            var resultQueryCheckHOLD = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sbUpdate.ToString(),
                SfcCommandType = SfcCommandType.Text
            });
            #endregion
            await InsertLogHold(serial_number, "", empNo, txtReason.Text, "HOLD-PROGRAM", "1");
            //LogUnHold(serial_number, empNo, txtReason.Text, "HOLD-PROGRAM", "1");
            if (cbxFG.Checked)
            {
                //ssql = "UPDATE SFISM4.Z_WIP_TRACKING_T SET GROUP_NAME = DECODE (SUBSTR (GROUP_NAME, 1, 5),'HOLD-', SUBSTR(GROUP_NAME,6,LENGTH(GROUP_NAME)),GROUP_NAME ), " +
                //        "NEXT_STATION = DECODE (SUBSTR (NEXT_STATION, 1, 5),'HOLD-', SUBSTR(NEXT_STATION,6, LENGTH(NEXT_STATION)), NEXT_STATION ), " +
                //        "WIP_GROUP = DECODE (SUBSTR (WIP_GROUP, 1, 5),'HOLD-', SUBSTR(WIP_GROUP,6,LENGTH(WIP_GROUP)),WIP_GROUP) " +
                //        "WHERE SERIAL_NUMBER = '" + serial_number + "'";
                ssql = String.Format(@"UPDATE  SFISM4.Z_WIP_TRACKING_T  AA  
                    SET AA.WIP_GROUP =
                    CASE WHEN  TO_NUMBER(REGEXP_INSTR(AA.WIP_GROUP, '-HOLD')) > 0 THEN SUBSTR(AA.WIP_GROUP,1, LENGTH(AA.WIP_GROUP) -5) 
                         WHEN  TO_NUMBER(REGEXP_INSTR(AA.WIP_GROUP, 'HOLD-')) > 0 THEN SUBSTR(AA.WIP_GROUP,6, LENGTH(AA.WIP_GROUP)) 
                         ELSE AA.WIP_GROUP END,
                    AA.GROUP_NAME = 
                    CASE WHEN  TO_NUMBER(REGEXP_INSTR(AA.GROUP_NAME, '-HOLD')) > 0 THEN SUBSTR(AA.GROUP_NAME,1, LENGTH(AA.GROUP_NAME) -5) 
                         WHEN  TO_NUMBER(REGEXP_INSTR(AA.GROUP_NAME, 'HOLD-')) > 0 THEN SUBSTR(AA.GROUP_NAME,6, LENGTH(AA.GROUP_NAME)) 
                         ELSE AA.GROUP_NAME END,
                    AA.NEXT_STATION=
                    CASE WHEN  TO_NUMBER(REGEXP_INSTR(AA.NEXT_STATION, '-HOLD')) > 0 THEN SUBSTR(AA.NEXT_STATION,1, LENGTH(AA.NEXT_STATION) -5) 
                         WHEN  TO_NUMBER(REGEXP_INSTR(AA.NEXT_STATION, 'HOLD-')) > 0 THEN SUBSTR(AA.NEXT_STATION,6, LENGTH(AA.NEXT_STATION)) 
                         ELSE AA.NEXT_STATION END
                        WHERE  SERIAL_NUMBER = '{0}'", serial_number);
            }
            else
            {
                ssql = "UPDATE SFISM4.R_WIP_TRACKING_T SET GROUP_NAME = DECODE (SUBSTR (GROUP_NAME, 1, 5),'HOLD-', SUBSTR(GROUP_NAME,6,LENGTH(GROUP_NAME)),GROUP_NAME ), " +
                        "NEXT_STATION = DECODE (SUBSTR (NEXT_STATION, 1,5),'HOLD-', SUBSTR(NEXT_STATION,6, LENGTH(NEXT_STATION)), NEXT_STATION), " +
                        "WIP_GROUP = DECODE (SUBSTR (WIP_GROUP, 1, 5),'HOLD-', SUBSTR(WIP_GROUP,6,LENGTH(WIP_GROUP)),WIP_GROUP) " +
                        "WHERE SERIAL_NUMBER = '" + serial_number + "'";
            }
            try
            {

                //dbsfis.ExecuteNonQuery(ssql);
                sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });
                #region sua cho Arlo B5
                ssql = "select * from sfis1.C_MODEL_DESC_T where MODEL_NAME = (select MODEL_NAME from sfism4.R107 where SERIAL_NUMBER='" + serial_number + "' and wip_group in ('RC','RC1','RC2','VI','VI2','STOCKIN','IP65')) and MODEL_SERIAL ='ARLO'";
                sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString(),
                    SfcCommandType = SfcCommandType.Text
                });
                dynamic ads = resultQuery.Data;
                string model_serial = ads[0]["model_serial"];
                if (model_serial.ToUpper().Equals("ARLO") && resultQueryCheckHOLD.Data.ToList().Count > 0)
                {
                    ssql = "select C.GROUP_NEXT from sfis1.C_ROUTE_CONTROL_T c,SFISM4.R107 D  where  C.ROUTE_CODE = D.SPECIAL_ROUTE AND D.SERIAL_NUMBER = '" + serial_number + @"' and C.STEP_SEQUENCE =(SELECT max(A.STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T A WHERE A.ROUTE_CODE=C.ROUTE_CODE  AND A.STATE_FLAG = 0 AND  A.GROUP_NAME NOT LIKE 'R\_%' ESCAPE '\')";
                    sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });
                    var lstgpdata = resultQuery.Data.ToList();
                    string lstgroup = "'"+lstgpdata[0]["group_next"].ToString() + "'";
                    ssql = "SELECT A.GROUP_NAME FROM SFIS1.C_ROUTE_CONTROL_T A, SFISM4.R107 D WHERE A.ROUTE_CODE = D.SPECIAL_ROUTE AND A.STATE_FLAG = 0 " +
                               "AND A.STEP_SEQUENCE IN(SELECT MAX(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T B WHERE A.ROUTE_CODE = B.ROUTE_CODE " +
                               @"AND A.STATE_FLAG = B.STATE_FLAG AND A.GROUP_NAME = B.GROUP_NAME ) AND A.GROUP_NAME NOT LIKE 'R\_%' ESCAPE '\' " +
                               "AND D.SERIAL_NUMBER = '" + serial_number + "' AND A.STEP_SEQUENCE >=(SELECT MAX(C.STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T C " +
                               "WHERE A.ROUTE_CODE = C.ROUTE_CODE AND A.STATE_FLAG = C.STATE_FLAG AND C.GROUP_NAME = 'NMRP' ) ";
                    sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });
                     lstgpdata = resultQuery.Data.ToList();
                    for (int i = 0; i < lstgpdata.Count; i++)
                    {
                        lstgroup += ",'" + lstgpdata[i]["group_name"].ToString() + "'";
                    }
                    ssql = "Select * from sfism4.r107 where SERIAL_NUMBER ='" + serial_number + "'";
                    sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    var resultQueryCheckSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString(),
                        SfcCommandType = SfcCommandType.Text
                    });
                    string wip = resultQueryCheckSN.Data["wip_group"].ToString();
                    string SSN = resultQueryCheckSN.Data["shipping_sn"].ToString();
                    string ModelC = resultQueryCheckSN.Data["model_name"].ToString();
                    if (wip.Contains("STOCKIN"))
                    {
                        ssql = "select * from sfism4.R108 where key_part_sn ='" + SSN + "'";
                        sbUpdate = new StringBuilder();
                        sbUpdate.Append(ssql);
                        resultQueryCheckSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbUpdate.ToString(),
                            SfcCommandType = SfcCommandType.Text
                        });
                        string SerialNumber = resultQueryCheckSN.Data["serial_number"].ToString();
                        ssql = "select * from sfism4.R107 where serial_number ='" + SerialNumber + "'";
                        sbUpdate = new StringBuilder();
                        sbUpdate.Append(ssql);
                        resultQueryCheckSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbUpdate.ToString(),
                            SfcCommandType = SfcCommandType.Text
                        });

                        if (!resultQueryCheckSN.Data["wip_group"].ToString().Equals("FG") && !string.IsNullOrEmpty(resultQueryCheckSN.Data["wip_group"].ToString()))
                        {
                            string ModelName = resultQueryCheckSN.Data["model_name"].ToString();
                            ssql = "Select * from sfis1.C_BOM_KEYPART_T where Bom_no ='" + ModelName + "' and KEY_PART_NO ='" + ModelC + "' and group_name not in('ASSY','VI')";
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            resultQueryCheckSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString(),
                                SfcCommandType = SfcCommandType.Text
                            });
                            string group = resultQueryCheckSN.Data["group_name"].ToString();
                            ssql = "SELECT A.GROUP_NAME FROM SFIS1.C_ROUTE_CONTROL_T A, SFISM4.R107 D WHERE A.ROUTE_CODE = D.SPECIAL_ROUTE AND A.STATE_FLAG = 0 " +
                                   "AND A.STEP_SEQUENCE IN(SELECT MAX(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T B WHERE A.ROUTE_CODE = B.ROUTE_CODE " +
                                   @"AND A.STATE_FLAG = B.STATE_FLAG AND A.GROUP_NAME = B.GROUP_NAME ) AND A.GROUP_NAME NOT LIKE 'R\_%' ESCAPE '\' " +
                                   "AND D.SERIAL_NUMBER = '" + SerialNumber + "' AND A.STEP_SEQUENCE >=(SELECT MAX(C.STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T C " +
                                   "WHERE A.ROUTE_CODE = C.ROUTE_CODE AND A.STATE_FLAG = C.STATE_FLAG AND C.GROUP_NAME = '" + group + "' ) ";
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                            ads = resultQuery.Data;
                            for (int i = 0; i < ads.Count; i++)
                            {
                                lstgroup += ",'" + ads[i]["group_name"].ToString() + "',";
                            }
                            ssql = "Insert into sfism4.P_WIP_KEYPARTS_T select * from sfism4.R108 where (group_name in (" + lstgroup + ",'KEYPART') or group_name is null) and serial_number ='" + SerialNumber + "'";
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                             resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                            ssql = "delete from sfism4.R108 where (group_name in (" + group + ",'KEYPART') or group_name is null) and serial_number ='" + SerialNumber + "'";
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                            ssql = "update sfism4.R107 set   next_station ='" + lstgroup + "',FINISH_FLAG=0,wip_group ='" + group + "',EMP_NO ='PROGRAM-HOLD' where serial_number='" + SerialNumber + "'";
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            #region update cho cam keypart
                            for (int i = 0; i < lstgpdata.Count; i++)
                            {
                                lstgroup += ",'" + lstgpdata[i]["group_name"].ToString() + "',";
                            }
                            resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                            ssql = "Insert into sfism4.P_WIP_KEYPARTS_T select * from sfism4.R108 where (group_name in (" + lstgroup + ",'KEYPART') or group_name is null) and serial_number ='" + serial_number + "'";
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                            ssql = "delete from sfism4.R108 where (group_name in (" + lstgroup + ",'KEYPART') or group_name is null) and serial_number ='" + serial_number + "'";
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                            if (ModelC.Contains("VSC"))
                            {
                                ssql = "SELECT A.GROUP_NAME FROM SFIS1.C_ROUTE_CONTROL_T A, SFISM4.R107 D WHERE A.ROUTE_CODE = D.SPECIAL_ROUTE AND A.STATE_FLAG = 0 "+
                                  " AND A.STEP_SEQUENCE IN(SELECT MAX(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T B WHERE A.ROUTE_CODE = B.ROUTE_CODE "+
                                  @" AND A.STATE_FLAG = B.STATE_FLAG AND A.GROUP_NAME = B.GROUP_NAME ) AND A.GROUP_NAME NOT LIKE 'R\_%' ESCAPE '\'"+
                                   " AND D.SERIAL_NUMBER = '"+serial_number+"' AND A.GROUP_NAME = 'RC0'";
                                sbUpdate = new StringBuilder();
                                sbUpdate.Append(ssql);
                                resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = sbUpdate.ToString()
                                });
                                if(resultQuery.Data.Count()>0)
                                {
                                    ssql = "update sfism4.R107 set   next_station ='RC0',wip_group ='RC0',EMP_NO ='PROGRAM-HOLD' where serial_number='" + serial_number + "'";
                                }
                                else
                                {
                                    return true;
                                }
                                
                            }
                           else if (ModelC.Contains("VMB"))
                            {
                                ssql = "SELECT A.GROUP_NAME FROM SFIS1.C_ROUTE_CONTROL_T A, SFISM4.R107 D WHERE A.ROUTE_CODE = D.SPECIAL_ROUTE AND A.STATE_FLAG = 0 " +
                                 " AND A.STEP_SEQUENCE IN(SELECT MAX(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T B WHERE A.ROUTE_CODE = B.ROUTE_CODE " +
                                 @" AND A.STATE_FLAG = B.STATE_FLAG AND A.GROUP_NAME = B.GROUP_NAME ) AND A.GROUP_NAME NOT LIKE 'R\_%' ESCAPE '\'" +
                                  " AND D.SERIAL_NUMBER = '" + serial_number + "' AND A.GROUP_NAME = 'RC'";
                                sbUpdate = new StringBuilder();
                                sbUpdate.Append(ssql);
                                resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = sbUpdate.ToString()
                                });
                                if (resultQuery.Data.Count() > 0)
                                {
                                    ssql = "update sfism4.R107 set   next_station ='RC',wip_group ='RC',EMP_NO ='PROGRAM-HOLD' where serial_number='" + serial_number + "'";
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                ssql = "update sfism4.R107 set   next_station ='NMRP',wip_group ='NMRP',EMP_NO ='PROGRAM-HOLD' where serial_number='" + serial_number + "'";
                            }
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                            #endregion

                        }
                    }
                    else 
                    {
                       
                        if (lstgroup.Contains(wip))
                        {
                            
                            ssql = "Insert into sfism4.P_WIP_KEYPARTS_T select * from sfism4.R108 where (group_name in (" + lstgroup + ",'KEYPART') or group_name is null) and serial_number ='" + serial_number + "'";
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                            ssql = "delete from sfism4.R108 where (group_name in (" + lstgroup + ",'KEYPART') or group_name is null) and serial_number ='" + serial_number + "'";
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                            if(ModelC.Contains("VSC"))
                            {
                                ssql = "SELECT A.GROUP_NAME FROM SFIS1.C_ROUTE_CONTROL_T A, SFISM4.R107 D WHERE A.ROUTE_CODE = D.SPECIAL_ROUTE AND A.STATE_FLAG = 0 " +
                                  " AND A.STEP_SEQUENCE IN(SELECT MAX(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T B WHERE A.ROUTE_CODE = B.ROUTE_CODE " +
                                  @" AND A.STATE_FLAG = B.STATE_FLAG AND A.GROUP_NAME = B.GROUP_NAME ) AND A.GROUP_NAME NOT LIKE 'R\_%' ESCAPE '\'" +
                                   " AND D.SERIAL_NUMBER = '" + serial_number + "' AND A.GROUP_NAME = 'RC0'";
                                sbUpdate = new StringBuilder();
                                sbUpdate.Append(ssql);
                                resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = sbUpdate.ToString()
                                });
                                if (resultQuery.Data.Count() > 0)
                                {
                                    ssql = "update sfism4.R107 set   next_station ='RC0',wip_group ='RC0',EMP_NO ='PROGRAM-HOLD' where serial_number='" + serial_number + "'";
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            else if (ModelC.Contains("VMB"))
                            {
                                ssql = "SELECT A.GROUP_NAME FROM SFIS1.C_ROUTE_CONTROL_T A, SFISM4.R107 D WHERE A.ROUTE_CODE = D.SPECIAL_ROUTE AND A.STATE_FLAG = 0 " +
                                 " AND A.STEP_SEQUENCE IN(SELECT MAX(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T B WHERE A.ROUTE_CODE = B.ROUTE_CODE " +
                                 @" AND A.STATE_FLAG = B.STATE_FLAG AND A.GROUP_NAME = B.GROUP_NAME ) AND A.GROUP_NAME NOT LIKE 'R\_%' ESCAPE '\'" +
                                  " AND D.SERIAL_NUMBER = '" + serial_number + "' AND A.GROUP_NAME = 'RC'";
                                sbUpdate = new StringBuilder();
                                sbUpdate.Append(ssql);
                                resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = sbUpdate.ToString()
                                });
                                if (resultQuery.Data.Count() > 0)
                                {
                                    ssql = "update sfism4.R107 set   next_station ='RC',wip_group ='RC',EMP_NO ='PROGRAM-HOLD' where serial_number='" + serial_number + "'";
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                ssql = "update sfism4.R107 set   next_station ='NMRP',wip_group ='NMRP',EMP_NO ='PROGRAM-HOLD' where serial_number='" + serial_number + "'";
                            }
                           
                            sbUpdate = new StringBuilder();
                            sbUpdate.Append(ssql);
                            resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sbUpdate.ToString()
                            });
                        }
                        else
                        {
                            return true;
                        }

                    }
                    //else
                    //{
                    //    ssql = "SELECT A.GROUP_NAME FROM SFIS1.C_ROUTE_CONTROL_T A, SFISM4.R107 D WHERE A.ROUTE_CODE = D.SPECIAL_ROUTE AND A.STATE_FLAG = 0 " +
                    //            "AND A.STEP_SEQUENCE IN(SELECT MAX(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T B WHERE A.ROUTE_CODE = B.ROUTE_CODE " +
                    //            @"AND A.STATE_FLAG = B.STATE_FLAG AND A.GROUP_NAME = B.GROUP_NAME ) AND A.GROUP_NAME NOT LIKE 'R\_%' ESCAPE '\' " +
                    //            "AND D.SERIAL_NUMBER = '" + serial_number + "' AND A.STEP_SEQUENCE >=(SELECT MAX(C.STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T C " +
                    //            "WHERE A.ROUTE_CODE = C.ROUTE_CODE AND A.STATE_FLAG = C.STATE_FLAG AND C.GROUP_NAME = 'NMRP' ) ";
                    //    sbUpdate = new StringBuilder();
                    //    sbUpdate.Append(ssql);
                    //    resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    //    {
                    //        CommandText = sbUpdate.ToString()
                    //    });
                    //    string lstgroup = "";
                    //    ads = resultQuery.Data;
                    //    for (int i = 0; i < ads.Count; i++)
                    //    {
                    //        lstgroup += "'" + ads[i]["group_name"].ToString() + "',";
                    //    }
                    //    ssql = "Insert into sfism4.P_WIP_KEYPARTS_T select * from sfism4.R108 where (group_name in (" + lstgroup + "'KEYPART') or group_name is null) and serial_number ='" + serial_number + "'";
                    //    sbUpdate = new StringBuilder();
                    //    sbUpdate.Append(ssql);
                    //    var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    //    {
                    //        CommandText = sbUpdate.ToString()
                    //    });
                    //    ssql = "delete from sfism4.R108 where (group_name in ('" + lstgroup + "','KEYPART') or group_name is null)and serial_number ='" + serial_number + "'";
                    //    sbUpdate = new StringBuilder();
                    //    sbUpdate.Append(ssql);
                    //    resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    //    {
                    //        CommandText = sbUpdate.ToString()
                    //    });
                    //    ssql = "update sfism4.R107 set   next_station ='NMRP',FINISH_FLAG=0,wip_group ='NMRP',EMP_NO ='PROGRAM-HOLD' where serial_number='" + serial_number + "'";
                    //    sbUpdate = new StringBuilder();
                    //    sbUpdate.Append(ssql);
                    //    resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    //    {
                    //        CommandText = sbUpdate.ToString()
                    //    });
                    //}
                   
                }
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> unholdbycamserial(string serial_number)
        {
            string ssql = "";
            try
            {
                await InsertLogHold(serial_number, "", empNo, txtReason.Text, "HOLD-PROGRAM", "1");
                //LogUnHold(serial_number, empNo, txtReason.Text, "HOLD-PROGRAM", "1");
                ssql = "UPDATE SFISM4.R_CAMERA_T SET " +
                    " SERIAL_NUMBER = DECODE(SUBSTR (SERIAL_NUMBER, 1, 5),'HOLD-', SUBSTR(SERIAL_NUMBER,6,LENGTH(SERIAL_NUMBER)),SERIAL_NUMBER ), " +
                    " MACID =  DECODE (SUBSTR(MACID, 1,5),'HOLD-', SUBSTR(MACID,6,LENGTH(MACID)),MACID) " +
                    " WHERE REPLACE(SERIAL_NUMBER,'HOLD-','') IN ('" + serial_number + "')";
                //dbsfis.ExecuteNonQuery(ssql);
                var sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> unholdroute(string mo_number, string route_code)
        {
            try
            {
                string ssql = "", tmproute_name = "", tmproutecode= "";
                ssql = "SELECT ROUTE_NAME FROM SFIS1.C_ROUTE_NAME_T WHERE ROUTE_CODE='" + route_code + "'";
                //DataTable dt = dbsfis.DoSelectQuery(ssql);
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (result.Data != null)
                {
                    var ROUTE = result.Data.ToObject<LogInformation>();
                    //tmproute_name = dt.Rows[0]["ROUTE_NAME"].ToString();
                    tmproute_name = ROUTE.ROUTE_NAME;
                }
                else
                {
                    showMessage("Route code:" + route_code + "  not found !!");
                    //MessageBox.Show("Route code:" + route_code + "  not found !!", errorstring);
                    //return false;
                }
                if (tmproute_name.IndexOf("HOLD") > 0)
                {
                    ssql = "UPDATE SFIS1.C_ROUTE_CONTROL_T SET GROUP_NEXT = DECODE (SUBSTR (GROUP_NEXT, 1, 5), 'HOLD-', SUBSTR (GROUP_NEXT, 6, LENGTH (GROUP_NEXT)), GROUP_NEXT)" +
                        " WHERE ROUTE_CODE = '" + route_code + "' AND GROUP_NEXT='" + cbbStation.Text + "'";
                    //dbsfis.ExecuteNonQuery(ssql);
                    var sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });
                    tmproute_name = tmproute_name.Substring(0, tmproute_name.IndexOf("HOLD"));
                }

                ssql = "SELECT * FROM SFIS1.C_ROUTE_CONTROL_T" +
                        " WHERE ROUTE_CODE='" + route_code + "' AND GROUP_NEXT LIKE 'HOLD%'";
                //dt = dbsfis.DoSelectQuery(ssql);
                var resultRoute = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (resultRoute.Data == null)
                {
                    //bool x = await findroutename(tmproute_name);
                    if (await findroutename(tmproute_name) == false)
                    //if(x == false)
                    {
                        ssql = "SELECT * FROM SFIS1.C_ROUTE_NAME_T WHERE ROUTE_NAME='" + tmproute_name + "'";
                        //dt = dbsfis.DoSelectQuery(ssql);
                        var resultQuery = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = ssql,
                            SfcCommandType = SfcCommandType.Text,
                        });
                        if (resultQuery.Data != null)
                        {
                            var ROUTE = result.Data.ToObject<LogInformation>();
                            //tmproutecode = dt.Rows[0]["ROUTE_CODE"].ToString();
                            tmproutecode = ROUTE.ROUTE_CODE;
                        }
                        if (await updateRouteCode(mo_number, tmproutecode))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        ssql = "UPDATE SFIS1.C_ROUTE_CONTROL_T SET GROUP_NEXT = DECODE(SUBSTR(GROUP_NEXT, 1, 5),'HOLD-', SUBSTR(GROUP_NEXT, 6, LENGTH (GROUP_NEXT)), GROUP_NEXT)" +
                            " WHERE ROUTE_CODE = '" + route_code + "'";
                        //dbsfis.ExecuteNonQuery(ssql);
                        var sbUpdate = new StringBuilder();
                        sbUpdate.Append(ssql);
                        var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbUpdate.ToString()
                        });
                        ssql = "UPDATE SFIS1.C_ROUTE_NAME_T SET ROUTE_NAME='" + tmproute_name + "'" +
                        " WHERE ROUTE_CODE = '" + route_code + "'";
                        //dbsfis.ExecuteNonQuery(ssql);
                        sbUpdate = new StringBuilder();
                        sbUpdate.Append(ssql);
                        resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbUpdate.ToString()
                        });
                    }
                }
                return true;
            }
            catch(Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<string> getroutecode(string mo_number)
        {
            string ssql = "",route_code = "";
            ssql = "SELECT * FROM SFISM4.R105 WHERE MO_NUMBER='" +mo_number+ "'";

            //DataTable dt = dbsfis.DoSelectQuery(ssql);

            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                var ROUTE = result.Data.ToObject<LogInformation>();
                //route_code = dt.Rows[0]["ROUTE_CODE"].ToString();
                route_code = ROUTE.ROUTE_CODE;
                return route_code;
            }
            else
            {
                return "";
            }
        }

        private async Task<bool> SENDMAIL(string mo_number, string model_name, string count)
        {
            string ssql = "";
            string MAIL_ID = "", MAIL_TO = "", MAIL_SUBJECT = "", MAIL_CONTENT = "", MAILSQL = "";
            try
            {
                if (txtCondition.Text.Contains("LIM"))
                {
                    ssql = "SELECT EMAIL_LIST FROM SFIS1.C_CUSTOMER_FTP_ACCOUNT_T WHERE CUSTOMER_CODE='CONFIG_LIMIT'";
                    //DataTable dt = dbsfis.DoSelectQuery(ssql);
                    var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    var MAILLIST = result.Data.ToObject<LogInformation>();
                    //MAIL_TO = dt.Rows[0]["EMAIL_LIST"].ToString();
                    MAIL_TO = MAILLIST.EMAIL_LIST;
                    if (MAIL_TO == "")
                    {
                        return false;
                    }
                    MAIL_SUBJECT = txtCondition.Text + " " + model_name + " " + mo_number + " " + count + "PCS 此批電性過期產品已解HOLD﹐請PQE確認是否有誤?";
                    MAIL_CONTENT = txtCondition.Text + " " + model_name + " " + mo_number + " " + count + "PCS 此批電性過期產品已解HOLD﹐請PQE確認是否有誤?";
                }
                else
                {
                    ssql = "SELECT * FROM SFIS1.C_CUSTOMER_FTP_ACCOUNT_T WHERE CUSTOMER_CODE='CONFIG_HOLD'";
                    //DataTable dt = dbsfis.DoSelectQuery(ssql);
                    var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = ssql,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    var MAILLIST = result.Data.ToObject<LogInformation>();
                    MAIL_TO = MAILLIST.EMAIL_LIST;
                    //MAIL_TO = dt.Rows[0]["EMAIL_LIST"].ToString();
                    if (MAIL_TO == "")
                    {
                        return false;
                    }
                    MAIL_SUBJECT = txtCondition.Text + " " + mo_number + " " + count + " CQA已解除HOLD,請PQE/PMC確認出貨數量及地點是否正確?";
                    MAIL_CONTENT = txtCondition.Text + " " + mo_number + " " + count + " CQA已解除HOLD,請PQE/PMC確認出貨數量及地點是否正確?";
                }
                MAIL_ID = DateTime.Now.ToString("yyyyMMddHHmmsszzzz");
                MAILSQL = "INSERT INTO SFIS1.C_MAIL_T(MAIL_ID, MAIL_TO, MAIL_FROM, MAIL_SUBJECT, MAIL_SEQUENCE, MAIL_CONTENT, MAIL_FLAG, MAIL_PROGRAM)" +
                        " VALUES('"+MAIL_ID+"','"+MAIL_TO+ "', 'HAND-HOLD', '"+MAIL_SUBJECT+"', '0', '"+MAIL_CONTENT+ "', '0', 'HAND-HOLD')";

                //dbsfis.ExecuteNonQuery(MAILSQL);
                var sbInsert = new StringBuilder();
                sbInsert.Append(MAILSQL);
                var resultInsert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbInsert.ToString()
                });
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> UPDATER105(string mo_number)
        {
            string ssql = "";
            try
            {
                ssql = "UPDATE SFISM4.R_MO_BASE_T SET UPC_CO='N'  WHERE MO_NUMBER='" + mo_number + "'";
                //dbsfis.ExecuteNonQuery(ssql);
                var sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> UPDATEHOLDZ107(string hold_no)
        {
            string ssql = "";
            try
            {
                ssql = "SELECT * FROM SFISM4.Z107 WHERE PMCC='" + txtCondition.Text + "'";
                var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });                
                if (result.Data != null)
                {
                    var a = result.Data.ToListObject<infcbxFG>();
                    List<infcbxFG> listSerial = a.Cast<infcbxFG>().ToList();
                    for (int i = 0; i < listSerial.Count; i++)
                    {
                        await InsertLogHold(listSerial[i].SERIAL_NUMBER.ToString(), "", empNo, txtReason.Text, "HOLD-PROGRAM", "1");
                    }                    
                }

                ssql = "UPDATE SFISM4.Z_WIP_TRACKING_T SET GROUP_NAME='STOCKIN',NEXT_STATION='N/A'," +
                "EMP_NO='" + empNo + "',PMCC='',WIP_GROUP='FG' " +
                "WHERE LENGTH(SHIP_NO)<4 AND PMCC='" + hold_no + "' ";

                var sbUpdate = new StringBuilder();
                sbUpdate.Append(ssql);
                var resultExe = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString()
                });
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private async Task<bool> UnHoldUnLineMo(string mo_number)
        {
            string ssql = "";
            try
            {
                //DataTable dt = new DataTable();
                ssql = "SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER='" + mo_number + "'";
                //dt = dbsfis.DoSelectQuery(ssql);
                var resultMoOnSAP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = ssql,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (resultMoOnSAP.Data != null)
                {
                    ssql = " UPDATE SFISM4.R_BPCS_MOPLAN_T SET LOC='' WHERE MO_NUMBER='" + mo_number + "' ";
                    var sbUpdate = new StringBuilder();
                    sbUpdate.Append(ssql);
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sbUpdate.ToString()
                    });
                    //dbsfis.ExecuteNonQuery(ssql);
                    return true;
                }
                else
                {
                    showMessage("Mo_number:" + mo_number + " not download from sap");
                    //MessageBox.Show("Mo_number:" + mo_number + " not download from sap", errorstring);
                    return false;
                }
            }
            catch (Exception e)
            {
                showMessage(e.Message);
                //MessageBox.Show(e.Message, errorstring);
                return false;
            }
        }

        private async Task<bool> CheckFlagHold()
        {
            string ssql = "";
            ssql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='HOLD' AND VR_ITEM='B01-HOLD'";
            //DataTable dt = dbsfis.DoSelectQuery(ssql);

            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text,
            });
            if (result.Data != null)
            {
                return true;
            }
            else
                return false;
        }

        private void cbxFG_CheckedChanged(object sender, EventArgs e)
        {
            cbxAteStation.Checked = false;
            cbxUnLineMo.Checked = false;
            cbxTestTime.Checked = false;
            cbxHoldStockin.Checked = false;
            cbxHoldByStation.Checked = false;
            if (cbxFG.Checked) 
            { 
                tab107 = "sfism4.z107";
                notWip = " AND WIP_GROUP not in('SHIPPING','CHECK_OUT','KITTING') ";
            }
            else
            {
                tab107 = "sfism4.r107";
                notWip = "AND WIP_GROUP <> 'FG' AND SUBSTR(WIP_GROUP,1,2) <> 'SC' ";
            }
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();
            zFlag = false;
        }

        private void cbxHoldStockin_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxHoldStockin.Checked)
            {
                cbbCondition.Text = "MO_NUMBER";
                cbxAteStation.Checked = false;
                cbxFG.Checked = false;
                cbxUnLineMo.Checked = false;
                cbxTestTime.Checked = false;
                cbxGroup.Checked = false;
                cbxHoldByStation.Checked = false;
            }
        }

        private void cbxUnLineMo_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxUnLineMo.Checked)
            {
                cbbCondition.Text = "MO_NUMBER";
                cbxAteStation.Checked = false;
                cbxFG.Checked = false;
                cbxTestTime.Checked = false;
                cbxGroup.Checked = false;
                cbxHoldStockin.Checked = false;
                cbxHoldByStation.Checked = false;
            }
        }

        private void cbxHoldByStation_CheckedChanged(object sender, EventArgs e)
        {
            cbxUnLineMo.Checked = false;
            cbxHoldStockin.Checked = false;
            cbxFG.Checked = false;
        }

        private void cbxHoldMaterial_CheckedChanged(object sender, EventArgs e)
        {
            cbxHoldStockin.Checked = false;
            cbxUnLineMo.Checked = false;
            cbxFG.Checked = false;
            cbxUnHoldMaterial.Checked = false;
            cbxGroup.Checked = false;
            cbxAteStation.Checked = false;
            cbxHoldByStation.Checked = false;
        }

        private void cbxUnHoldMaterial_CheckedChanged(object sender, EventArgs e)
        {
            cbxHoldStockin.Checked = false;
            cbxUnLineMo.Checked = false;
            cbxFG.Checked = false;
            cbxHoldMaterial.Checked = false;
            cbxGroup.Checked = false;
            cbxAteStation.Checked = false;
            cbxHoldByStation.Checked = false;
        }

        private void BtnExcel_Click(object sender, EventArgs e)
        {
            ExportFileExcel(dataGridView1);
        }
        private void ExportFileExcel(DataGridView dataGridView1)
        {
            DateTime dtNow = DateTime.Now;
            DateTime excelDate = dtNow.AddYears(-1);
            int intExcelID = 0;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = dataGridView1.Rows.Count;
            saveFileDialog1.InitialDirectory = "C:";
            saveFileDialog1.Title = "Save as Excel File";
            saveFileDialog1.FileName = "";
            saveFileDialog1.Filter = "Excel Files (2003)|*.xls|Excel Files (2007)|*.xlsx";
            Microsoft.Office.Interop.Excel.Application Excelapp = new Microsoft.Office.Interop.Excel.Application();
            foreach (System.Diagnostics.Process excelProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
            {
                if (excelProc.StartTime > excelDate)
                {
                    excelDate = excelProc.StartTime;
                    intExcelID = excelProc.Id;
                }
            }
            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                try
                {
                    Excelapp.Application.Workbooks.Add(Type.Missing);
                    for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
                    {
                        Excelapp.Cells[1, i] = dataGridView1.Columns[i - 1].HeaderText;
                    }

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            string temp = "";
                            if (dataGridView1.Rows[i].Cells[j].Value == null)
                            {
                                temp = "";
                            }
                            else
                            {
                                temp = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            }
                            Excelapp.Cells[i + 2, j + 1] = temp;
                        }
                        toolStripProgressBar1.Value++;
                    }
                    Excelapp.ActiveWorkbook.SaveCopyAs(saveFileDialog1.FileName.ToString());
                    Excelapp.ActiveWorkbook.Saved = true;
                    Excelapp.Quit();

                    MessageBox.Show("Export to Excel successful! Path: " + saveFileDialog1.FileName, "Message");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ExportToExcel: Excel file could not be saved! Check filepath.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                foreach (System.Diagnostics.Process excelProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                {
                    if (excelProc.Id == intExcelID)
                    {
                        excelProc.Kill();
                    }
                }
            }
        }

        private void chkTest_CheckedChanged(object sender, EventArgs e)
        {
            if(chkTest.Checked) 
            {
                cbbCondition.SelectedIndex = 7;//IMEI
                label3.Visible = false;
                lblTotalQty.Visible = false;
                label8.Visible = false;
                lblSuccessQty.Visible = false;
                label7.Visible = false;
                lblFailQty.Visible = false;
            }
            else
            {
                label3.Visible = true;
                lblTotalQty.Visible = true;
                label8.Visible = true;
                lblSuccessQty.Visible = true;
                label7.Visible = true;
                lblFailQty.Visible = true;
            }
            dataGridView1.DataSource = null;
            dataGridView1.Refresh();
            zFlag = false;
        }

        private void cbbCondition_SelectedValueChanged(object sender, EventArgs e)
        {
            //string temp = cbbCondition.SelectedValue.ToString();
        }

        private void cbbCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbbCondition.SelectedItem.ToString()!="IMEI")
            {
                chkTest.Checked = false;
            }
        }
        private void showMessage(string Message)
        {
            if (Message.IndexOf("|") != -1)
            {
                if (FormMain.lang == "EN")
                    Message=Message.Split('|')[0].ToString();
                else Message = Message.Split('|')[1].ToString();
            }
            StackFrame CallStack = new StackFrame(1, true);
            if (!chkShowStack.Checked)
                MessageBox.Show(Message);
            else
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }
    }
}
