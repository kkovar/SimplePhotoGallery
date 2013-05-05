namespace SimplePhotoGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GalleryImages",
                c => new
                    {
                        GalleryImageId = c.Int(nullable: false, identity: true),
                        Filename = c.String(),
                        Title = c.String(),
                        Commentary = c.String(),
                        ScaledSize = c.String(),
                        ParentImage_GalleryImageId = c.Int(),
                    })
                .PrimaryKey(t => t.GalleryImageId)
                .ForeignKey("dbo.GalleryImages", t => t.ParentImage_GalleryImageId)
                .Index(t => t.ParentImage_GalleryImageId);
            
            CreateTable(
                "dbo.Galleries",
                c => new
                    {
                        GalleryId = c.Int(nullable: false, identity: true),
                        ArtistsStatement = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.GalleryId);
            
            CreateTable(
                "dbo.GalleryGalleryImages",
                c => new
                    {
                        Gallery_GalleryId = c.Int(nullable: false),
                        GalleryImage_GalleryImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Gallery_GalleryId, t.GalleryImage_GalleryImageId })
                .ForeignKey("dbo.Galleries", t => t.Gallery_GalleryId, cascadeDelete: true)
                .ForeignKey("dbo.GalleryImages", t => t.GalleryImage_GalleryImageId, cascadeDelete: true)
                .Index(t => t.Gallery_GalleryId)
                .Index(t => t.GalleryImage_GalleryImageId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.GalleryGalleryImages", new[] { "GalleryImage_GalleryImageId" });
            DropIndex("dbo.GalleryGalleryImages", new[] { "Gallery_GalleryId" });
            DropIndex("dbo.GalleryImages", new[] { "ParentImage_GalleryImageId" });
            DropForeignKey("dbo.GalleryGalleryImages", "GalleryImage_GalleryImageId", "dbo.GalleryImages");
            DropForeignKey("dbo.GalleryGalleryImages", "Gallery_GalleryId", "dbo.Galleries");
            DropForeignKey("dbo.GalleryImages", "ParentImage_GalleryImageId", "dbo.GalleryImages");
            DropTable("dbo.GalleryGalleryImages");
            DropTable("dbo.Galleries");
            DropTable("dbo.GalleryImages");
        }
    }
}
