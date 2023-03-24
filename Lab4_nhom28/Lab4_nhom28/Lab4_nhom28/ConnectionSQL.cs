using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4_nhom28
{
    class ConnectionSQL
    {
        public static SqlConnection LayKetNoi()
        {
            SqlConnection ketnoi = null;
            string strConection = @"Data Source=DESKTOP-L65OT1A;Initial Catalog=QLSVNhom;Persist Security Info=True;User ID=sa;Password=123;";
            
            try
            {
                if (ketnoi == null)
                {
                    ketnoi = new SqlConnection(strConection);
                }

                if (ketnoi.State == ConnectionState.Closed)
                {
                    ketnoi.Open();
                    Console.WriteLine("Ket noi CSDL thanh cong");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("khong ket noi duoc voi CSDL");
            }
            return ketnoi;
        }

    }
}
