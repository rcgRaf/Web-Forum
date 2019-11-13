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

namespace ForumApp.Controllers
{
    public class AdminController : Controller
    {

        private static Logger logger = LogManager.GetLogger("dbloger");


        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            using (var set = new ForumContext())
            {
                ViewData.Model = await set
                    .Users
                    .Where(x=> !x.IsAdmin)
                    .ToListAsync();


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

                return RedirectToAction("Login", user);
            }
            else
            {
                return View("Login");
            }

        }



        [HttpGet]
        public ActionResult CreateThread(int topicId)
        {

            ViewData["topicId"] = topicId;
            return View();

        }


        [HttpPost]
        public async Task<ActionResult> CreateThread(Thread newThread)
        {
            using (var set = new ForumContext())
            {

                newThread.AuthorId = ((User)Session["User"]).Id;


                set.Threads.Add(newThread);
                await set.SaveChangesAsync();

                logger.Info($"New Thread: {newThread.Name} by user {Session["Username"] as string} ");

                return RedirectToAction("Index");
            }
        }


        [NoDirectAccess]
        public async Task<ActionResult> CloseOrOpenThread(bool close, int threadId)
        {
            using (var set = new ForumContext())
            {
                var thread = set.Threads.FirstOrDefault(t => t.Id == threadId);
                thread.IsClosed = close;

                await set.SaveChangesAsync();


                logger.Info($"Thread: {thread.Name} was {(close? "closed": "opened")} by user {Session["Username"] as string}");

                return RedirectToAction("ThreadDetails", new { threadId });
            }
        }


        [HttpGet]
        public ActionResult CreateTopic()
        {
            return View();

        }


        [HttpPost]
        public async Task<ActionResult> CreateTopic(Topic newTopic)
        {
            using (var set = new ForumContext())
            {



                set.Topics.Add(newTopic);
                await set.SaveChangesAsync();

                logger.Info($"Topic: {newTopic.Name} was created by user { Session["Username"] as string}");

                return RedirectToAction("Index");
            }
        }



        [HttpPost]
        public async Task<ActionResult> CreatePost(string username, string text, int threadId)
        {
            using (var set = new ForumContext())
            {

                var user = set.Users.FirstOrDefault(b => b.Username == username);

                var result = new Post()
                {
                    Text = text,
                    Thread = set.Threads.FirstOrDefault(t => t.Id == threadId),
                    User = user

                };

                set.Threads.FirstOrDefault(t => t.Id == threadId).LastPost = DateTime.UtcNow;

                set.Posts.Add(result); 
                await set.SaveChangesAsync();


                logger.Info($"A new post was created by user: {Session["Username"] as string}");
                return RedirectToAction("ThreadDetails", new { threadId });
            }
        }

        [NoDirectAccess]
        public async Task<ActionResult> TopicDetails(int topicId)
        {
            using (var set = new ForumContext())
            {

                var thread = await GetThreadsByTopicAsync(topicId);

                ViewData["Posts"] = await set.Posts.Include(p => p.User).ToListAsync();

                return View(thread);
            }

        }


        [NoDirectAccess]
        public async Task<ActionResult> ThreadDetails(int threadId)
        {

            var postThread = await GetThreadAsync(threadId);
            var posts = await GetPostsByTopicAsync(threadId);

            ViewData["Thread"] = postThread;
            return View(posts);

        }



        [NoDirectAccess]
        public async Task<ActionResult> DeletePost(int postId)
        {
            using (var set = new ForumContext())
            {

                var post = set.Posts.Include(p=> p.User).FirstOrDefault(p => p.Id == postId);

                var threadId = post.ThreadId;

                        
                set.Posts.Remove(post);
                
                await set.SaveChangesAsync();

                logger.Info($"Post byId:{postId} was deleted by user {Session["Username"] as string}");

                return RedirectToAction("ThreadDetails", new { threadId });
            }

        }



        private async Task<List<Thread>> GetThreadsByTopicAsync(int topicId)
        {
            using (var set = new ForumContext())
            {

                var result = await set.Threads.Include(t => t.Posts).Include(t => t.User).Where(t => t.TopicId == topicId).ToListAsync();
                return result;
            }
        }

        private async Task<List<Post>> GetPostsByTopicAsync(int threadId)
        {
            using (var set = new ForumContext())
            {
                set.Configuration.LazyLoadingEnabled = false;
                var result = await set.Posts.Include(t => t.User).Where(t => t.ThreadId == threadId).ToListAsync();
                return result;
            }
        }


        private async Task<Thread> GetThreadAsync(int threadId)
        {
            using (var set = new ForumContext())
            {
                var thread = set.Threads
                    .Include(t => t.User)
                    .FirstOrDefault(t => t.Id == threadId);



                return thread;
            }
        }


        public async Task<ActionResult> Logout()
        {
            var user = (User)Session["User"];
            logger.Info("User logged out:" + user.Name);
            Session.Abandon();

            return RedirectToAction("Login");
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

                    logger.Info($"User: { Session["Username"] as string} added a new thread \"{newItem.Name}\" ");

                }
            }
        }


        [NoDirectAccess]
        public async Task<ActionResult> Vote(bool upvote, int postId)
        {
            using (var set = new ForumContext())
            {
                
                    var user = (User)Session["User"];
                    var dbuser = set.Users.FirstOrDefault(u=>u.Id==user.Id);


                    var votedPost = set.Posts.FirstOrDefault(p => p.Id == postId);

                    var votes = await set.SentVotes.ToListAsync();


                if (!votes.Select(p=>p.postId).ToList().Contains(votedPost.Id))
                {
                    if (upvote)
                    {
                        votedPost.Votes++;
                        logger.Info($"User: { Session["Username"] as string} upvoted created {votedPost.User.Username}s post");
                    }
                    else
                    {
                        votedPost.Votes--;
                        logger.Info($"User: { Session["Username"] as string} downvoted created {votedPost.User.Username}s post");
                    }
                    set.SentVotes.Add(new SentVote() { userId = dbuser.Id, postId = votedPost.Id });


                    await set.SaveChangesAsync();
                }


                    return RedirectToAction("ThreadDetails", new { threadId =votedPost.ThreadId });
                }
            
        }




        // not implemented
        private async Task<List<Thread>> SearchByThread(string searchText, string category1, string category2)
        {
            using (var set = new ForumContext())
            {
                dynamic items;
                if (!string.IsNullOrEmpty(searchText))
                {
                    switch (category1)
                    {
                        case "Thread name":
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




        #region Methods

        private async Task<bool> NewUser(UserDTO user)
        {
            using (var set = new ForumContext())
            {
                var list = set.Users.Select(p => p.Email).Concat(set.Users.Select(p => p.Username));
                if (!ModelState.IsValid || list.Contains(user.Email) || list.Contains(user.Username))
                {
                    if (list.Contains(user.Email))
                    {
                        logger.Info("Failed attempt to register a registered user:" + user.Email + user.Username);
                        ModelState.AddModelError("Email", "Such email is already registered");
                    }
                    else if (list.Contains(user.Username))
                    {

                        logger.Info("Failed attempt to register a new user:" + user.Username + user.Email);
                        ModelState.AddModelError("Username", "Such username is already registered");
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
                        City = user.City

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