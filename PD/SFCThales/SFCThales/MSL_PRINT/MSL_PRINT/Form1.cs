using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace LoginForm
{
    public partial class Form1 : Form
    {
        Random rd = new Random();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                int bgVideoIndex = rd.Next(1, 5);
                picBoxBackGround.Invoke(new MethodInvoker(delegate ()
                {
                    switch (bgVideoIndex)
                    {
                        case 1:
                            picBoxBackGround.Image = Properties.Resources.background_1;
                            break;
                        case 2:
                            picBoxBackGround.Image = Properties.Resources.background_2;
                            break;
                        case 3:
                            picBoxBackGround.Image = Properties.Resources.background_3;
                            break;
                        case 4:
                            picBoxBackGround.Image = Properties.Resources.background_4;
                            break;
                        case 5:
                            picBoxBackGround.Image = Properties.Resources.background_5;
                            break;
                        default:
                            picBoxBackGround.Image = Properties.Resources.background_1;
                            break;
                    }
                }));

            }).Start();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

        }
    }
}
