using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI.WebControls;
using WEB.BE.Models;
using WEB.BE.Models.ViewModel;

namespace WEB.BE.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private MyStoreEntities1 db = new MyStoreEntities1();

        // Hàm giả định mã hóa mật khẩu (BẠN PHẢI THAY THẾ BẰNG THUẬT TOÁN HASH THẬT)
        private string HashPassword(string password)
        {
            // Tạm thời trả về mật khẩu gốc để test
            return password;
        }

        // ======================= ĐĂNG KÝ (REGISTER) =======================
        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kiểm tra tên đăng nhập đã tồn tại chưa
                var existingUser = db.Users.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }

                // 2. Tạo bản ghi thông tin tài khoản trong bảng User
                // Trong AccountController.cs (Register POST)

                // ...
                var user = new User
                {
                    Username = model.Username,
                    Password = HashPassword(model.Password),
                    UserRole = "C" // ĐÃ SỬA: Đảm bảo sử dụng "C" nhất quán
                };
                db.Users.Add(user);
                // ...

                // 3. Tạo bản ghi thông tin khách hàng trong bảng Customer
                var customer = new Customer
                {
                    Username = model.Username,
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress
                };
                db.Customers.Add(customer);

                // 4. Lưu thông tin và chuyển hướng
                db.SaveChanges();
                FormsAuthentication.SetAuthCookie(user.Username, false);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // ======================= ĐĂNG NHẬP (LOGIN) =======================
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // File: Controllers/AccountController.cs

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                // 1. Mã hóa mật khẩu đầu vào
                string hashedPassword = HashPassword(model.Password);

                // 2. KHẮC PHỤC LỖI: Kiểm tra Username, Password, và UserRole
                // UserRole phải khớp với giá trị đơn ký tự "C" mà bạn đã dùng khi Đăng ký
                var user = db.Users.SingleOrDefault(u =>
                    u.Username == model.Username &&
                    u.Password == hashedPassword &&
                    u.UserRole == "C" // ĐÃ SỬA: Dùng "C" để khớp với dữ liệu Đăng ký
                );

                if (user != null)
                {
                    // ... (Logic Đăng nhập thành công)
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }
        // ======================= ĐĂNG XUẤT (LOGOUT) =======================
        // GET: Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear(); // Xóa Session đăng nhập
            return RedirectToAction("Index", "Home");
        }

        // ======================= XEM/SỬA THÔNG TIN (PROFILE) =======================

        // GET: Account/ProfileInfo (Hiển thị form thông tin cá nhân)
        [Authorize]
        public ActionResult ProfileInfo()
        {
            string username = User.Identity.Name;

            // Lấy dữ liệu cho ProfileInfoVM
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);

            if (customer == null)
            {
                // Khách hàng không tồn tại hoặc chưa tạo thông tin cá nhân
                return RedirectToAction("Login", "Account");
            }

            // Map Entity sang ViewModel
            var model = new ProfileInfoVM
            {
                Username = username,
                CustomerID = customer.CustomerID,
                CustomerName = customer.CustomerName,
                CustomerEmail = customer.CustomerEmail,
                CustomerPhone = customer.CustomerPhone,
                CustomerAddress = customer.CustomerAddress
            };

            return View(model); // View này sẽ là ProfileInfo.cshtml
        }

        // POST: Account/ProfileInfo (Cập nhật thông tin cá nhân)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ProfileInfo(ProfileInfoVM model)
        {
            if (ModelState.IsValid)
            {
                var customerToUpdate = db.Customers.SingleOrDefault(c => c.CustomerID == model.CustomerID);

                if (customerToUpdate != null)
                {
                    // Cập nhật các trường thông tin cá nhân
                    customerToUpdate.CustomerName = model.CustomerName;
                    customerToUpdate.CustomerEmail = model.CustomerEmail;
                    customerToUpdate.CustomerPhone = model.CustomerPhone;
                    customerToUpdate.CustomerAddress = model.CustomerAddress;

                    db.SaveChanges();
                    model.StatusMessage = "Cập nhật thông tin thành công!";
                    return View(model);
                }
            }

            model.StatusMessage = "Cập nhật thất bại. Vui lòng kiểm tra lại.";
            return View(model);
        }

        // ======================= ĐỔI MẬT KHẨU (CHANGE PASSWORD) =======================

        // GET: Account/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            // View này sẽ sử dụng ViewModel riêng để nhập 3 trường (Mật khẩu cũ, mới, xác nhận)
            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (!newPassword.Equals(confirmPassword))
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp.");
                return View();
            }

            string username = User.Identity.Name;
            var user = db.Users.SingleOrDefault(u => u.Username == username);

            if (user != null && user.Password == HashPassword(oldPassword))
            {
                // Cập nhật mật khẩu mới (đã hash)
                user.Password = HashPassword(newPassword);
                db.SaveChanges();

                // Chuyển hướng hoặc hiển thị thông báo thành công
                return RedirectToAction("ProfileInfo");
            }

            ModelState.AddModelError("oldPassword", "Mật khẩu cũ không đúng.");
            return View();
        }
    }
}