using SimplePhotoGallery.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace SimplePhotoGallery.Models
{
    public class Thumbnail
    {
        public int ThumbnailId { get; set; }
        public int MaxHeight { get; set; }
        public int MaxWidth { get; set; }
        // something like "tiny", "small", "large", etc.
        // tiny thumbnails will be used in overview galleries
        // medium will be used in the main gallery page
        // larger scaled images will be used to fill the page
        public string Description { get; set; }

    }

    public class GalleryImage
    {
        public GalleryImage()
        {
            ProcessedImages = new List<ProcessedImage>();
            Galleries = new List<Gallery>();
        }
        public int GalleryImageId { get; set; }
 
        // local file system name
        public string Filename { get; set; }
        // this would be something creative like "Lady in White Dress"!
        public string Title { get; set; }
        // longer description or critical note
        public string Commentary { get; set; }
 
        // any scaled, enhanced, colorized images based on this image
        public List<ProcessedImage> ProcessedImages  { get; set; }
 
        


        // navigation property to garlleries which it belongs to
        public List<Gallery> Galleries { get; set; }

        public GalleryImage(GalleryImage rhs) 
        {
            this.Commentary = rhs.Commentary;
            this.Title = rhs.Title;

        }



    }

    public class OriginalImage : GalleryImage
    {
        // fields to help filter/organize into galleries.
        // "Kiszka", Eve, etc
        public string Subject { get; set; }
        // "Michigan Ave" 
        public string Location { get; set; }
        // "Eve's Bday"
        public string Event { get; set; }

        // note that we also have EXIF metadata that can be extracted

        // this is maybe problematic since it ties this class to ScaledImage
        public void AddThumbs(List<Thumbnail> thumbsToGenerate)
        {
            // get the standard thumbs
            foreach (var th in thumbsToGenerate)
            {
                ScaledImage si = new ScaledImage(th, this);
                si.Process();
            }
        }

    }

    // represents a image that has been altered, in this iteration it will be 
    // scaling to the appropriate thumbnail size
    public abstract class ProcessedImage : GalleryImage
    {
        public virtual GalleryImage BaseImage { get; set; }
        // override to provide image manipulation
        public abstract void Process() ;

    }

    // I am inclined to put the processing code into the processed image,
    // as the original image should not depend on and have code for all image types
    public class ScaledImage : ProcessedImage
    {
        // parameterless constructor for use with EF
        public ScaledImage()
        {
        }

        public ScaledImage(Thumbnail thumbNail, GalleryImage baseImage)
        {
            BaseImage = baseImage;
            Thumbnail = thumbNail;
        }
        // property that contains a reference to a thumbnail,set
        public virtual Thumbnail Thumbnail { get; set; }

        public override void Process()
        {

            try
            {
                // this assumes that the parent image has saved itself
                Image imgPhotoVert = Image.FromFile(this.BaseImage.Filename);

                Image imgPhoto = ImageResize.ConstrainProportions(imgPhotoVert, Thumbnail.MaxWidth, ImageResize.Dimensions.Width);

                // todo: give user a way to specify an alternative file name for the thumb
                Filename =  Path.Combine(Path.GetDirectoryName(BaseImage.Filename),( Path.GetFileNameWithoutExtension(BaseImage.Filename) + "_" + Thumbnail.Description + Path.GetExtension(BaseImage.Filename)));


                imgPhoto.Save(Filename, ImageFormat.Jpeg);
                imgPhoto.Dispose();
                // todo, test that thumbnail images have a column that contains a parent image id
                BaseImage.ProcessedImages.Add(this);

            }
            catch (Exception e)
            {
                // log the exception
                return ;
            }
        }        

    }

    // class to handle the image processing and persistence so the
    // file uploader does not have to
    // maybe should be a singleton?
    public class ImageProcessor
    {
        private GalleryContext db = new GalleryContext();

        public void ProcessPostUpload(OriginalImage img)
        {
            // right now, the thumbnail table contains only the
            // thumbs I need, if that changes, use a new query
            var thumbs = from th in db.Thumbnails select th;

            img.AddThumbs(thumbs.ToList());
            db.Images.Add(img);
            db.SaveChanges();

        }

    }

    public class Artist
    {
        public int ArtistId { get; set; }
        // navigation property to the galleries that
        public List<Gallery> Galleries { get; set; }
        public string ArtistsStatement { get; set; }
        public string Bio { get; set; }

        // simple minded contact info
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

       public string Phone { get; set; }
    }
    public class Gallery
    {
        public int GalleryId { get; set; }
        // navigation property to the images
        public List<GalleryImage> Images { get; set; }
        // n
        public List<Artist> Contributors { get; set; }
        public string ArtistsStatement { get; set; }
        public string Description { get; set; }

    }

    public class GalleryContext : DbContext
    {
        public DbSet<GalleryImage> Images { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Thumbnail> Thumbnails { get; set; }
        public DbSet<Artist> Artists { get; set; }
        //public DbSet<ProcessedImage> ProcessedImages { get; set; }

    }
}