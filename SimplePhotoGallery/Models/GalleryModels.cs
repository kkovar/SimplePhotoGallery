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
        public string Title { get; set; }
        public string Commentary { get; set; }
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

        public Thumbnail Thumbnail { get; set; }
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

                Image imgPhoto = ImageResize.ConstrainProportions(imgPhotoVert, Thumbnail.MaxWidth, ImageResize.Dimensions.Width);
                var fn = thumbName;
                if (fn == null)
                {
                    fn = Filename + "_" + tn.Description + Extension;

                }
                imgPhoto.Save(fn, ImageFormat.Jpeg);
                // do we need to do this????? imgPhoto.Dispose();
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

    public class Gallery
    {
        public int GalleryId { get; set; }
        // navigation property to the images
        public virtual ICollection<GalleryImage> Images { get; set; }
        public string ArtistsStatement { get; set; }
        public string Description { get; set; }

    }

    public class GalleryContext : DbContext
    {
        public DbSet<GalleryImage> Images { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Thumbnail> Thumbnails { get; set; }

    }
}