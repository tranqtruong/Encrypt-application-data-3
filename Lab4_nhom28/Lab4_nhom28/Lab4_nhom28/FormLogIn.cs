using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4_nhom28
{
    public partial class FormLogIn : Form
    {
        public FormLogIn()
        {
            InitializeComponent();
        }

        private void button_dangNhap_Click(object sender, EventArgs e)
        {
            //nút đăng nhập
            string username = textBox_dangNhap.Text.ToUpper();
            string passwd = textBox_Password.Text;
            //mã hóa mật khẩu
            string passwdHash = Cipher.Hash_SHA1(passwd);
            
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec Login '{username}', {passwdHash}";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            SqlDataReader read = cmd.ExecuteReader();
            if (read.Read() == true)
            {
                //Đăng nhập thành công thì hiển thị màn hình quản lý
                this.Hide();
                Home home = new Home(username, passwdHash);
                home.ShowDialog();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không hợp lệ");
            }

            ketnoi.Close();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
