using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication27.Models;
using PagedList;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize("Manage Users")]

    public class UsersController : Controller
    {
        private DB_Model db = new DB_Model();

        // GET: Users
        [HttpGet]
        public ActionResult Index(int? page, string name, string Filter_Value)
        {
    
                const int pagesize = 5;//number of items or rows per each page
                int pagenum = (page ?? 1); //defult page

                if (name != null)
                {
                    page = 1;
                }
                else
                {

                    name = Filter_Value; //to filter the list by name across all pages ... not only the first page

                }
                ViewBag.FilterValue = name;

                List<USER> data = db.USERS.OrderBy(x => x.FULL_NAME).ToList(); //list to all users ...ordred by name


                if (!String.IsNullOrEmpty(name) && name.Length < 150)
                {
                     data = data.Where(m => m.FULL_NAME.ToLower().Contains(name.ToLower()) || m.EMAIL.ToLower().Contains(name.ToLower())).ToList(); //filter by the name

                }
            var Users = new List<USER>(); // make a list of roles 
            foreach (var item in data)
            {

                Users.Add(new USER { USER_ID = item.USER_ID, EMAIL = item.EMAIL }); //add every item in data query to the (Users) list
            }

            ViewBag.TotalToActiveUsers= data.Where(x=>x.STATUS == "To Active").Count();



            return View(data.ToPagedList(pagenum, pagesize));

            
 
        }


        [HttpGet]
        public ActionResult GetToActiveUsers()
        {
            List<USER> data = db.USERS.OrderBy(x => x.FULL_NAME).Where(x=>x.STATUS == "To Active").ToList(); //list to all new users  ...ordred by name ..
            
            if (data != null)
            {
                var users = new List<Users_>(); // make a list of roles 

                foreach (var item in data)
                {
                    users.Add(new Users_ { id = item.USER_ID, email = item.EMAIL, Checked = false, name = item.FULL_NAME}); //add every item in Result query to the (Roles) list

                }
                ActiveUsersModel User = new ActiveUsersModel();
                User.UserList = users;
                return PartialView(User);

            }
            return RedirectToAction("Index", "Error");

        }


        [HttpPost]
        public ActionResult activateUsers(ActiveUsersModel model)
        {

                    foreach (var item in model.UserList)
                    {
                        var UserIdCheck = db.USERS.Where(x => x.USER_ID == item.id).FirstOrDefault(); //Check if given user id exists

                        if (item.Checked && UserIdCheck != null)
                        {
                            UserIdCheck.STATUS = "Active";// update user status 
                            db.SaveChanges();
                        }
                    }

                    TempData["Success"] = "The changes are saved successfully";//display a message to inform that  (changes are saved successfully)
                    return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(decimal id)
        {
            var model = db.USERS.FirstOrDefault(p => p.USER_ID == id);
            if (model != null)
            {
                return View(_getUser(id));

            }
            return RedirectToAction("Index", "Error");



        }

        [HttpGet]

        public ActionResult Details(decimal id)
        {
            var model = db.USERS.FirstOrDefault(p => p.USER_ID == id);
            if (model != null)
            {
            return View(_getUser(id));

            }
            return RedirectToAction("Index", "Error");

        }

        [HttpPost]
        public ActionResult EditUser(UsersModel model)
        {
            try
            {
                USER data = db.USERS.FirstOrDefault(Models => Models.USER_ID == model.USER_ID);
                decimal id = model.USER_ID;
          
                 
                if (data != null && model.RoleList.Count != 0 && !String.IsNullOrEmpty(model.STATUS) && model.STATUS.Length <= 10)
                {
                    //save status 
                    data.STATUS = model.STATUS;
                    db.SaveChanges();

                    foreach (var item in db.USER_ROLE)  //delete all user id in User_Roles table 
                    {

                        if (item.USER_ID == model.USER_ID)
                        {
                            db.Entry(item).State = System.Data.EntityState.Deleted;
                        }

                    }

                    foreach (var item in model.RoleList) // add user roles
                    {
                        var RoleIdCheck = db.ROLES.Where(x => x.ROLE_ID == item.id).Any(); //Check if given role id exists

                        if (item.Checked && RoleIdCheck)
                        {
                            db.USER_ROLE.Add(new USER_ROLE() { USER_ID = id, ROLE_ID = item.id }); //insert all checked roles

                            db.SaveChanges();

                        }

                    }
                    TempData["Success"] = "The changes are saved successfully";//display a message to inform that  (changes are saved successfully)
                    return RedirectToAction("Index");



                }

                else
                {
     
                    TempData["Danger"] = "please enter all required fields";
                    return RedirectToAction("Edit", new { id });
                }
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }

    }


        [HttpGet]
        public ActionResult GetID3(int id)
        {
          
                USER model = db.USERS.Where(Models => Models.USER_ID == id).SingleOrDefault();//get user data

                var check = db.CASES.Where(x => x.USER_ID == id).FirstOrDefault(); //check if the given id is not assigned to any case


                if (model != null && check == null)
                {
                    return PartialView(model);
                }
                if (model != null && check != null)
                {

                    return PartialView("DeleteUsed");// allowed to delete


                }

                return RedirectToAction("Index");



        }

        [HttpPost]
        public ActionResult deleteUser(UsersModel model)
        {
            try
            {

                USER data = db.USERS.Where(Models => Models.USER_ID == model.USER_ID).FirstOrDefault();
                if (data != null)
                {


                    //delete all user id in user_roles table
                    foreach (var item in db.USER_ROLE)
                    {

                        if (item.USER_ID == model.USER_ID)
                        {
                            db.Entry(item).State = System.Data.EntityState.Deleted;

                        }

                    }

                    //remove user
                    db.USERS.Remove(data);
                    db.SaveChanges();


                    //display a message to inform that (The user is deleted successfully)
                    TempData["Success"] = "User deleted successfully";
                    return RedirectToAction("Index");

                }
                else
                {
                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-02292"))
            {
                TempData["Danger"] = "You cannot delete this user";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }
        }


        private UsersModel _getUser(decimal id)
        {
            var model = db.USERS.FirstOrDefault(p => p.USER_ID == id);

            UsersModel User = new UsersModel();
            User.USER_ID = model.USER_ID;
            User.FULL_NAME = model.FULL_NAME;
            User.EMAIL = model.EMAIL;
            User.STATUS = model.STATUS;

           
            var Result = from b in db.ROLES
                         orderby (b.ROLE_NAME)
                         select new
                         {
                             b.ROLE_ID,
                             b.ROLE_NAME,
                             Checked = ((from ab in db.USER_ROLE
                                         where (ab.USER_ID == id) & (ab.ROLE_ID == b.ROLE_ID)
                                         select ab).Count() > 0)

                         }; //get roles to the given user's id

            var Roles = new List<roles_>(); // make a list of roles 


            foreach (var item in Result)
            {

                Roles.Add(new roles_ { id = item.ROLE_ID, name = item.ROLE_NAME, Checked = item.Checked }); //add every item in Result query to the (Roles) list



            }


            User.RoleList = Roles;
            return User;
        }


    }

}
