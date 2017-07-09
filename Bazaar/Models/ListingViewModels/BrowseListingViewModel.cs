using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bazaar.Models.ListingViewModels
{
    public class BrowseListingViewModel
    {
        public IEnumerable<Listing> Listings { get; set; }
        public int MaxPages { get; set; }
        public int CurrentPage { get; set; }
        public string Category { get; set; }
        public int Distance { get; set; }


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