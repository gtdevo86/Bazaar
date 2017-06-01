﻿using System;
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
    }
}