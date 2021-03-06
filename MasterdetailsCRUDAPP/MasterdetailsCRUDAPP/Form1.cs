using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace MasterdetailsCRUDAPP
{
    public partial class Form1 : Form
    {
        int inEmpID = 0;
        bool isDefaultImage = true;
        String strconnectionString = @"Data Source=DESKTOP-T5PG0RU\SQLEXPRESS; Initial Catalog=MasterDetailsDB;Integrated Security=True;",strPreviousImage="";
        OpenFileDialog ofd = new OpenFileDialog();
        public Form1()
        
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dgvEmpCompany_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Clear();
        }
        void Clear()
        {
            txtEmpCode.Text = txtEmpName.Text = "";
            cmbPosition.SelectedIndex = cmbGender.SelectedIndex = 0;
            dtpDOB.Value = DateTime.Now;
            rbtRegular.Checked = true;
            if (dgvEmpCompany.DataSource == null)
                dgvEmpCompany.Rows.Clear();
            else
                dgvEmpCompany.DataSource = (dgvEmpCompany.DataSource as DataTable).Clone();
            inEmpID = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            pbxPhoto.Image = Image.FromFile(Application.StartupPath + "\\Image\\defaultimage.png");
            isDefaultImage = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PositionComboBoxFill();
            FillEmplooyeDataGridView();
            Clear();
        }

        private void btnImageBrowse_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Image(.jpg,.png)|*.png;*.jpg";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                pbxPhoto.Image = new Bitmap(ofd.FileName);
                isDefaultImage = false;
                strPreviousImage = "";
            }
        }

        private void btnImageClear_Click(object sender, EventArgs e)
        {
            pbxPhoto.Image = new Bitmap(Application.StartupPath + "\\Image\\defaultImage.png");
            isDefaultImage = true;
            strPreviousImage = "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(ValidateMasterDetailForm())
            {
                int _EmpID=0;
                using (SqlConnection sqlCon = new SqlConnection(strconnectionString))
                {
                    sqlCon.Open();
                    //Master
                    SqlCommand sqlCmd = new SqlCommand("EmplooyeAddOrEdit", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@EmpID", inEmpID);
                    sqlCmd.Parameters.AddWithValue("@EmpCode", txtEmpCode.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@EmpName", txtEmpName.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@PositionID", Convert.ToInt32(cmbPosition.SelectedValue));
                    sqlCmd.Parameters.AddWithValue("@DOB", dtpDOB.Value);
                    sqlCmd.Parameters.AddWithValue("@Gender", cmbGender.Text);
                    sqlCmd.Parameters.AddWithValue("@State", rbtRegular.Checked ? "Regular":"Contractual");
                    if (isDefaultImage)
                        sqlCmd.Parameters.AddWithValue("ImagePath", DBNull.Value);
                    else if (inEmpID > 0 && strPreviousImage != "")
                        sqlCmd.Parameters.AddWithValue("@ImagePath", strPreviousImage);
                    else
                        sqlCmd.Parameters.AddWithValue("ImagePath", SaveImage(ofd.FileName));
                    _EmpID = Convert.ToInt32(sqlCmd.ExecuteScalar());
                }

                using (SqlConnection sqlCon = new SqlConnection(strconnectionString))
                {
                    sqlCon.Open();
                    foreach (DataGridViewRow dgvRow in dgvEmpCompany.Rows)
                    {
                        if (dgvRow.IsNewRow) break;
                        else
                        {
                            SqlCommand sqlCmd = new SqlCommand("EmplooyeAddOrEdit", sqlCon);
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@EmpCmpID", Convert.ToInt32(dgvRow.Cells["dgvtxtEmpCompID"].Value == DBNull.Value ? "0" : dgvRow.Cells
                                ["dgvtxtEmpCompID"].Value));

                            sqlCmd.Parameters.AddWithValue("@EmpID", _EmpID);
                            sqlCmd.Parameters.AddWithValue("@CompanyName", dgvRow.Cells["dgvtxtCompanyName"].Value == DBNull.Value ? "" : dgvRow.Cells
                                ["dgvtxtCompanyName"].Value);
                            sqlCmd.Parameters.AddWithValue("@PositionID", Convert.ToInt32(dgvRow.Cells["dgvcmbPosition"].Value == DBNull.Value ? "0" : dgvRow.Cells
                                ["dgvcmbPosition"].Value));
                            sqlCmd.Parameters.AddWithValue("@ExpYear", Convert.ToInt32(dgvRow.Cells["dgvtxtYear"].Value == DBNull.Value ? "0" : dgvRow.Cells
                                ["dgvtxtYear"].Value));
                            sqlCmd.ExecuteNonQuery();

                        }
                    }
                }
                FillEmplooyeDataGridView();
                Clear();
                MessageBox.Show("Submitted Sucessfully");
            }

        }

        bool ValidateMasterDetailForm()
        {
            bool _isValid=true;
            if (txtEmpName.Text.Trim() =="")
            {
                MessageBox.Show("Emplooyee Name is Required");
                _isValid = false;

            }
            return _isValid;
        }

        String SaveImage(string _imagePath)
        {
            string _fileName = Path.GetFileNameWithoutExtension(_imagePath);
            string _extension = Path.GetExtension(_imagePath);
            //
            _fileName = _fileName.Length <= 15 ? _fileName : _fileName.Substring(0, 15);
            _fileName = _fileName + DateTime.Now.ToString("yymmssfff") + _extension;
            pbxPhoto.Image.Save(Application.StartupPath + "\\Images\\" + _fileName);
            return _fileName;
        }

        void FillEmplooyeDataGridView()
        {
            using (SqlConnection sqlCon = new SqlConnection(strconnectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("EmployeViewAll", sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtb1 = new DataTable();
                sqlDa.Fill(dtb1);
                dgvEmployee.DataSource = dtb1;
                dgvEmployee.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvEmployee.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvEmployee.Columns[0].Visible = false;
            }
        }
            

        void PositionComboBoxFill()
        {
            using (SqlConnection sqlCon = new SqlConnection(strconnectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM Position", sqlCon);
                DataTable dtb1 = new DataTable();
                sqlDa.Fill(dtb1);
                DataRow topIteam = dtb1.NewRow();
                topIteam[0] = 0;
                topIteam[1] = "-Select-";
                dtb1.Rows.InsertAt(topIteam, 0);
             
                cmbPosition.ValueMember = dgvcmbPosition.ValueMember = "PositionID";
                cmbPosition.DisplayMember = dgvcmbPosition.DisplayMember = "Position";
                cmbPosition.DataSource = dtb1;
                dgvcmbPosition.DataSource = dtb1.Copy();

            }
        }
    }
}
