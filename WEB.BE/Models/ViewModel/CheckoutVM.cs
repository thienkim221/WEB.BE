using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WEB.BE.Models.ViewModel
{
    public class CheckoutVM
    {
        // Thông tin Giỏ hàng
        public List<CartItem> CartItems { get; set; }

        // Thông tin Khách hàng (Lấy từ DB)
        public int CustomerID { get; set; }
        public string Username { get; set; }

        // Thông tin Đơn hàng (Ánh xạ từ Order.cs)
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Tổng giá trị")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        [Required(ErrorMessage = "Vui lòng chọn hình thức thanh toán.")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Phương thức giao hàng")]
        [Required(ErrorMessage = "Vui lòng chọn phương thức giao hàng.")]
        public string ShippingMethod { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        [Required(ErrorMessage = "Vui lòng cung cấp địa chỉ giao hàng.")]
        public string ShippingAddress { get; set; }

        // Các chi tiết đơn hàng
        public List<OrderDetail> OrderDetails { get; set; } // Phải using _24DH110457_MyStore.Models;
    }
}
