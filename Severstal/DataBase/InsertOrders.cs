using MySql.Data.MySqlClient;
using Severstal.ErrorHandler;
using Severstal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Severstal.DataBase
{
    class InsertOrders
    {
        private bool isInserted = false;
        public bool insert(List<Order> orderList)
        {
            MySqlConnection conn = new MySqlConnection();
            try
            {
                ConnectionStringSettings conString;
                conString = ConfigurationManager.ConnectionStrings["connStr"];
                conn.ConnectionString = conString.ConnectionString;
                conn.Open();

                for (int i = 0; i < orderList.Count; i++)
                {
                    Order order = orderList[i];

                    string insertGoodsQuery = "INSERT INTO loads (id_prod, amount, price, total_price, date) " +
                        "VALUES (@id_prod, @amount, @price, @total_price, @date)";
                    MySqlCommand insertGoodsCommand = new MySqlCommand(insertGoodsQuery, conn);
                    insertGoodsCommand.Parameters.AddWithValue("@id_prod", order.getProductId());
                    insertGoodsCommand.Parameters.AddWithValue("@amount", order.getNumOf());
                    insertGoodsCommand.Parameters.AddWithValue("@price", order.getPrice());
                    insertGoodsCommand.Parameters.AddWithValue("@total_price", order.getTotalPrice());
                    insertGoodsCommand.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));

                    int rowsAffected = insertGoodsCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        isInserted = true;
                    }
                    else
                    {
                        throw new Exception("Ошибка вставки записи!");
                    }
                }
            }
            catch (Exception ex)
            {
                isInserted = false;
                Error error = new Error(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return isInserted;
        }
    }
}
