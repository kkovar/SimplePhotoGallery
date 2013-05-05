using System.Collections.Generic;
using System.Data.Entity;

namespace SimplePhotoGallery.Models
{

    public class GalleryImage
    {
        public int GalleryImageId { get; set; }
        // will be set if this a scaled image
        public GalleryImage ParentImage { get; set; }
        public string Filename { get; set; }
        public string Title { get; set; }
        public string Commentary { get; set; }
        // something like "tiny", "small", "large", etc.
        // tiny thumbnails will be used in overview galleries
        // medium will be used in the main gallery page
        // larger scaled images will be used to fill the page
        public string ScaledSize { get; set; }
        // navigation property to garlleries which it belongs to
        public virtual ICollection<Gallery> Galleries { get; set; }

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

    }
}