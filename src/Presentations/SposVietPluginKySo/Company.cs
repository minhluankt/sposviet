using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
namespace SposVietPluginKySo
{
    public partial class Company : Form
    {
        public Company()
        {
            InitializeComponent();
        }

        private void btnSaveCompany_Click(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.ComId = int.Parse(txtComid.Text.Trim());
                Properties.Settings.Default.Company = txtCompany.Text.Trim();
                Properties.Settings.Default.MST = txtTaxcode.Text.Trim();
                Properties.Settings.Default.Address = txtAddress.Text.Trim();
                Properties.Settings.Default.Domain = txtDomain.Text.Trim();
                Properties.Settings.Default.Save();
                MessageBox.Show("Lưu thành công");
            }
            catch (Exception ex)
            {
                LogControl.Write("Lưu cấu hình faile");
                LogControl.Write(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void Company_Load(object sender, EventArgs e)
        {
            txtComid.Text= Properties.Settings.Default.ComId.ToString();
            txtCompany.Text= Properties.Settings.Default.Company;
            txtTaxcode.Text=Properties.Settings.Default.MST;
            txtAddress.Text =Properties.Settings.Default.Address;
            txtDomain.Text=Properties.Settings.Default.Domain;
        }
    }
}
