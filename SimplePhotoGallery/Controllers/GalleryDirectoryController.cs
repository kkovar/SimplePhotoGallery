using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimplePhotoGallery;

namespace SimplePhotoGallery.Controllers
{
    public class GalleryDirectoryController : Controller
    {
        //
        // GET: /GalleryDirectory/

        public ActionResult Index()
        {
            // get the gallery table from the database

            Models.GalleryContext db = new Models.GalleryContext();

            // get the originals and their thumbs
            // if needed they will be rotated and assigned to galleries.
            var picsWithouGalleries = db.Images.Where(img => img.Galleries == null && img is Models.OriginalImage);
            return View(picsWithouGalleries.ToList());
        }

        //
        // GET: /GalleryDirectory/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /GalleryDirectory/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /GalleryDirectory/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /GalleryDirectory/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /GalleryDirectory/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /GalleryDirectory/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /GalleryDirectory/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
