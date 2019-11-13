using ForumApp.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using customEncrypt;
using System.Diagnostics;
using System.Net.Mail;
using ForumApp.Helpers;

namespace ForumApp.Controllers
{
    public class AdminController : Controller
    {

        private static Logger logger = LogManager.GetLogger("dbloger");

        [ValidateAntiForgeryToken]
        [CustomAuthorization]
        public async Task<ActionResult> Index()
        {
            using (var set = new ForumContext())
            {
                var data= await set
                    .Users
                    .Where(x=> !x.IsAdmin)
                    .ToListAsync();

                var result = data.Select(x => new UserDTO 
                { 
                    Id =x.Id,
                    Password = System.Text.Encoding.UTF8.GetString(x.Password),
                    Username= x.Username,
                    Email = x.Email
                });

                ViewData.Model = result;
                return View();
            }
        }


        [HttpPost]
        [NoDirectAccess]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(UserDTO user)
        {

            using (var set = new ForumContext())
            {
                var result = set.Users.FirstOrDefault(x => x.Id == user.Id);

                result.Password = await CustomEncryptor.EncryptAsync(user.Password);
                result.Email = user.Email;
                result.Username = user.Username;


                await set.SaveChangesAsync();

                if (((User)Session["User"]).IsAdmin)
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Index","Home");
            }

        }

        [HttpGet]
        [NoDirectAccess]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(int id)
        {
            using (var set = new ForumContext())
            {
                var result = set.Users.FirstOrDefault(x => x.Id == id);


                return View(new UserDTO()
                { 
                    Id=result.Id,
                    Password = System.Text.Encoding.UTF8.GetString(result.Password),
                    Email=result.Email,
                    Username=result.Username
                });
            }

        }



    }
}