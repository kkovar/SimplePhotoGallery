namespace SimplePhotoGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProcessedImageNavigation : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GalleryGalleryImages", newName: "GalleryImageGalleries");
            RenameTable(name: "dbo.ArtistGalleries", newName: "GalleryArtists");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.GalleryArtists", newName: "ArtistGalleries");
            RenameTable(name: "dbo.GalleryImageGalleries", newName: "GalleryGalleryImages");
        }
    }
}
