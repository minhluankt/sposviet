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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtComid = new System.Windows.Forms.TextBox();
            this.txtCompany = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtTaxcode = new System.Windows.Forms.TextBox();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.btnSaveCompany = new System.Windows.Forms.Button();
            this.btnlogout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mã định danh";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Tên công ty";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Địa chỉ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Mã số thuế";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 184);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "Link domain hệ thống";
            // 
            // txtComid
            // 
            this.txtComid.Location = new System.Drawing.Point(166, 20);
            this.txtComid.Name = "txtComid";
            this.txtComid.Size = new System.Drawing.Size(302, 23);
            this.txtComid.TabIndex = 1;
            // 
            // txtCompany
            // 
            this.txtCompany.Location = new System.Drawing.Point(166, 59);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.Size = new System.Drawing.Size(302, 23);
            this.txtCompany.TabIndex = 1;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(166, 98);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(302, 23);
            this.txtAddress.TabIndex = 1;
            // 
            // txtTaxcode
            // 
            this.txtTaxcode.Location = new System.Drawing.Point(166, 134);
            this.txtTaxcode.Name = "txtTaxcode";
            this.txtTaxcode.Size = new System.Drawing.Size(302, 23);
            this.txtTaxcode.TabIndex = 1;
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(166, 176);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(302, 23);
            this.txtDomain.TabIndex = 1;
            // 
            // btnSaveCompany
            // 
            this.btnSaveCompany.Location = new System.Drawing.Point(283, 236);
            this.btnSaveCompany.Name = "btnSaveCompany";
            this.btnSaveCompany.Size = new System.Drawing.Size(166, 46);
            this.btnSaveCompany.TabIndex = 2;
            this.btnSaveCompany.Text = "Lưu cấu hình";
            this.btnSaveCompany.UseVisualStyleBackColor = true;
            this.btnSaveCompany.Click += new System.EventHandler(this.btnSaveCompany_Click);
            // 
            // btnlogout
            // 
            this.btnlogout.Location = new System.Drawing.Point(64, 236);
            this.btnlogout.Name = "btnlogout";
            this.btnlogout.Size = new System.Drawing.Size(145, 46);
            this.btnlogout.TabIndex = 3;
            this.btnlogout.Text = "Đăng xuất";
            this.btnlogout.UseVisualStyleBackColor = true;
            this.btnlogout.Click += new System.EventHandler(this.btnlogout_Click);
            // 
            // Company
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 294);
            this.Controls.Add(this.btnlogout);
            this.Controls.Add(this.btnSaveCompany);
            this.Controls.Add(this.txtDomain);
            this.Controls.Add(this.txtTaxcode);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.txtCompany);
            this.Controls.Add(this.txtComid);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Company";
            this.Text = "Cấu hình thông tin công ty";
            this.Load += new System.EventHandler(this.Company_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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