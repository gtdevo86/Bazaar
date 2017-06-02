using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Bazaar.Models;
using My.Data.Annotations;
using System.Linq;
using System;

namespace Bazaar.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [Required]
        [StringLength(5)]
        [DataType(DataType.PostalCode)]
        public string zipcode { get; set; }

        [Required]
        [Precision(9, 6)]
        public Nullable<decimal> Longitude { get; set; }


        [Required]
        [Precision(9, 6)]
        public Nullable<decimal> Latitude { get; set; }

        [DefaultValue(0)]
        public int reviews { get; set; }

        [DefaultValue(0)]
        public float rating { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Latitude", this.Latitude.ToString()));
            userIdentity.AddClaim(new Claim("Longitude", this.Longitude.ToString()));
            userIdentity.AddClaim(new Claim("ZipCode", this.zipcode));
            userIdentity.AddClaim(new Claim("UserName", this.UserName));
            return userIdentity;
        }
        //IndexableCollection
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Listing> Listings { get; set; }
        public DbSet<ZipCodeManager> ZipCodes { get; set; }

        public ApplicationDbContext()   
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Properties()
                .Where(x => x.GetCustomAttributes(false).OfType<Precision>().Any())
                .Configure(c =>
                {
                    var attr = (Precision)c.ClrPropertyInfo.GetCustomAttributes(typeof(Precision), true).FirstOrDefault();
                    c.HasPrecision(attr.precision, attr.scale);
                });

            builder.Entity<Listing>().ToTable("Listing");
            builder.Entity<ZipCodeManager>().ToTable("ZipCodes");
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}