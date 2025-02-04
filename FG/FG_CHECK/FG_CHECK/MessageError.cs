using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FG_CHECK
{
    public partial class MessageError : Form
    {
        public SfcHttpClient sfcClient;
        public string errorcode = "";
        public bool CustomFlag = false;
        public string MessageVietNam = "";
        public string MessageEnglish = "";
        string sql ;
        internal SfcHttpClient _sfcHttpClient;
    

        public MessageError()
        {
            InitializeComponent();
        }

        private async void MessageError_Load(object sender, EventArgs e)
        {
             if (CustomFlag)
            {
                Lab_English.Text = MessageEnglish;
                Lab_TV.Text = MessageVietNam;
            }
            else
            {
                sql = " SELECT * FROM SFIS1.C_PROMPT_CODE_T WHERE PROMPT_CODE = '" + errorcode + "' ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                if (result.Data != null)
                {
                    Lab_English.Text = errorcode + " - " + result.Data["prompt_english"].ToString();
                    Lab_TV.Text = errorcode + " - " + result.Data["prompt_chinese"].ToString();
                }
                else
                {
                    Lab_English.Text =  errorcode;
                    Lab_TV.Text =  errorcode;
                }
            }
        }

        private void Butt_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
