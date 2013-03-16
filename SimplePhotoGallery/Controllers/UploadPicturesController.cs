using SimplePhotoGallery.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimplePhotoGallery.Controllers
{
    public class UploadPicturesController : Controller
    {
        //
        // GET: /UploadPictures/

        public ActionResult Index()
        {
            return View();
        }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Save()
    {
        foreach (string file in Request.Files)
        {
            var hpf = Request.Files[file]  ;
            if (hpf.ContentLength == 0)
                continue;
            string savedFileName = Path.Combine(
               AppDomain.CurrentDomain.BaseDirectory + @"\images",
               Path.GetFileName(hpf.FileName));
            hpf.SaveAs(savedFileName);

            //create a image object 
            Image imgPhotoOriginal = Image.FromFile(savedFileName);
            Image imgPhoto = null;

            imgPhoto = ImageResize.ScaleByPercent(imgPhotoOriginal, 20);

            // todo: rename file if it exists
            imgPhoto.Save(AppDomain.CurrentDomain.BaseDirectory + @"\images\thumbs\" + hpf.FileName, ImageFormat.Jpeg,);
            imgPhoto.Dispose();


        }
        return View();
    }




    }
}
