namespace Bazaar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v4 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Listing");
            AlterColumn("dbo.Listing", "ListingId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Listing", "ListingId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Listing");
            AlterColumn("dbo.Listing", "ListingId", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.Listing", "ListingId");
        }
    }
}