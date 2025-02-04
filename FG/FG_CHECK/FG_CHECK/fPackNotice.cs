using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFC_Library;
using System.IO;
using Sfc.Library.HttpClient;

namespace FG_CHECK
{
    public partial class fPackNotice : Form
    {
        public EmployeeInfomation empInfo = new EmployeeInfomation();
        dbsfis dbsfis;
        public string DN_ITEM_NO, DN_NO, SO_NUMBER, SO_LINE, CUST_PO, CUST_NAME, CUST_ID;
        public static SfcHttpClient sfcClient;
        sfcconnect sfc;
        public int SHIPPING_QTY;
        //khai bao con tro
        public delegate void SendData(string item_no, string dn, string so_number, string so_line, int shipping_qty, string nw, string gw, string cust_po, string model, string cust_name, string cust_id, string ver);
        //tao doi tuong Sendata
        public SendData Sender;
        public fPackNotice(SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            //tao con tro toi ham GetData
            sfcClient = _sfcClient;
            dbsfis = new dbsfis();
            Sender = new SendData(GetData);
        }

        private void GetData(string item_no, string dn, string so_number, string so_line, int shipping_qty, string nw, string gw, string cust_po, string model, string cust_name, string cust_id, string ver)
        {
            lbModel.Text = model;
            txbVersion.Text = ver;
            txbNW.Text = nw;
            txbGW.Text = gw;
            DN_ITEM_NO = item_no;
            DN_NO = dn;
            SO_NUMBER = so_number;
            SO_LINE = so_line;
            SHIPPING_QTY = shipping_qty;
            CUST_PO = cust_po;
            CUST_NAME = cust_name;
            CUST_ID = cust_id;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = null;
                float NW = float.Parse(txbNW.Text);
                float GW = float.Parse(txbGW.Text);
                string DIMEN = txbDIMEN.Text;
                string Version = txbVersion.Text;
                string MODEL_NAME = lbModel.Text;
                string query1 = "select * from SFISM4.R_BPCS_INVOICE_T Where INV_NO= '{0}' and INVOICE='{1}' and SITE = 'HCAB01'";
             dt =   await dbsfis.ExcuteSelectSQL(string.Format(query1, DN_ITEM_NO, DN_NO), sfcClient);
                if (dt.Rows.Count == 0)//neu DN chua ton tai trong BPCS_INVOICE
                {
                    string query2 = "Insert Into SFISM4.R_BPCS_INVOICE_T " +
                    "(INV_NO, INVOICE, SO_NUMBER, SO_LINE, SO_QTY, UNIT_NW, UNIT_GW, DIMEN, CUST_PO, MODEL_NAME, CUST_NAME, CUST_ID, MO_NO, MODEL_VER, SITE) " +
                    "VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}', 'N / A','{12}', 'HCAB01')";
                    await dbsfis.ExcuteNonQuerySQL(string.Format(query2, DN_ITEM_NO, DN_NO, SO_NUMBER, SO_LINE, SHIPPING_QTY, NW, GW, DIMEN, CUST_PO, MODEL_NAME, CUST_NAME, CUST_ID, Version), sfcClient);
                }
                else//neu ton tai
                {
                    string query2 = "UPDATE SFISM4.R_BPCS_INVOICE_T  " +
                        "SET INV_NO='{0}',INVOICE='{1}',SO_NUMBER='{2}',SO_LINE='{3}',SO_QTY='{4}',UNIT_NW='{5}',UNIT_GW='{6}',DIMEN='{7}',CUST_PO='{8}',MODEL_NAME='{9}',CUST_NAME='{10}',CUST_ID='{11}',MODEL_VER='{12}',SITE = 'HCAB01' WHERE INV_NO='{13}' AND INVOICE='{14}'";
                   await dbsfis.ExcuteNonQuerySQL(string.Format(query2, DN_ITEM_NO, DN_NO, SO_NUMBER, SO_LINE, SHIPPING_QTY, NW, GW, DIMEN, CUST_PO, MODEL_NAME, CUST_NAME, CUST_ID, Version, DN_ITEM_NO, DN_NO), sfcClient);

                }
                string query3 = "UPDATE SFISM4.R_SAP_DN_DETAIL_T SET SHOW_FLAG='Y' ,material_group='{0}',create_date=sysdate  WHERE DN_NO= '{1}' AND DN_ITEM_NO= '{2}'";
                await  dbsfis.ExcuteNonQuerySQL(string.Format(query3, Form1.empNO, DN_NO, DN_ITEM_NO), sfcClient);//update show_flag, ma the,thoi gian
                MessageBox.Show("Update db ok!", "Message");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
