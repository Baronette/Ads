using Ads.Data;
using Ads.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ads.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Listings;Integrated Security=true;";
        public IActionResult Index()
        {
            var repo = new AdsRepository(_connectionString);

            var vm = new AdsViewModel
            {
                Ads = repo.GetAds(),
            };
            if (User.Identity.IsAuthenticated)
            {
                User user = repo.GetUserByEmail(User.Identity.Name);
                vm.Ads.ForEach(a => a.Delete = user.Id == a.UserId);
            }
            return View(vm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var repo = new AdsRepository(_connectionString);
            User currenUser = repo.GetUserByEmail(User.Identity.Name);
            ad.UserId = currenUser.Id;
            ad.Name = currenUser.Name;
            repo.AddListing(ad);
            return Redirect("/");
        }
        public IActionResult DeleteAd(int id)
        {
            var repo = new AdsRepository(_connectionString);
            repo.DeleteAd(id);
            return Redirect("/");
        }

    }
}
