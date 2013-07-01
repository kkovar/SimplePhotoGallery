using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimplePhotoGallery.Models;
using System.Web.Optimization;

namespace SimplePhotoGallery.Controllers
{
    public class GalleryManagementController : Controller
    {
        private GalleryContext db = new GalleryContext();

        //
        // GET: /GalleryManagement/

        public ActionResult Index()
        {
            return View(db.Galleries.ToList());
        }

        //
        // GET: /GalleryManagement/Details/5

        public ActionResult Details(int id = 0)
        {
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        //
        // GET: /GalleryManagement/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /GalleryManagement/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Gallery gallery)
        {
            if (ModelState.IsValid)
            {
                db.Galleries.Add(gallery);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gallery);
        }

        //
        // GET: /GalleryManagement/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        //
        // POST: /GalleryManagement/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Gallery gallery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gallery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gallery);
        }

        //
        // GET: /GalleryManagement/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Gallery gallery = db.Galleries.Find(id);
            if (gallery == null)
            {
                return HttpNotFound();
            }
            return View(gallery);
        }

        //
        // POST: /GalleryManagement/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Gallery gallery = db.Galleries.Find(id);
            db.Galleries.Remove(gallery);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}