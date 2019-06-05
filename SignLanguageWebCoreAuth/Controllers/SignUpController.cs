using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignLanguageWebCoreAuth.Data;
using SignLanguageWebCoreAuth.Helpers;
using SignLanguageWebCoreAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace SignLanguageWebCoreAuth.Controllers
{
    [Authorize(Policy = "IsItAdmin")]
    public class SignUpController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IConfiguration configuration;

        public SignUpController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignUp([FromBody]SignUpPageViewModel model)
        {
            var redirectUrl = "";

            if (!ModelState.IsValid)
            {
                redirectUrl = Url.Action("Index", "SignUp");
                //return RedirectToAction("Index", "SignUp");
                return Json(new { redirectUrl });
            }

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var passwordEncryption = new PasswordEncription();
            string encryptPassword = passwordEncryption.EncryptPassword(model.Password, configuration["AppSettings:PasswordKey"]);

            Users user = new Users();
            user.Email = model.Email;
            user.Password = encryptPassword;
            user.Username = model.Username;
            user.Role = Roles.User;
            user.Salt = Encoding.UTF8.GetString(salt, 0, salt.Length);

            var userDb = db.Users.Where(x => x.Email == model.Email).FirstOrDefault();

            if(userDb != null)
            {
                //throw an error
            }

            using (var db = new ApplicationDbContext())
            {
                var users = db.Set<Users>();
                users.Add(user);

                db.SaveChanges();
            }

            var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, user.Email)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            var claim = new Claim(ClaimTypes.Role, "User");
            identity.AddClaim(claim);

            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            redirectUrl = Url.Action("Index", "Home");
            //return RedirectToAction("Index", "Home");
            return Json(new { redirectUrl });


        }




    }
}