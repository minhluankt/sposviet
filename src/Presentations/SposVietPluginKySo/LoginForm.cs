using Newtonsoft.Json;
using SposVietPluginKySo.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                };
                var getapi = await CommonApi.GetApiAsync("identity/login", JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {

                
            }
        }
    }
}
