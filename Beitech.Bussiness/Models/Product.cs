using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeitechBussiness.Models
{
    public class GetProductModelOut
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Int16 State { get; set; }
        public int CountSale { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class GetProductSaleModelOut
    {
        public int YearNumber { get; set; }
        public int MonthNumber { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountSale { get; set; }
        public decimal TotalPrice { get; set; }
        public int PercentPrice { get; set; }
    }
}
