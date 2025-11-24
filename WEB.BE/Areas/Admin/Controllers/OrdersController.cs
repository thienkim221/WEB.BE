using System.Linq;
using System.Web.Mvc;
using WEB.BE.Models;

namespace WEB.BE.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include("Customer").ToList();
            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int id)
        {
            var order = db.Orders
                .Include("Customer")
                .Include("OrderDetails.Product")
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
                return HttpNotFound();

            return View(order); 
        }

    }
}
