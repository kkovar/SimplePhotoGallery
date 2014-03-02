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

            // 2/26/14 as is the url /Gallery does not work becase the /Gallery/index does not have an Id
            // if this is null, return an error page?  A default value may not be found
            Object galleryDir = Id;
            return View(galleryDir);
        }

    }
}
