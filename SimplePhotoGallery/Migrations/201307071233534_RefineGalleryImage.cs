namespace SimplePhotoGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefineGalleryImage : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.GalleryImages", "Extension");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GalleryImages", "Extension", c => c.String());
        }
    }
}
