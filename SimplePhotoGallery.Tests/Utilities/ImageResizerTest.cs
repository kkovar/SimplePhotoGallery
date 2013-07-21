using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplePhotoGallery.Utilities;
using System.Drawing;
using System.Drawing.Imaging;
using SimplePhotoGallery.Models;
using System.Linq;

namespace SimplePhotoGallery.Tests.Utilities
{
    [TestClass]
    public class ImageResizerTest
    {
        //[TestMethod]
        public void TestMethod1()
        {
            //set a working directory
            string WorkingDirectory = @".\";

            // note that this worked in the sample project but the file needs to be copied
            //create a image object containing a vertical photograph
            Image imgPhotoVert = Image.FromFile(WorkingDirectory + @"\imageresize_vert.jpg");
            Image imgPhotoHoriz = Image.FromFile(WorkingDirectory + @"\imageresize_horiz.jpg");
            Image imgPhoto = null;

            imgPhoto = ImageResize.ScaleByPercent(imgPhotoVert, 50);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_1.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageResize.ConstrainProportions(imgPhotoVert, 200, ImageResize.Dimensions.Width);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_2.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageResize.FixedSize(imgPhotoVert, 200, 200);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_3.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageResize.Crop(imgPhotoVert, 200, 200, ImageResize.AnchorPosition.Center);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_4.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageResize.Crop(imgPhotoHoriz, 200, 200, ImageResize.AnchorPosition.Center);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_5.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();


        }

        [TestMethod]
        public void SetupThumbnails()
        {
            var db = new GalleryContext();

            //db.Thumbnails.Where(t => t.Description == "medium");

            var thumbs = from t in db.Thumbnails where t.MaxHeight == 600 select t;

            var tu = thumbs.First();

           // var tu = db.Thumbnails.();

            //Thumbnail tn = new Thumbnail();
            //tn.Description = "medium";
            //tn.MaxWidth = 600;
            //db.Thumbnails.Add(tn);
            //db.Thumbnails.SaveChanges();

            GalleryImage master = new GalleryImage();
            master.Filename = @"C:\Users\Ken\Documents\GitHub\SimplePhotoGallery\SimplePhotoGallery\Images\img_0519.jpg";
            db.Images.Add(master);

            var mediumThumb = new ScaledImage(tu, master);

            // this adds the medium thumb to the ProcessedImages collection
            mediumThumb.Process();

            // todo: add initialization code to ScaledImage to pass in BaseImage and Thumbnail
            //master.CreateThumbnail(tu, "medium");
            db.Images.Add(mediumThumb);

            db.SaveChanges();
            


        }
        [TestMethod]
        public void RetrieveParentImage()
        {
            // when the images are used, we need to convert the images to urls. So we should have
            // base filename and path in addition to extension so we can help build the url with routing
            // for this test we can use the file names
          
            GalleryContext db = new GalleryContext();

            //db.Thumbnails.Where(t => t.Description == "medium");

            //var images = from t in db.Images where t.ParentImage == null select t;

            var imgs = db.Images.OfType<ProcessedImage>().Where(i => i.BaseImage != null).ToList();

            var xi = 1;

            //var testMasterImage = images.First();
          


        }

    }
}
