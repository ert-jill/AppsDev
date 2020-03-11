using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Immunization
{
    public partial class ImmunizationUpdateForm : Form
    {
        OleDbConnection thisConnection = new OleDbConnection();
        String connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Q:\Emerson Benatiro\Database\APPSDEV.mdb";
        bool val1 = false;
        bool val2 = false;
        bool val3 = false;
        bool val4 = false;
        bool val5 = false;
        bool val6 = false;
        bool val7 = false;
        bool val8 = false;
        bool val9 = false;
        bool val10 = false;
        public ImmunizationUpdateForm()
        {
            InitializeComponent();
            thisConnection.ConnectionString = connectionString;
        }
        public void onlyDigit(KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !(e.KeyChar == (char)Keys.Back))
                e.Handled = true;
        }
        private void immunizationNoTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlyDigit(e);
        }
        
    }
}
