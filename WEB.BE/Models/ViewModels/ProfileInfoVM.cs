using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WEB.BE.Models.ViewModels
{
    public class ProfileInfoVM
    {
        public string Username { get; set; }
        public int CustomerID { get; set; }

        [Display(Name = "Họ tên")]
        public string CustomerName { get; set; }

        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }

        public string StatusMessage { get; set; }
    }
}