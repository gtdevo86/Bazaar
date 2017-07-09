namespace Bazaar.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Listing", "Name", c => c.String(nullable: false, maxLength: 37));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Listing", "Name", c => c.String(nullable: false, maxLength: 40));
        }
    }
}
