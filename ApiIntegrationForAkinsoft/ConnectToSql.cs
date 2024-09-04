using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiIntegrationForAkinsoft
{
    public static class ConnectToSql
    {
        public static SqlConnection SqlConnection()
        {
            Dapper.SqlMapper.Settings.CommandTimeout = 0;
            string conString= @"Data Source=DESKTOP-2AFIS8M\AKINSOFT; Initial Catalog=WOLVOX9_DEHASOFTDEMO_2024_WOLVOX;Integrated Security=True;";
            SqlConnection connection = new SqlConnection(conString);
            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            return connection;
        }
    }
}
