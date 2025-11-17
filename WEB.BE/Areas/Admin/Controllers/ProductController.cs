using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WEB.BE.Data;
using WEB.BE.Models;

namespace WEB.BE.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private WEBBEContext db = new WEBBEContext();

        // GET: Admin/Products
        public ActionResult Index(int? page, string q, decimal? minPrice, decimal? maxPrice)
        {
            var products = db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                products = products.Where(x =>
                    x.ProductName.Contains(q) ||
                    x.ProductDecription.Contains(q) ||
                    x.Category.CategoryName.Contains(q)
                );
            }

            if (minPrice.HasValue)
                products = products.Where(x => x.ProductPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(x => x.ProductPrice <= maxPrice.Value);

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(products.OrderBy(x => x.ProductID).ToPagedList(pageNumber, pageSize));
        }




        // GET: Admin/Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = await db.Products.FindAsync(id);
            if (product == null) return HttpNotFound();

            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            var categories = db.Categories.ToList();
            ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");


            return View();
        }


        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage")] Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Upload ảnh nếu có
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                    ImageFile.SaveAs(path);
                    product.ProductImage = fileName; // gán tên file
                }

                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);

            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = await db.Products.FindAsync(id);
            if (product == null) return HttpNotFound();

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage")] Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Upload ảnh mới nếu có
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                    ImageFile.SaveAs(path);
                    product.ProductImage = fileName;
                }

                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Product product = await db.Products.FindAsync(id);
            if (product == null) return HttpNotFound();

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}