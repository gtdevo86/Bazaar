using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bazaar.Models.ListingViewModels
{
    public class BrowseListingViewModel
    {
        public IEnumerable<Listing> Listings { get; set; }
    }
}