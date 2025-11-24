using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB.BE.Models;

namespace WEB.BE.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // INDEX
        public ActionResult Index()
        {
            var admins = db.Users.Where(u => u.UserRole == "Admin").ToList();
            return View(admins);
        }

        // CREATE
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.UserRole = "Admin";
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // EDIT
        public ActionResult Edit(int id)
        {
            var user = db.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // DELETE
        public ActionResult Delete(int id)
        {
            var user = db.Users.Find(id);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }

}