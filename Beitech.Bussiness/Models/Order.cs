using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeitechBussiness.Models
{

   


    public class NewOrderModelIn
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public string DeliveryAddress { get; set; }
        [Required]
        public List<NewOrderDetailModelIn> OrderDetails { get; set; }


        public NewOrderModelIn()
        {
            
        }

    }

    public class NewOrderDetailModelIn
    {
        
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Count { get; set; }
        
    }

    public class NewOrderModelOut
    {
        public int Id { get; set; }
    }



    public class GetOrderByCustomerModelIn
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
    public class GetOrderModelOut
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Int16 State { get; set; }
        public string StateName { get; set; }
        public DateTime DeliveryDate { get; set; }

        public decimal TotalPrice { get; set; }

        public List<GetOrderDetailModelOut> OrderDetails { get; set; }
    }

    public class GetOrderDetailModelOut
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int CountSale { get; set; }
        public decimal Price { get; set; }
        public string DescriptionProduct { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public class GetResponseOrderDb
    {
        public int state { get; set; }
        public int id { get; set; }
        public string msg { get; set; }
    }
}
