
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApplication1
{


    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            System.Data.DataTable dt = new DataTable();
            dt.Columns.Add("column1", typeof(string));
            dt.Columns.Add("column2", typeof(string));
            dt.Columns.Add("column3", typeof(string));

            this.dgvDisplayData.DataSource = dt.DefaultView;
        }


    }


}
