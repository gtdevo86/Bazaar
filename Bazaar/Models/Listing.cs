using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bazaar.Models
{
    public class Listing
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ListingId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public float Price { get; set; }

        [Required]
        [StringLength(37)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(450)]
        public String OwnerUserName { get; set; }

        [Required]
        [StringLength(5)]
        public string OwnerZipcode { get; set; }

        [StringLength(450)]
        public String BuyerUserName { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Completed { get; set; }

        [Required]
        [StringLength(40)]
        public String Category { get; set; }

        public Listing(string tName, float tPrice, string tDescription, string tImgUrl, String tCategory, string tUserName, string tZipcode)
        {
            Name = tName;
            Price = tPrice;
            Description = tDescription;
            Image = tImgUrl;
            Category = tCategory;
            Completed = false;
            OwnerUserName = tUserName;
            OwnerZipcode = tZipcode;
        }

        public Listing()
        {

        }
    }
}