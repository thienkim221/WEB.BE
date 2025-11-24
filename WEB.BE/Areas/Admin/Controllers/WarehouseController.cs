using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.BE.Models;

namespace WEB.BE.Areas.Admin.Controllers
{
    public class WarehouseController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        // GET: Admin/Warehouse
        public ActionResult Index()
        {
            var products = db.Products.ToList();
            return View(products);
        }
    }
}