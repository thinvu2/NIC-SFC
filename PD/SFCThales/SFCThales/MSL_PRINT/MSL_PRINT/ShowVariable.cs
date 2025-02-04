using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSL_PRINT
{
    public partial class ShowVariable : Form
    {
        public ShowVariable(Dictionary<string,string> _dic)
        {
            InitializeComponent();
            dic = _dic;
        }
        
        Dictionary<string, string> dic;
        private void ShowVariable_Load(object sender, EventArgs e)
        {
            var keys = dic.Keys;
            foreach(var k in keys)
            {
                dataGridView1.Rows.Add(k,dic[k]);
            }
        }
    }
}
