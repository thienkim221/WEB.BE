using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.BE.Models;
using WEB.BE.Models.ViewModel;
using System.Data.Entity; // Required for .Include()
using PagedList;


namespace WEB.BE.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // Lấy dịch vụ giỏ hàng
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // GET: Cart/Index (Hiển thị giỏ hàng)
        public ActionResult Index(int? page)
        {
            var cart = GetCartService().GetCart();

            // Xử lý phân trang cho Sản phẩm tương tự (Similar Products)
            if (cart.Items.Any())
            {
                // Lấy tất cả CategoryID trong giỏ hàng để tìm sản phẩm tương tự
                var categoryIdsInCart = cart.Items
                    // Lấy CategoryID bằng cách lookup CategoryName trong DB
                    .Select(i => db.Categories.FirstOrDefault(c => c.CategoryName == i.Category)?.CategoryID)
                    .Where(id => id.HasValue)
                    .Cast<int>()
                    .ToList();

                // Lấy ProductIDs hiện có trong giỏ
                var productIdsInCart = cart.Items.Select(i => i.ProductID).ToList();

                // Lấy các sản phẩm tương tự: cùng CategoryID, không nằm trong giỏ
                var similarProducts = db.Products
                    .Where(p => categoryIdsInCart.Contains(p.CategoryID) && !productIdsInCart.Contains(p.ProductID))
                    .OrderBy(p => p.ProductName)
                    .AsQueryable();

                // Áp dụng phân trang
                int pageNumber = page ?? 1;
                cart.SimilarProducts = similarProducts.ToPagedList(pageNumber, cart.PageSize);
            }

            return View(cart);
        }

        // POST: Cart/AddToCart (Thêm sản phẩm vào giỏ)
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            // Eager loading Category để lấy CategoryName
            Product product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);

            if (product != null)
            {
                var cartService = GetCartService();
                cartService.GetCart().AddItem(
                    product.ProductID,
                    product.ProductImage,
                    product.ProductName,
                    product.ProductPrice,
                    quantity,
                    // Lấy tên danh mục
                    product.Category.CategoryName
                );
            }
            return RedirectToAction("Index");
        }

        // POST: Cart/RemoveItem (Xóa sản phẩm khỏi giỏ)
        public ActionResult RemoveItem(int id)
        {
            GetCartService().GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        // POST: Cart/ClearCart (Xóa hết giỏ hàng)
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        // POST: Cart/UpdateQuantity (Cập nhật số lượng)
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            GetCartService().GetCart().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
    }
}
