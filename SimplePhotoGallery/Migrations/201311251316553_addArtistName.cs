namespace SimplePhotoGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addArtistName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Artists", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Artists", "Name");
        }
    }
}
