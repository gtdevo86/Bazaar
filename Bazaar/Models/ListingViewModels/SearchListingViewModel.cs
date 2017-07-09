using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bazaar.Models.ListingViewModels
{
    public class SearchListingViewModel
    {
        public IEnumerable<Listing> Listings { get; set; }
        public string SearchTerms { get; set; }
        public int Distance { get; set; }
        public int MaxPages { get; set; }
        public int CurrentPage { get; set; }

        public class Category
        {
            public int CategoryId { get; set; }
            public string Value { get; set; }
        }

        public IEnumerable<Category> CategoryOptions = new List<Category>
        {
            new Category {CategoryId = 0, Value = "All"},
            new Category {CategoryId = 1, Value = "Auto Parts"},
            new Category {CategoryId = 2, Value = "Books"},
            new Category {CategoryId = 3, Value = "Clothes"},
            new Category {CategoryId = 4, Value = "Computers"},
            new Category {CategoryId = 5, Value = "DVDs"},
            new Category {CategoryId = 6, Value = "Electronics"},
            new Category {CategoryId = 7, Value = "Furniture"},
            new Category {CategoryId = 8, Value = "Jewelry"},
            new Category {CategoryId = 9, Value = "Kitchen Appliances"},
            new Category {CategoryId = 10, Value = "Tools"},
            new Category {CategoryId = 11, Value = "Toys"},
            new Category {CategoryId = 12, Value = "TVs"},
            new Category {CategoryId = 13, Value = "Video Games"},
            new Category {CategoryId = 0, Value = "Other"}
         };
        public String CategoryType { get; set; }

        public class DistanceItem
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

        public IEnumerable<DistanceItem> DistanceDropDown = new List<DistanceItem>
        {
            new DistanceItem {Value = 5, Text = "Within 5 Miles"},
            new DistanceItem {Value = 10, Text = "Within 10 Miles"},
            new DistanceItem {Value = 25, Text = "Within 25 Miles"},
            new DistanceItem {Value = 100, Text = "Within 100 Miles"},
            new DistanceItem {Value = 300, Text = "Within 300 Miles"},
            new DistanceItem {Value = 1000, Text = "Within 1000 Miles"},
         };
    }
}