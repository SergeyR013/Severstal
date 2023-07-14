using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Severstal.Models
{
    class Order
    {
        string product;
        string product_id;
        decimal numOf = 0;
        decimal price = 0;
        decimal TotalPrice;

        public void setProduct(string _product)
        {
            product = _product;
        }
        public string getProduct()
        {
            return product;
        }
        public void setProductId(string _productId)
        {
            product_id = _productId;
        }
        public string getProductId()
        {
            return product_id;
        }
        public void setNumOf(decimal _numOf)
        {
            numOf = _numOf;
        }
        public decimal getNumOf()
        {
            return numOf;
        }
        public void setPrice(decimal _price)
        {
            price = _price;
        }
        public decimal getPrice()
        {
            return price;
        }
        public void FindTotalPrice()
        {
            TotalPrice = price * numOf;
        }
        public decimal getTotalPrice()
        {
            return TotalPrice;
        }
        public void print()
        {
            MessageBox.Show("id_prod: " + product_id + "\n" +
                            "prod: " + product + "\n" +
                            "num_of: " + numOf + "\n" +
                            "price: " + price+ "\n" +
                            "total_price: " + TotalPrice + "\n");
        }
    }
}
