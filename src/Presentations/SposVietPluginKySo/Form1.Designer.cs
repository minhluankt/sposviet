namespace SposVietPluginKySo
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
            components = new System.ComponentModel.Container();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            thoátToolStripMenuItem = new ToolStripMenuItem();
            mởỨngDụngToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            Companyname = new Label();
            Taxcode = new Label();
            Address = new Label();
            Domain = new Label();
            ComId = new Label();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // notifyIcon1
            // 
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { thoátToolStripMenuItem, mởỨngDụngToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(148, 48);
            // 
            // thoátToolStripMenuItem
            // 
            thoátToolStripMenuItem.Name = "thoátToolStripMenuItem";
            thoátToolStripMenuItem.Size = new Size(147, 22);
            thoátToolStripMenuItem.Text = "Thoát";
            thoátToolStripMenuItem.Click += thoátToolStripMenuItem_Click;
            // 
            // mởỨngDụngToolStripMenuItem
            // 
            mởỨngDụngToolStripMenuItem.Name = "mởỨngDụngToolStripMenuItem";
            mởỨngDụngToolStripMenuItem.Size = new Size(147, 22);
            mởỨngDụngToolStripMenuItem.Text = "Mở ứng dụng";
            mởỨngDụngToolStripMenuItem.Click += mởỨngDụngToolStripMenuItem_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(45, 47);
            label1.Name = "label1";
            label1.Size = new Size(49, 15);
            label1.TabIndex = 1;
            label1.Text = "Công ty";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(45, 82);
            label2.Name = "label2";
            label2.Size = new Size(66, 15);
            label2.TabIndex = 1;
            label2.Text = "Mã số thuế";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(45, 123);
            label3.Name = "label3";
            label3.Size = new Size(43, 15);
            label3.TabIndex = 1;
            label3.Text = "Địa chỉ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(45, 163);
            label4.Name = "label4";
            label4.Size = new Size(80, 15);
            label4.TabIndex = 1;
            label4.Text = "Link hệ thống";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(45, 210);
            label5.Name = "label5";
            label5.Size = new Size(83, 15);
            label5.TabIndex = 1;
            label5.Text = "Mã Định Danh";
            // 
            // Companyname
            // 
            Companyname.AutoSize = true;
            Companyname.Location = new Point(145, 47);
            Companyname.Name = "Companyname";
            Companyname.Size = new Size(49, 15);
            Companyname.TabIndex = 1;
            Companyname.Text = "Công ty";
            // 
            // Taxcode
            // 
            Taxcode.AutoSize = true;
            Taxcode.Location = new Point(145, 82);
            Taxcode.Name = "Taxcode";
            Taxcode.Size = new Size(49, 15);
            Taxcode.TabIndex = 1;
            Taxcode.Text = "Công ty";
            // 
            // Address
            // 
            Address.AutoSize = true;
            Address.Location = new Point(145, 123);
            Address.Name = "Address";
            Address.Size = new Size(49, 15);
            Address.TabIndex = 1;
            Address.Text = "Công ty";
            // 
            // Domain
            // 
            Domain.AutoSize = true;
            Domain.Location = new Point(145, 163);
            Domain.Name = "Domain";
            Domain.Size = new Size(49, 15);
            Domain.TabIndex = 1;
            Domain.Text = "Công ty";
            // 
            // ComId
            // 
            ComId.AutoSize = true;
            ComId.Location = new Point(145, 210);
            ComId.Name = "ComId";
            ComId.Size = new Size(49, 15);
            ComId.TabIndex = 1;
            ComId.Text = "Công ty";
            // 
            // sposvietform
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(749, 293);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(ComId);
            Controls.Add(Domain);
            Controls.Add(Address);
            Controls.Add(Taxcode);
            Controls.Add(Companyname);
            Controls.Add(label1);
            Name = "sposvietform";
            Text = "SposViet - Plugin ký số";
            Load += sposvietform_Load;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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