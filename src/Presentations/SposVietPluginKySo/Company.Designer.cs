namespace SposVietPluginKySo
{
    partial class Company
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Company));
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            txtComid = new TextBox();
            txtCompany = new TextBox();
            txtAddress = new TextBox();
            txtTaxcode = new TextBox();
            txtDomain = new TextBox();
            btnSaveCompany = new Button();
            btnlogout = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(38, 28);
            label1.Name = "label1";
            label1.Size = new Size(81, 15);
            label1.TabIndex = 0;
            label1.Text = "Mã định danh";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(38, 67);
            label2.Name = "label2";
            label2.Size = new Size(68, 15);
            label2.TabIndex = 0;
            label2.Text = "Tên công ty";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(38, 106);
            label3.Name = "label3";
            label3.Size = new Size(43, 15);
            label3.TabIndex = 0;
            label3.Text = "Địa chỉ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(38, 142);
            label4.Name = "label4";
            label4.Size = new Size(66, 15);
            label4.TabIndex = 0;
            label4.Text = "Mã số thuế";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(38, 184);
            label5.Name = "label5";
            label5.Size = new Size(124, 15);
            label5.TabIndex = 0;
            label5.Text = "Link domain hệ thống";
            // 
            // txtComid
            // 
            txtComid.Location = new Point(166, 20);
            txtComid.Name = "txtComid";
            txtComid.Size = new Size(302, 23);
            txtComid.TabIndex = 1;
            // 
            // txtCompany
            // 
            txtCompany.Location = new Point(166, 59);
            txtCompany.Name = "txtCompany";
            txtCompany.Size = new Size(302, 23);
            txtCompany.TabIndex = 1;
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(166, 98);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(302, 23);
            txtAddress.TabIndex = 1;
            // 
            // txtTaxcode
            // 
            txtTaxcode.Location = new Point(166, 134);
            txtTaxcode.Name = "txtTaxcode";
            txtTaxcode.Size = new Size(302, 23);
            txtTaxcode.TabIndex = 1;
            // 
            // txtDomain
            // 
            txtDomain.Location = new Point(166, 176);
            txtDomain.Name = "txtDomain";
            txtDomain.Size = new Size(302, 23);
            txtDomain.TabIndex = 1;
            // 
            // btnSaveCompany
            // 
            btnSaveCompany.Location = new Point(283, 236);
            btnSaveCompany.Name = "btnSaveCompany";
            btnSaveCompany.Size = new Size(166, 46);
            btnSaveCompany.TabIndex = 2;
            btnSaveCompany.Text = "Lưu cấu hình";
            btnSaveCompany.UseVisualStyleBackColor = true;
            btnSaveCompany.Click += btnSaveCompany_Click;
            // 
            // btnlogout
            // 
            btnlogout.Location = new Point(64, 236);
            btnlogout.Name = "btnlogout";
            btnlogout.Size = new Size(145, 46);
            btnlogout.TabIndex = 3;
            btnlogout.Text = "Đăng xuất";
            btnlogout.UseVisualStyleBackColor = true;
            btnlogout.Click += btnlogout_Click;
            // 
            // Company
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(504, 294);
            Controls.Add(btnlogout);
            Controls.Add(btnSaveCompany);
            Controls.Add(txtDomain);
            Controls.Add(txtTaxcode);
            Controls.Add(txtAddress);
            Controls.Add(txtCompany);
            Controls.Add(txtComid);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Company";
            Text = "Cấu hình thông tin công ty";
            Load += Company_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private TextBox txtComid;
        private TextBox txtCompany;
        private TextBox txtAddress;
        private TextBox txtTaxcode;
        private TextBox txtDomain;
        private Button btnSaveCompany;
        private Button btnlogout;
    }
}