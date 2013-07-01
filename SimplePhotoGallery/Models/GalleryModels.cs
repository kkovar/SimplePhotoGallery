using SimplePhotoGallery.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;

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
        }
        public int GalleryImageId { get; set; }
        // will be set if this a scaled image
        public virtual GalleryImage ParentImage { get; set; }

        public string Filename { get; set; }
        // this would be something creative like "Lady in White Dress"!
        public string Title { get; set; }
        // longer description or critical note
        public string Commentary { get; set; }
        // fields to help filter/organize into galleries.
        // "Kiszka", Eve, etc
        public string Subject { get; set; }
        // "Michigan Ave" 
        public string Location { get; set; }
        // "Eve's Bday"
        public string Event { get; set; }
        // note that we also have EXIF metadata that can be extracted

        private string _ext;
        public string Extension
        {
            get
            {
                if (_ext == null)
                {
                    return ".jpg";
                }
                else return _ext;

            }
            set 
            {
                _ext = value;

            }
        }

        // property that contains a reference to a thumbnail,set
        public virtual Thumbnail Thumbnail { get; set; }

        // navigation property to garlleries which it belongs to
        public virtual ICollection<Gallery> Galleries { get; set; }

        public GalleryImage(GalleryImage rhs) 
        {
            this.Commentary = rhs.Commentary;
            this.Extension = rhs.Extension;
            this.Title = rhs.Title;

        }

        public GalleryImage CreateThumbnail(Thumbnail tn, string thumbName)
        {
           try
            {
                Image imgPhotoVert = Image.FromFile(Filename);

                Image imgPhoto = ImageResize.ConstrainProportions(imgPhotoVert, tn.MaxWidth, ImageResize.Dimensions.Width);
                var fn = thumbName;

                if (fn == null)
                {
                    fn = Filename + "_" + tn.Description + Extension;

                }
                imgPhoto.Save(fn, ImageFormat.Jpeg);
                imgPhoto.Dispose();
                var thumbImage = new GalleryImage(this);
               // todo, test that thumbnail images have a column that contains a parent image id
                thumbImage.ParentImage = this;
                thumbImage.Filename = fn;
                thumbImage.Thumbnail = tn;
                return thumbImage;

            }
            catch (Exception e)
            {
                // log the exception
                return null;
            }
        }

    }

    public class Artist
    {
        public int ArtistId { get; set; }
        // navigation property to the galleries that
        public virtual ICollection<Gallery> Galleries { get; set; }
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
        public virtual ICollection<GalleryImage> Images { get; set; }
        // n
        public virtual ICollection<Artist> Contributors { get; set; }
        public string ArtistsStatement { get; set; }
        public string Description { get; set; }

    }

    public class GalleryContext : DbContext
    {
        public DbSet<GalleryImage> Images { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Thumbnail> Thumbnails { get; set; }
        public DbSet<Artist> Artists { get; set; }

    }
}