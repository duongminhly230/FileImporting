
namespace TNH_DATA_IMPORT
{
    partial class frmmain
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
            this.components = new System.ComponentModel.Container();
            this.btn_ccbs = new System.Windows.Forms.Button();
            this.txtmk = new System.Windows.Forms.TextBox();
            this.txtusername = new System.Windows.Forms.TextBox();
            this.btn_capnhatmk_CCBS = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_thang = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_trangthai = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.maintab = new System.Windows.Forms.TabControl();
            this.tb_ccbs_home = new System.Windows.Forms.TabPage();
            this.tb_dkg_moi = new System.Windows.Forms.TabPage();
            this.tb_dkg_huy = new System.Windows.Forms.TabPage();
            this.tb_ptm = new System.Windows.Forms.TabPage();
            this.tb_kh = new System.Windows.Forms.TabPage();
            this.tb_home_ptm = new System.Windows.Forms.TabPage();
            this.tb_home_huy = new System.Windows.Forms.TabPage();
            this.tb_ctv_xhh = new System.Windows.Forms.TabPage();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tb_ggms = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.maintab.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_ccbs
            // 
            this.btn_ccbs.Location = new System.Drawing.Point(250, 35);
            this.btn_ccbs.Name = "btn_ccbs";
            this.btn_ccbs.Size = new System.Drawing.Size(107, 32);
            this.btn_ccbs.TabIndex = 0;
            this.btn_ccbs.Text = "Chạy UI";
            this.btn_ccbs.UseVisualStyleBackColor = true;
            this.btn_ccbs.Click += new System.EventHandler(this.btn_ccbs_Click);
            // 
            // txtmk
            // 
            this.txtmk.Location = new System.Drawing.Point(91, 78);
            this.txtmk.Name = "txtmk";
            this.txtmk.Size = new System.Drawing.Size(153, 22);
            this.txtmk.TabIndex = 1;
            this.txtmk.Text = "123456";
            // 
            // txtusername
            // 
            this.txtusername.Location = new System.Drawing.Point(91, 50);
            this.txtusername.Name = "txtusername";
            this.txtusername.Size = new System.Drawing.Size(153, 22);
            this.txtusername.TabIndex = 2;
            this.txtusername.Text = "minhly_tnh";
            // 
            // btn_capnhatmk_CCBS
            // 
            this.btn_capnhatmk_CCBS.Location = new System.Drawing.Point(250, 73);
            this.btn_capnhatmk_CCBS.Name = "btn_capnhatmk_CCBS";
            this.btn_capnhatmk_CCBS.Size = new System.Drawing.Size(107, 32);
            this.btn_capnhatmk_CCBS.TabIndex = 3;
            this.btn_capnhatmk_CCBS.Text = "Cập nhật MK";
            this.btn_capnhatmk_CCBS.UseVisualStyleBackColor = true;
            this.btn_capnhatmk_CCBS.Click += new System.EventHandler(this.btn_capnhatmk_CCBS_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Tháng:";
            // 
            // txt_thang
            // 
            this.txt_thang.Location = new System.Drawing.Point(91, 21);
            this.txt_thang.Name = "txt_thang";
            this.txt_thang.Size = new System.Drawing.Size(100, 22);
            this.txt_thang.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 19);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_trangthai);
            this.groupBox1.Controls.Add(this.txtmk);
            this.groupBox1.Controls.Add(this.txt_thang);
            this.groupBox1.Controls.Add(this.btn_ccbs);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtusername);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btn_capnhatmk_CCBS);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 143);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CCBS";
            // 
            // lbl_trangthai
            // 
            this.lbl_trangthai.AutoSize = true;
            this.lbl_trangthai.Location = new System.Drawing.Point(16, 114);
            this.lbl_trangthai.Name = "lbl_trangthai";
            this.lbl_trangthai.Size = new System.Drawing.Size(62, 15);
            this.lbl_trangthai.TabIndex = 7;
            this.lbl_trangthai.Text = "Trạng thái";
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(12, 158);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(363, 514);
            this.listBox1.TabIndex = 7;
            // 
            // maintab
            // 
            this.maintab.Controls.Add(this.tb_ggms);
            this.maintab.Controls.Add(this.tb_ccbs_home);
            this.maintab.Controls.Add(this.tb_dkg_moi);
            this.maintab.Controls.Add(this.tb_dkg_huy);
            this.maintab.Controls.Add(this.tb_ptm);
            this.maintab.Controls.Add(this.tb_kh);
            this.maintab.Controls.Add(this.tb_home_ptm);
            this.maintab.Controls.Add(this.tb_home_huy);
            this.maintab.Controls.Add(this.tb_ctv_xhh);
            this.maintab.Location = new System.Drawing.Point(381, 12);
            this.maintab.Name = "maintab";
            this.maintab.SelectedIndex = 0;
            this.maintab.Size = new System.Drawing.Size(1035, 660);
            this.maintab.TabIndex = 8;
            // 
            // tb_ccbs_home
            // 
            this.tb_ccbs_home.Location = new System.Drawing.Point(4, 22);
            this.tb_ccbs_home.Name = "tb_ccbs_home";
            this.tb_ccbs_home.Padding = new System.Windows.Forms.Padding(3);
            this.tb_ccbs_home.Size = new System.Drawing.Size(1027, 634);
            this.tb_ccbs_home.TabIndex = 0;
            this.tb_ccbs_home.Text = "CCBS Home";
            this.tb_ccbs_home.UseVisualStyleBackColor = true;
            // 
            // tb_dkg_moi
            // 
            this.tb_dkg_moi.Location = new System.Drawing.Point(4, 22);
            this.tb_dkg_moi.Name = "tb_dkg_moi";
            this.tb_dkg_moi.Size = new System.Drawing.Size(1027, 634);
            this.tb_dkg_moi.TabIndex = 1;
            this.tb_dkg_moi.Text = "1_DKM_GOICUOC";
            this.tb_dkg_moi.UseVisualStyleBackColor = true;
            // 
            // tb_dkg_huy
            // 
            this.tb_dkg_huy.Location = new System.Drawing.Point(4, 22);
            this.tb_dkg_huy.Name = "tb_dkg_huy";
            this.tb_dkg_huy.Size = new System.Drawing.Size(1027, 634);
            this.tb_dkg_huy.TabIndex = 2;
            this.tb_dkg_huy.Text = "2_HUY_GOICUOC";
            this.tb_dkg_huy.UseVisualStyleBackColor = true;
            // 
            // tb_ptm
            // 
            this.tb_ptm.Location = new System.Drawing.Point(4, 22);
            this.tb_ptm.Name = "tb_ptm";
            this.tb_ptm.Size = new System.Drawing.Size(1027, 634);
            this.tb_ptm.TabIndex = 3;
            this.tb_ptm.Text = "3_PTM";
            this.tb_ptm.UseVisualStyleBackColor = true;
            // 
            // tb_kh
            // 
            this.tb_kh.Location = new System.Drawing.Point(4, 22);
            this.tb_kh.Name = "tb_kh";
            this.tb_kh.Size = new System.Drawing.Size(1027, 634);
            this.tb_kh.TabIndex = 4;
            this.tb_kh.Text = "4_DDTT_KH";
            this.tb_kh.UseVisualStyleBackColor = true;
            // 
            // tb_home_ptm
            // 
            this.tb_home_ptm.Location = new System.Drawing.Point(4, 22);
            this.tb_home_ptm.Name = "tb_home_ptm";
            this.tb_home_ptm.Size = new System.Drawing.Size(1027, 634);
            this.tb_home_ptm.TabIndex = 5;
            this.tb_home_ptm.Text = "5_HOME_HUY";
            this.tb_home_ptm.UseVisualStyleBackColor = true;
            // 
            // tb_home_huy
            // 
            this.tb_home_huy.Location = new System.Drawing.Point(4, 22);
            this.tb_home_huy.Name = "tb_home_huy";
            this.tb_home_huy.Size = new System.Drawing.Size(1027, 634);
            this.tb_home_huy.TabIndex = 6;
            this.tb_home_huy.Text = "5_HOME_PTM";
            this.tb_home_huy.UseVisualStyleBackColor = true;
            // 
            // tb_ctv_xhh
            // 
            this.tb_ctv_xhh.Location = new System.Drawing.Point(4, 22);
            this.tb_ctv_xhh.Name = "tb_ctv_xhh";
            this.tb_ctv_xhh.Size = new System.Drawing.Size(1027, 634);
            this.tb_ctv_xhh.TabIndex = 7;
            this.tb_ctv_xhh.Text = "CTV XHH";
            this.tb_ctv_xhh.UseVisualStyleBackColor = true;
            // 
            // tb_ggms
            // 
            this.tb_ggms.Location = new System.Drawing.Point(4, 22);
            this.tb_ggms.Name = "tb_ggms";
            this.tb_ggms.Size = new System.Drawing.Size(1027, 634);
            this.tb_ggms.TabIndex = 8;
            this.tb_ggms.Text = "GG MS";
            this.tb_ggms.UseVisualStyleBackColor = true;
            // 
            // frmmain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 686);
            this.Controls.Add(this.maintab);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmmain";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.frmmain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.maintab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_ccbs;
        private System.Windows.Forms.TextBox txtmk;
        private System.Windows.Forms.TextBox txtusername;
        private System.Windows.Forms.Button btn_capnhatmk_CCBS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_thang;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbl_trangthai;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabControl maintab;
        private System.Windows.Forms.TabPage tb_ccbs_home;
        private System.Windows.Forms.TabPage tb_dkg_moi;
        private System.Windows.Forms.TabPage tb_dkg_huy;
        private System.Windows.Forms.TabPage tb_ptm;
        private System.Windows.Forms.TabPage tb_kh;
        private System.Windows.Forms.TabPage tb_home_ptm;
        private System.Windows.Forms.TabPage tb_home_huy;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tb_ctv_xhh;
        private System.Windows.Forms.TabPage tb_ggms;
    }
}

