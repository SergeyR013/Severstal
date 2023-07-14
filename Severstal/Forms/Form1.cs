using Severstal.DataBase;
using Severstal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Severstal
{
    public partial class Form1 : Form
    {
        List<Order> listOrder = new List<Order>();
        public Form1()
        {
            InitializeComponent();

            LoadContragents load = new LoadContragents();

            Task<DataTable> getDataTask = Task.Run(() => load.get());
            getDataTask.ContinueWith(task =>
            {
                if (task.Result.Rows.Count > 0)
                {
                    contragentCB.DataSource = task.Result;

                    contragentCB.ValueMember = "name";
                    contragentCB.DisplayMember = "name";
                    startLoadBtn.Enabled = true;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private string findId(ComboBox cb)
        {
            string id = "";
            if (cb.SelectedItem is DataRowView selectedRow)
            {
                object[] itemArray = selectedRow.Row.ItemArray;
                if (itemArray.Length > 0)
                {
                    id = itemArray[0].ToString();
                }
            }
            return id;
        }

        private void startLoadBtn_Click(object sender, EventArgs e)
        {
            contragentCB.Enabled = false;
            startLoadBtn.Enabled = false;
            productCB.Enabled = true;
            numberOf.Enabled = true;
            Price.Enabled = true;
            cancelBtn.Enabled = true;

            string id_contragent = findId(contragentCB);

            GetProducts getProd = new GetProducts();

            Task<DataTable> getDataTask = Task.Run(() => getProd.get(id_contragent));
            getDataTask.ContinueWith(task =>
            {
                if (task.Result.Rows.Count > 0)
                {
                    productCB.DataSource = task.Result;
                    productCB.ValueMember = "name";
                    productCB.DisplayMember = "name";
                    addBtn.Enabled = true;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            sendToDB.Enabled = false;
            contragentCB.Enabled = true;
            startLoadBtn.Enabled = true;
            addBtn.Enabled = false;
            productCB.Enabled = false;
            numberOf.Enabled = false;
            Price.Enabled = false;
            cancelBtn.Enabled = false;
            productCB.DataSource = null;
            dataGridView1.Rows.Clear();
            Price.Value = Price.Minimum;
            numberOf.Value = numberOf.Minimum;
            listOrder.Clear();
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            if (!sendToDB.Enabled) sendToDB.Enabled = true;
            Order order = new Order();
            order.setProduct(productCB.Text);
            order.setProductId(findId(productCB));
            order.setNumOf(numberOf.Value);
            order.setPrice(Price.Value);
            order.FindTotalPrice();
            listOrder.Add(order);

            dataGridView1.Rows.Add(order.getProduct(), order.getNumOf().ToString(), order.getPrice().ToString(), order.getTotalPrice().ToString());
        }

        private void sendToDB_Click(object sender, EventArgs e)
        {
            sendToDB.Enabled = false;
            sendToDB.Text = "Отправка...";
            InsertOrders insert = new InsertOrders();

            Task<bool> isInserted = Task.Run(() => insert.insert(listOrder));
            isInserted.ContinueWith(task =>
            {
                if (task.Result)
                {
                    PopupMessage popup = new PopupMessage();
                    popup.ShowPopupMessage("Товар успешн внесен в БД!", 1500, Color.LightGreen);
                }
                sendToDB.Enabled = true;
                sendToDB.Text = "Внести в БД";
                cancelBtn_Click(sender, e);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i = dataGridView1.SelectedRows.Count - 1; i >= 0; i--)
            {
                int index = dataGridView1.SelectedRows[i].Index;
                dataGridView1.Rows.RemoveAt(index);
                listOrder.RemoveAt(index);
            }
            if(dataGridView1.Rows.Count < 1)
            {
                sendToDB.Enabled = false;
            }
        }

        private void dateFrom_ValueChanged(object sender, EventArgs e)
        {
            if (dateFrom.Value > dateTo.Value)
                dateTo.Value = dateFrom.Value;
        }

        private void dateTo_ValueChanged(object sender, EventArgs e)
        {
            if (dateTo.Value < dateFrom.Value)
                dateTo.Value = dateFrom.Value;
        }
    }
}
