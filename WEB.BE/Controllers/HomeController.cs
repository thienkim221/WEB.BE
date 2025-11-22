using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.BE.Models;
using WEB.BE.Models.ViewModel;
using PagedList;

namespace WEB.BE.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // GET: Home/Index (Hiển thị Trang chủ)
        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            IQueryable<Product> products = db.Products.AsQueryable();

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

            // Lấy 20 sản phẩm mới và phân trang
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize; // Mặc định 6 sản phẩm / trang

            model.NewProducts = products
                .OrderBy(p => p.OrderDetails.Count()) // Ít người mua nhất
                .Take(20) // Lấy 20 sản phẩm
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
    }
}