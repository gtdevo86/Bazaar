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
using NinjaNye.SearchExtensions;
namespace Bazaar.Controllers
{
    /// <summary>
    /// Listing controller class defaults to Authorized attribute for most actions unless speficied 
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    [Authorize]
    public class ListingController : Controller
    {
   
        /// <summary>
        /// The Claim value for the current authorized Longitude
        /// </summary>
        private Decimal userLong;
        /// <summary>
        /// The Claim value for the current authorized latitude
        /// </summary>
        private Decimal userLat;

        /// <summary>
        /// The Claim value for the current authorized zipcode
        /// </summary>
        private string userZipcode;

        /// <summary>
        /// The Claim value for the current authorized username
        /// </summary>
        /// 
        public string userName;

        /// <summary>
        /// The records per page
        /// Value how many many records to show per page
        /// currently hardcoded in
        /// TODO: Eventually change this to a dropdown and allow the user to pick a manual record per page
        /// </summary>
        private int RecordsPerPage = 16;


        /// <summary>
        /// The Placeholder until I allow the user to manually upload a photo
        /// </summary>
        public string PlaceHolder = "~/Content/images/Placeholder.png";


        /// <summary>
        /// Initializes a new instance of the <see cref="ListingController"/> class.
        /// Grabs the Claims assocated with the authorized user.
        /// </summary>
        public ListingController()
        {
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            userLat = System.Convert.ToDecimal(prinicpal.Claims.Where(c => c.Type == "Latitude").Select(c => c.Value).SingleOrDefault());
            userLong = System.Convert.ToDecimal(prinicpal.Claims.Where(c => c.Type == "Longitude").Select(c => c.Value).SingleOrDefault());
            userZipcode =prinicpal.Claims.Where(c => c.Type == "ZipCode").Select(c => c.Value).SingleOrDefault();
            userName = prinicpal.Claims.Where(c => c.Type == "UserName").Select(c => c.Value).SingleOrDefault();
                
        }

        // GET: /Listing
        /// <summary>
        /// Indexes this instance.
        /// Default action for the Listing routing, Reroutes to the browse 
        /// </summary>
        /// <returns>returns a copy of the current view</returns>
        [AllowAnonymous]
        public ActionResult Index()
        {
            RedirectToAction("Browse");
            return View();
        }

        // GET: /Listing/Add
        /// <summary>
        /// Adds this instance using the Get method
        /// Creates the initial viewmodel for adding a listing to the database 
        /// Sets the default Categorytype to Other
        /// </summary>
        /// <returns>returns an empty instance of the listing view model with the category type defaulted to other</returns>
        [HttpGet]
        public ActionResult Add()
        {
            var model = new AddListingViewModel();
            model.CategoryType = "Other";
            return View(model);
        }

        // POST: /Listing/Add
        /// <summary>
        /// Post method for the add instance
        /// If the webform has all proper data, add it to the database
        /// </summary>
        /// <param name="model">This is the viewmodel of the objects from the form</param>
        /// <returns>If sucessfull reduirect to the homepage, otherwise show the model with the errors on it</returns>
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

        // GET: /Listing/Browse
        /// <summary>
        /// Set the viewdata type to browse so the Pageinator knows
        /// This allows for annonymous users, If so give each the claims a default value
        /// IF any of  the Queries are empty give them a default value
        /// 1. Get a list of all the current zipcodes that are close enough to users location
        /// 2. Figure out how many pages there are total
        /// 3. IF the query of current page is greater than max page then set the current page to max page
        /// 4a.     Filter the listings so each listing is located inside the filtered zipcodes in step 1 above
        /// 4b.     Filter the listings further so the Catoeries are equal, or show all if category equals all
        /// 4c,     Filter the listings once more by only showing listings that arent completed
        /// 5. Set the viewmodel equal to these Filtered Listings
        /// </summary>
        /// <param name="Category">Filter the database for this category type unless its equall to "all"</param>
        /// <param name="distance">Distance to show results by</param>
        /// <param name="page">The  current page to filter results by</param>
        /// <returns>retuns the viewmodel conataining a list of the filtered listings</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Browse(string Category, int? distance, int? page)
        {

            ViewData["Type"] = "Browse";
            if (Request.QueryString["page"] == null) page = 1;
            if (Request.QueryString["distance"] == null) distance = 5;
            if (Request.QueryString["Category"] == null) Category = "All";

            var model = new BrowseListingViewModel();
            float MaxPagesRaw;
            using (var context = new ApplicationDbContext())
            {
              
                model.CurrentPage = (int)page;
                model.Distance = (int)distance;
                model.Category = Category;


                //set a default location if not logged in
                if(!Request.IsAuthenticated)
                {
                    var defaultZip = context.ZipCodes.Find("89128");
                    userLat = (decimal)defaultZip.Latitude;
                    userLong = (decimal)defaultZip.Longitude;
                    distance = 3000;

                }

                //max Distance to find
                Decimal DeltaDegrees = System.Convert.ToDecimal(distance / 69.0);
                var FilteredZipCodes = from z in context.ZipCodes
                                       where (z.Latitude >= userLat - DeltaDegrees && z.Latitude <= userLat + DeltaDegrees &&
                                       z.Longitude >= userLong - DeltaDegrees && z.Longitude <= userLong + DeltaDegrees)
                                       select z;
                MaxPagesRaw = (from l in context.Listings
                                  where (FilteredZipCodes.Select(z => z.Zipcode).Contains(l.OwnerZipcode) && (l.category == Category || Category == "All") && !l.completed)
                                  select l).OrderBy(x => x.name).Count() / (float)RecordsPerPage;

                model.MaxPages = (int)Math.Ceiling(MaxPagesRaw);
                if (model.MaxPages > 0)
                {
                    if (page > model.MaxPages)
                    {
                        page = model.MaxPages;
                        model.CurrentPage = model.MaxPages;
                    }

                    var FilteredListing = (from l in context.Listings
                                           where (FilteredZipCodes.Select(z => z.Zipcode).Contains(l.OwnerZipcode) && (l.category == Category || Category == "All") && !l.completed)
                                           select l).OrderBy(x => x.name).Skip(((int)page - 1) * RecordsPerPage).Take(RecordsPerPage);

                    model.Listings = FilteredListing.ToList<Listing>();
                }
            }

            
            
            return View(model);
        }

        // GET: /Listing/Manage
        /// <summary>
        /// Shows every listing that the current owner has created
        ///  /// Set the viewdata type to browse so the Pageinator knows
        ///  1. Figure out max pages
        ///  2. Make sure current page doesnt exceed max pages
        ///  2. Filter the listings so the listing ownername equals the authorized username
        ///  3. Truncate the list to only show listings on the current page
        /// </summary>
        /// <param name="page">Page of the current listings to show/param>
        /// <returns>REturns the viewmodel containing a list of the current listings to show</returns>
        [HttpGet]
        public ActionResult Manage(int? page)
        {
            ViewData["Type"] = "Manage";
            if (Request.QueryString["page"] == null) return RedirectToAction("Index","Home",null);
            var model = new ManageListingViewModel();
            float MaxPagesRaw;
            using (var context = new ApplicationDbContext())
            {

                model.CurrentPage = (int)page;
                MaxPagesRaw = (from l in context.Listings
                               where (l.OwnerUserName == userName)
                               select l).OrderBy(x => x.ListingId).Count() / (float)RecordsPerPage;

                model.MaxPages = (int)Math.Ceiling(MaxPagesRaw);
                if (model.MaxPages > 0)
                {
                    if (page > model.MaxPages)
                    {
                        page = model.MaxPages;
                        model.CurrentPage = model.MaxPages;
                    }

                    var FilteredListing = (from l in context.Listings
                                           where (l.OwnerUserName == userName)
                                           select l).OrderBy(x => x.ListingId).Skip(((int)page - 1) * RecordsPerPage).Take(RecordsPerPage);

                    model.Listings = FilteredListing.ToList<Listing>();
                }
            }
            
            
            return View(model);
        }

        /// <summary>
        /// Views the specified listing
        /// Reads the speficied listing from the database by the listing id
        /// </summary>
        /// <param name="ListingId">The listing identifier.</param>
        /// <returns>returns a model containing the data of the listing</returns>
        [HttpGet]
        [AllowAnonymous]
        //Get: /Listings/View
        public ActionResult View(int? ListingId)
        {
            if (Request.QueryString["ListingId"] == null) return RedirectToAction("Index", "Home", null);
            var model = new ViewListingViewModel();
            using (var context = new ApplicationDbContext())
            {
                var ListingToView = context.Listings.Find(ListingId);
                model.CurrentListing = ListingToView;
            }
            return View(model);
        }

        /// <summary>
        /// Get method for editing a listing
        /// if method no listing id is given return to home
        /// get the url to return to after they edit the listing
        /// set the viewmodel values equal to the database values of the current listing id
        /// </summary>
        /// <param name="ListingId">The listing identifier.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>returns a view model of the data to be edited</returns>
        [HttpGet]
        //Get: /Listings/Edit
        public ActionResult Edit(int? ListingId, string returnUrl)
        {
            if (ListingId== null) return RedirectToAction("Index", "Home", null);
            var decodeUrl = HttpUtility.UrlDecode(returnUrl);
            ViewBag.ReturnUrl = decodeUrl;
            var model = new EditListingViewModel();
            using (var context = new ApplicationDbContext())
            {
                var ListingToEdit = context.Listings.Find(ListingId);
                model.ListingId = (int)ListingId;
                model.CategoryType = ListingToEdit.category;
                model.description = ListingToEdit.description;
                model.iurl = ListingToEdit.image;
                model.Name = ListingToEdit.name;
                model.price = ListingToEdit.price;
                model.OwnerUserName = ListingToEdit.OwnerUserName;
                model.BuyerUserName = ListingToEdit.BuyerUserName;
                model.Completed = ListingToEdit.completed;
                model.OwnerZipCode= ListingToEdit.OwnerZipcode;
                model.Completed = ListingToEdit.completed;
            }
            return View(model);
        }

        /// <summary>
        /// Edits the specified listing.
        /// updates the database with the post data from the form
        /// </summary>
        /// <param name="model">The viewmodel containing the values edited by the form</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        [HttpPost]
        //Post: /Listings/Edit
        public ActionResult Edit(EditListingViewModel model, string returnUrl)
        {
            using (var context = new ApplicationDbContext())
            {
                var url2 = ViewBag.ReturnUrl;
                var ListingToEdit = context.Listings.Find(model.ListingId);
                ListingToEdit.category= model.CategoryType;
                ListingToEdit.description = model.description;
                ListingToEdit.image= model.iurl;
                ListingToEdit.name= model.Name;
                ListingToEdit.price= model.price;
                ListingToEdit.completed = model.Completed;
                context.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        //Get: /Listings/Delete
        /// <summary>
        /// Deletes the specified listing .
        /// 1. Make sure the current person trying to delete the listing is the ower
        /// 2. Remove the listing from the database
        /// </summary>
        /// <param name="ListingId">The listing identifier.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Redirects to the given url</returns>
        /// <exception cref="HttpException">If a user does tries to delete a listing they do not own throw an exception/exception>
        [HttpGet]
        public ActionResult Delete(int ListingId, string returnUrl)
        {
            var decodeUrl = HttpUtility.UrlDecode(returnUrl);
            ViewData["ReturnUrl"] = decodeUrl;
            if (ModelState.IsValid)
            {
               
                using (var context = new ApplicationDbContext())
                {
                    var ListingToDelete = context.Listings.Find(ListingId);
                    if (ListingToDelete.OwnerUserName == userName)
                    {
                        var result = context.Listings.Remove(ListingToDelete);
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new HttpException(401, "Unauthorized access");
                    }
                }
                return Redirect(ViewData["ReturnUrl"].ToString());
            }
            return Redirect(ViewData["ReturnUrl"].ToString());
        }

        // GET: /Listing/Search
        /// <summary>
        /// This allows for annonymous users, If so give each the claims a default value
        /// IF any of  the Queries are empty give them a default value
        /// convert the string into an array of searchterms
        /// 1. Get a list of all the current zipcodes that are close enough to users location
        /// 2. Figure out how many pages there are total
        /// 3. IF the query of current page is greater than max page then set the current page to max page
        /// 4a.     find only items within the current location and category and arent completed
        /// 4b      run a ranked search to filter out items that contain at least 1 keyword and sort them by the most keywords
        /// </summary>
        /// <param name="Searchterm">The term to search by</param>
        /// <param name="Category">The category of objects to seach by</param>
        /// <param name="distance">The distance of objects to search by</param>
        /// <param name="page">The current page to show results of</param>
        /// <returns>view model of the filtered listings</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Search(String Searchterm, string Category, int distance, int page)
        {
            var model = new BrowseListingViewModel();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] searchTerms = Searchterm.Split(delimiterChars);

            float MaxPagesRaw;
            using (var context = new ApplicationDbContext())
            {
                //fige out max degrees to find
                Decimal DeltaDegrees = System.Convert.ToDecimal(distance / 69.0);
                var FilteredZipCodes = from z in context.ZipCodes
                                       where (z.Latitude >= userLat - DeltaDegrees && z.Latitude <= userLat + DeltaDegrees &&
                                       z.Longitude >= userLong - DeltaDegrees && z.Longitude <= userLong + DeltaDegrees)
                                       select z;

                MaxPagesRaw = context.Listings.Search(x => x.name, x => x.description)
                                .Containing(searchTerms)
                                .Where(f => (f.category == Category || Category == "All") && !f.completed)
                                .Count() / (float)RecordsPerPage; 

                model.MaxPages = (int)Math.Ceiling(MaxPagesRaw);

                var FilterByLocation = from l in context.Listings
                                        where (FilteredZipCodes.Select(z => z.Zipcode).Contains(l.OwnerZipcode) && (l.category == Category || Category == "All") && !l.completed)
                                        select l;
                var FilteredListing = FilterByLocation.Search(x => x.name, x => x.description)
                                    .Containing(searchTerms)
                                    .ToRanked()
                                    .OrderByDescending(r => r.Hits)
                                    .Skip((page - 1) * RecordsPerPage)
                                    .Take(RecordsPerPage)
                                    .ToList();
                model.Listings = (IEnumerable<Listing>)FilteredListing;
            }
            return View(model);
        }
    }
}