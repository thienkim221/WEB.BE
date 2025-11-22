using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.BE.Models;
using System.Data.Entity; // Required for .Include()
using System.Web.Security; // Required for FormsAuthentication/User.Identity.Name
using WEB.BE.Models.ViewModel;

namespace WEB.BE.Controllers
{
    // Yêu cầu người dùng phải đăng nhập để đặt hàng
    [Authorize]
    public class OrderController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // Hàm lấy giỏ hàng (tương tự như CartController)
        private CartService GetCartService() => new CartService(Session);

        // GET: Order/Checkout (Hiển thị trang thanh toán)
        public ActionResult Checkout()
        {
            var cart = GetCartService().GetCart();

            // 1. Kiểm tra giỏ hàng rỗng
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // 2. Lấy thông tin người dùng đã đăng nhập
            string username = User.Identity.Name; // Requires System.Web.Security or similar setup
            var user = db.Users.SingleOrDefault(u => u.Username == username);

            if (user == null)
            {
                // Nếu User không khớp với DB (hoặc chưa đăng nhập), chuyển hướng đến Login
                return RedirectToAction("Login", "Account");
            }

            // 3. Lấy thông tin khách hàng để điền địa chỉ mặc định
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);

            if (customer == null)
            {
                // Nếu không có thông tin khách hàng, chuyển hướng đến trang thông tin cá nhân
                return RedirectToAction("ProfileInfo", "Account");
            }

            // 4. Tạo ViewModel và gán dữ liệu
            var model = new CheckoutVM
            {
                CartItems = cart.Items.ToList(),
                TotalAmount = cart.TotalValue(),
                CustomerID = customer.CustomerID,
                Username = customer.Username,
                ShippingAddress = customer.CustomerAddress, // Địa chỉ mặc định
                OrderDate = DateTime.Now,
                PaymentStatus = "Pending"
            };

            return View(model);
        }

        // POST: Order/Checkout (Xử lý đặt hàng)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            var cart = GetCartService().GetCart();

            // Kiểm tra Validation và Giỏ hàng
            if (!ModelState.IsValid || cart == null || !cart.Items.Any())
            {
                // Cần gán lại CartItems nếu trả về View
                model.CartItems = cart.Items.ToList();
                return View(model);
            }

            // 1. Thiết lập trạng thái thanh toán
            string paymentStatus = (model.PaymentMethod == "Tiền mặt") ? "Chờ thanh toán" : "Pending";

            // 2. Tạo đối tượng Order và OrderDetail
            var order = new Order
            {
                CustomerID = model.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = model.TotalAmount,
                PaymentStatus = paymentStatus,

                // CÁC THUỘC TÍNH TỪ CHECKOUTVM (Cần có trong Order.cs)
                PaymentMethod = model.PaymentMethod,
                ShippingMethod = model.ShippingMethod,
                ShippingAddress = model.ShippingAddress,

                OrderDetails = cart.Items.Select(item => new OrderDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    // TotalPrice được tính tự động (Computed Column)
                }).ToList()
            };

            // 3. Lưu đơn hàng vào DB
            db.Orders.Add(order);
            db.SaveChanges();

            // 4. Xóa giỏ hàng và chuyển hướng
            GetCartService().ClearCart();
            return RedirectToAction("OrderSuccess", new { id = order.OrderID });
        }

        // GET: Order/OrderSuccess/5 (Xác nhận đơn hàng)
        public ActionResult OrderSuccess(int id)
        {
            // Lấy thông tin Order và chi tiết đơn hàng (OrderDetails)
            var order = db.Orders.Include("OrderDetails").SingleOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Order/OrderHistory (Tra cứu Lịch sử mua hàng)
        public ActionResult OrderHistory()
        {
            // Lấy username người dùng hiện tại
            string username = User.Identity.Name;

            // Lấy CustomerID
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách đơn hàng của khách hàng này
            var orders = db.Orders
                            .Where(o => o.CustomerID == customer.CustomerID)
                            .OrderByDescending(o => o.OrderDate)
                            .ToList();

            return View(orders);
        }
    }
}