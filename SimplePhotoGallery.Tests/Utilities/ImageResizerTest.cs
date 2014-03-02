using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplePhotoGallery.Utilities;
using System.Drawing;
using System.Drawing.Imaging;
using SimplePhotoGallery.Models;
using System.Linq;
using System.IO;
using System.Web.Mvc;
using System.Data.Entity;


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

            imgPhoto = ImageProcessor.ScaleByPercent(imgPhotoVert, 50);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_1.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageProcessor.ConstrainProportions(imgPhotoVert, 200, ImageProcessor.Dimensions.Width);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_2.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageProcessor.FixedSize(imgPhotoVert, 200, 200);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_3.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageProcessor.Crop(imgPhotoVert, 200, 200, ImageProcessor.AnchorPosition.Center);
            imgPhoto.Save(WorkingDirectory + @"\images\imageresize_4.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

            imgPhoto = ImageProcessor.Crop(imgPhotoHoriz, 200, 200, ImageProcessor.AnchorPosition.Center);
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

        [TestMethod]
        public void ShowRotateForm()
        {
            var f = new Form1();

            f.ShowDialog();
        }

        [TestMethod]
        public void RotateImage()
        {
            // take an image file and create a new file that is rotated by 90
            float angle = 90;
            //create a image object containing a vertical photograph
            Image imgPhotoToRotate = Image.FromFile(@"C:\Users\Ken\Documents\GitHub\simplePGClone\SimplePhotoGallery\SimplePhotoGallery\GalleryImages\dsc_0348.jpg");

            Image imgPhoto = ImageProcessor.RotateImage(imgPhotoToRotate, angle);
            imgPhoto.Save(@"C:\Users\Ken\Documents\GitHub\simplePGClone\SimplePhotoGallery\SimplePhotoGallery\GalleryImages\dsc_0348rotated.jpg", ImageFormat.Jpeg);
            imgPhoto.Dispose();

 
                 
        }

        [TestMethod]
        public void GetAllImages()
        {
            GalleryContext db = new GalleryContext();
            var allImages = db.Images.ToList();

        }

        [TestMethod]
        public void GetOriginalImages()
        {
            GalleryContext db = new GalleryContext();
            // get the original images
            // var allImages = db.Images.Where(img => img is ProcessedImage);

            var originals = from oi in db.Images.Include("ProcessedImages") where oi is OriginalImage select oi ;

            foreach (var orig in originals)
            {
            }
            // we will foreach 
        }



        [TestMethod]
        public void TestImageProcessor()
        {
            
            var ip = new GalleryImageProcessor();
            OriginalImage img = new OriginalImage();
            img.Filename = @"C:\Users\Ken\Documents\GitHub\SimplePhotoGallery\SimplePhotoGallery\Galleries\Canon1IMG_0050.JPG";
            img.Title = "Renee smiling";
            ip.ProcessPostUpload(img);
            
             
            
            // now we should test that we have four thumbnails:
            // large, medium, small and tiny
            GalleryContext db = new GalleryContext();
            
            var originalImage = db.Images.ToList();
                        // .Where(b => b.Title.Contains("James")).ToList();

            /*
                        .FirstOrDefault();
             *                         .Include("ProcessedImages")

            */


            foreach (var processedImage in originalImage)
            {
                var fn = processedImage.Filename;
            }
                
        }


        // ad hoc method to put url in the images
        [TestMethod]
        public void UpdateURLsInImages()
        {
            GalleryContext db = new GalleryContext();
            foreach (var img in db.Images)
            {
                img.UrlPath = "~/GalleryImages/" + Path.GetFileName(img.Filename);
                foreach (var thumb in img.ProcessedImages)
                {
                    thumb.UrlPath = "~/GalleryImages/" + Path.GetFileName(thumb.Filename);
                }
            }
            db.SaveChanges();

        }

        [TestMethod]
        public void AddImageToGallery()
        {

            GalleryContext db = new GalleryContext();
            var unassigned = db.Images.Include(img => img.Galleries).ToArray();

            var originals = unassigned.Where(img => (img is OriginalImage && img.Galleries.Count == 0));

            // hard code value, take third image
            var galleryimage = originals.ElementAt(3);

            //var galleries =
            //    new SelectList(db.Galleries.ToArray().Select(x => new { value = x.GalleryId, text = x.Name }),
            //    "value", "text", "");

            // hard coded value
            var galleryId = 2;
            if (galleryId != null)
            {
                var gallery = db.Galleries.Find(galleryId);
                if (gallery != null)
                {
                    galleryimage.Galleries.Add(gallery);
                    gallery.Images.Add(galleryimage);
                    db.Entry(galleryimage).State = EntityState.Modified;
                    db.Entry(gallery).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            originals = unassigned.Where(img => (img is OriginalImage && img.Galleries.Count == 0));

        }
    }
}
