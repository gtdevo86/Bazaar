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
        public float price { get; set; }

        [Required]
        [StringLength(20)]
        public string name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string description { get; set; }

        [Required]
        [StringLength(450)]
        public String OwnerId { get; set; }

        [Required]
        [StringLength(5)]
        public string OwnerZipcode { get; set; }

        [StringLength(450)]
        public String BuyerId { get; set; }

        [Required]
        [DataType(DataType.ImageUrl)]
        public string image { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool completed { get; set; }

        [Required]
        [StringLength(40)]
        public String category { get; set; }

        public Listing(string Name, float Price, string Description, string imgUrl, String Category, string OwnerID, string zipcode)
        {
            name = Name;
            price = Price;
            description = Description;
            image = imgUrl;
            category = Category;
            completed = false;
            OwnerId = OwnerID;
            OwnerZipcode = zipcode;
        }

        public Listing()
        {

        }
    }
}