using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeitechBussiness.Models
{
    public class GetCustomerModelOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public Int16 State { get; set; }
        public int OrdersCount { get; set; }
        public int ProductsCount { get; set; }
        public decimal PriceTotal { get; set; }
    }


    public class GetProductsAvailableModelOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberProductsAvaible { get; set; }
    }
}
