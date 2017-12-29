﻿using ForumApp.Models;
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

namespace ForumApp.Controllers
{
    public class HomeController : Controller
    {

        private static Logger logger = LogManager.GetLogger("dbloger");


        [HttpGet]
        public ActionResult Login(bool error=false)
        {
            if(error)
            {
                ModelState.AddModelError("Login", "Wrong password or email");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string pass)
        {
            using (var set = new ForumContext())
            {
                var usr = await set.Users.FirstOrDefaultAsync(m => m.Email == email);

                if (await UserLogin(email, pass, usr))
                {
                    return RedirectToAction("Index", FormMethod.Get);
                }
                else
                {
                    return RedirectToAction("Login",new { error= true });
                }
            }
        }



        [NoDirectAccess]
        public async Task<ActionResult> Index()
        {
            using (var set = new ForumContext())
            {
                ViewData.Model = await set.Topics.ToListAsync();
                return View();
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(UserDTO user)
        {

            if (await NewUser(user))
            {

                System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                        new System.Net.Mail.MailAddress("sender@mydomain.com", "Web Registration"),
                        new System.Net.Mail.MailAddress(user.Email));
                m.Subject = "Email confirmation";
                m.Body = string.Format("Dear {0}<BR/>Thank you for your registration, please click on the below link to comlete your registration: <a href=\"{1}\" title=\"User Email Confirm\">{1}</a>",
                    user.Username,
                    Url.Action("ConfirmEmail", "Account", new { Token = user.Username, Email = user.Email },
                    Request.Url.Scheme));



                m.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.mydomain.com", 25);
                smtp.Credentials = new System.Net.NetworkCredential("urugluxner@mail.ru", "Arahylyhopararaf5Rafulik97");
                smtp.EnableSsl = true;
                try
                {
                    await smtp.SendMailAsync(m);
                }
                catch (SmtpException e)
                {
                    throw new ArgumentException();
                }

                return RedirectToAction("Login", user);
            }
            else
            {

                return View("Index");
            }

        }


        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmEmail(string Token, string Email)
        {
            using (var set = new ForumContext())
            {
                var user = await set.Users.FirstAsync(u => u.Username == Token);
                if (user != null)
                {
                    if (user.Email == Email)
                    {
                        user.EmailConfirmed = true;
                        await set.SaveChangesAsync();

                        return RedirectToAction("Login");
                    }
                    else
                    {
                        return RedirectToAction("Confirm", "Account", new { Email = user.Email });
                    }
                }
                else
                {
                    return RedirectToAction("Confirm", "Account", new { Email = "" });
                }
            }

        }


        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        public ActionResult Details(int id)
        {
            using (var set = new ForumContext())
            {
                var book = set.Posts.FirstOrDefault(b => b.Id == id);
                return View(book);
            }
        }

        public async Task<ActionResult> Catalogue()
        {
            using (var set = new ForumContext())
            {
                var topics = await set.Topics.ToListAsync();
                ViewBag.Topics = topics;

                return View();
            }

        }

        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Buy(int id)
        //{
        //    await BookPurchase(id);

        //    return RedirectToAction("Catalogue");

        //}

        public async Task<ActionResult> Logout()
        {
            var user = (User)Session["User"];
            logger.Info("User logged out:" + user.Name);
            Session.Abandon();
            return RedirectToAction("Index");
        }

        public ActionResult NewBook()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NewThread(Thread item)
        {
            await AddNewThread(item);
            return RedirectToAction("Catalogue");
        }

        public async Task<ActionResult> Search(string threadname, string category1, string category2)
        {
            var items = await SearchByThread(threadname, category1, category2);
            return View("Catalogue", items);

        }



        #region Methods

        private async Task AddNewThread(Thread item)
        {
            using (var set = new ForumContext())
            {
                var dbThread = set.Threads
                    .SingleOrDefault(t => t.Name == item.Name);

                if (dbThread == null)
                {
                    User user = (User)Session["User"];

                    var author = await set.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

                    Thread newItem = new Thread()
                    {
                        User = author,
                        Name = item.Name
                    };

                    set.Threads.Add(newItem);
                    await set.SaveChangesAsync();

                }
            }
        }
        private async Task<List<Thread>> SearchByThread(string searchText, string category1, string category2)
        {
            using (var set = new ForumContext())
            {
                dynamic items;
                if (!string.IsNullOrEmpty(searchText))
                {
                    switch (category1)
                    {
                        case "Name":
                            items = await set.Threads.Where(x => x.Name.StartsWith(searchText)).ToListAsync();
                            break;
                        case "Price":
                            items = await set.Threads.Where(x => x.Name.Contains(searchText)).ToListAsync();
                            break;
                        default:
                            items = await set.Threads.ToListAsync();
                            break;

                    }

                }
                else
                {
                    items = await set.Threads.ToListAsync();
                }

                return items;
            }
        }
        //private async Task BookPurchase(int id)
        //{
        //    User user = (User)Session["User"];
        //    Owned_books newbook = new Owned_books { bookid = id, userid = user.Id };
        //    if (user.account_ - db.Books.FirstOrDefault(b => b.Id == id).Price >= 0)
        //    {
        //        user.account_ -= db.Books.FirstOrDefault(b => b.Id == id).Price;
        //        db.Owned_books.Add(newbook);
        //        db.SaveChanges();
        //        var list = (List<Book>)Session["Books"];
        //        list.Add(db.Books.FirstOrDefault(b => b.Id == id));
        //        Session["Books"] = list;
        //        Session["User"] = user;
        //        logger.Info("User:" + user.Email + " bought the book titled:" + db.Books.FirstOrDefault(b => b.Id == id).Name);
        //    }
        //    else
        //    {
        //        logger.Info("Failed attempt to buy a book with insufficient funds:" + user.Email);
        //        TempData["msg"] = "<script>alert('Insufficient funds');</script>";

        //    }
        //}
        private async Task<bool> NewUser(UserDTO user)
        {
            using (var set = new ForumContext())
            {
                var list = set.Users.Select(p => p.Email).Concat(set.Users.Select(p=>p.Username));
                if (!ModelState.IsValid || list.Contains(user.Email) || list.Contains(user.Username))
                {
                    if (list.Contains(user.Email))
                    {
                        logger.Info("Failed attempt to register a registered user:" + user.Email+user.Username);
                        ModelState.AddModelError("Email", "Such email is already registered");
                    }
                    else if (list.Contains(user.Username))
                    {
                   
                        logger.Info("Failed attempt to register a new user:" + user.Username + user.Email);
                        ModelState.AddModelError("Email", "Such username is already registered");
                    }
                    else
                    {
                        logger.Info(ModelState
                       .Values
                       .FirstOrDefault(e => e.Errors.Count != 0)
                       .Errors.FirstOrDefault()
                       .ErrorMessage);
                    }
                    return false;
                }
                else
                {
                    User usr = new User
                    {

                        Name = user.Name,
                        LastName = user.LastName,
                        Email = user.Email,
                        Username = user.Username,
                        Password = await CustomEncryptor.EncryptAsync(user.Password),
                        City = "temp"

                    };
                    set.Users.Add(usr);
                    set.SaveChanges();
                    logger.Info("Registered a new user:" + user.Email);
                    return true;
                }
            }
        }
        private async Task<bool> UserLogin(string email, string pass, User usr)
        {

            if (usr != null)
            {

                var bytepass = await CustomEncryptor.EncryptAsync(pass);
                var test = System.Text.Encoding.UTF8.GetString(bytepass);
                var usrpass = System.Text.Encoding.UTF8.GetString(usr.Password);
                if (usrpass == test)
                {

                    GlobalDiagnosticsContext.Set("Email", usr.Email);

                    Session["Username"] = usr.Username;
                    Session["User"] = usr;
                    logger.Info("Successful login, username:" + usr.Email);
                    return true;
                }
                else
                {

                    logger.Info("Failed attempt to login:" + email);
                   
                    return false;
                }
            }
            else
            {
                logger.Info("Failed attempt to login:" + email);
                return false;
            }
        }
        #endregion



    }
}