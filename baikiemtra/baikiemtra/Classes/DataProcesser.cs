using System.Data;
using System.Data.SqlClient;

namespace baikiemtra.Classes
{
    internal class DataProcesser
    {
        
        string strConnect = "Data Source=DESKTOP-8DRMUIQ;Initial Catalog=DiemThi;Integrated Security=True;Encrypt=False";

        
        public DataTable DocBang(string sql, params SqlParameter[] thamSo)
        {
            using (SqlConnection ketNoi = new SqlConnection(strConnect))
            using (SqlCommand lenh = new SqlCommand(sql, ketNoi))
            using (SqlDataAdapter boDoc = new SqlDataAdapter(lenh))
            {
                lenh.Parameters.AddRange(thamSo);
                DataTable bang = new DataTable();
                ketNoi.Open();
                boDoc.Fill(bang);
                return bang;
            }
        }

        
        public int CapNhatDuLieu(string sql, params SqlParameter[] thamSo)
        {
            using (SqlConnection ketNoi = new SqlConnection(strConnect))
            using (SqlCommand lenh = new SqlCommand(sql, ketNoi))
            {
                lenh.Parameters.AddRange(thamSo);
                ketNoi.Open();
                return lenh.ExecuteNonQuery();
            }
        }
    }
}

