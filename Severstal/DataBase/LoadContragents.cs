using MySql.Data.MySqlClient;
using Severstal.ErrorHandler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Severstal.DataBase
{
    class LoadContragents
    {
        public DataTable get()
        {
            MySqlConnection conn = new MySqlConnection();
            DataTable dataTable = new DataTable();

            try
            {
                ConnectionStringSettings conString;
                conString = ConfigurationManager.ConnectionStrings["connStr"];
                conn.ConnectionString = conString.ConnectionString;
                conn.Open();

                string query = "SELECT id, name FROM contragents";



                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }

            }
            catch (Exception ex)
            {
                Error error = new Error(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return dataTable;

        }
    }
}
