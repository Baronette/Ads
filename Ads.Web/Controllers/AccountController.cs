using Ads.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ads.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Listings;Integrated Security=true;";

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            var repo = new AdsRepository(_connectionString);
            repo.AddUser(user);
            return RedirectToAction("login");
        }
        public IActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LogIn(User user)
        {
            var repo = new AdsRepository(_connectionString);
            User current = repo.VerifyLogin(user);
            if (current == null)
            {
                return RedirectToAction("login");
            }
            var claims = new List<Claim>
            {

                new Claim("user", user.Email)
                    };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                        new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();
            return Redirect("/home/newad");
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new AdsRepository(_connectionString);
            repo.DeleteAd(repo.GetUserByEmail(User.Identity.Name).Id);
            return Redirect("/");
        } 
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("login");
        }
    }
}
