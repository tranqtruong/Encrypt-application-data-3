using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab4_nhom28
{
    public partial class Home : Form
    {
        static Dictionary<string, string> listPublicKey = new Dictionary<string, string>();
        static Dictionary<string, string> listPrivateKey = new Dictionary<string, string>();
        

        UnicodeEncoding ByteConverter = new UnicodeEncoding();

        string MANV_LOGIN;
        string PUBKEYNAME;

        Dictionary<string, string> whoEncryptedDiem = new Dictionary<string, string>();

        public Home()
        {
            InitializeComponent();

        }

        public Home(string username, string password)
        {
            InitializeComponent();
            loadUserInfo(username, password);
            refresh();
        }

        private void refresh()
        {
            loadKey();
            loadFileLog();
            loadTableNhanVien();
            loadLopHoc();
            loadComboBoxMANV();
            loadAllSV();
            loadComboBoxMaLop();
            loadComboBoxChonLop();
            loadHocPhan();
            loadComboBoxHOCPHAN();
            loadBangDiem();


        }

        private void loadKey()
        {
            listPublicKey = FileClass.ReadFile("listPublicKey.bin");
            listPrivateKey = FileClass.ReadFile("listPrivateKey.bin");
        }

        private void updateFileKey()
        {
            FileClass.WriteFile(listPublicKey, "listPublicKey.bin");
            FileClass.WriteFile(listPrivateKey, "listPrivateKey.bin");
        }

        private void loadUserInfo(string username, string password)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec SP_SEL_PUBLIC_ENCRYPT_NHANVIEN '{username}', {password}";
            SqlCommand scmd = new SqlCommand(sql, ketnoi);
            SqlDataReader sdr = scmd.ExecuteReader();
            string manv = "";
            string hoTen = "";
            string email = "";
            string pubKey = "";

            while (sdr.Read())
            {
                manv = sdr.GetString(0);
                hoTen = sdr.GetString(1);
                email = sdr.GetString(2);
                pubKey = sdr.GetString(4);
            }
            textBox_manv.Text = manv;
            textBox_hoten.Text = hoTen;
            textBox_email.Text = email;
            MANV_LOGIN = manv;
            PUBKEYNAME = pubKey;

            ketnoi.Close();
        }

        private void loadTableNhanVien()
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = @"exec LOAD_NHANVIEN";

            SqlCommand scmd = new SqlCommand(sql, ketnoi);
            SqlDataReader sdr = scmd.ExecuteReader();

            DataTable dt = new DataTable();

            for (int i = 0; i < table_NhanVien.ColumnCount; ++i)
            {
                dt.Columns.Add(new DataColumn(table_NhanVien.Columns[i].Name));
                table_NhanVien.Columns[i].DataPropertyName = table_NhanVien.Columns[i].Name;
            }

            while (sdr.Read())
            {
                string manv = sdr.GetString(0);//MaNV
                string hoTen = sdr.GetString(1);//HoTen
                string email = sdr.GetString(2);//Email
                byte[] luongEncrypted = (byte[])sdr.GetValue(3);//Luong

                //giải mã lương nhân viên
                RSAParameters PRIVATEKEY = Cipher.StringToKey(listPrivateKey[manv]);
                byte[] LuongDecrypted = Cipher.RSADecrypt(luongEncrypted, PRIVATEKEY, false);
                string luong = ByteConverter.GetString(LuongDecrypted);

                dt.Rows.Add(manv, hoTen, email, luong);
            }
            //table_NhanVien.Columns.Clear();
            table_NhanVien.DataSource = dt;


            ketnoi.Close();
        }

        


        private void themNhanVien(string maNv, string hoTen, string email, string luong, string tenDn, string matKhau, string pubKeyName)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"EXEC SP_INS_PUBLIC_ENCRYPT_NHANVIEN '{maNv}', '{hoTen}', '{email}', {luong}, '{tenDn}', {matKhau}, '{pubKeyName}'";
            SqlCommand scmd = new SqlCommand(sql, ketnoi);
            scmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void suaNhanVien(string maNv, string hoTen, string email, string luong, string tenDn, string matKhau)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec UPDATE_NHANVIEN '{maNv}', '{hoTen}', '{email}', {luong}, '{tenDn}', {matKhau}";
            SqlCommand scmd = new SqlCommand(sql, ketnoi);
            scmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void xoaNhanVien(string MANV)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec DELETE_NHANVIEN '{MANV}'";
            SqlCommand scmd = new SqlCommand(sql, ketnoi);
            scmd.ExecuteNonQuery();
            ketnoi.Close();
        }



        private void button_update_Click(object sender, EventArgs e)
        {
            //sửa 1 nhân viên
            string maNv = this.textBox1_manv.Text;
            string hoTen = this.textBox33_hoten.Text;
            string email = this.textBox22_email.Text;
            string luong = textBox_luong.Text;
            string tenDn = textBox_tenDangNhap.Text;
            string matKhau = textBox_matkhau.Text;
            string pubKeyName = "PubKey" + maNv;
            //mã hóa input
            string passEncrypted = Cipher.Hash_SHA1(matKhau);
            byte[] luongToEncrypt = ByteConverter.GetBytes(luong);
            //lấy public key
            RSAParameters PUBLICKEY = Cipher.StringToKey(listPublicKey[pubKeyName]);
            //mã hóa lương RSA
            byte[] encryptedLuong = Cipher.RSAEncrypt(luongToEncrypt, PUBLICKEY, false);
            string encryptedLuongString = "0x" + Cipher.ByteArrayToString(encryptedLuong);
            //sửa thông tin nhân viên
            suaNhanVien(maNv, hoTen, email, encryptedLuongString, tenDn, passEncrypted);

            refresh();

        }

        private void button_insert_Click(object sender, EventArgs e)
        {
            //thêm nhân viên
            //lay input
            string maNv = this.textBox1_manv.Text;
            string hoTen = this.textBox33_hoten.Text;
            string email = this.textBox22_email.Text;
            string luong = textBox_luong.Text;
            string tenDn = textBox_tenDangNhap.Text;
            string matKhau = textBox_matkhau.Text;
            string pubKeyName = "PubKey" + maNv;
            //mã hóa input
            string passEncrypted = Cipher.Hash_SHA1(matKhau);
            byte[] luongToEncrypt = ByteConverter.GetBytes(luong);
            //tạo cặp khóa 
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(512);
            RSAParameters publicKey = RSA.ExportParameters(false);
            RSAParameters privateKey = RSA.ExportParameters(true);
            string pubKeyString = Cipher.KeyToString(publicKey);
            string priKeyString = Cipher.KeyToString(privateKey);
            //luu public key, private key vao dictionary
            listPublicKey.Add(pubKeyName, pubKeyString);
            listPrivateKey.Add(maNv, priKeyString);
            //lưu dictionary vào file
            updateFileKey();
            loadKey();
            //mã hóa lương RSA
            byte[] encryptedLuong = Cipher.RSAEncrypt(luongToEncrypt, publicKey, false);
            string encryptedLuongString = "0x" + Cipher.ByteArrayToString(encryptedLuong);
            //insert nhanvien to database
            themNhanVien(maNv, hoTen, email, encryptedLuongString, tenDn, passEncrypted, pubKeyName);
            //loadTableNhanVien();
            refresh();
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            string maNv = this.textBox1_manv.Text;
            string pubKeyName = "PubKey" + maNv;
            //xóa nhân viên khỏi DB
            xoaNhanVien(maNv);

            //xóa key của nhân viên đó
            listPrivateKey.Remove(maNv);
            listPublicKey.Remove(pubKeyName);
            updateFileKey();
            loadKey();
            refresh();
        }


        /*----------------------------------------------------------------------------------------------------*/


        private void loadLopHoc()
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = @"exec LOAD_LOPHOC";

            SqlCommand scmd = new SqlCommand(sql, ketnoi);
            SqlDataReader sdr = scmd.ExecuteReader();

            DataTable dt = new DataTable();

            for (int i = 0; i < table_LOPHOC.ColumnCount; ++i)
            {
                dt.Columns.Add(new DataColumn(table_LOPHOC.Columns[i].Name));
                table_LOPHOC.Columns[i].DataPropertyName = table_LOPHOC.Columns[i].Name;
            }

            while (sdr.Read())
            {
                string maLop = sdr.GetString(0);
                string tenLop = sdr.GetString(1);
                string maNv = sdr.GetString(2);

                dt.Rows.Add(maLop, tenLop, maNv);
            }
            table_LOPHOC.DataSource = dt;

            ketnoi.Close();
        }


        private void loadComboBoxMANV()
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = @"select MANV from NHANVIEN";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            SqlDataReader rd = cmd.ExecuteReader();
            comboBox_maNV.Items.Clear();
            while (rd.Read())
            {
                string maNV = rd.GetString(0);
                comboBox_maNV.Items.Add(maNV);
            }

            ketnoi.Close();
        }

        private void themLopHoc(string MALOP, string TENLOP, string MANV)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec INSERT_LOPHOC '{MALOP}', '{TENLOP}', '{MANV}'";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }


        private void button_insertLop_Click(object sender, EventArgs e)
        {
            //get input
            string MALOP = this.textBox_maLop.Text;
            string TENLOP = this.textBox_tenLop.Text;
            string MANV = this.comboBox_maNV.SelectedItem.ToString();
            //inser to DB
            themLopHoc(MALOP, TENLOP, MANV);
            refresh();
        }

        private void button111_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void xoaLopHoc(string MALOP)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec DELETE_LOPHOC '{MALOP}'";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();

        }

        private void button2_deleteLop_Click(object sender, EventArgs e)
        {
            string MALOP = this.textBox_maLop.Text;

            xoaLopHoc(MALOP);
            refresh();
        }

        private void suaLopHoc(string MALOP, string TENLOP, string MANV)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec MODIFY_LOPHOC '{MALOP}', '{TENLOP}', '{MANV}'";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void button3_ModifyLop_Click(object sender, EventArgs e)
        {
            string MALOP = this.textBox_maLop.Text;
            string TENLOP = this.textBox_tenLop.Text;
            string MANV = this.comboBox_maNV.SelectedItem.ToString();

            suaLopHoc(MALOP, TENLOP, MANV);
            refresh();
        }

        


        //------------------------------------------------------------------------------------------------------------

        private void loadAllSV()
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = @"exec LOAD_ALL_SINHVIEN";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            SqlDataReader rd = cmd.ExecuteReader();

            DataTable dt = new DataTable();

            for (int i = 0; i < table_SinhVien.ColumnCount; ++i)
            {
                dt.Columns.Add(new DataColumn(table_SinhVien.Columns[i].Name));
                table_SinhVien.Columns[i].DataPropertyName = table_SinhVien.Columns[i].Name;
            }

            while (rd.Read())
            {
                string maSv = rd.GetString(0);
                string hoTen = rd.GetString(1);
                string ngaySinh = rd.GetDateTime(2).ToString();
                string diaChi = rd.GetString(3);
                string maLop = rd.GetString(4);

                dt.Rows.Add(maSv, hoTen, ngaySinh, diaChi, maLop);
            }
            table_SinhVien.DataSource = dt;

            ketnoi.Close();

        }

        private void loadComboBoxMaLop()
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = @"select MALOP from LOP";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            SqlDataReader rd = cmd.ExecuteReader();
            comboBox2_malopSV.Items.Clear();
            while (rd.Read())
            {
                string maLop = rd.GetString(0);
                comboBox2_malopSV.Items.Add(maLop);
            }

            ketnoi.Close();
        }

        private void loadComboBoxChonLop()
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = @"select MALOP from SINHVIEN";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            SqlDataReader rd = cmd.ExecuteReader();
            comboBox1_chonLopHoc.Items.Clear();
            while (rd.Read())
            {
                string maLOP = rd.GetString(0);
                comboBox1_chonLopHoc.Items.Add(maLOP);
            }
            comboBox1_chonLopHoc.Items.Add("ALL");

            ketnoi.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void themSV(string MASV, string HOTEN, string NGAYSINH, string DIACHI, string MALOP, string TENDN, string MATKHAU)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec INSERT_SV '{MASV}', '{HOTEN}', '{NGAYSINH}', '{DIACHI}', '{MALOP}', '{TENDN}', {MATKHAU}";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void xoaSV(string MASV)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec DELETE_SINHVIEN '{MASV}'";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void suaSV(string MASV, string HOTEN, string NGAYSINH, string DIACHI, string MALOP, string TENDN, string MATKHAU)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec MODIFY_SINHVIEN '{MASV}', '{HOTEN}', '{NGAYSINH}', '{DIACHI}', '{MALOP}', '{TENDN}', {MATKHAU}";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void filterSV(string MALOP)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec LOAD_SINHVIEN '{MALOP}'";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            SqlDataReader rd = cmd.ExecuteReader();
            DataTable dt = new DataTable();

            for (int i = 0; i < table_SinhVien.ColumnCount; ++i)
            {
                dt.Columns.Add(new DataColumn(table_SinhVien.Columns[i].Name));
                table_SinhVien.Columns[i].DataPropertyName = table_SinhVien.Columns[i].Name;
            }

            while (rd.Read())
            {
                string maSv = rd.GetString(0);
                string hoTen = rd.GetString(1);
                string ngaySinh = rd.GetDateTime(2).ToString();
                string diaChi = rd.GetString(3);
                string maLop = rd.GetString(4);

                dt.Rows.Add(maSv, hoTen, ngaySinh, diaChi, maLop);
            }
            table_SinhVien.DataSource = dt;

            ketnoi.Close();
        }

        private void button3_insertSV_Click(object sender, EventArgs e)
        {
            string MASV = textBox1_masv.Text; 
            string HOTEN = textBox2_hotenSV.Text; 
            string NGAYSINH = textBox3_ngaySinhSV.Text; 
            string DIACHI = textBox4_diaChiSV.Text; 
            string MALOP = comboBox2_malopSV.SelectedItem.ToString(); 
            string TENDN = textBox5_tenDnSV.Text; 
            string MATKHAU = Cipher.Hash_SHA1(textBox6_matKhauSV.Text);

            themSV(MASV, HOTEN, NGAYSINH, DIACHI, MALOP, TENDN, MATKHAU);
            refresh();
        }

        private void button2_filterSV_Click(object sender, EventArgs e)
        {
            string chonLop = null;
            try
            {
                chonLop = comboBox1_chonLopHoc.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                chonLop = "";
            }


            if (chonLop.Equals("ALL") || chonLop.Equals(""))
            {
                loadAllSV();
            }
            else
            {
                filterSV(chonLop);
            }

        }

        private bool Permission(string MASV)
        {


            //lấy mã lớp của sv đó
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql1 = $@"select MALOP from SINHVIEN where MASV = '{MASV}'";
            SqlCommand cmd1 = new SqlCommand(sql1, ketnoi);
            SqlDataReader rd1 = cmd1.ExecuteReader();
            rd1.Read();
            string MALOP = rd1.GetString(0);
            Console.WriteLine("Ma Lop: " + MALOP);
            //rd1.Close();
            ketnoi.Close();

            //lấy mã nhân viên quản lý lớp đó
            SqlConnection ketnoi2 = ConnectionSQL.LayKetNoi();
            string sql2 = $@"select MANV from LOP where MALOP = '{MALOP}'";
            SqlCommand cmd2 = new SqlCommand(sql2, ketnoi2);
            SqlDataReader rd2 = cmd2.ExecuteReader();
            rd2.Read();
            string MANV = rd2.GetString(0);
            Console.WriteLine("Ma NV: " + MANV);
            ketnoi2.Close();


            //nếu mã nhân viên đó trùng với nhân viên đang đăng nhập thì có quyền chỉnh sửa
            if (MANV.Equals(MANV_LOGIN))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button4_delSV_Click(object sender, EventArgs e)
        {
            string MASV = textBox1_masv.Text;

            if (Permission(MASV))
            {
                xoaSV(MASV);
                //loadAllSV();
                refresh();
            }
            else
            {
                MessageBox.Show("Bạn không có quyền chỉnh sửa sinh viên này!");
            }
            

        }

        private void button5_modifySV_Click(object sender, EventArgs e)
        {
            string MASV = textBox1_masv.Text;
            string HOTEN = textBox2_hotenSV.Text;
            string NGAYSINH = textBox3_ngaySinhSV.Text;
            string DIACHI = textBox4_diaChiSV.Text;
            string MALOP = comboBox2_malopSV.SelectedItem.ToString();
            string TENDN = textBox5_tenDnSV.Text;
            string MATKHAU = Cipher.Hash_SHA1(textBox6_matKhauSV.Text);

            if (Permission(MASV))
            {
                suaSV(MASV, HOTEN, NGAYSINH, DIACHI, MALOP, TENDN, MATKHAU);
                refresh();
            }
            else
            {
                MessageBox.Show("Bạn không có quyền chỉnh sửa sinh viên này!");
            }

            refresh();
        }
        //-------------------------------------------------------------------------------------------------

        private void loadHocPhan()
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = @"exec LOAD_HOCPHAN";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            SqlDataReader rd = cmd.ExecuteReader();

            DataTable dt = new DataTable();

            for (int i = 0; i < table_HocPhan.ColumnCount; ++i)
            {
                dt.Columns.Add(new DataColumn(table_HocPhan.Columns[i].Name));
                table_HocPhan.Columns[i].DataPropertyName = table_HocPhan.Columns[i].Name;
            }
            
            while (rd.Read())
            {
                string MAHP = rd.GetString(0);
                string TENHP = rd.GetString(1);
                int SOTC = rd.GetInt32(2);
                

                dt.Rows.Add(MAHP, TENHP, SOTC);
            }
            table_HocPhan.DataSource = dt;

            ketnoi.Close();

        }

        private void themHocPhan(string MAHP, string TENHP, string SOTC)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec INSERT_HOCPHAN '{MAHP}', '{TENHP}', {SOTC}";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void suaHocPhan(string MAHP, string TENHP, string SOTC)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec UPDATE_HOCPHAN '{MAHP}', '{TENHP}', {SOTC}";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void xoaHocPhan(string MAHP)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec DELETE_HOCPHAN '{MAHP}'";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }


        private void button2_insertHocPhan_Click(object sender, EventArgs e)
        {
            string MAHP = textBox1_maHocPhan.Text;
            string TENHP = textBox2_tenHocPhan.Text;
            string SOTC = textBox3_soTC.Text;

            themHocPhan(MAHP, TENHP, SOTC);
            refresh();

        }

        private void button4_modifyHocPhan_Click(object sender, EventArgs e)
        {
            string MAHP = textBox1_maHocPhan.Text;
            string TENHP = textBox2_tenHocPhan.Text;
            string SOTC = textBox3_soTC.Text;

            suaHocPhan(MAHP, TENHP, SOTC);
            refresh();
        }

        private void button3_delHocPhan_Click(object sender, EventArgs e)
        {
            string MAHP = textBox1_maHocPhan.Text;
            xoaHocPhan(MAHP);
            refresh();
        }

        //-----------------------------------------------------------------------------------------------------------------

        private void loadFileLog()
        {
            //file log ghi lại mã của nhân viên nào nhập điểm cho sv nào
            
            try
            {
                whoEncryptedDiem = FileClass.ReadFile("log.bin");
            }
            catch (Exception ex)
            {

            }
        }
        
        private void loadBangDiem()
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = @"exec LOAD_DIEM";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            SqlDataReader rd = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            for (int i = 0; i < table_DIEM.ColumnCount; ++i)
            {
                dt.Columns.Add(new DataColumn(table_DIEM.Columns[i].Name));
                table_DIEM.Columns[i].DataPropertyName = table_DIEM.Columns[i].Name;
            }

            while (rd.Read())
            {
                string MASV = rd.GetString(0);
                string MAHP = rd.GetString(1);
                string TENHP = rd.GetString(2);
                byte[] DIEMTHI = (byte[])rd.GetValue(3);//Điểm thi bị mã hóa
                // lấy mã nhân viên đã mã hóa điểm sinh viên này trong dictionary whoEncryptedDiem
                string MANV = whoEncryptedDiem[MASV + MAHP];
                // tìm PRIVATE KEY trong listPrivateKey
                string privateKeyString = listPrivateKey[MANV];
                // convert PRIVATEKEY
                RSAParameters PRIVATEKEY = Cipher.StringToKey(privateKeyString);
                //giải mã điểm thi
                byte[] DiemDecrypted = Cipher.RSADecrypt(DIEMTHI, PRIVATEKEY, false);
                string diem = ByteConverter.GetString(DiemDecrypted);

                dt.Rows.Add(MASV, MAHP, TENHP, diem);
            }
            table_DIEM.DataSource = dt;

            ketnoi.Close();
        }

        private void loadComboBoxHOCPHAN()
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = @"select MAHP, TENHP from HOCPHAN";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            SqlDataReader rd = cmd.ExecuteReader();
            comboBox1_MAHP_DIEM.Items.Clear();
            while (rd.Read())
            {
                string MAHP = rd.GetString(0);
                string TENHP = rd.GetString(1);

                comboBox1_MAHP_DIEM.Items.Add(MAHP + " - " + TENHP);
            }
            

            ketnoi.Close();
        }

        private void nhapDiem(string MASV, string MAHP, string DIEM)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec INSERT_DIEM '{MASV}', '{MAHP}', {DIEM}";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void suaDiem(string MASV, string MAHP, string DIEM)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec UPDATE_DIEM '{MASV}', '{MAHP}', {DIEM}";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void xoaDiem(string MASV, string MAHP)
        {
            SqlConnection ketnoi = ConnectionSQL.LayKetNoi();
            string sql = $@"exec DELETE_DIEM '{MASV}', '{MAHP}'";
            SqlCommand cmd = new SqlCommand(sql, ketnoi);
            cmd.ExecuteNonQuery();
            ketnoi.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_insertDIEM_Click(object sender, EventArgs e)
        {
            string MASV = textBox1_MSSV_Diem.Text.ToUpper();
            string MAHP = comboBox1_MAHP_DIEM.SelectedItem.ToString();
            string[] mang = MAHP.Split(' ');
            MAHP = mang[0];
            string DIEMTHI = textBox2_DIEMTHI.Text;

            //mã hóa điểm thi bằng public key của nhân viên đang đăng nhập
            byte[] diemToEncrypt = ByteConverter.GetBytes(DIEMTHI);
            RSAParameters publicKey = Cipher.StringToKey(listPublicKey[PUBKEYNAME]);
            byte[] encryptedDiem = Cipher.RSAEncrypt(diemToEncrypt, publicKey, false);
            string encryptedDiemString = "0x" + Cipher.ByteArrayToString(encryptedDiem);
            //insert DB
            nhapDiem(MASV, MAHP, encryptedDiemString);
            //lưu lại mã nhân viên nhập điểm
            whoEncryptedDiem.Add(MASV + MAHP, MANV_LOGIN);
            FileClass.WriteFile(whoEncryptedDiem, "log.bin");

            refresh();
            
        }

        private void button4_updateDIEM_Click(object sender, EventArgs e)
        {
            string MASV = textBox1_MSSV_Diem.Text.ToUpper();
            string MAHP = comboBox1_MAHP_DIEM.SelectedItem.ToString();
            string[] mang = MAHP.Split(' ');
            MAHP = mang[0];
            string DIEMTHI = textBox2_DIEMTHI.Text;

            //mã hóa điểm thi bằng public key của nhân viên đang đăng nhập
            byte[] diemToEncrypt = ByteConverter.GetBytes(DIEMTHI);

            RSAParameters publicKey = Cipher.StringToKey(listPublicKey[PUBKEYNAME]);

            byte[] encryptedDiem = Cipher.RSAEncrypt(diemToEncrypt, publicKey, false);
            string encryptedDiemString = "0x" + Cipher.ByteArrayToString(encryptedDiem);

            //update DB
            suaDiem(MASV, MAHP, encryptedDiemString);

            whoEncryptedDiem[MASV + MAHP] = MANV_LOGIN;
            FileClass.WriteFile(whoEncryptedDiem, "log.bin");

            refresh();
        }

        private void button3_deleteDIEM_Click(object sender, EventArgs e)
        {
            string MASV = textBox1_MSSV_Diem.Text.ToUpper();
            string MAHP = comboBox1_MAHP_DIEM.SelectedItem.ToString();
            string[] mang = MAHP.Split(' ');
            MAHP = mang[0];
            //xóa điểm đã nhập trong DB
            xoaDiem(MASV, MAHP);
            //xóa mã nhân viên đã nhập điểm đó
            whoEncryptedDiem.Remove(MASV + MAHP);
            FileClass.WriteFile(whoEncryptedDiem, "log.bin");

            refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
