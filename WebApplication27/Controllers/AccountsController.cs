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
            try
            {
                if (ModelState.IsValid)
                {
                    var f_password = GetMD5(model.PASSWORD);
                    //query to check if the email and password of the user are correct
                    bool IsValidUser = db.USERS.Any(user => user.EMAIL.ToLower() == model.EMAIL.ToLower() && user.PASSWORD == f_password);

                    //check if the given email is assigned to an active user       
                    bool IsActiveUser = db.USERS.Any(user => user.EMAIL.ToLower() == model.EMAIL.ToLower() && user.PASSWORD == f_password && user.STATUS =="Active");


                    if (IsValidUser && IsActiveUser)
                    {

                        //query to get the user ID to assign the user ID in the Forms Authentication as an authenticated user
                        var userid1 = db.USERS.Where(x => x.EMAIL == model.EMAIL).Select(x => x.USER_ID).FirstOrDefault();
                        string userid = userid1.ToString();



                        FormsAuthentication.SetAuthCookie(userid, false);
                        return RedirectToAction("Home", "Accounts");
                    }
                    else if(IsValidUser && IsActiveUser == false)
                    {
                        TempData["Danger"] = "Sorry, Your Account is disabled";
                        return RedirectToAction("Login");

                    }

                    ModelState.AddModelError("Email", "invalid Username or Password");
                    return View();
                }
                else
                {
                    return View(model);

                }
            }

            catch (OracleException e)
            {
                string errorMessage = "Code: " + e.ErrorCode + "\n" +
                         "Message: " + e.Message;
                return View();

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
                newobj.STATUS = "Active";
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


    }
}