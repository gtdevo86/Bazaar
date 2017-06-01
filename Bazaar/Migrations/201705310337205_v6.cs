namespace Bazaar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v6 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Listing", "OwnerId", "OwnerUserName");
            RenameColumn("dbo.Listing", "BuyerId", "BuyerUserName");

        }
        
        public override void Down()
        {

        }
    }
}
