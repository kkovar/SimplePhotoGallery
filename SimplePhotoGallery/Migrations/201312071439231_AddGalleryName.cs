namespace SimplePhotoGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGalleryName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Galleries", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Galleries", "Name");
        }
    }
}
