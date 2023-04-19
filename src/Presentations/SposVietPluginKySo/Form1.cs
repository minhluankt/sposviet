using System;
using System.Drawing;
using System.Windows.Forms;

namespace SposVietPluginKySo
{
    public partial class sposvietform : System.Windows.Forms.Form
    {
        public sposvietform()
        {
            InitializeComponent();
            TrayMenuContext();
            this.ContextMenuStrip = contextMenuStrip1;
            this.Visible = false;
            notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("favicon.ico"));
            notifyIcon1.Text = "SPOSVIET - PLUGIN";
            notifyIcon1.Visible = true;
            notifyIcon1.BalloonTipTitle = "SPOSVIET - PLUGIN";
            notifyIcon1.BalloonTipText = "SPOSVIET - PLUGIN ĐÃ CHẠY";
            notifyIcon1.ShowBalloonTip(200);
        }
        private void TrayMenuContext()
        {
            this.notifyIcon1.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.notifyIcon1.ContextMenuStrip.Items.Add("Thoát ứng dụng",null, this.MenuTest1_Click);
            //this.notifyIcon1.ContextMenuStrip.Items.Add("Mở ứng dụng", null, this.MenuTest2_Click);
            this.notifyIcon1.ContextMenuStrip.ShowImageMargin= false;
        }

        void MenuTest1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void MenuTest2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void MenuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.Visible = false;
        }

        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mởỨngDụngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new sposvietform().Visible = true;
            new sposvietform().Show();
        }

        private void sposvietform_Load(object sender, EventArgs e)
        {

        }
    }
}