using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.BE.Models;

namespace WEB.BE.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            ViewBag.ProductCount = db.Products.Count();
            ViewBag.CategoryCount = db.Categories.Count();
            ViewBag.OrderCount = db.Orders.Count();
            ViewBag.UserCount = db.Users.Count();

            return View();
        }
    }
    
}