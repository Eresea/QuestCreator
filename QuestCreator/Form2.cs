using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DialogueCreator3
{
    public partial class Form2 : Form
    {
        public static int nb;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            for(int i=5;i>= nb;i--)
            {
                switch(i)
                {
                    case 5:
                        textBox5.Enabled = false;
                        break;
                    case 4:
                        textBox4.Enabled = false;
                        break;
                    case 3:
                        textBox3.Enabled = false;
                        break;
                    case 2:
                        textBox2.Enabled = false;
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
