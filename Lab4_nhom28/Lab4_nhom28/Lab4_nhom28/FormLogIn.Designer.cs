
namespace Lab4_nhom28
{
    partial class FormLogIn
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
            this.button2 = new System.Windows.Forms.Button();
            this.button_dangNhap = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.textBox_dangNhap = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(386, 197);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Thoát";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button_dangNhap
            // 
            this.button_dangNhap.Location = new System.Drawing.Point(264, 197);
            this.button_dangNhap.Name = "button_dangNhap";
            this.button_dangNhap.Size = new System.Drawing.Size(92, 23);
            this.button_dangNhap.TabIndex = 10;
            this.button_dangNhap.Text = "Đăng nhập";
            this.button_dangNhap.UseVisualStyleBackColor = true;
            this.button_dangNhap.Click += new System.EventHandler(this.button_dangNhap_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(141, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Mật khẩu";
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(264, 142);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(197, 22);
            this.textBox_Password.TabIndex = 8;
            // 
            // textBox_dangNhap
            // 
            this.textBox_dangNhap.Location = new System.Drawing.Point(264, 96);
            this.textBox_dangNhap.Name = "textBox_dangNhap";
            this.textBox_dangNhap.Size = new System.Drawing.Size(197, 22);
            this.textBox_dangNhap.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(138, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Tên đăng nhập";
            // 
            // FormLogIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 298);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button_dangNhap);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_Password);
            this.Controls.Add(this.textBox_dangNhap);
            this.Controls.Add(this.label1);
            this.Name = "FormLogIn";
            this.Text = "FormLogin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button_dangNhap;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.TextBox textBox_dangNhap;
        private System.Windows.Forms.Label label1;
    }
}

