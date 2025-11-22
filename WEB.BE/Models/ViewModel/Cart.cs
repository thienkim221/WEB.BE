using PagedList; // Requires the PagedList NuGet package
using System.Collections.Generic;
using System.Linq;
using WEB.BE.Models.ViewModel; // Requires Product.cs and other entity models
namespace WEB.BE.Models.ViewModel
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>(); // REQUIRES CartItem.cs
        public IEnumerable<CartItem> Items => items;

        // Grouped items logic (for advanced cart view)
        public List<IGrouping<string, CartItem>> GroupedItems => items.GroupBy(i => i.Category).ToList();

        // Pagination properties
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 6;

        // Similar Products property
        public IPagedList<Product> SimilarProducts { get; set; }

        // Core methods used by CartController:

        public void AddItem(int productID, string productImage, string productName, decimal unitPrice, int quantity, string category)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductID == productID);
            if (existingItem == null)
            {
                items.Add(new CartItem { ProductID = productID, ProductImage = productImage, ProductName = productName, UnitPrice = unitPrice, Quantity = quantity, Category = category });
            }
            else
            {
                existingItem.Quantity += quantity;
            }
        }

        public void RemoveItem(int productId)
        {
            items.RemoveAll(i => i.ProductID == productId);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }

        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }
    }
}