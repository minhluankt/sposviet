using System.Windows.Forms;

namespace SposVietPlugin_net_4._6._1
{
    partial class sposvietform
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.thoátToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mởỨngDụngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Companyname = new System.Windows.Forms.Label();
            this.Taxcode = new System.Windows.Forms.Label();
            this.Address = new System.Windows.Forms.Label();
            this.Domain = new System.Windows.Forms.Label();
            this.ComId = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.thoátToolStripMenuItem,
            this.mởỨngDụngToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(148, 48);
            // 
            // thoátToolStripMenuItem
            // 
            this.thoátToolStripMenuItem.Name = "thoátToolStripMenuItem";
            this.thoátToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.thoátToolStripMenuItem.Text = "Thoát";
            // 
            // mởỨngDụngToolStripMenuItem
            // 
            this.mởỨngDụngToolStripMenuItem.Name = "mởỨngDụngToolStripMenuItem";
            this.mởỨngDụngToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.mởỨngDụngToolStripMenuItem.Text = "Mở ứng dụng";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Công ty";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Mã số thuế";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(45, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Địa chỉ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Link hệ thống";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(45, 210);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "Mã Định Danh";
            // 
            // Companyname
            // 
            this.Companyname.AutoSize = true;
            this.Companyname.Location = new System.Drawing.Point(145, 47);
            this.Companyname.Name = "Companyname";
            this.Companyname.Size = new System.Drawing.Size(49, 15);
            this.Companyname.TabIndex = 1;
            this.Companyname.Text = "Công ty";
            // 
            // Taxcode
            // 
            this.Taxcode.AutoSize = true;
            this.Taxcode.Location = new System.Drawing.Point(145, 82);
            this.Taxcode.Name = "Taxcode";
            this.Taxcode.Size = new System.Drawing.Size(49, 15);
            this.Taxcode.TabIndex = 1;
            this.Taxcode.Text = "Công ty";
            // 
            // Address
            // 
            this.Address.AutoSize = true;
            this.Address.Location = new System.Drawing.Point(145, 123);
            this.Address.Name = "Address";
            this.Address.Size = new System.Drawing.Size(49, 15);
            this.Address.TabIndex = 1;
            this.Address.Text = "Công ty";
            // 
            // Domain
            // 
            this.Domain.AutoSize = true;
            this.Domain.Location = new System.Drawing.Point(145, 163);
            this.Domain.Name = "Domain";
            this.Domain.Size = new System.Drawing.Size(49, 15);
            this.Domain.TabIndex = 1;
            this.Domain.Text = "Công ty";
            // 
            // ComId
            // 
            this.ComId.AutoSize = true;
            this.ComId.Location = new System.Drawing.Point(145, 210);
            this.ComId.Name = "ComId";
            this.ComId.Size = new System.Drawing.Size(49, 15);
            this.ComId.TabIndex = 1;
            this.ComId.Text = "Công ty";
            // 
            // sposvietform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 293);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ComId);
            this.Controls.Add(this.Domain);
            this.Controls.Add(this.Address);
            this.Controls.Add(this.Taxcode);
            this.Controls.Add(this.Companyname);
            this.Controls.Add(this.label1);
            this.Name = "sposvietform";
            this.Text = "SposViet - Plugin ký số";
            this.Load += new System.EventHandler(this.sposvietform_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem thoátToolStripMenuItem;
        private ToolStripMenuItem mởỨngDụngToolStripMenuItem;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label Companyname;
        private Label Taxcode;
        private Label Address;
        private Label Domain;
        private Label ComId;
    }
}