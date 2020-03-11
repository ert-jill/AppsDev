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
    public partial class ConsultationForm : Form
    {
        OleDbConnection thisConnection = new OleDbConnection();
        public ConsultationForm()
        {
            InitializeComponent();
            thisConnection.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Q:\Emerson Benatiro\Database\APPSDEV.mdb";
        }

        bool val1 = false;//Consultation No. Validation
        bool val2 = false;//Immunization (referral) No. Validation
        bool val3 = false;//Patient Code Validation
        bool val4 = false;//weight Validation
        bool val5 = false;//Height Validation
        bool val6 = false;//Body Temp... Validation
        bool val7 = false;//Diagnostic Code Validation
        bool val8 = false;//Physician Notes Validation
        bool val9 = false;//Diagnostic Data Grid Validation
        bool val10 = false;//Prepared By Employee Validation
        bool val11 = false;//Examined By Employee Validation
        String refSlip = "NA";

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            clearAll();

        }
        private void clearAll()
        {
            consultationNoTextBox.Clear();
            immunizationReferralNo.Clear();
            patientCodeTextBox.Clear();
            clearPatientInfo();
            diagnosticCodeTextBox.Clear();
            physicianNotesTextBox.Clear();
            diagnosticDataGridView.Rows.Clear();
            admissionCheckBox.Checked = false;
            laboratoryCheckBox.Checked = false;
            preparedByTextBox.Clear();
            preparedByLabel.Text = "";
            examinedByTextBox.Clear();
            examinedByLabel.Text = "";
            resetControlsErrorProvider();
        }

        private void ConsultationForm_Load(object sender, EventArgs e)
        {

        }

        private void correctProviderMethod(TextBox txtbox, String msg)
        {
            errorProvider.SetError(txtbox, "");
            correctProvider.SetError(txtbox, msg);
        }

        private void errorProviderMethod(TextBox txtbox, String msg)
        {
            correctProvider.SetError(txtbox, "");
            errorProvider.SetError(txtbox, msg);
        }

        private void consultationNoTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (consultationNoTextBox.TextLength == 5)
            {
                if (!isFound("CONSULTATIONHEADERFILE", " WHERE CONHNO='" + consultationNoTextBox.Text + "'"))
                {
                    correctProviderMethod(consultationNoTextBox, "Accepted");
                    val1 = true;
                    if (e.KeyCode == Keys.Enter)
                        immunizationReferralNo.Focus();
                }
                else
                {
                    errorProviderMethod(consultationNoTextBox, "Duplicate Entry!");
                    val1 = false;
                }
            }
            else
            {
                errorProviderMethod(consultationNoTextBox, "INVALID INPUT, CONSULTATION NO MUST HAVE 5 DIGIT");
                val1 = false;
            }
        }

        public void onlyDigit(KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !(e.KeyChar == (char)Keys.Back))
                e.Handled = true;
        }

        private void consultationNoTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlyDigit(e);
        }
        private bool isFound(String table, String comp)
        {
            String sql = "SELECT * FROM " + table + " " + comp;
            OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
            thisConnection.Open();
            OleDbDataReader thisReader = thisCommand.ExecuteReader();
            bool found = thisReader.Read();
            thisConnection.Close();
            if (found)
                return true;
            else
                return false;


        }

        private void immunizationReferralNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            onlyDigit(e);
            if (e.KeyChar == (char)Keys.Back)
            {
                patientCodeTextBox.Clear();
                clearPatientInfo();
            }
        }

        private void immunizationReferralNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (immunizationReferralNo.Text == "")
            {
                val2 = true;
                val3 = false;
                patientCodeTextBox.Enabled = true;
                correctProviderMethod(immunizationReferralNo, "ACCEPTED");
                if (e.KeyCode == Keys.Enter)
                    patientCodeTextBox.Focus();
            }
            else if (immunizationReferralNo.TextLength == 5)
            {
                if (isFound("IMMUNIZATIONHEADERFILE", " WHERE IMMHIMMUNO='" + immunizationReferralNo.Text + "'"))
                {
                    correctProviderMethod(immunizationReferralNo, "ACCEPTED");
                    val2 = true;
                    if (e.KeyCode == Keys.Enter)
                    {
                        String sql = "SELECT * FROM IMMUNIZATIONHEADERFILE WHERE IMMHIMMUNO='" + immunizationReferralNo.Text + "'";
                        OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                        thisConnection.Open();
                        OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
                        thisDataReader.Read();
                        String pCode = thisDataReader["IMMHPATCODE"].ToString();
                        thisConnection.Close();
                        patientCodeTextBox.Text = pCode;
                        val3 = true;
                        getPatientInfo(pCode);
                        patientCodeTextBox.Enabled = false;
                    }
                }
                else
                {
                    errorProviderMethod(immunizationReferralNo, "RECORD NOT FOUND!");
                    val2 = false;
                    val3 = false;
                }
            }
            else
            {
                errorProviderMethod(immunizationReferralNo, "INVALID INPUT, IMMUNIZATION REFERRAL MUST \nHAVE 5 DIGITS! ");
                val2 = false;
                val3 = false;
                patientCodeTextBox.Enabled = true;
            }
        }

        private void patientCodeTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                clearPatientInfo();
            //if (val2)
            //{
            if (patientCodeTextBox.TextLength == 5)
            {
                if (isFound("PATIENTFILE", " WHERE PATIENTCODE='" + patientCodeTextBox.Text
                    + "' AND ACTIVESTATUS = 'AC'"))
                {
                    correctProviderMethod(patientCodeTextBox, "ACCEPTED");
                    val3 = true;
                    if (e.KeyCode == Keys.Enter)
                        getPatientInfo(patientCodeTextBox.Text);
                }
                else if (isFound("PATIENTFILE", " WHERE PATIENTCODE='" + patientCodeTextBox.Text
                + "'"))
                {
                    errorProviderMethod(patientCodeTextBox, "PATIENT IS INACTIVE!");
                    val3 = false;
                }
                else
                {
                    errorProviderMethod(patientCodeTextBox, "RECORD NOT FOUND!");
                    val3 = false;
                }
            }
            else
            {
                errorProviderMethod(patientCodeTextBox, "INVALID INPUT, PATIENT CODEMUST HAVE 5 CHARACTERS!");
                val3 = false;
            }
            //}
            //else
            //{
            //    MessageBox.Show("INPUT REFERRAL NUMBER FIRST!");
            //    patientCodeTextBox.Clear();
            //    immunizationReferralNo.Focus();
            //    val3 = false;
            //}
        }
        private void getPatientInfo(String pCode)
        {
            String sql = "SELECT * FROM PATIENTFILE WHERE PATIENTCODE='" + pCode + "'";
            OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
            thisConnection.Open();
            OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
            thisDataReader.Read();
            patientNameLabel.Text = thisDataReader["PATIENTLASTNAME"] + ", " + thisDataReader["PATIENTFIRSTNAME"];
            addressLabel.Text = thisDataReader["PATIENTADDRESS"] + "";
            genderLabel.Text = thisDataReader["PATIENTGENDER"] + "";
            birthdayLabel.Text = thisDataReader["PATIENTBIRTHDAY"] + "";
            ageLabel.Text = thisDataReader["PATIENTAGE"] + "";
            telephoneLabel.Text = thisDataReader["PATIENTTELEPHONE"] + "";
            fathersNameLabel.Text = thisDataReader["PATIENTFATHER"] + "";
            mothersNameLabel.Text = thisDataReader["PATIENTMOTHER"] + "";

            weightTextBox.Focus();
            thisConnection.Close();
        }

        private void onlyDigitToNumberValues(object sender, KeyPressEventArgs e)
        {
            onlyDigit(e);
        }

        private void weightTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (weightTextBox.Text != "" && Int32.Parse(weightTextBox.Text) > 0)
            {
                correctProviderMethod(weightTextBox, "ACCEPTED");
                val4 = true;
                if (e.KeyCode == Keys.Enter)
                    heightTextBox.Focus();
            }
            else
            {
                errorProviderMethod(weightTextBox, "INVALID INPUT, WEIGHT MUST CONTAIN ONLY A DIGIT/S GEATER THAN 0!");
                val4 = false;
            }
        }

        private void heightTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (heightTextBox.Text != "" && Int32.Parse(heightTextBox.Text) > 0)
            {
                correctProviderMethod(heightTextBox, "ACCEPTED");
                val5 = true;
                if (e.KeyCode == Keys.Enter)
                    bodyTempTextBox.Focus();
            }
            else
            {
                errorProviderMethod(heightTextBox, "INVALID INPUT, HEIGHT MUST CONTAIN ONLY A DIGIT/S GEATER THAN 0!");
                val5 = false;
            }
        }

        private void bodyTempTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (bodyTempTextBox.Text != "" && Int32.Parse(bodyTempTextBox.Text) > 0 && Int32.Parse(bodyTempTextBox.Text) <= 45)
            {
                correctProviderMethod(bodyTempTextBox, "ACCEPTED");
                val6 = true;
                if (e.KeyCode == Keys.Enter)
                    diagnosticCodeTextBox.Focus();
            }
            else
            {
                errorProviderMethod(bodyTempTextBox, "INVALID INPUT, WEIGHT MUST CONTAIN ONLY A DIGIT/S GEATER THAN 0 \nAND LESS THAN OR EQUAL 45!");
                val6 = false;
            }
        }

        private void diagnosticCodeTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && diagnosticDataGridView.Rows.Count >= 2 && diagnosticCodeTextBox.Text == "")
            {
                preparedByTextBox.Focus();
            }
            else
            {


                if (diagnosticCodeTextBox.TextLength == 5)
                {
                    if (isFound("DIAGNOSISFILE", "WHERE DIAGCODE='" + diagnosticCodeTextBox.Text + "'"))
                    {
                        bool found = false;

                        if (diagnosticDataGridView.Rows.Count > 1)
                        {
                            for (int c = 0; c < diagnosticDataGridView.Rows.Count - 1; c++)
                            {
                                if (diagnosticDataGridView.Rows[c].Cells["dataGridDiagnosisCode"].Value.ToString().Equals(diagnosticCodeTextBox.Text, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            correctProviderMethod(diagnosticCodeTextBox, "ACCEPTED, RECORD FOUND.");
                            val7 = true;
                            if (e.KeyCode == Keys.Enter)
                                addRow();
                        }
                        else
                        {
                            errorProviderMethod(diagnosticCodeTextBox, "DIAGNOSIS ALREADY ENTERED!");
                        }
                    }
                    else
                    {
                        errorProviderMethod(diagnosticCodeTextBox, "RECORD NOT FOUND!");
                        val7 = false;
                    }
                }
                else
                {
                    errorProviderMethod(diagnosticCodeTextBox, "INVALID INPUT, DIAGNOSTIC CODE MUST HAVE FIVE CHARACTERS!");
                    val7 = false;
                }
            }
        }
        private void addRow()
        {
            if (!val7)
            {
                diagnosticCodeTextBox.Focus();
            }
            else if (!val8)
            {
                physicianNotesTextBox.Focus();
            }
            else
            {
                String sql = "SELECT * FROM DIAGNOSISFILE WHERE DIAGCODE='" + diagnosticCodeTextBox.Text + "'";
                OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                thisConnection.Open();
                OleDbDataReader thisReader = thisCommand.ExecuteReader();
                thisReader.Read();
                diagnosticDataGridView.Rows.Add(thisReader["DIAGCODE"].ToString(), thisReader["DIAGNAME"].ToString(), physicianNotesTextBox.Text, thisReader["DIAGSTATUS"].ToString());
                thisConnection.Close();
                diagnosticCodeTextBox.Clear();
                physicianNotesTextBox.Clear();
                resetProvider(diagnosticCodeTextBox);
                resetProvider(physicianNotesTextBox);
                diagnosticCodeTextBox.Focus();
                val7 = false;
                val8 = false;
            }
        }

        private void physicianNotesTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (physicianNotesTextBox.Text != "")
            {
                correctProviderMethod(physicianNotesTextBox, "ACCEPTED");
                val8 = true;
                if (e.KeyCode == Keys.Enter)
                    addRow();
            }
            else
            {
                errorProviderMethod(physicianNotesTextBox, "INVALID INPUT, PHYSICIAN NOTE'S MUST NOT BE EMPTY!");
                val8 = false;
            }
        }
        private void resetProvider(TextBox t)
        {
            correctProviderMethod(t, "");
            errorProviderMethod(t, "");
        }
        private void refSlipMethod()
        {
            if (admissionCheckBox.Checked == false && laboratoryCheckBox.Checked == true)
                refSlip = "LT";
            else if (admissionCheckBox.Checked == true && laboratoryCheckBox.Checked == false)
                refSlip = "AD";
            else if (admissionCheckBox.Checked == true && laboratoryCheckBox.Checked == true)
                refSlip = "BT";
            else
                refSlip = "NA";
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            refSlipMethod();

            if (!val1)
            {
                MessageBox.Show("PLEASE INPUT VALID CONSULTATION CODE!");
                consultationNoTextBox.Focus();
            }
            else if (!val2)
            {
                MessageBox.Show("PLEASE INPUT VALID IMMUNIZATION REFERRAL NO.!");
                immunizationReferralNo.Focus();
            }
            else if (!val3)
            {
                MessageBox.Show("PLEASE INPUT PATIENT CODE IN RELATION TO REFERRAL NO.!");
                patientCodeTextBox.Focus();
            }
            else if (!val4)
            {
                MessageBox.Show("PLEASE INPUT WEIGHT!");
                weightTextBox.Focus();
            }
            else if (!val5)
            {
                MessageBox.Show("PLEASE INPUT HEIGHT!");
                heightTextBox.Focus();
            }
            else if (!val6)
            {
                MessageBox.Show("PLEASE INPUT BODY TEMPEARTURE!");
                bodyTempTextBox.Focus();
            }
            else if (!val9)
            {
                MessageBox.Show("PLEASE ADD DIAGNOSIS!");
                diagnosticCodeTextBox.Focus();
            }
            else if (!val10)
            {
                MessageBox.Show("PLEASE INPUT VALID STAFF CODE!");
                preparedByTextBox.Focus();
            }
            else if (!val11)
            {
                MessageBox.Show("PLEASE INPUT VALID DR CODE!");
                examinedByTextBox.Focus();
            }
            else if (isFound("CONSULTATIONHEADERFILE", "WHERE CONHNO='" + consultationNoTextBox.Text + "'"))
            {
                MessageBox.Show("DUPLICATE CONSULTATION NO. ENTRIES DETECTED!");
                consultationNoTextBox.Focus();
                consultationNoTextBox.Clear();
            }
            else
            {
                String sql1 = "SELECT * FROM CONSULTATIONHEADERFILE";
                OleDbDataAdapter thisDataAdapter1 = new OleDbDataAdapter(sql1, thisConnection);
                OleDbCommandBuilder thisCommandBuilder1 = new OleDbCommandBuilder(thisDataAdapter1);
                DataSet thisDataSet1 = new DataSet();

                thisDataAdapter1.Fill(thisDataSet1, "CONSULTATIONHEADERFILE");
                DataRow thisDataRow1 = thisDataSet1.Tables["CONSULTATIONHEADERFILE"].NewRow();
                thisDataRow1["CONHNO"] = consultationNoTextBox.Text;
                thisDataRow1["CONHIMMREF"] = immunizationReferralNo.Text;
                thisDataRow1["CONHDATE"] = dateTimePicker.Text;
                thisDataRow1["CONHPATCODE"] = patientCodeTextBox.Text;
                thisDataRow1["CONHPATWEIGHT"] = Int32.Parse(weightTextBox.Text);
                thisDataRow1["CONHPATHEIGHT"] = Int32.Parse(heightTextBox.Text);
                thisDataRow1["CONHPATBODYTEMP"] = Int32.Parse(bodyTempTextBox.Text);
                thisDataRow1["CONHREFSLIPS"] = refSlip;
                thisDataRow1["CONHPREPBY"] = preparedByTextBox.Text;
                thisDataRow1["CONHEXAMBY"] = examinedByTextBox.Text;
                // thisDataRow1["CONHSTATUS"] = 
                thisDataSet1.Tables["CONSULTATIONHEADERFILE"].Rows.Add(thisDataRow1);
                thisDataAdapter1.Update(thisDataSet1, "CONSULTATIONHEADERFILE");

                String sql2 = "SELECT * FROM CONSULTATIONDETAILFILE";
                OleDbDataAdapter thisDataAdapter2 = new OleDbDataAdapter(sql2, thisConnection);
                OleDbCommandBuilder thisCommandBuilder2 = new OleDbCommandBuilder(thisDataAdapter2);
                DataSet thisDataSet2 = new DataSet();
                thisDataAdapter2.Fill(thisDataSet2, "CONSULTATIONDETAILFILE");
                DataRow[] thisDataRow2 = new DataRow[diagnosticDataGridView.Rows.Count - 1];

                for (int x = 0; x < diagnosticDataGridView.Rows.Count - 1; x++)
                {
                    thisDataRow2[x] = thisDataSet2.Tables["CONSULTATIONDETAILFILE"].NewRow();
                    thisDataRow2[x]["CONDNO"] = consultationNoTextBox.Text;
                    thisDataRow2[x]["CONDDIAGCODE"] = diagnosticDataGridView.Rows[x].Cells["dataGridDiagnosisCode"].Value.ToString();
                    thisDataRow2[x]["CONDNOTES"] = diagnosticDataGridView.Rows[x].Cells["dataGridPhysicianNotes"].Value.ToString();
                    thisDataRow2[x]["CONDSTATUS"] = diagnosticDataGridView.Rows[x].Cells["dataGridStatus"].Value.ToString();
                    thisDataSet2.Tables["CONSULTATIONDETAILFILE"].Rows.Add(thisDataRow2[x]);
                    thisDataAdapter2.Update(thisDataSet2, "CONSULTATIONDETAILFILE");
                }
                MessageBox.Show("ENTRIES RECORDED.");
                clearAll();

            }

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
                    val10 = true;
                    if (e.KeyCode == Keys.Enter)
                    {
                        preparedByLabel.Text = thisDataReader["EMPLOYEEFIRSTNAME"] + " " + thisDataReader["EMPLOYEELASTNAME"];
                        examinedByTextBox.Focus();
                    }
                }
                else
                {
                    errorProviderMethod(preparedByTextBox, "EMPLOYEE NOT FOUND / UNABLE / NOT AVAILABLE TO DO THE WORK!");
                    val10 = false;
                }
                thisConnection.Close();
            }
            else
            {
                errorProviderMethod(preparedByTextBox, "INVALID INPUT, EMPLOYEE CODE MUST HAVE 5 CHARACTERS!");
                val10 = false;

            }
        }

        private void examinedByTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
                examinedByLabel.Text = "";
            if (examinedByTextBox.TextLength == 5)
            {
                String sql = "SELECT * FROM EMPLOYEEFILE WHERE EMPLOYEEID = '" + examinedByTextBox.Text + "' AND EMPLOYEEPOSITION='DR'";
                OleDbCommand thisCommand = new OleDbCommand(sql, thisConnection);
                thisConnection.Open();
                OleDbDataReader thisDataReader = thisCommand.ExecuteReader();
                if (thisDataReader.Read())
                {
                    correctProviderMethod(examinedByTextBox, "ACCEPTED, RECORD FOUND.");
                    val11 = true;
                    if (e.KeyCode == Keys.Enter)
                    {
                        examinedByLabel.Text = thisDataReader["EMPLOYEEFIRSTNAME"] + " " + thisDataReader["EMPLOYEELASTNAME"];
                        saveButton.Focus();
                    }
                }
                else
                {
                    errorProviderMethod(examinedByTextBox, "EMPLOYEE NOT FOUND / UNABLE / NOT AVAILABLE TO DO THE WORK!");
                    val11 = false;
                }
                thisConnection.Close();
            }
            else
            {
                errorProviderMethod(examinedByTextBox, "INVALID INPUT, EMPLOYEE CODE MUST HAVE 5 CHARACTERS!");
                val11 = false;
            }
        }

        private void diagnosticDataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            val9 = diagnosticDataGridView.Rows.Count > 1;
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
            weightTextBox.Text = "";
            heightTextBox.Text = "";
            bodyTempTextBox.Text = "";
        }
        public void resetControlsErrorProvider()
        {
            resetProvider(consultationNoTextBox);
            resetProvider(immunizationReferralNo);
            resetProvider(patientCodeTextBox);
            resetProvider(weightTextBox);
            resetProvider(heightTextBox);
            resetProvider(bodyTempTextBox);
            resetProvider(diagnosticCodeTextBox);
            resetProvider(physicianNotesTextBox);
            resetProvider(preparedByTextBox);
            resetProvider(examinedByTextBox);

        }

        private void consultationNoTextBox_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
