using System.Linq;
using System.Net;
using System.Web.Mvc;
using WEB.BE.Models;

namespace WEB.BE.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Customers
        public ActionResult Index()
        {
            var customers = db.Customers.ToList();
            return View(customers);
        }

        // GET: Admin/Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var customer = db.Customers.Find(id);
            if (customer == null)
                return HttpNotFound();

            // Lấy danh sách đơn hàng của khách hàng
            var orders = db.Orders
                           .Where(o => o.CustomerID == id)
                           .OrderByDescending(o => o.OrderDate)
                           .ToList();

            ViewBag.Orders = orders;

            return View(customer);
        }
    }
}
