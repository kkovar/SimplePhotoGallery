using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimplePhotoGallery.Controllers
{
    public class GalleryController : Controller
    {
        //
        // GET: /Gallery/

        public ActionResult Index(string Id)
        {
            // todo, improve the path mapping
            Object galleryDir = Id;
            return View(galleryDir);
        }

    }
}
