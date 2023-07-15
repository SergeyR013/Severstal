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

        private int numberOfItemOnPage = 0;
        private int numberOfPrintedItemsSoFarBerezka = 0;
        private int numberOfPrintedItemsSoFarSadovod = 0;
        private int numberOfPrintedItemsSoFarCombinat = 0;
        private List<Order> berezka = new List<Order>();
        private List<Order> combinat = new List<Order>();
        private List<Order> sadovod = new List<Order>();


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

        private void searchBtn_Click(object sender, EventArgs e)
        {
            searchBtn.Enabled = false;
            searchBtn.Text = "Поиск...";
            dataGridView2.DataSource = null;


            String where = $"WHERE l.date between '{dateFrom.Value.ToString("yyyy-MM-dd")}' and '{dateTo.Value.ToString("yyyy-MM-dd")}'";
            SearchOrders search = new SearchOrders();

            Task<DataTable> getDataTask = Task.Run(() => search.Find(where));
            getDataTask.ContinueWith(task =>
            {
                if (task.Result.Rows.Count > 0)
                {
                    dataGridView2.DataSource = task.Result;
                }
                else
                {
                    PopupMessage popup = new PopupMessage();
                    popup.ShowPopupMessage("Товар не был найден!", 1500, Color.OrangeRed);
                }
                searchBtn.Enabled = true;
                searchBtn.Text = "Поиск";
                setLists();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            
        }
        private void setLists()
        {
            berezka.Clear();
            sadovod.Clear();
            combinat.Clear();
            if (dataGridView2.Rows.Count > 0)
            {

                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    Order order = new Order();
                    order.setProduct(dataGridView2.Rows[i].Cells[0].Value.ToString());
                    order.setNumOf(decimal.Parse(dataGridView2.Rows[i].Cells[1].Value.ToString()));
                    order.setPrice(decimal.Parse(dataGridView2.Rows[i].Cells[2].Value.ToString()));
                    order.FindTotalPrice();
                    order.setDate(DateTime.Parse(dataGridView2.Rows[i].Cells[5].Value.ToString()).ToString("dd.MM.yyyy"));

                    if (dataGridView2.Rows[i].Cells[4].Value.ToString() == "Березка Фуд")
                    {
                        berezka.Add(order);
                    }
                    else if (dataGridView2.Rows[i].Cells[4].Value.ToString() == "ООО «Садовод»")
                    {
                        sadovod.Add(order);
                    }
                    else
                    {
                        combinat.Add(order);
                    }
                }
            }
        }

        private void printBtn_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog1 = new PrintDialog();
            DialogResult result = printDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                printDocument1.PrinterSettings = printDialog1.PrinterSettings;
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
            }
        }

        private int drawColumns(String Contragent, int yPos, int xPosContragent, int xPosProduct, int xPosNumOf, int xPosPrice, int xPosTotalPrice, int xPosDate, int font, String line, Font myFont12, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("Поставщик: " + Contragent, myFont12, Brushes.Black, new Point(xPosContragent, yPos));
            yPos += 15;
            e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, yPos));
            yPos += 15;
            e.Graphics.DrawString("Продукт", myFont12, Brushes.Black, new Point(xPosProduct, yPos));
            e.Graphics.DrawString("Вес, кг", myFont12, Brushes.Black, new Point(xPosNumOf, yPos));
            e.Graphics.DrawString("Цена за кг, руб", myFont12, Brushes.Black, new Point(xPosPrice, yPos));
            e.Graphics.DrawString("Итого", myFont12, Brushes.Black, new Point(xPosTotalPrice, yPos));
            e.Graphics.DrawString("Дата", myFont12, Brushes.Black, new Point(xPosDate, yPos));
            yPos += 15;
            e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, yPos));
            yPos += 15;
            return yPos;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            int xPosContragent = 30,
                xPosProduct = 30,
                xPosNumOf = 220,
                xPosPrice = 300,
                xPosTotalPrice = 500,
                xPosDate = 650,
                font = 9,
                lineLenght = 137,
                items = 27,
                xPosOthcet = 350;
            
            decimal allWeight = 0,
                allPrice = 0;

            if (e.PageSettings.Landscape)
            {
                xPosProduct += 50;
                xPosNumOf += 180;
                xPosPrice += 200;
                xPosTotalPrice += 300;
                xPosDate += 300;
                lineLenght = 200;
                items = 9; 
                xPosOthcet = 550;
            }

            Font myFont12 = new Font("Times New Roman", 12, FontStyle.Regular);
            Font myFont38 = new Font("Times New Roman", 38, FontStyle.Regular);

            e.Graphics.DrawString("Отчет", myFont38, Brushes.Black, new Point(xPosOthcet, 50));
            e.Graphics.DrawString("Период: " + dateFrom.Value.ToShortDateString() + " - " + dateTo.Value.ToShortDateString(), myFont12, Brushes.Black, new Point(25, 180));
            
            String line = new String('-', lineLenght);
            e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, 235));

            int yPos = 255;
            decimal TotalWeight = 0,
                    TotalTotalPrice = 0;
            if (berezka.Count > 0 && numberOfPrintedItemsSoFarBerezka < berezka.Count)
            {
                yPos = drawColumns("Березка Фуд", yPos, xPosContragent, xPosProduct, xPosNumOf, xPosPrice, xPosTotalPrice, xPosDate, font, line, myFont12, e);

                using (Font regularFont = new Font("Times New Roman", font, FontStyle.Regular))
                {
                    
                    for (int i = numberOfPrintedItemsSoFarBerezka; i < berezka.Count; i++)
                    {
                        numberOfItemOnPage++;
                        if (numberOfItemOnPage <= items)
                        {
                            numberOfPrintedItemsSoFarBerezka++;
                            if (numberOfPrintedItemsSoFarBerezka <= berezka.Count)
                            {
                                e.Graphics.DrawString(" | " + berezka[i].getProduct(), regularFont, Brushes.Black, new Point(xPosProduct, yPos));
                                e.Graphics.DrawString(" | " + berezka[i].getNumOf(), regularFont, Brushes.Black, new Point(xPosNumOf, yPos));
                                e.Graphics.DrawString(" | " + berezka[i].getPrice(), regularFont, Brushes.Black, new Point(xPosPrice, yPos));
                                e.Graphics.DrawString(" | " + berezka[i].getTotalPrice(), regularFont, Brushes.Black, new Point(xPosTotalPrice, yPos));
                                e.Graphics.DrawString(" | " + berezka[i].getDate(), regularFont, Brushes.Black, new Point(xPosDate, yPos));
                                yPos += 20;
                                TotalWeight += berezka[i].getNumOf();
                                TotalTotalPrice += berezka[i].getTotalPrice();
                            }
                            else
                            {
                                e.HasMorePages = false;
                            }
                        }
                        else
                        {
                            numberOfItemOnPage = 0;
                            e.HasMorePages = true;
                            return;
                        }
                    }
                    e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, yPos));
                    yPos += 20;
                    e.Graphics.DrawString(" | Общий вес: " + TotalWeight + " кг.", regularFont, Brushes.Black, new Point(xPosTotalPrice, yPos));
                    e.Graphics.DrawString(" | Общая сумма: " + TotalTotalPrice + " руб.", regularFont, Brushes.Black, new Point(xPosDate, yPos));
                    yPos += 20;
                    e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, yPos));
                    yPos += 20;
                    allWeight += TotalWeight;
                    allPrice += TotalTotalPrice;
                }
            }

            if (sadovod.Count > 0 && numberOfPrintedItemsSoFarSadovod < sadovod.Count)
            {
                TotalWeight = 0;
                TotalTotalPrice = 0;
                yPos = drawColumns("ООО «Садовод»", yPos, xPosContragent, xPosProduct, xPosNumOf, xPosPrice, xPosTotalPrice, xPosDate, font, line, myFont12, e);
                using (Font regularFont = new Font("Times New Roman", font, FontStyle.Regular))
                {
                    for (int i = numberOfPrintedItemsSoFarSadovod; i < sadovod.Count; i++)
                    {
                        numberOfItemOnPage++;
                        if (numberOfItemOnPage <= items)
                        {
                            numberOfPrintedItemsSoFarSadovod++;
                            if (numberOfPrintedItemsSoFarSadovod <= sadovod.Count)
                            {
                                e.Graphics.DrawString(" | " + sadovod[i].getProduct(), regularFont, Brushes.Black, new Point(xPosProduct, yPos));
                                e.Graphics.DrawString(" | " + sadovod[i].getNumOf(), regularFont, Brushes.Black, new Point(xPosNumOf, yPos));
                                e.Graphics.DrawString(" | " + sadovod[i].getPrice(), regularFont, Brushes.Black, new Point(xPosPrice, yPos));
                                e.Graphics.DrawString(" | " + sadovod[i].getTotalPrice(), regularFont, Brushes.Black, new Point(xPosTotalPrice, yPos));
                                e.Graphics.DrawString(" | " + sadovod[i].getDate(), regularFont, Brushes.Black, new Point(xPosDate, yPos));
                                yPos += 20;
                                TotalWeight += sadovod[i].getNumOf();
                                TotalTotalPrice += sadovod[i].getTotalPrice();
                            }
                            else
                            {
                                e.HasMorePages = false;
                            }
                        }
                        else
                        {
                            numberOfItemOnPage = 0;
                            e.HasMorePages = true;
                            return;
                        }
                            
                    }
                    e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, yPos));
                    yPos += 20;
                    e.Graphics.DrawString(" | Общий вес: " + TotalWeight + " кг.", regularFont, Brushes.Black, new Point(xPosTotalPrice, yPos));
                    e.Graphics.DrawString(" | Общая сумма: " + TotalTotalPrice + " руб.", regularFont, Brushes.Black, new Point(xPosDate, yPos));
                    yPos += 20;
                    e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, yPos));
                    yPos += 20;
                    allWeight += TotalWeight;
                    allPrice += TotalTotalPrice;
                }
            }

            if (combinat.Count > 0 && numberOfPrintedItemsSoFarCombinat < combinat.Count)
            {
                TotalWeight = 0;
                TotalTotalPrice = 0;
                yPos = drawColumns("ООО Комбинат Западный", yPos, xPosContragent, xPosProduct, xPosNumOf, xPosPrice, xPosTotalPrice, xPosDate, font, line, myFont12, e);
                using (Font regularFont = new Font("Times New Roman", font, FontStyle.Regular))
                {
                    for (int i = numberOfPrintedItemsSoFarCombinat; i < combinat.Count; i++)
                    {
                        numberOfItemOnPage++;
                        if (numberOfItemOnPage <= items)
                        {
                            numberOfPrintedItemsSoFarCombinat++;
                            if (numberOfPrintedItemsSoFarCombinat <= combinat.Count)
                            {
                                e.Graphics.DrawString(" | " + combinat[i].getProduct(), regularFont, Brushes.Black, new Point(xPosProduct, yPos));
                                e.Graphics.DrawString(" | " + combinat[i].getNumOf(), regularFont, Brushes.Black, new Point(xPosNumOf, yPos));
                                e.Graphics.DrawString(" | " + combinat[i].getPrice(), regularFont, Brushes.Black, new Point(xPosPrice, yPos));
                                e.Graphics.DrawString(" | " + combinat[i].getTotalPrice(), regularFont, Brushes.Black, new Point(xPosTotalPrice, yPos));
                                e.Graphics.DrawString(" | " + combinat[i].getDate(), regularFont, Brushes.Black, new Point(xPosDate, yPos));
                                yPos += 20;
                                TotalWeight += combinat[i].getNumOf();
                                TotalTotalPrice += combinat[i].getTotalPrice();
                            }
                            else
                            {
                                e.HasMorePages = false;
                            }
                        }
                        else
                        {
                            numberOfItemOnPage = 0;
                            e.HasMorePages = true;
                            return;
                        }
                            
                    }
                    e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, yPos));
                    yPos += 20;
                    e.Graphics.DrawString(" | Общий вес: " + TotalWeight + " кг.", regularFont, Brushes.Black, new Point(xPosTotalPrice, yPos));
                    e.Graphics.DrawString(" | Общая сумма: " + TotalTotalPrice + " руб.", regularFont, Brushes.Black, new Point(xPosDate, yPos));
                    yPos += 20;
                    e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, yPos));
                    yPos += 20;
                    allWeight += TotalWeight;
                    allPrice += TotalTotalPrice;
                }
            }
            e.Graphics.DrawString(" | Общий вес: " + allWeight + " кг.", myFont12, Brushes.Black, new Point(xPosTotalPrice - 70, yPos));
            e.Graphics.DrawString(" | Общая сумма: " + allPrice + " руб.", myFont12, Brushes.Black, new Point(xPosDate - 50, yPos));
            yPos += 20;
            e.Graphics.DrawString(line, myFont12, Brushes.Black, new Point(25, yPos));
            numberOfItemOnPage = 0;
            numberOfPrintedItemsSoFarBerezka = 0;
            numberOfPrintedItemsSoFarSadovod = 0;
            numberOfPrintedItemsSoFarCombinat = 0;
        }
    }
}
