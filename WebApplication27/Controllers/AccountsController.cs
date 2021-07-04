using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using WebApplication27.Models;
using System.Web.Mvc;
using WebApplication27.Models.Infrastructure;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication27.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Accounts
        private DB_Model db = new DB_Model();

        [HttpGet]
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {

            if (ModelState.IsValid)
            {
                var h_password = GetMD5(model.PASSWORD);
                string userSituation = userChecker(model.EMAIL, model.PASSWORD);
                var UserID = db.USERS.Where(user => user.EMAIL.ToLower() == model.EMAIL.ToLower() && user.PASSWORD == h_password).Select(x=>x.USER_ID).FirstOrDefault();

                switch (userSituation)
                {
                    case "Active & Valid":

                        FormsAuthentication.SetAuthCookie(UserID.ToString(), false);
                        return RedirectToAction("Home", "Accounts");

                    case "Inctive & Valid":

                        TempData["Danger"] = "Sorry, Your Account is Inactive.";
                        return RedirectToAction("Login");

                    case "To Active & Valid":

                        TempData["alert"] = "Your account has not been activated.";
                        return RedirectToAction("Login");

                    case "Email Exists & Invalid":
                        ModelState.AddModelError("Email", "invalid Username or Password");
                        return View();

                    case "Email not Exists":
                        return RedirectToAction("Register");

                    default:
                        return RedirectToAction("Register");

                }
            }

            else
            {
                return View(model);

            }

        }
        [HttpGet]
        public ActionResult Register()
        {


            return View();
        }
 
        [HttpPost]
        public ActionResult Register(RegisterModel obj)
        {
            if (ModelState.IsValid == true)
            {
                //Check if given email exists
                var isEmailAlreadyExists = db.USERS.Any(x => x.EMAIL.ToLower() == obj.EMAIL.ToLower());

            if (isEmailAlreadyExists)
            {
                ModelState.AddModelError("Email", "User with this email already exists");
                return View(obj);
            }
                //insert user 
                USER newobj = new USER();
                newobj.FULL_NAME = obj.FULL_NAME;
                newobj.EMAIL = obj.EMAIL;
                newobj.PASSWORD = GetMD5(obj.PASSWORD);
                newobj.STATUS = "To Active";
                db.USERS.Add(newobj);
                db.SaveChanges();
                decimal Id = newobj.USER_ID;
                //add a role to the current registered user
                decimal roleId = IsAdmin(obj.EMAIL, obj.User_Type);
                USER_ROLE UserRole = new USER_ROLE();
                UserRole.USER_ID = Id;
                UserRole.ROLE_ID = roleId;
                db.USER_ROLE.Add(UserRole);
                db.SaveChanges();
                return RedirectToAction("login");

            }
            else
            {
                return View(obj);
            }




        }

        [CustomAuthenticationFilter]
        [HttpGet]
        public ActionResult Home()
        {
          
            decimal UserID = Convert.ToDecimal(User.Identity.Name);  //cruuent user ID

            List<Case> data = db.CASES.Where(x => x.USER_ID == UserID).Where(c => c.CASE_STATUS == "Pending").OrderByDescending(x => x.CASE_START_DATE).Take(3).ToList(); //query to get 3 recent pending cases to current user's ID
            
            List<REQUESTER> requesters = db.REQUESTERS.OrderByDescending(c=>c.CREATED_DATE).Take(3).ToList();//query to get recent requester 

            var myviewmodel = new CaseViewModel();
            myviewmodel.userCases = data;
            myviewmodel.RequesterList = requesters;

            //get permissions to loged in user
            var userPermission = (from u in db.USERS
                                  from rr in db.ROLES
                                  join r in db.USER_ROLE on rr.ROLE_ID equals r.ROLE_ID
                                  join rp in db.ROLE_PERMISSIONS on rr.ROLE_ID equals rp.ROLE_ID
                                  join p in db.PERMISSIONS on rp.PERMISSION_ID equals p.PERMISSION_ID
                                  where u.USER_ID == UserID && r.USER_ID == UserID

                                  select p);
            if (userPermission.Where(x=>x.PERMISSION_NAME == "Requesters").Any())
            {
                TempData["Requesters"] = "Requesters";
            }
            if (userPermission.Where(x => x.PERMISSION_NAME == "New Case").Any())
            {
                TempData["Case"] = "Case Found";
            }


            //make list of permissions
            var Menulist = new List<PERMISSION>();


            foreach (var item in userPermission)
            {
                //add every item in userPermission query to the (Menulist) list
                Menulist.Add(new PERMISSION { PERMISSION_ID = item.PERMISSION_ID, PERMISSION_NAME = item.PERMISSION_NAME, ACTION_NAME = item.ACTION_NAME, CONTROLLER_NAME = item.CONTROLLER_NAME, TYPE = item.TYPE });

            }

            myviewmodel.PermissionList = Menulist;

            return View(myviewmodel);
        }

        [CustomAuthenticationFilter]
        [ChildActionOnly]
        public ActionResult menu()
        {
            //cruuent user ID
            decimal UserID = Convert.ToDecimal(User.Identity.Name);
            //permissions model 
            var myviewmodel = new CaseViewModel();

            //check if the user has more than one role 
            bool Checked = ((from ab in db.USER_ROLE
                             where ab.USER_ID == UserID
                             select ab).Count() > 1);

            //get permissions to loged in user
            var userPermission = (from u in db.USERS
                                  from rr in db.ROLES
                                  join r in db.USER_ROLE on rr.ROLE_ID equals r.ROLE_ID
                                  join rp in db.ROLE_PERMISSIONS on rr.ROLE_ID equals rp.ROLE_ID
                                  join p in db.PERMISSIONS on rp.PERMISSION_ID equals p.PERMISSION_ID
                                  where u.USER_ID == UserID &&  r.USER_ID == UserID

                                  select p);

            //count the users with status (To Active)
            var ActiveUsers = db.USERS.Where(x => x.STATUS == "To Active").Count();

            ViewBag.TotalToActiveUsers = ActiveUsers;


            //make list of permissions
            var Menulist = new List<PERMISSION>();


            foreach (var item in userPermission)
            {
                //add every item in userPermission query to the (Menulist) list
                Menulist.Add(new PERMISSION { PERMISSION_ID = item.PERMISSION_ID, PERMISSION_NAME = item.PERMISSION_NAME, ACTION_NAME = item.ACTION_NAME, CONTROLLER_NAME = item.CONTROLLER_NAME, TYPE =item.TYPE});
                
            }

            myviewmodel.PermissionList = Menulist;
            return PartialView("_menu",myviewmodel);
        }

        [CustomAuthenticationFilter]
        [HttpGet]
        public ActionResult profile()
        {
            
            var UserId = Convert.ToDecimal(User.Identity.Name);
            USER model = db.USERS.Where(Models => Models.USER_ID == UserId).SingleOrDefault();

            if (model != null)
            {
                //get user data 
                Profile data = new Profile();
                data.FULL_NAME = model.FULL_NAME;
                data.EMAIL = model.EMAIL;
                data.PASSWORD = model.PASSWORD;
                return View(data);

            }
            else
            {
                return View();

            }
        }

        [CustomAuthenticationFilter]
        [HttpGet]
        public ActionResult EditName()
        {
            var UserId = Convert.ToDecimal(User.Identity.Name);
            USER model = db.USERS.Where(Models => Models.USER_ID == UserId).SingleOrDefault();
            if (model != null)
            {   //get user data 
                Profile data = new Profile();
                data.FULL_NAME = model.FULL_NAME;
                data.EMAIL = model.EMAIL;
                data.PASSWORD = model.PASSWORD;
                return View(data);

            }
            else
            {
                return View();

            }
        }

        [CustomAuthenticationFilter]
        [HttpPost]
        public ActionResult UpdateName(Profile data)
        {
            var UserId = Convert.ToDecimal(User.Identity.Name);
            USER model = db.USERS.Where(Models => Models.USER_ID == UserId).SingleOrDefault();
            if (model != null && ModelState.IsValid)
            {
                //update name
                model.FULL_NAME = data.FULL_NAME;
                db.SaveChanges();
                return RedirectToAction("profile");

            }
            else
            {
                return View("EditName", data);

            }
        }
        [CustomAuthenticationFilter]
        [HttpGet]
        public ActionResult EditEmail()
        {
            var UserId = Convert.ToDecimal(User.Identity.Name);
            USER model = db.USERS.Where(Models => Models.USER_ID == UserId).SingleOrDefault();
            if (model != null)
            {
                //get user data 
                Profile data = new Profile();
                data.FULL_NAME = model.FULL_NAME;
                data.EMAIL = model.EMAIL;
                data.PASSWORD = model.PASSWORD;
                return View(data);

            }
            else
            {
                return View();

            }
        }

        [CustomAuthenticationFilter]
        [HttpPost]
        public ActionResult UpdateEmail(Profile data)
        {
            var isEmailAlreadyExists = db.USERS.Any(x => x.EMAIL.ToLower() == data.EMAIL.ToLower()); // Check if given email exists

            if (isEmailAlreadyExists)
            {
                ModelState.AddModelError("Email", "User with this email already exists");
                return View("EditEmail", data);
            }


            var UserId = Convert.ToDecimal(User.Identity.Name);
            USER model = db.USERS.Where(Models => Models.USER_ID == UserId).SingleOrDefault();
            if (model != null && ModelState.IsValid && isEmailAlreadyExists == false)
            {
                //update email
                model.EMAIL = data.EMAIL;
                db.SaveChanges();
                return RedirectToAction("profile");

            }
            else
            {
                return View("EditEmail", data);

            }
        }

        [CustomAuthenticationFilter]
        [HttpGet]
        public ActionResult EditPassword()
        {
            var UserId = Convert.ToDecimal(User.Identity.Name);
            USER model = db.USERS.Where(Models => Models.USER_ID == UserId).SingleOrDefault();
            if (model != null)
            {
                Profile data = new Profile();
                data.FULL_NAME = model.FULL_NAME;
                data.EMAIL = model.EMAIL;
                data.PASSWORD = model.PASSWORD;
                return View(data);

            }
            else
            {
                return View();

            }
        }
        public ActionResult LogOut()
        {
            //logout
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            return RedirectToAction("login");

        }

        public ActionResult UnAuthorized()
        {
            ViewBag.Message = "Un Authorized Page!";

            return View();
        }


        private decimal IsAdmin(string email , string UserType)
        {

            decimal roleid = 0;
             if (UserType == "Student" && email != "admin2@gmail.com")
             {
                var Role = db.ROLES.Where(x => x.ROLE_NAME == "Student").Select(z => z.ROLE_ID).FirstOrDefault(); // assign student role
                roleid = Role;

              }
            else if (UserType == "pharmacist" && email != "admin2@gmail.com")
            {
                var Role = db.ROLES.Where(x => x.ROLE_NAME == "pharmacist").Select(z => z.ROLE_ID).FirstOrDefault();// assign pharmacist role
                roleid = Role;


            }
            else if (UserType == "pharmacist" && email == "admin2@gmail.com")
            {
                var Role = db.ROLES.Where(x => x.ROLE_NAME == "Admin").Select(z => z.ROLE_ID).FirstOrDefault(); // assign admin role
                roleid = Role;


            }
            else
            {
                var Role = db.ROLES.Where(x => x.ROLE_NAME == "Student").Select(z => z.ROLE_ID).FirstOrDefault();
                roleid = Role;


            }

            return roleid;


        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }


        private string userChecker(string email, string password)
        {

            string userSituation = "";

            var h_password = GetMD5(password);

            //query to check if the email and password of the user are correct
            bool IsValidUser = db.USERS.Any(user => user.EMAIL.ToLower() == email.ToLower() && user.PASSWORD == h_password);

            //query to check if the email is not exists in DB to open Register View
            bool emailExists = db.USERS.Any(user => user.EMAIL.ToLower() == email.ToLower());

            //check if the given email is assigned to an active user       
            var IsActiveUser = db.USERS.Where(user => user.EMAIL.ToLower() == email.ToLower() && user.PASSWORD == h_password).Select(x => x.STATUS).FirstOrDefault();

            if (IsValidUser && IsActiveUser == "Active")
            {
                userSituation = "Active & Valid";

            }
            else if (IsValidUser && IsActiveUser == "Inactive")
            {

                userSituation = "Inctive & Valid";

            }
            else if (IsValidUser && IsActiveUser == "To Active")
            {
                userSituation = "To Active & Valid";


            }
            else if (emailExists == true && IsValidUser == false)
            {
                userSituation = "Email Exists & Invalid";

            }
            else
            {
                userSituation = "Email not Exists";

            }

            return userSituation;



        }



    }
}