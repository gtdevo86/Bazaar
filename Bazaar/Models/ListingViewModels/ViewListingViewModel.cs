using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bazaar.Models.ListingViewModels
{
    public class ViewListingViewModel
    {
        public Listing CurrentListing { get; set; }
    }
}