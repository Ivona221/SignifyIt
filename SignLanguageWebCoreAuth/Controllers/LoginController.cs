using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SignLanguageWebCoreAuth.Data;
using SignLanguageWebCoreAuth.Helpers;
using SignLanguageWebCoreAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SignLanguageWebCoreAuth.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        IConfiguration configuration;

        ApplicationDbContext db;
        public LoginController(IConfiguration _configuration)
        {
            configuration = _configuration;
            db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody]LoginPageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var redirectUrl = Url.Action("Index", "Login");
                //return RedirectToAction("Index", "Login");
                return Json(new { redirectUrl });
            }

            Users user = new Users() { Email = model.Email, Password = model.Password };

            var passwordEncryption = new PasswordEncription();
            
            user = db.Users.Where(x => x.Email == model.Email).FirstOrDefault();

            var decryptedPass = passwordEncryption.DecryptPassword(user.Password, configuration["AppSettings:PasswordKey"]);
            if(model.Password != decryptedPass)
            {
                user = null;
            }

            if (user != null)
            {
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, user.Email)
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                if (user.Email == "ivonamilanova221@gmail.com")
                {
                    var claim = new Claim(ClaimTypes.Role, "Admin");
                    identity.AddClaim(claim);
                }
               
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                var redirectUrl = Url.Action("Index", "Synonyms");
                //return RedirectToAction("Index", "Synonyms");
                return Json(new { redirectUrl });
                
            }

            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                var redirectUrl = Url.Action("Index", "Login");
                //return RedirectToAction("Index", "Login");
                return Json(new { redirectUrl });
            }
        }

        //[HttpPost]
        
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync();
            HttpContext.User =
                new GenericPrincipal(new GenericIdentity(string.Empty), null);
            HttpContext.User = null;
            HttpContext?.SignOutAsync().Wait();
            Thread.CurrentPrincipal = null;
            if (HttpContext != null) HttpContext.User = null;
            var redirectUrl = Url.Action("Index", "Home");
            return RedirectToAction("Index", "Home");
        }

    }
}