using SimplePhotoGallery.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using LevDan.Exif;

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
 
        // foreign key to self
        public int ProcessedImagesGalleryImageId;
        // navigation property to any scaled, enhanced, colorized images based on this image
        public virtual ICollection<ProcessedImage> ProcessedImages  { get; set; }
 
        // the path to use in the url
        public string UrlPath { get; set; }
        

        // navigation property to galleries which it belongs to
        public List<Gallery> Galleries { get; set; }

        public GalleryImage(GalleryImage rhs) 
        {
            this.Commentary = rhs.Commentary;
            this.Title = rhs.Title;

        }



    }

    /// problem with design:
    /// the base class contains fields like commentary that should apply to the image as
    /// a photo that was taken with some intent by the photographer
    /// the processed images are not created with the same intent, they are just copies that
    /// exist only to provide an alternative representation to enhance display
    /// One alternative might be to have a set of processing objects that can be associated with
    /// an image. These would be able to generate/retrieve an altered image based on a specified
    /// transformation. This way the processed images would not have the creation specific attributes 
    /// like location, subject, and comments/observations/motivations from the photographer 
    /// of the original image.
    /// The OriginalImage class does have some of this separation of concerns but having a class to 
    /// represent processing as opposed to creation would be very useful and introduces a
    /// decoupling that would let us to have an unlimited set of derived images based on the 
    /// original without the constraints of classes like scaledImage, etc.
    /// 


    public class OriginalImage : GalleryImage
    {

        // fields to help filter/organize into galleries.
        // "Kiszka", Eve, etc
        // could be provided by Exif
        public string Subject { get; set; }
        // "Michigan Ave" this could be suplemented by EXIF geolocation
        public string Location { get; set; }
        // "Eve's Bday"
        public string Event { get; set; }

        // note that we also have EXIF metadata that can be extracted

        // this is problematic since it ties this class to ScaledImage
        // 
        public void AddThumbs(List<Thumbnail> thumbsToGenerate)
        {
            // read metadata and if the orientation is 90 cw or 90 ccw, perform the rotation
            // todo, refactor the tag processing into its own function, as AddThumbs should do just that
            Image imgPhotoToRotate = Image.FromFile(Filename);

            var exif = new ExifTagCollection(Filename);

            // todo, use other fields such as artist and exposure data and to populate related fields?

            var PropertyItems = imgPhotoToRotate.PropertyItems;
            // to
            var orientation = exif.Where(ex => ex.FieldName == "Orientation").FirstOrDefault();
            // if no orinentation property exists, then the camera figured out the correct height and width (eg cell phone cam, with accelerometer)
            // or if the orientation is reported as "normal" no need to rotate
            if (orientation != null &&
                orientation.Value != "The 0th row is at the visual top of the image, and the 0th column is the visual left-hand side.")
            {
                int angle = 0;
                if (orientation.Value == "The 0th row is the visual right-hand side of the image, and the 0th column is the visual top.")
                {
                    // rotate 90 degrees, this seems to be most common with camera that sense the orientation but somehow
                    // keep width to be the larger dimension and height the smaller, as in a point and shoot
                    angle = 90;
                }
                else if (orientation.Value ==  "The 0th row is the visual left-hand side of the image, and the 0th column is the visual bottom.")
                {
                    angle = 270;
                }
                Image imgPhoto = ImageProcessor.RotateImage(imgPhotoToRotate, angle);
                foreach(var pi in imgPhotoToRotate.PropertyItems)
                {
                    // todo, create symbolic constants for Id values
                    // copy properties, except for orientation, which we set to "normal"
                    // i.e. "The 0th row is at the visual top of the image, and the 0th column is the visual left-hand side."
                    if (pi.Id == 0x112)
                    {
                        ushort normal = 1;
                        pi.Value = BitConverter.GetBytes(normal);
                    }
                    imgPhoto.SetPropertyItem(pi);
                }
                imgPhotoToRotate.Dispose();
                imgPhoto.Save(Filename, ImageFormat.Jpeg);

            }

            // here I want to set the orientation to "1" or normal after rotating it
            // imgPhotoToRotate.SetPropertyItem(PropertyItems[0]);

            //

            // get the standard thumbs
            foreach (var th in thumbsToGenerate)
            {
                ScaledImage si = new ScaledImage(th, this);
                si.Process();
            }
        }

        public void RotateSelfAndThumbs(float angle = 90)
        {
            // take an image file and create a new file that is rotated by 90

            // todo: create a new file and update the file name
            Image imgPhotoToRotate = Image.FromFile(Filename);
            
            Image imgPhoto = ImageProcessor.RotateImage(imgPhotoToRotate, angle);
            imgPhotoToRotate.Dispose();
            imgPhoto.Save(Filename, ImageFormat.Jpeg);
            foreach (var thumb in ProcessedImages)
            {
                imgPhotoToRotate = Image.FromFile(thumb.Filename);
                imgPhoto = ImageProcessor.RotateImage(imgPhotoToRotate, angle);
                imgPhotoToRotate.Dispose();
                imgPhoto.Save(thumb.Filename, ImageFormat.Jpeg);
            }
            // imgPhoto.Dispose();



        }


    }

    // represents a image that has been altered, in this iteration it will be 
    // scaling to the appropriate thumbnail size
    public abstract class ProcessedImage : GalleryImage
    {
        // foreign key (to base image in GalleryImage table)
        public int BaseImageId;
        // navigation property to base image
        public virtual GalleryImage BaseImage { get; set; }
        // override to provide image manipulation
        public abstract void Process() ;

    }

    // I am inclined to put the processing code into the processed image,
    // as the original image should not depend on and have code for all image types
    public class ScaledImage : ProcessedImage
    {
        private Object myLock = new object();
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
                Image imgPhotoBase = Image.FromFile(this.BaseImage.Filename);

                Image imgPhoto = ImageProcessor.ConstrainProportions(imgPhotoBase, Thumbnail.MaxWidth, ImageProcessor.Dimensions.Width);
                imgPhotoBase.Dispose();

                // todo: give user a way to specify an alternative file name for the thumb
                Filename = Path.Combine(Path.GetDirectoryName(BaseImage.Filename), (Path.GetFileNameWithoutExtension(BaseImage.Filename) + "_" + Thumbnail.Description + Path.GetExtension(BaseImage.Filename)));

                imgPhoto.Save(Filename, ImageFormat.Jpeg);
               
                imgPhoto.Dispose();
                // todo, test that thumbnail images have a column that contains a parent image id
                // if the thumbnail exists, do not re-add it to the collection
                if (!BaseImage.ProcessedImages.Any(si => ((ScaledImage)si).Thumbnail.ThumbnailId == Thumbnail.ThumbnailId))
                {
                    BaseImage.ProcessedImages.Add(this);
                }
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
    public class GalleryImageProcessor
    {
        private GalleryContext db = new GalleryContext();

        public void ProcessPostUpload(OriginalImage img)
        {
            // right now, the thumbnail table contains only the
            // thumbs I need, if that changes, use a new query
            // todo: improve the extensibility of this-- it is tied to the database
            var thumbs = from th in db.Thumbnails select th;

            img.AddThumbs(thumbs.ToList());
            db.Images.Add(img);
            db.SaveChanges();

        }

    }

    public class Artist
    {
        public Artist()
        {
            Galleries = new List<Gallery>();
        }
        public int ArtistId { get; set; }
        public string Name { get; set; }
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
        public Gallery()
        {
            Images = new List<GalleryImage>();
        }
        public int GalleryId { get; set; }
        public string Name { get; set; }
        // navigation property to the images
        public List<GalleryImage> Images { get; set; }
        // n
        public List<Artist> Contributors { get; set; }
        public string ArtistsStatement { get; set; }
        public string Description { get; set; }

    }

    // wraps a directory with additional info about the images contained
    // for example "rank" or relevance based on the sizes and attributes of the image files
    // and the root directory (for example a subdirectory of Program Files would likely be a
    // location for non-user generated images)
    public class LocalImageDirectory
    {
        // if < 100 kb or so, probably just images used in a app
        public int averageImageSize;
        public string

    }

    // wrap a FileSystemInfo with additional data about the image
    // such as exif
    public class LocalImage : FileSystemInfo
    {
        public LocalImage()
        {

        }

        public int LocalImageId { get; set; }
        public string FileName { get { return this.Name ;} 
            set; }
        //public int F
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