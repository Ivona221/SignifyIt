using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignLanguageWebCore;
using SignLanguageWebCoreAuth.Data;
using SignLanguageWebCoreAuth.Helpers;
using SignLanguageWebCoreAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Aerospike.Client;

namespace SignLanguageWebCoreAuth.Controllers
{
    [Authorize(Policy = "IsItAdmin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //AerospikeClient client = new AerospikeClient("192.168.2.232", 3000);

        public ActionResult Index()
        {
            var users = db.Users.ToList();

            UsersPageViewModel model = new UsersPageViewModel();

            users.ForEach(x=>x.Password = null);
            users.ForEach(x => x.Salt = null);

            model.Users = users;

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateUser([FromBody]SignUpPageViewModel model)
        {
            var redirectUrl = "";
            if (!ModelState.IsValid)
            {
                redirectUrl = Url.Action("Index", "SignUp");
                return Json(new { MessageТype = MessageType.Error, Message = "Error" });
            }

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var passwordEncryption = new PasswordEncription();
            string encryptPassword = passwordEncryption.EncryptPassword(model.Password, "E546C8DF278CD5931069B522E695D4F2");

            Users user = new Users();
            user.Email = model.Email;
            user.Password = encryptPassword;
            user.Username = model.Username;
            user.Role = Roles.User;
            user.Salt = Encoding.UTF8.GetString(salt, 0, salt.Length);

            var userDb = db.Users.Where(x => x.Email == model.Email).FirstOrDefault();

            if (userDb != null)
            {
                //throw an error
            }
            using (var db = new ApplicationDbContext())
            {
                var users = db.Set<Users>();
                users.Add(user);

                db.SaveChanges();
            }

            return Json(new { MessageТype = MessageType.Success, Message = "Success" });

        }
    }
}
