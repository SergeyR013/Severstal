using MySql.Data.MySqlClient;
using Severstal.ErrorHandler;
using System;
using System.Configuration;
using System.Data;

namespace Severstal.DataBase
{
    class SearchOrders
    {
        public DataTable Find(String where)
        {
            MySqlConnection conn = new MySqlConnection();
            DataTable dataTable = new DataTable();

            try
            {
                ConnectionStringSettings conString;
                conString = ConfigurationManager.ConnectionStrings["connStr"];
                conn.ConnectionString = conString.ConnectionString;
                conn.Open();


                string query_part1 = "Select p.name as Продукт, l.amount as Вес_кг, l.price as Цена_за_кг, l.total_price as Итого, c.name as Поставщик, l.date as Дата_поставки from loads as l " +
                         "inner join products as p on p.id = l.id_prod " +
                         "inner join contragents as c on c.id = p.id_contragent ";

                String query = query_part1 + where + " order by c.name";

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
