namespace SimplePhotoGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddURLToGalleryImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GalleryImages", "UrlPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GalleryImages", "UrlPath");
        }
    }
}
