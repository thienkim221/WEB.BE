using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WEB.BE.Models;
using WEB.BE.Models.ViewModels;
using PagedList;

namespace WEB.BE.Views.Cart
{
    public class Cart
    {
        // Items list: REQUIRES CartItem.cs
        private List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items => items;

        // Grouped items logic
        public List<IGrouping<string, CartItem>> GroupedItems => items.GroupBy(i => i.Category).ToList();

        // Pagination properties
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 6;

        // Similar Products property
        // REQUIRES Product entity to be available via using _24DH110457_MyStore.Models; 
        public IPagedList<Product> SimilarProducts { get; set; }

        // Methods used by the View (@Model.TotalValue()) and Controller:
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

        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }

        public void ClearCart()
        {
            items.Clear();
        }
    }
}