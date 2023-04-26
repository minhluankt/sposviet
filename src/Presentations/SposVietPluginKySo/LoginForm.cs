using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SposVietPluginKySo.Helper;
using SposVietPluginKySo.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.util;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace SposVietPluginKySo
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string user = (string)txtUsername.Text;
                string pass = (string)txtPassword.Text;
                var data = new
                {
                    email = user,
                    password = pass,
                    isOwner=true
                };
                var getapi = await CommonApi.PostApiAsync("identity/login", JsonConvert.SerializeObject(data));
                var model = JsonConvert.DeserializeObject<ApiResponse<UserModel>>(getapi);
                if (!model.IsError)
                {
                  
                    Dictionary<string, string> AuthorList = new Dictionary<string, string>();
                    AuthorList.Add("Authorization", "Bearer "+ model.result.jwToken);

                    var getcompany = await CommonApi.GetApiAsync($"Company/search?Id={model.result.comId}", AuthorList);
                    var modelcompany = JsonConvert.DeserializeObject<ApiResponse<CompanyModel>>(getcompany);
                    if (!modelcompany.IsError)
                    {
                        Properties.Settings.Default.ComId = modelcompany.result.Id;
                        Properties.Settings.Default.Company = modelcompany.result.Name;
                        Properties.Settings.Default.Address = modelcompany.result.Address;
                        Properties.Settings.Default.MST = modelcompany.result.Taxcode;
                        Properties.Settings.Default.Domain = modelcompany.result.Domain;
                        Properties.Settings.Default.Save();
                        this.Hide();
                        Company myNewForm = new Company();
                        myNewForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi xảy ra: " + modelcompany.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra: " + model.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




    }
}
