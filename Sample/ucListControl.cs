using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sample
{
    public partial class ucListControl : UserControl
    {
        private bool isOpen { get; set; }
        private string aux;

        public string Aux { 
            get { 
                return this.aux; 
            } 
            set { 
                this.aux = value; 
                button1.Text = this.aux; 
            } 
        }

        public ucListControl(string Label)
        {
            InitializeComponent();

            label1.Text = Label;
            this.Aux = "button";
            this.isOpen = false;
        }

        private void ucListControl_Load(object sender, EventArgs e)
        {
            
        }

        public void UpdateText(string Text)
        {
            label1.Text = Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(label1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.OpenCloseMenu();
        }

        private void OpenCloseMenu()
        {
            this.isOpen = !this.isOpen;
            if (this.isOpen)
            {
                this.Height += 3 * 51;
            }
            else
            {
                this.Height = 51;
            }
        }
    }
}
