using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimplePhotoGallery.Models;
using System.Web.Routing;

namespace SimplePhotoGallery.Controllers
{
    public class ImagesController : Controller
    {
        private GalleryContext db = new GalleryContext();

        //
        // GET: /Images/
        // todo 2/26/14 determine if this is needed
        [OutputCache(Duration = 1, VaryByParam = "none")]
        public ActionResult Index()
        {
            // select only the images that have not been assigned to galleries
            //  Where(i => (i.Galleries == null))
            //  todo, improve the model

            // note that the Include call populates the Galleries member, if left off, Galleries.Count is 0
            // example: http://stackoverflow.com/questions/19131306/populating-navigation-properties-of-navigation-properties
            var unassigned = db.Images.Include(img => img.Galleries).ToArray();

            var originals = unassigned.Where(img => (img is OriginalImage && img.Galleries.Count == 0));
                
            var galleries = 
                new SelectList(db.Galleries.ToArray().Select(x => new {value = x.GalleryId, text = x.Name}), 
                "value", "text", "");

            //var galleryList = db.Galleries.

            ViewBag.galleries = galleries;
            //    .Where();
            return View(originals);
        }

        public ActionResult IndexAll()
        {
            // select only the images that have not been assigned to galleries
            //  Where(i => (i.Galleries == null))
            //  todo, improve the model

            // note that the Include call populates the Galleries member, if left off, Galleries.Count is 0
            // example: http://stackoverflow.com/questions/19131306/populating-navigation-properties-of-navigation-properties
            var unassigned = db.Images.Include(img => img.Galleries).ToArray();

            var originals = unassigned.Where(img => (img is OriginalImage));

            var galleries =
                new SelectList(db.Galleries.ToArray().Select(x => new { value = x.GalleryId, text = x.Name }),
                "value", "text", "");

            //var galleryList = db.Galleries.

            ViewBag.galleries = galleries;
            //    .Where();
            return View("Index", originals);
        }

        // PUT: /Images/Index


        //
        // GET: /Images/Details/5

        public ActionResult Details(int id = 0)
        {
            GalleryImage galleryimage = db.Images.Find(id);
            if (galleryimage == null)
            {
                return HttpNotFound();
            }
            return View(galleryimage);
        }

        //
        // GET: /Images/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Images/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GalleryImage galleryimage)
        {
            if (ModelState.IsValid)
            {
                db.Images.Add(galleryimage);
                db.SaveChanges();
                return RedirectToAction("Index", "Images");
            }

            return View(galleryimage);
        }

        //
        // GET: /Images/Edit/5

        [OutputCache(Duration = 1, VaryByParam = "none")]
        public ActionResult Edit(int id = 0)
        {
            GalleryImage galleryimage = db.Images.Find(id);
            if (galleryimage == null)
            {
                return HttpNotFound();
            }
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return View(galleryimage);
        }

        //
        // POST: /Images/Edit/5

        [HttpPost]
        // todo, restore this when I put this token in the assign gallery form: [ValidateAntiForgeryToken]
        public ActionResult Edit(GalleryImage galleryimage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(galleryimage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(galleryimage);
        }
        [HttpPost]
        // todo, restore this when I put this token in the assign gallery form: [ValidateAntiForgeryToken]
        public ActionResult AddGallery(int id = 0)
        {
            GalleryImage galleryimage = db.Images.Find(id);
            if (galleryimage == null)
            {
                return HttpNotFound();
            }
 
            var galleryId = int.Parse(Request.Form["gallery"]);
            if (galleryId != null)
            {
                var gallery = db.Galleries.Find(galleryId);
                if (gallery != null && ModelState.IsValid)
                {
                    galleryimage.Galleries.Add(gallery);
                    gallery.Images.Add(galleryimage);
                    db.Entry(galleryimage).State = EntityState.Modified;
                    db.Entry(gallery).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Images/Delete/5

        public ActionResult Delete(int id = 0)
        {
            GalleryImage galleryimage = db.Images.Find(id);
            if (galleryimage == null)
            {
                return HttpNotFound();
            }
            return View(galleryimage);
        }

        //
        // POST: /Images/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GalleryImage galleryimage = db.Images.Find(id);
            db.Images.Remove(galleryimage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Images/Delete/5

        public ActionResult Rotate(int id, float angle)
        {
            GalleryImage galleryimage = db.Images.Find(id);
            if (galleryimage == null)
            {
                return HttpNotFound();
            }
            // now rotate
            var oi = galleryimage as OriginalImage;
            if (oi != null)
            {
                oi.RotateSelfAndThumbs(angle);
            }
            else
            {
                var pi = galleryimage as ProcessedImage;
                if (pi != null)
                {
                    // todo, try catch?
                    ((OriginalImage)pi.BaseImage).RotateSelfAndThumbs(angle);
                }

            }
            // refresh image with new
            //            galleryimage = db.Images.Find(id)
            RouteValueDictionary param = new RouteValueDictionary();
            param["id"] = galleryimage.GalleryImageId;
    
            return RedirectToAction("Edit", "Images", param);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}