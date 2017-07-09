namespace Bazaar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Listing", "Name", c => c.String(nullable: false, maxLength: 40));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Listing", "Name", c => c.String(nullable: false, maxLength: 20));
        }
    }
}
