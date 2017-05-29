using Glimpse.Ado.AlternateType;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Bazaar.Models;
using Bazaar.Models.ListingViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Bazaar.Controllers
{
    [Authorize]
    public class ListingController : Controller
    {

        private Decimal userLong;
        private Decimal userLat;
        private string userZipcode;
        private int RecordsPerPage = 25;
        public string PlaceHolder = "~/Content/images/Placeholder.png";

        public ListingController()
        {
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            userLat = System.Convert.ToDecimal(prinicpal.Claims.Where(c => c.Type == "Latitude").Select(c => c.Value).SingleOrDefault());
            userLong = System.Convert.ToDecimal(prinicpal.Claims.Where(c => c.Type == "Longitude").Select(c => c.Value).SingleOrDefault());
            userZipcode =prinicpal.Claims.Where(c => c.Type == "ZipCode").Select(c => c.Value).SingleOrDefault();
        }

        // GET: /Listings
        [AllowAnonymous]
        public ActionResult Index()
        {
            RedirectToAction("Browse");
            return View();
        }

        // GET: /Listings/Add
        [HttpGet]
        public ActionResult Add()
        {
            var model = new AddListingViewModel();
            model.CategoryType = "Other";
            return View(model);
        }

        // POST: /Listings/Add
        [HttpPost]
        public ActionResult Add(AddListingViewModel model)
        {
            if (ModelState.IsValid)
            {

                using (var context = new ApplicationDbContext())
                { 
                    var result = context.Listings.Add(new Listing(model.Name, model.price, model.description, PlaceHolder, model.CategoryType, User.Identity.GetUserId(),userZipcode));
                    context.SaveChanges();
                }
                return RedirectToAction("Index","Home",null);
            }
            return View(model);
        }

        // GET: /Listings/Browse
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Browse(string Category, int distance, int page)
        {
            var model = new BrowseListingViewModel();
            using (var context = new ApplicationDbContext())
            {

                //fige out max degrees to find
                Decimal DeltaDegrees = System.Convert.ToDecimal(distance / 69.0);
                var FilteredZipCodes = from z in context.ZipCodes
                                       where (z.Latitude >= userLat - DeltaDegrees && z.Latitude <= userLat + DeltaDegrees &&
                                       z.Longitude >= userLong - DeltaDegrees && z.Longitude <= userLong + DeltaDegrees)
                                       select z;
                var FilteredListing = (from l in context.Listings
                                       where (FilteredZipCodes.Select(z=>z.Zipcode).Contains(l.OwnerZipcode) && (l.category == Category || Category == "All"))
                                       select l).Skip((page - 1) * RecordsPerPage).Take(RecordsPerPage);
   
                model.Listings = FilteredListing;
            }
            return View(model);
        }

        // GET: /Listings/Manage
        [HttpGet]
        public ActionResult Manage()
        {
            var model = new ManageListingViewModel();
            using (var context = new ApplicationDbContext())
            {
               
                var FilteredListing = from l in context.Listings
                                       where (l.OwnerId == User.Identity.GetUserId())
                                       select l;

                model.Listings = FilteredListing;
            }
            return View(model);
        }

        // POST: /Listings/Delete
        [HttpGet]
        [Authorize]
        public ActionResult Delete(RemoveListingViewModel model, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {

                using (var context = new ApplicationDbContext())
                {
                    if (context.Listings.Find(model.ListingId).OwnerId == User.Identity.GetUserId())
                    {
                        var result = context.Listings.Remove(context.Listings.Find(model.ListingId));
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new HttpException(401, "Unauthorized access");
                    }
                }
                return RedirectToAction(ViewData["ReturnUrl"].ToString());
            }
            return View();
        }

        // GET: /Listings/Search
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Search(String Searchterm, string Category, int distance, int page)
        {
            var model = new BrowseListingViewModel();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] searchTerms = Searchterm.Split(delimiterChars);
            using (var context = new ApplicationDbContext())
            {

                //fige out max degrees to find
                Decimal DeltaDegrees = System.Convert.ToDecimal(distance / 69.0);
                var FilteredZipCodes = from z in context.ZipCodes
                                       where (z.Latitude >= userLat - DeltaDegrees && z.Latitude <= userLat + DeltaDegrees &&
                                       z.Longitude >= userLong - DeltaDegrees && z.Longitude <= userLong + DeltaDegrees)
                                       select z;
                var FilteredListing = (from l in context.Listings
                                       where (FilteredZipCodes.Select(z => z.Zipcode).Contains(l.OwnerZipcode) && (l.category == Category || Category == "All"))
                                       where (searchTerms.Any(term => l.name.Contains(term)))
                                       select l).Skip((page - 1) * RecordsPerPage).Take(RecordsPerPage);

                model.Listings = FilteredListing;
            }
            return View(model);
        }
    }
}