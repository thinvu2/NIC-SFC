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
using CQC.ViewModel;
using Sfc.Library.HttpClient.Helpers;
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using Microsoft.Win32;

namespace CQC
{
    /// <summary>
    /// Interaction logic for qryoobaWindow.xaml
    /// </summary>
    public partial class qryoobaWindow : Window
    {
        private SfcHttpClient sfcClient;

        public qryoobaWindow(SfcHttpClient _sfcClient)
        {
            sfcClient = _sfcClient;
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            GETMOLIST();
        }

        async void GETMOLIST()
        {
            string MYSQL = "SELECT DISTINCT MO_NUMBER FROM SFISM4.R102 A WHERE WORK_DATE>TO_CHAR(SYSDATE-3,'YYYYMMDD') " +
            " AND  EXISTS  " +
            " (SELECT 1 FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME=A.MODEL_NAME AND MODEL_TYPE LIKE '%201%') " +
            " AND EXISTS  " +
            " (SELECT 1 FROM SFISM4.R102 WHERE MODEL_NAME=A.MODEL_NAME  AND GROUP_NAME LIKE 'OOBA%') ORDER BY MO_NUMBER";

            ComboBox1.Items.Clear();
            var Query1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = MYSQL,
                SfcCommandType = SfcCommandType.Text
            });
            List<R107> molist = new List<R107>();
            molist = Query1.Data.ToListObject<R107>().ToList();
            ComboBox1.ItemsSource = molist;
            ComboBox1.DisplayMemberPath = "MO_NUMBER";
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox1.SelectedIndex < 0)
                return;
            R107 tmp = ComboBox1.Items[ComboBox1.SelectedIndex] as R107;
            Edit1.Text = tmp.MO_NUMBER;
            Edit1_KeyDown(new object(), new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return));
        }

        int getOOBASampleSize(int qty, int mo_needQty1)
        {
            if (1 <= qty && qty <= 15)
                mo_needQty1 = qty;
            else if (16 <= qty && qty <= 280)
                mo_needQty1 = 20;
            else if (281 <= qty && qty <= 1200)
                mo_needQty1 = 47;
            else if (1201 <= qty && qty <= 3200)
                mo_needQty1 = 53;
            else if (qty % 3200 == 0)
            {
                mo_needQty1 = (qty / 3000) * 53;
            }
            else
            {
                int result1 = qty - ((qty / 3200) * 3200);
                if (1 <= result1 && result1 <= 15)
                    mo_needQty1 = result1 + ((qty / 3200) * 53);
                else if (16 <= result1 && result1 <= 280)
                    mo_needQty1 = 20 + ((qty / 3200) * 53);
                else if (281 <= result1 && result1 <= 1200)
                    mo_needQty1 = 47 + ((qty / 3200) * 53);
                else if (1201 <= result1 && result1 <= 3200)
                    mo_needQty1 = 53 + ((qty / 3200) * 53);
            }
            return mo_needQty1;
        }

        int getOOBASampleSize_0_25(int qty, int mo_needQty1)
        {
            if (1 <= qty && qty <= 51)
                mo_needQty1 = qty;
            else if (51 <= qty && qty <= 500)
                mo_needQty1 = 60;
            else if (501 <= qty && qty <= 1200)
                mo_needQty1 = 75;
            else if (1201 <= qty && qty <= 3200)
                mo_needQty1 = 116;
            else if (qty % 3200 == 0)
            {
                mo_needQty1 = (qty / 3200) * 116;
            }
            else
            {
                int result1 = qty - ((qty / 3200) * 3200);
                if (1 <= result1 && result1 <= 51)
                    mo_needQty1 = result1 + ((qty / 3200) * 116);
                else if (51 <= result1 && result1 <= 500)
                    mo_needQty1 = 60 + ((qty / 3200) * 116);
                else if (501 <= result1 && result1 <= 1200)
                    mo_needQty1 = 75 + ((qty / 3200) * 116);
                else if (1201 <= result1 && result1 <= 3200)
                    mo_needQty1 = 116 + ((qty / 3200) * 116);
            }
            return mo_needQty1;
        }

        private async void Edit1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                try
                {
                    col1.Header = "MO__Number";
                    col2.Header = "Model__name";
                    col3.Header = "Version";
                    col4.Header = "M/O-QTY";
                    List<OOBA> datalist = new List<OOBA>();
                    bool CHECK_AQL;
                    string TXT, MYSQL, CHECK_value = "";
                    double mo_needqty, pack_needqty, packparam, packparamair, packparamairau, pack_needqtyair, pack_needqtyairau, packparamairhk, pack_needqtyairhk;
                    double dd, qtypallet, qtypallet1, ddair, qtypalletair, qtypallet1air, ddairhk, qtypalletairhk, qtypallet1airhk, ddairau, qtypalletairau, qtypallet1airau;
                    int mo_qty;
                    CHECK_AQL = false;
                    TXT = Edit1.Text;
                    string c_mo, c_model, c_version;
                    var query1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SELECT * FROM SFISM4.R105 WHERE MO_NUMBER='" + TXT + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query1.Data == null)
                        return;
                    c_model = "Model: " + query1.Data["model_name"].ToString();
                    c_version = "Version: " + query1.Data["version_code"].ToString();
                    datalist.Add(new OOBA()//dong 1
                    {
                        HEAD = "WO:",
                        MO_NUMBER = TXT,
                        MODEL_NAME = c_model,
                        VERSION_CODE = c_version
                    });
                    MYSQL = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE VR_ITEM = SUBSTR ('" + query1.Data["model_name"].ToString() + "' , VR_NAME ,VR_VALUE  )   AND PRG_NAME  ='CQC'  AND VR_CLASS LIKE 'USE 0.45'";

                    var Query3 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = MYSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (Query3.Data != null)
                    {
                        CHECK_value = Query3.Data["vr_desc"].ToString();
                        CHECK_AQL = true;
                    }

                    MYSQL = "SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER='" + TXT + "' AND MODEL_NAME IN(SELECT BOM_NO FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO=KEY_PART_NO)";
                    Query3 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = MYSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (Query3.Data != null)
                        mo_qty = int.Parse(Query3.Data["target_qty"].ToString());
                    else
                        mo_qty = int.Parse(query1.Data["target_qty"].ToString());

                    //dong 2
                    datalist.Add(new OOBA()//dong 1
                    {
                        STT = "2.",
                        HEAD = "MO_Qty:",
                        MO_NUMBER = mo_qty.ToString()
                    });

                    if (CHECK_AQL)
                        c_mo = CHECK_value;
                    else
                        c_mo = "0.65";

                    //dong 2
                    datalist.Add(new OOBA()
                    {
                        STT = "3.",
                        HEAD = "AQL:",
                        MO_NUMBER = c_mo
                    });


                    //dong 3
                    if (c_mo == "0.65")
                    {
                        mo_needqty = getOOBASampleSize(mo_qty, 0);
                    }
                    else
                    {
                        mo_needqty = getOOBASampleSize_0_25(mo_qty, 0);
                    }

                    datalist.Add(new OOBA()
                    {
                        STT = "4.",
                        HEAD = "MO_Need:",
                        MO_NUMBER=mo_needqty.ToString()
                    });

                    packparam = await getPackQTY(query1.Data["model_name"].ToString(), query1.Data["version_code"].ToString(), -1);
                    //dong 4
                    datalist.Add(new OOBA()
                    {
                        STT = "1.",
                        HEAD = "Pack_Param by sea:",
                        MO_NUMBER = packparam.ToString()
                    });

                    if (packparam == 0)
                    {
                        qtypallet1 = 0;
                        c_mo = "0";
                    }
                    else
                    {
                        qtypallet = mo_qty / packparam;
                        qtypallet1 = Math.Ceiling(qtypallet);
                        dd = mo_needqty / qtypallet1;
                        pack_needqty = Math.Ceiling(dd);
                        c_mo = pack_needqty.ToString();
                    }
                    //dong 5
                    datalist.Add(new OOBA()
                    {
                        STT = "5.",
                        HEAD = "PA_Need by sea:",
                        MO_NUMBER = c_mo
                    });

                    packparamairau = await getPackQTYairau(query1.Data["model_name"].ToString(), 0);
                    //dong 6
                    datalist.Add(new OOBA()
                    {
                        STT = "6.",
                        HEAD = "Pack_Param by air Australia:",
                        MO_NUMBER = packparamairau.ToString()
                    });

                    if (packparamairau == 0)
                    {
                        qtypallet1airau = 0;
                        c_mo = "0";
                    }
                    else
                    {
                        qtypalletairau = mo_qty / packparamairau;
                        qtypallet1airau = Math.Ceiling(qtypalletairau);
                        ddairau = mo_needqty / qtypallet1airau;
                        pack_needqtyairau = Math.Ceiling(ddairau);
                        c_mo = pack_needqtyairau.ToString();
                    }
                    //dong 7
                    datalist.Add(new OOBA()
                    {
                        STT = "7.",
                        HEAD = "PA_Need by air Australia:",
                        MO_NUMBER = c_mo
                    });

                    packparamairhk = await getPackQTYairhk(query1.Data["model_name"].ToString(), 0);

                    //dong 8
                    datalist.Add(new OOBA()
                    {
                        STT = "8.",
                        HEAD = "Pack_Param by air Hong Kong:",
                        MO_NUMBER = packparamairhk.ToString()
                    });

                    if (packparamairhk == 0)
                    {
                        qtypallet1airhk = 0;
                        c_mo = "0";
                    }
                    else
                    {
                        qtypalletairhk = mo_qty / packparamairhk;
                        qtypallet1airhk = Math.Ceiling(qtypalletairhk);
                        ddairhk = mo_needqty / qtypallet1airhk;
                        pack_needqtyairhk = Math.Ceiling(ddairhk);
                        c_mo = pack_needqtyairhk.ToString();
                    }

                    //dong 9
                    datalist.Add(new OOBA()
                    {
                        STT = "9.",
                        HEAD = "PA_Need by air Hong Kong:",
                        MO_NUMBER = c_mo
                    });

                    packparamair = await getPackQTYair(query1.Data["model_name"].ToString(), 0);

                    //dong 10
                    datalist.Add(new OOBA()
                    {
                        STT = "10.",
                        HEAD = "Pack_Param by air:",
                        MO_NUMBER = packparamair.ToString()
                    });

                    if (packparamair == 0)
                    {
                        qtypallet1air = 0;
                        c_mo = "0";
                    }
                    else
                    {
                        qtypalletair = mo_qty / packparamair;
                        qtypallet1air = Math.Ceiling(qtypalletair);
                        ddair = mo_needqty / qtypallet1air;
                        pack_needqtyair = Math.Ceiling(ddair);
                        c_mo = pack_needqtyair.ToString();
                    }

                    //dong 11
                    datalist.Add(new OOBA()
                    {
                        STT = "11.",
                        HEAD = "PA_Need by air:",
                        MO_NUMBER = packparamair.ToString()
                    });

                    c_mo = (await getPass(TXT, 0)).ToString();
                    //dong 12
                    datalist.Add(new OOBA()
                    {
                        HEAD = "Qty production line:",
                        MO_NUMBER = c_mo
                    });

                    c_mo = (await getSampleQty(TXT, 0)).ToString();
                    //dong 13
                    datalist.Add(new OOBA()
                    {
                        HEAD = "Qty have pass FQA:",
                        MO_NUMBER = c_mo
                    });

                    c_mo = (await getpassOOBA1(TXT, 0)).ToString();

                    //dong 14
                    datalist.Add(new OOBA()
                    {
                        HEAD = "Qty test pass OOBA1:",
                        MO_NUMBER = c_mo
                    });

                    c_mo = (await getpassOOBA2(TXT, 0)).ToString();

                    //dong 15
                    datalist.Add(new OOBA()
                    {
                        HEAD = "Qty test pass OOBA2:",
                        MO_NUMBER = c_mo
                    });
                    ColorStringGrid1.ItemsSource = datalist;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }
            }
        }

        async Task<int> getpassOOBA2(string mo_number, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT COUNT(*) bb FROM SFISM4.R107 WHERE SHIPPING_SN IN (SELECT Y3INVN FROM SFISM4.R_EDI_AEDI03F  WHERE  GROUP_NAME='OOBA2') and mo_number='" + mo_number + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
                result = int.Parse(qury.Data["bb"].ToString());
            return result;
        }
        async Task<int> getpassOOBA1(string mo_number, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT COUNT(*) bb FROM SFISM4.R107 WHERE SHIPPING_SN IN (SELECT Y3INVN FROM SFISM4.R_EDI_AEDI03F  WHERE  GROUP_NAME='OOBA1') and mo_number='" + mo_number + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data.Count > 0 && qury.Data != null)
                result = int.Parse(qury.Data["bb"].ToString());
            return result;
        }

        async Task<int> getSampleQty(string mo_number, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select count(1) bb from sfism4.r_qc_sn_t  where serial_number in (select serial_number from sfism4.r107 where mo_number='" + mo_number + "')",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
                result = int.Parse(qury.Data["bb"].ToString());
            return result;
        }
        async Task<int> getPass(string mo_number, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select count(1) bb from sfism4.r107 where mo_number='" + mo_number + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
                result = int.Parse(qury.Data["bb"].ToString());
            return result;
        }
        async Task<int> getPackQTYair(string model, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select pallet_qty*carton_qty qty from sfis1.c_pack_param_t where model_name='" + model + "' and version_code='HK'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
                result = int.Parse(qury.Data["qty"].ToString());
            return result;
        }
        async Task<int> getPackQTYairhk(string model, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select pallet_qty*carton_qty qty from sfis1.c_pack_param_t where model_name='" + model + "' and version_code='HK'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
                result = int.Parse(qury.Data["qty"].ToString());
            return result;
        }

        async Task<int> getPackQTYairau(string model, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select pallet_qty*carton_qty qty from sfis1.c_pack_param_t where model_name='" + model + "' and version_code='AI'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
                result = int.Parse(qury.Data["qty"].ToString());
            return result;
        }

        async Task<int> getPackQTY(string model, string ver, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select pallet_qty*carton_qty qty from sfis1.c_pack_param_t where model_name='" + model + "' and version_code='" + ver + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
                result = int.Parse(qury.Data["qty"].ToString());
            return result;
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            List<OOBA> excellist = ColorStringGrid1.ItemsSource as List<OOBA>;
            ExportDgvToXML(excellist);
        }
        public void ExportDgvToXML(List<OOBA> list)
        {
            try
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("STT");
                dt.Columns.Add("HEAD");
                dt.Columns.Add("MO_NUMBER");
                dt.Columns.Add("MODEL_NAME");
                dt.Columns.Add("VERSION_CODE");
                foreach (OOBA items in list)
                {
                    dt.Rows.Add(items.STT, items.HEAD, items.MO_NUMBER, items.MODEL_NAME, items.VERSION_CODE);
                }
                dt.TableName = "DATA";
                SaveFileDialog sfd = new SaveFileDialog();
                //sfd.Filter = "XML|*.xml";
                sfd.Filter = "Excel Files |*.xls";
                // Show save file dialog box
                Nullable<bool> result = sfd.ShowDialog();
                if (result == true)
                {
                    try
                    {
                        dt.WriteXml(sfd.FileName);
                        MessageBox.Show("Export Successfully", "Finish");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string TXT = Edit1.Text.Trim();
                string mysql = $@"SELECT distinct mo_number head, shipping_sn mo_number,macid model_name,pin_code version_code
                             FROM sfism4.r_netg_prin_all_t  
                             WHERE print_flag in ('M','N') and  (shipping_sn, macid) IN ( 
                                      SELECT ssn1, mac1   
                                      FROM sfism4.r_custsn_t 
                                      WHERE serial_number IN ( 
                                       select serial_number from sfism4.r_qc_sn_t  where serial_number in 
                                     (select serial_number from sfism4.r107 where mo_number='{TXT}')
                                                            ))   order by shipping_sn ";
                var query = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = mysql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (query.Data.Count() != 0)
                {
                    col1.Header = "MO__NUMBER";
                    col2.Header = "SHIPPING__SN";
                    col3.Header = "MACID";
                    col4.Header = "PIN__CODE";
                    List<OOBA> snlist = new List<OOBA>();
                    snlist = query.Data.ToListObject<OOBA>().ToList();
                    ColorStringGrid1.ItemsSource = snlist;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
        }
    }
}
