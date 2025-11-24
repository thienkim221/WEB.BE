using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB.BE.Models.ViewModels
{
    public class OrderDetailVM
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }

        public List<OrderItemVM> Items { get; set; }
    }
    public class OrderItemVM
    {
        public string ProductName { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
