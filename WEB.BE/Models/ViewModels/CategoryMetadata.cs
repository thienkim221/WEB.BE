using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WEB.BE.Models.ViewModels
{
    public class CategoryMetadata
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}