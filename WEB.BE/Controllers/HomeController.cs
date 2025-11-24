using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.BE.Models;
using WEB.BE.Models.ViewModels;
using PagedList;

namespace WEB.BE.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            IQueryable<Product> products = db.Products.AsQueryable();

            // Load danh mục vào ViewModel
            model.Categories = db.Categories.ToList();

            // Tìm kiếm sản phẩm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDescription.Contains(searchTerm) ||
                    p.Category.CategoryName.Contains(searchTerm));
            }

            // Lấy 10 sản phẩm bán chạy nhất
            model.FeaturedProducts = products
                .OrderByDescending(p => p.OrderDetails.Count())
                .Take(10)
                .ToList();

            // Lấy 20 sản phẩm ít bán nhất + phân trang
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;

            model.NewProducts = products
                .OrderBy(p => p.OrderDetails.Count())
                .Take(20)
                .ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Home/ProductDetail/5 (Hiển thị chi tiết sản phẩm)
        public ActionResult ProductDetail(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Product pro = db.Products.Find(id);
            if (pro == null)
            {
                return HttpNotFound();
            }

            ProductDetailsVM model = new ProductDetailsVM();
            model.Product = pro;

            // Lấy các sản phẩm cùng danh mục (Loại trừ sản phẩm hiện tại)
            var relatedProductsQuery = db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID != pro.ProductID).AsQueryable();

            // Sản phẩm tương tự (Related Products)
            model.RelatedProducts = relatedProductsQuery.Take(8).ToList(); // 8 sản phẩm cùng danh mục

            // Sản phẩm bán chạy nhất cùng danh mục (Top Products)
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize; // Mặc định 3 sản phẩm/trang

            model.TopProducts = relatedProductsQuery
                .OrderByDescending(p => p.OrderDetails.Count())
                .Take(8) // Lấy 8 sản phẩm bán chạy nhất
                .ToPagedList(pageNumber, pageSize);

            // Tính toán giá trị tạm thời (estimatedValue)
            if (quantity.HasValue)
            {
                model.Quantity = quantity.Value;
                model.EstimatedValue = model.Quantity * model.Product.ProductPrice;
            }
            return View(model);
        }
        // POST: Home/ProductDetail
        [HttpPost]
        public ActionResult ProductDetail(ProductDetailsVM model)
        {
            // Logic xử lý nút Thêm vào Giỏ hàng tại đây (sẽ gọi CartController.AddItem)
            // ...
            return RedirectToAction("ProductDetail", new { id = model.Product.ProductID, quantity = model.Quantity });
        }
        public ActionResult ProductList(string searchTerm, int? page)
        {
            IQueryable<Product> products = db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDescription.Contains(searchTerm) ||
                    p.Category.CategoryName.Contains(searchTerm));
            }

            int pageNumber = page ?? 1;
            int pageSize = 12; // số lượng sản phẩm mỗi trang

            var model = products
                .OrderBy(p => p.ProductName)
                .ToPagedList(pageNumber, pageSize);

            return View(model);
        }
        public ActionResult ProductByCategory(int id, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 12; // số sản phẩm mỗi trang

            var products = db.Products
                .Where(p => p.CategoryID == id)
                .OrderBy(p => p.ProductName)
                .ToPagedList(pageNumber, pageSize);

            ViewBag.CategoryName = db.Categories
                .Where(c => c.CategoryID == id)
                .Select(c => c.CategoryName)
                .FirstOrDefault();

            return View(products);
        }

    }
}