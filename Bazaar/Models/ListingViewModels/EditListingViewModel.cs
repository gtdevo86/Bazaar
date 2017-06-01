using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bazaar.Models.ListingViewModels
{
    public class EditListingViewModel
    {
        [Required]
        public int ListingId { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(.01, float.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Display(Name = "Price")]
        public float price { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Name must be at least 5 characters", MinimumLength = 5)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "Description must be at least 5 characters", MinimumLength = 5)]
        [Display(Name = "Description of item \n(1000 characters max)")]
        public string description { get; set; }

        //[Required]
        [Display(Name = "Upload an image")]
        public string iurl { get; set; }

        public class Category
        {
            public int CategoryId { get; set; }
            public string Value { get; set; }
        }

        public IEnumerable<Category> CategoryOptions = new List<Category>
        {
            new Category {CategoryId = 0, Value = "Other"},
            new Category {CategoryId = 1, Value = "Video Games"}
         };

        [Required]
        [DisplayName("Pick a category")]
        public String CategoryType { get; set; }

        [Required]
        public String OwnerUserName { get; set; }

        [Required]
        public String BuyerUserName { get; set; }

        [Required]
        public bool Completed { get; set; }

        [Required]
        public string OwnerZipCode { get; set; }
    }
}