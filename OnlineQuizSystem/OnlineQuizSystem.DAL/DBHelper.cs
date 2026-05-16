using System.Data;
using System.Data.SqlClient;

namespace OnlineQuizSystem.DAL
{
    public class DBHelper
    {
        string conStr = "Server=.;Database=OnlineQuizDB;Trusted_Connection=True;";

        public SqlConnection GetCon()
        {
            return new SqlConnection(conStr);
        }

        public DataTable Select(string query)
        {
            SqlConnection con = GetCon();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public void Execute(string query)
        {
            SqlConnection con = GetCon();
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}