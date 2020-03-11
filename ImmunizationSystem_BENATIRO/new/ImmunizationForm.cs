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
    public partial class ImmunizationForm : Form
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

        public ImmunizationForm()
        {
            InitializeComponent();
            thisConnection.ConnectionString = connectionString;
        }

        private void immunizationNoTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlyDigit(e);
        }
        private void immunizationNoTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (immunizationNoTextBox.TextLength == 5)
            {
                String sql = "SELECT * FROM IMMUNIZATIONHEADERFILE WHERE IMMHIMMUNO='" + immunizationNoTextBox.Text + "'";
                OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                thisConnection.Open();
                OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
                bool read = thisDataReader.Read();
                thisConnection.Close();
                if (!read)
                {
                    correctProviderMethod(immunizationNoTextBox, "ACCEPTED");
                    val1 = true;
                    if (e.KeyCode == Keys.Enter)
                    {
                        patientCodeTextBox.Focus();
                    }
                }
                else
                {
                    errorProviderMethod(immunizationNoTextBox, "DUPLICATE ENTRY DETECTED!");
                    val1 = false;
                }
            }
            else
            {
                errorProviderMethod(immunizationNoTextBox, "INVALID INPUT, IMMUNIZATION NO. MUST HAVE 5 DIGIT!");
                val1 = false;
            }
        }

        private void patientCodeTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                clearPatientInfo();
            }

            if (patientCodeTextBox.TextLength == 5)
            {
                String sql = "SELECT * FROM PATIENTFILE WHERE PATIENTCODE='" + patientCodeTextBox.Text + "'";
                OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                thisConnection.Open();
                OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
                bool read = thisDataReader.Read();

                if (read)
                {
                    if (thisDataReader["ACTIVESTATUS"].ToString().Equals("AC"))
                    {
                        correctProviderMethod(patientCodeTextBox, "ACCEPTED, RECORD FOUND.");
                        val2 = true;
                        resetErrorProvider(vaccineCodeTextBox);
                        resetErrorProvider(reactionTextBox);
                        resetErrorProvider(shotNumberTextBox);
                        vaccineCodeTextBox.Clear();
                        reactionTextBox.Clear();
                        shotNumberTextBox.Clear();
                        if (e.KeyCode == Keys.Enter)
                        {
                            patientNameLabel.Text = thisDataReader["PATIENTLASTNAME"] + ", " + thisDataReader["PATIENTFIRSTNAME"];
                            addressLabel.Text = thisDataReader["PATIENTADDRESS"] + "";
                            genderLabel.Text = thisDataReader["PATIENTGENDER"] + "";
                            birthdayLabel.Text = thisDataReader["PATIENTBIRTHDAY"] + "";
                            ageLabel.Text = thisDataReader["PATIENTAGE"] + "";
                            telephoneLabel.Text = thisDataReader["PATIENTTELEPHONE"] + "";
                            fathersNameLabel.Text = thisDataReader["PATIENTFATHER"] + "";
                            mothersNameLabel.Text = thisDataReader["PATIENTMOTHER"] + "";
                            weightTextBox.Focus();
                        }
                    }
                    else
                    {
                        errorProviderMethod(patientCodeTextBox,"PATIENT IS NOT ACTIVE!");
                        val2 = false;
                    }

                }
                else
                {
                    errorProviderMethod(patientCodeTextBox, "PATIENT CODE NOT FOUND!");
                    val2 = false;
                }
            }
            else
            {
                errorProviderMethod(patientCodeTextBox, "INVALID INPUT, PATIENT CODE MUST HAVE 5 CHARACTER!");
                val2 = false;
            }
            thisConnection.Close();
        }
        private void weightTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlyDigit(e);
        }
        private void weightTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (weightTextBox.Text == "")
            {
                errorProviderMethod(weightTextBox, "INPUT WEIGHT!");
                val3 = false;
            }
            else if (Convert.ToInt32(weightTextBox.Text) < 1)
            {
                errorProviderMethod(weightTextBox, "INVALID INPUT, WEIGHT Must Not Zero Or Less!");
            }
            else
            {
                correctProviderMethod(weightTextBox, "ACCEPTED");
                val3 = true;
                if (e.KeyCode == Keys.Enter)
                {
                    heightTextBox.Focus();
                }
            }
        }
        private void heightTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlyDigit(e);
        }
        private void heightTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (heightTextBox.Text == "")
            {
                errorProviderMethod(heightTextBox, "INPUT HEIGHT!");
                val4 = false;
            }
            else if (Convert.ToInt32(heightTextBox.Text) < 1)
            {
                errorProviderMethod(heightTextBox, "INVALID INPUT, HEIGHT Must Not Zero Or Less!");
            }
            else
            {
                correctProviderMethod(heightTextBox, "ACCEPTED");
                val4 = true;
                if (e.KeyCode == Keys.Enter)
                {
                    vaccineCodeTextBox.Focus();
                }
            }
        }
        private void shotNumberTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlyDigit(e);
        }
        private void shotNumberTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (val5 && val2)
            {
                String sql = "SELECT * FROM VACCINEFILE WHERE VACCODE='" + vaccineCodeTextBox.Text + "'";
                OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                thisConnection.Open();
                OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
                thisDataReader.Read();
                if (shotNumberTextBox.Text == "")
                {
                    errorProviderMethod(shotNumberTextBox, "INPUT SHOT NUMBER!");
                    val6 = false;
                }
                else if (Convert.ToInt32(shotNumberTextBox.Text) <= Convert.ToInt32(thisDataReader["VACNUMSHOT"].ToString()) && Convert.ToInt32(shotNumberTextBox.Text) >= 1)
                {
                    String sql1 = "SELECT * FROM VACCINEPATIENTFILE WHERE VACVCODE = '" + vaccineCodeTextBox.Text + "' AND VACPCODE ='" + patientCodeTextBox.Text + "'";
                    OleDbCommand thisCommand1 = new OleDbCommand(sql1, thisConnection);

                    OleDbDataReader thisDataReader1 = thisCommand1.ExecuteReader();
                    bool equalShotNum = false;
                    while (thisDataReader1.Read())
                    {
                        if (Convert.ToInt32(thisDataReader1["VACPATSHOTNUM"].ToString()) == Convert.ToInt32(shotNumberTextBox.Text))
                            equalShotNum = true;
                    }

                    if (!equalShotNum)
                    {
                        correctProviderMethod(shotNumberTextBox, "ACCEPTED");
                        val6 = true;
                        if (e.KeyCode == Keys.Enter)
                        {
                            addRow();
                        }
                    }
                    else
                    {
                        errorProviderMethod(shotNumberTextBox, "PATIENT ALREADY TAKE THIS SHOT!");
                        val6 = false;
                    }
                }
                else
                {
                    errorProviderMethod(shotNumberTextBox, "INVALID INPUT, SHOT NUMBER MUST BE GREATER THAN ZERO \nAND NOT GREATER THAN MAXIMUM NUMSHOT OF THE VACCINE!");
                    val6 = false;

                }
                thisConnection.Close();
            }
            else
            {

                shotNumberTextBox.Clear();

                val6 = false;
                if (!val2)
                {
                    errorProviderMethod(shotNumberTextBox, "Input Patient Code First!");
                    shotNumberTextBox.Clear();
                    patientCodeTextBox.Focus();
                }
                else
                {

                    MessageBox.Show("INPUT VACCINE CODE FIRST");
                    vaccineCodeTextBox.Focus();
                }
            }
        }
        private void reactionTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (val5 && val2)
            {
                if (reactionTextBox.Text != "")
                {
                    correctProviderMethod(reactionTextBox, "ACCEPTED");
                    val7 = true;
                    if (e.KeyCode == Keys.Enter)
                        addRow();
                }
                else
                {
                    errorProviderMethod(reactionTextBox, "INPUT REACTION!");
                    val7 = false;
                }
            }
            else
            {
                reactionTextBox.Clear();

                val7 = false;
                if (!val2)
                {
                    errorProviderMethod(reactionTextBox, "Input Patient Code First!");
                }
                else
                {
                    MessageBox.Show("INPUT VACCINE CODE FIRST");
                    vaccineCodeTextBox.Focus();
                }
            }
        }
        private void vaccineDataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            val8 = vaccineDataGridView.Rows.Count > 1;
        }

        private void preparedByTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                preparedByLabel.Text = "";
            if (preparedByTextBox.TextLength == 5)
            {
                String sql = "SELECT * FROM EMPLOYEEFILE WHERE EMPLOYEEID = '" + preparedByTextBox.Text + "' AND EMPLOYEEPOSITION='ST' AND EMPLOYEEAVAILABILITY='AV'";
                OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                thisConnection.Open();
                OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
                if (thisDataReader.Read())
                {
                    correctProviderMethod(preparedByTextBox, "ACCEPTED, RECORD EXIST!");
                    val9 = true;
                    if (e.KeyCode == Keys.Enter)
                    {
                        preparedByLabel.Text = thisDataReader["EMPLOYEEFIRSTNAME"] + " " + thisDataReader["EMPLOYEELASTNAME"];
                        immunizedByTextBox.Focus();
                    }
                }
                else
                {
                    errorProviderMethod(preparedByTextBox, "EMPLOYEE NOT FOUND / UNABLE / NOT AVAILABLE TO DO THE WORK!");
                    val9 = false;
                }
                thisConnection.Close();
            }
            else
            {
                errorProviderMethod(preparedByTextBox, "INVALID INPUT, EMPLOYEE CODE MUST HAVE 5 CHARACTERS!");
                val9 = false;

            }
        }
        private void immunizedByTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                immunizedByLabel.Text = "";
            if (immunizedByTextBox.TextLength == 5)
            {
                String sql = "SELECT * FROM EMPLOYEEFILE WHERE EMPLOYEEID = '" + immunizedByTextBox.Text + "' AND EMPLOYEEPOSITION='DR'";
                OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                thisConnection.Open();
                OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
                if (thisDataReader.Read())
                {
                    correctProviderMethod(immunizedByTextBox, "ACCEPTED, RECORD FOUND.");
                    val10 = true;
                    if (e.KeyCode == Keys.Enter)
                    {
                        immunizedByLabel.Text = thisDataReader["EMPLOYEEFIRSTNAME"] + " " + thisDataReader["EMPLOYEELASTNAME"];
                        saveButton.Focus();
                    }
                }
                else
                {
                    errorProviderMethod(immunizedByTextBox, "EMPLOYEE NOT FOUND / UNABLE / NOT AVAILABLE TO DO THE WORK!");
                    val10 = false;
                }
                thisConnection.Close();
            }
            else
            {
                errorProviderMethod(immunizedByTextBox, "INVALID INPUT, EMPLOYEE CODE MUST HAVE 5 CHARACTERS!");
                val10 = false;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!val1)
            {
                immunizationNoTextBox.Focus();
                MessageBox.Show("Please Input Immunization NO.");
            }
            else if (!val2)
            {
                patientCodeTextBox.Focus();
                MessageBox.Show("Please Input Patient Code.");
            }
            else if (!val3)
            {
                weightTextBox.Focus();
                MessageBox.Show("Please Input Weight.");
            }
            else if (!val4)
            {
                heightTextBox.Focus();
                MessageBox.Show("Please Input Height.");
            }
            else if (!val8)
            {
                MessageBox.Show("Please ENTER Vaccine!");
                vaccineCodeTextBox.Focus();
            }
            else if (!val9)
            {
                preparedByTextBox.Focus();
                MessageBox.Show("Please ENTER Prepared By, Employee Code!");
            }
            else if (!val10)
            {
                immunizedByTextBox.Focus();
                MessageBox.Show("Please ENTER Immunized By, Employee Code!");
            }
            else
            {
                String sql1 = "SELECT * FROM IMMUNIZATIONDETAILFILE";
                OleDbDataAdapter thisDataAdapter1 = new OleDbDataAdapter(sql1, thisConnection);
                OleDbCommandBuilder thisCommandBuilder1 = new OleDbCommandBuilder(thisDataAdapter1);
                DataSet thisDataSet1 = new DataSet();
                thisDataAdapter1.Fill(thisDataSet1, "IMMUNIZATIONDETAILFILE");
                DataRow[] thisDataRow1 = new DataRow[vaccineDataGridView.Rows.Count - 1];

                String sql0 = "SELECT * FROM IMMUNIZATIONHEADERFILE";
                OleDbDataAdapter thisDataAdapter0 = new OleDbDataAdapter(sql0, thisConnection);
                OleDbCommandBuilder thisCommandBuilder0 = new OleDbCommandBuilder(thisDataAdapter0);
                DataSet thisDataSet0 = new DataSet();

                thisDataAdapter0.Fill(thisDataSet0, "IMMUNIZATIONHEADERFILE");
                DataRow thisDataRow0 = thisDataSet0.Tables["IMMUNIZATIONHEADERFILE"].NewRow();
                thisDataRow0["IMMHIMMUNO"] = immunizationNoTextBox.Text;
                thisDataRow0["IMMHDATE"] = dateTimePicker.Text;
                thisDataRow0["IMMHPATCODE"] = patientCodeTextBox.Text;
                thisDataRow0["IMMHPATWEIGHT"] = Convert.ToInt32(weightTextBox.Text);
                thisDataRow0["IMMHPATHEIGHT"] = Convert.ToInt32(heightTextBox.Text);
                thisDataRow0["IMMHPREPBY"] = preparedByTextBox.Text;
                thisDataRow0["IMMHIMMUBY"] = immunizedByTextBox.Text;
                thisDataSet0.Tables["IMMUNIZATIONHEADERFILE"].Rows.Add(thisDataRow0);
                thisDataAdapter0.Update(thisDataSet0, "IMMUNIZATIONHEADERFILE");


                for (int x = 0; x < vaccineDataGridView.Rows.Count - 1; x++)
                {
                    thisDataRow1[x] = thisDataSet1.Tables["IMMUNIZATIONDETAILFILE"].NewRow();
                    thisDataRow1[x]["IMMDIMMUHNO"] = immunizationNoTextBox.Text;
                    thisDataRow1[x]["IMMDVACCODE"] = vaccineDataGridView.Rows[x].Cells["dataGridVacCode"].Value.ToString();
                    thisDataRow1[x]["IMMDSHOTNUM"] = Convert.ToInt32(vaccineDataGridView.Rows[x].Cells["dataGridVacShotNum"].Value.ToString());
                    thisDataRow1[x]["IMMDREACTION"] = vaccineDataGridView.Rows[x].Cells["dataGridVacReact"].Value.ToString();
                    thisDataSet1.Tables["IMMUNIZATIONDETAILFILE"].Rows.Add(thisDataRow1[x]);
                    thisDataAdapter1.Update(thisDataSet1, "IMMUNIZATIONDETAILFILE");
                }
                MessageBox.Show("Data Recorded.");
                clear();
                resetAllErrorProvider();
            }
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            resetAllErrorProvider();
            clear();
        }
        private void errorProviderMethod(TextBox txtBox, String text)
        {
            correctProvider.SetError(txtBox, "");
            errorProvider.SetError(txtBox, text);
        }
        public void correctProviderMethod(TextBox txtBox, String text)
        {
            errorProvider.SetError(txtBox, "");
            correctProvider.SetError(txtBox, text);
        }
        private void resetErrorProvider(TextBox textBoxes)
        {
            errorProvider.SetError(textBoxes, "");
            correctProvider.SetError(textBoxes, "");
        }
        private void resetAllErrorProvider()
        {
            resetErrorProvider(immunizationNoTextBox);
            resetErrorProvider(patientCodeTextBox);
            resetErrorProvider(weightTextBox);
            resetErrorProvider(heightTextBox);
            resetErrorProvider(vaccineCodeTextBox);
            resetErrorProvider(shotNumberTextBox);
            resetErrorProvider(reactionTextBox);
            resetErrorProvider(preparedByTextBox);
            resetErrorProvider(immunizedByTextBox);
        }
        private void clear()
        {
            immunizationNoTextBox.Clear();
            patientCodeTextBox.Clear();
            clearPatientInfo();
            vaccineCodeTextBox.Clear();
            clearVaccineInfo();
            preparedByTextBox.Clear();
            preparedByLabel.Text = "";
            immunizedByTextBox.Clear();
            immunizedByLabel.Text = "";
        }
        private void clearVaccineInfo()
        {
            shotNumberTextBox.Clear();
            reactionTextBox.Clear();
            vaccineDataGridView.Rows.Clear();
        }
        private void clearPatientInfo()
        {
            patientNameLabel.Text = "";
            addressLabel.Text = "";
            telephoneLabel.Text = "";
            fathersNameLabel.Text = "";
            mothersNameLabel.Text = "";
            genderLabel.Text = "";
            birthdayLabel.Text = "";
            ageLabel.Text = "";
            weightTextBox.Clear();
            heightTextBox.Clear();
            resetErrorProvider(weightTextBox);
            resetErrorProvider(heightTextBox);
        }
        public void onlyDigit(KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !(e.KeyChar == (char)Keys.Back))
                e.Handled = true;
        }
        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void addRow()
        {
            if (!val5)
            {
                vaccineCodeTextBox.Focus();
            }
            else if (!val6)
            {
                shotNumberTextBox.Focus();
            }
            else if (!val7)
            {
                reactionTextBox.Focus();
            }
            else
            {
                int shotNum = Convert.ToInt32(shotNumberTextBox.Text);

                try
                {
                    String reaction = reactionTextBox.Text;
                    String sql = "SELECT * FROM VACCINEFILE WHERE VACCODE='" + vaccineCodeTextBox.Text + "'";
                    OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                    thisConnection.Open();
                    OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
                    thisDataReader.Read();
                    vaccineDataGridView.Rows.Add(thisDataReader["VACCODE"].ToString().ToUpper(),
                        thisDataReader["VACNAME"].ToString(),
                        thisDataReader["VACDESC"].ToString(),
                        shotNumberTextBox.Text,
                        reactionTextBox.Text);
                    thisConnection.Close();
                }
                catch
                {
                    MessageBox.Show("ENTER VACCINE AGAIN!");
                }

                thisConnection.Close();
                vaccineCodeTextBox.Clear();
                shotNumberTextBox.Clear();
                reactionTextBox.Clear();
                vaccineCodeTextBox.Focus();
                resetErrorProvider(vaccineCodeTextBox);
                resetErrorProvider(shotNumberTextBox);
                resetErrorProvider(reactionTextBox);
                val5 = false;
                val6 = false;
                val7 = false;
            }
        }

        private void vaccineCodeTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (val2)
            {
                if (vaccineCodeTextBox.TextLength == 5)
                {
                    String sql = "SELECT * FROM VACCINEFILE WHERE VACCODE='" + vaccineCodeTextBox.Text + "'";
                    OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                    thisConnection.Open();
                    OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
                    bool read = thisDataReader.Read();
                    thisConnection.Close();
                    if (read)
                    {
                        bool repeat = false;
                        if (vaccineDataGridView.Rows.Count > 1)
                        {
                            for (int c = 0; c < vaccineDataGridView.Rows.Count - 1; c++)
                            {
                                if (vaccineDataGridView.Rows[c].Cells["dataGridVacCode"].Value.ToString().Equals(vaccineCodeTextBox.Text, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    repeat = true;
                                    break;
                                }
                            }
                        }
                        if (!repeat)
                        {
                            correctProviderMethod(vaccineCodeTextBox, "ACCEPTED, RECORD FOUND.");
                            val5 = true;
                            if (e.KeyCode == Keys.Enter)
                                addRow();
                        }
                        else
                        {
                            errorProviderMethod(vaccineCodeTextBox, "DUPLICATE ENTRY DETECTED!");
                            val5 = false;
                        }
                    }
                    else
                    {
                        errorProviderMethod(vaccineCodeTextBox, "VACCINE CODE NOT FOUND!");
                        val5 = false;
                    }
                }
                else if (e.KeyCode == Keys.Enter && vaccineCodeTextBox.Text == "" && vaccineDataGridView.Rows.Count > 1)
                {
                    preparedByTextBox.Focus();
                    resetErrorProvider(vaccineCodeTextBox);
                    resetErrorProvider(shotNumberTextBox);
                    resetErrorProvider(reactionTextBox);
                }
                else
                {
                    errorProviderMethod(vaccineCodeTextBox, "INVALID INPUT, VACCINE CODE MUST HAVE 5 CHARACHTERS!");
                    val5 = false;
                }
            }
            else
            {
                errorProviderMethod(vaccineCodeTextBox, "Input Patient Code First!");
                vaccineCodeTextBox.Clear();
                patientCodeTextBox.Focus();
            }

        }

        private void immunizationNoTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
