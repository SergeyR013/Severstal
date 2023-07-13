using System;
using System.Windows.Forms;

namespace Severstal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void startLoadBtn_Click(object sender, EventArgs e)
        {
            contragentCB.Enabled = false;
            startLoadBtn.Enabled = false;
            addBtn.Enabled = true;
            productCB.Enabled = true;
            numberOf.Enabled = true;
            Price.Enabled = true;
            cancelBtn.Enabled = true;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            contragentCB.Enabled = true;
            startLoadBtn.Enabled = true;
            addBtn.Enabled = false;
            productCB.Enabled = false;
            numberOf.Enabled = false;
            Price.Enabled = false;
            cancelBtn.Enabled = false;
            dataGridView1.Rows.Clear();
        }
    }
}
