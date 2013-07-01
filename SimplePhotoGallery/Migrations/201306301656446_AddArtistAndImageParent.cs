namespace SimplePhotoGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArtistAndImageParent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Artists",
                c => new
                    {
                        ArtistId = c.Int(nullable: false, identity: true),
                        ArtistsStatement = c.String(),
                        Bio = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        PostalCode = c.String(),
                        Country = c.String(),
                        Phone = c.String(),
                    })
                .PrimaryKey(t => t.ArtistId);
            
            CreateTable(
                "dbo.ArtistGalleries",
                c => new
                    {
                        Artist_ArtistId = c.Int(nullable: false),
                        Gallery_GalleryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Artist_ArtistId, t.Gallery_GalleryId })
                .ForeignKey("dbo.Artists", t => t.Artist_ArtistId, cascadeDelete: true)
                .ForeignKey("dbo.Galleries", t => t.Gallery_GalleryId, cascadeDelete: true)
                .Index(t => t.Artist_ArtistId)
                .Index(t => t.Gallery_GalleryId);
            
            AddColumn("dbo.GalleryImages", "Subject", c => c.String());
            AddColumn("dbo.GalleryImages", "Location", c => c.String());
            AddColumn("dbo.GalleryImages", "Event", c => c.String());
        }
        
        public override void Down()
        {
            DropIndex("dbo.ArtistGalleries", new[] { "Gallery_GalleryId" });
            DropIndex("dbo.ArtistGalleries", new[] { "Artist_ArtistId" });
            DropForeignKey("dbo.ArtistGalleries", "Gallery_GalleryId", "dbo.Galleries");
            DropForeignKey("dbo.ArtistGalleries", "Artist_ArtistId", "dbo.Artists");
            DropColumn("dbo.GalleryImages", "Event");
            DropColumn("dbo.GalleryImages", "Location");
            DropColumn("dbo.GalleryImages", "Subject");
            DropTable("dbo.ArtistGalleries");
            DropTable("dbo.Artists");
        }
    }
}
