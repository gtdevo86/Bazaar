using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bazaar.Models.ListingViewModels
{
    public class AddListingViewModel
    {
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
            new Category {CategoryId = 1, Value = "Auto Parts"},
            new Category {CategoryId = 2, Value = "Books"},
            new Category {CategoryId = 3, Value = "Cloths"},
            new Category {CategoryId = 4, Value = "Computers"},
            new Category {CategoryId = 5, Value = "DVDs"},
            new Category {CategoryId = 6, Value = "Electronics"},
            new Category {CategoryId = 7, Value = "Furnature"},
            new Category {CategoryId = 8, Value = "Jewelry"},
            new Category {CategoryId = 9, Value = "Kitchen Appliances"},
            new Category {CategoryId = 10, Value = "Tools"},
            new Category {CategoryId = 11, Value = "Toys"},
            new Category {CategoryId = 12, Value = "Tvs"},
            new Category {CategoryId = 13, Value = "Video Games"}
         };

        [Required]
        [DisplayName("Pick a category")]
        public String CategoryType { get; set; }

}
}