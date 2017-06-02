namespace Bazaar.Migrations
{
    using Bazaar.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Bazaar.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Bazaar.Models.ApplicationDbContext context)
        {
            for (var x = 1; x <= 100; x++)
            {
                context.Listings.AddOrUpdate(
                    new Listing("Test object " + x, 5, "objects to test the program", "Content/images/Placeholder.png", "Other", "fosh", "89128")
                    );
            }
        }
    }
}
