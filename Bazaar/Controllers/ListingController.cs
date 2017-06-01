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
        private UserManager<ApplicationUser> manager;
        private Decimal userLong;
        private Decimal userLat;
        private string userZipcode;
        private int RecordsPerPage = 16;
        public string PlaceHolder = "~/Content/images/Placeholder.png";
        public string userName;

        public ListingController()
        {
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            userLat = System.Convert.ToDecimal(prinicpal.Claims.Where(c => c.Type == "Latitude").Select(c => c.Value).SingleOrDefault());
            userLong = System.Convert.ToDecimal(prinicpal.Claims.Where(c => c.Type == "Longitude").Select(c => c.Value).SingleOrDefault());
            userZipcode =prinicpal.Claims.Where(c => c.Type == "ZipCode").Select(c => c.Value).SingleOrDefault();
            userName = prinicpal.Claims.Where(c => c.Type == "UserName").Select(c => c.Value).SingleOrDefault();
                
        }

        // GET: /Listing
        [AllowAnonymous]
        public ActionResult Index()
        {
            RedirectToAction("Browse");
            return View();
        }

        // GET: /Listing/Add
        [HttpGet]
        public ActionResult Add()
        {
            var model = new AddListingViewModel();
            model.CategoryType = "Other";
            return View(model);
        }

        // POST: /Listing/Add
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
               

                //figURE out max degrees to find
                Decimal DeltaDegrees = System.Convert.ToDecimal(distance / 69.0);
                var FilteredZipCodes = from z in context.ZipCodes
                                       where (z.Latitude >= userLat - DeltaDegrees && z.Latitude <= userLat + DeltaDegrees &&
                                       z.Longitude >= userLong - DeltaDegrees && z.Longitude <= userLong + DeltaDegrees)
                                       select z;
                MaxPagesRaw = (from l in context.Listings
                                  where (FilteredZipCodes.Select(z => z.Zipcode).Contains(l.OwnerZipcode) && (l.category == Category || Category == "All"))
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
                                           where (FilteredZipCodes.Select(z => z.Zipcode).Contains(l.OwnerZipcode) && (l.category == Category || Category == "All"))
                                           select l).OrderBy(x => x.name).Skip(((int)page - 1) * RecordsPerPage).Take(RecordsPerPage);

                    model.Listings = FilteredListing.ToList<Listing>();
                }
            }

            
            
            return View(model);
        }

        // GET: /Listing/Manage
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

        [HttpGet]
        //Get: /Listings/Edit
        public ActionResult Edit(int ListingId, string returnUrl)
        {
            var decodeUrl = HttpUtility.UrlDecode(returnUrl);
            ViewBag.ReturnUrl = decodeUrl;
            var model = new EditListingViewModel();
            using (var context = new ApplicationDbContext())
            {
                var ListingToEdit = context.Listings.Find(ListingId);
                model.ListingId = ListingId;
                model.CategoryType = ListingToEdit.category;
                model.description = ListingToEdit.description;
                model.iurl = ListingToEdit.image;
                model.Name = ListingToEdit.name;
                model.price = ListingToEdit.price;
                model.OwnerUserName = ListingToEdit.OwnerUserName;
                model.BuyerUserName = ListingToEdit.BuyerUserName;
                model.Completed = ListingToEdit.completed;
                model.OwnerZipCode= ListingToEdit.OwnerZipcode;
            }
            return View(model);
        }

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
                context.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        //Get: /Listings/Delete
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
                                       select l).OrderBy(x => x.name).Skip((page - 1) * RecordsPerPage).Take(RecordsPerPage);

                model.Listings = FilteredListing.ToList<Listing>();
            }
            return View(model);
        }
    }
}