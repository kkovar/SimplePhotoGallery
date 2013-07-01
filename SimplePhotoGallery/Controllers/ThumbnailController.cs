using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimplePhotoGallery.Models;

namespace SimplePhotoGallery.Controllers
{
    public class ThumbnailController : Controller
    {
        private GalleryContext db = new GalleryContext();

        //
        // GET: /Thumbnail/

        public ActionResult Index()
        {
            return View(db.Thumbnails.ToList());
        }

        //
        // GET: /Thumbnail/Details/5

        public ActionResult Details(int id = 0)
        {
            Thumbnail thumbnail = db.Thumbnails.Find(id);
            if (thumbnail == null)
            {
                return HttpNotFound();
            }
            return View(thumbnail);
        }

        //
        // GET: /Thumbnail/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Thumbnail/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Thumbnail thumbnail)
        {
            if (ModelState.IsValid)
            {
                db.Thumbnails.Add(thumbnail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thumbnail);
        }

        //
        // GET: /Thumbnail/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Thumbnail thumbnail = db.Thumbnails.Find(id);
            if (thumbnail == null)
            {
                return HttpNotFound();
            }
            return View(thumbnail);
        }

        //
        // POST: /Thumbnail/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Thumbnail thumbnail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thumbnail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thumbnail);
        }

        //
        // GET: /Thumbnail/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Thumbnail thumbnail = db.Thumbnails.Find(id);
            if (thumbnail == null)
            {
                return HttpNotFound();
            }
            return View(thumbnail);
        }

        //
        // POST: /Thumbnail/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Thumbnail thumbnail = db.Thumbnails.Find(id);
            db.Thumbnails.Remove(thumbnail);
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