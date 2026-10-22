using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Ktra.Classes
{
    internal class DataProcessor
    {
        string strConnect = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DuLieu;Integrated Security=True";
        SqlConnection sqlConnect = null;


        private void KetNoiCSDL()
        {
            sqlConnect = new SqlConnection(strConnect);
            if(sqlConnect.State != ConnectionState.Open)
            {
                sqlConnect.Open();
            }
        }

        private void DongKetNoiCSDL()
        {
            if (sqlConnect != null && sqlConnect.State != ConnectionState.Closed)
            {
                sqlConnect.Close();
            }
        }

        public void CapNhatDuLieu(string sql)
        {
            KetNoiCSDL();
            SqlCommand cmd = new SqlCommand(sql, sqlConnect);
            cmd.ExecuteNonQuery();
            DongKetNoiCSDL();
            cmd.Dispose();
        }

        public int CapNhatDuLieu(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(strConnect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddRange(parameters);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public  DataTable DocBang(string sql , params SqlParameter[] parameters)
        {
            using(SqlConnection connection = new SqlConnection(strConnect))
            using(SqlCommand command = new SqlCommand(sql,connection))
            using(SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                command.Parameters.AddRange(parameters);
                DataTable bang = new DataTable();
                adapter.Fill(bang);
                return bang;
            }
        }
    }
}
