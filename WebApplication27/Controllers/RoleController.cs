using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication27.Models;
using PagedList;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize("Manage Roles")]

    public class RoleController : Controller
    {
        private DB_Model db = new DB_Model();

        // GET: Role

        [HttpGet]
        public ActionResult Index(int? page, string name , string Filter_Value)
        {
                const int pagesize = 5;//number of items or rows per each page
                int pagenum = (page ?? 1); //defult page
                if (name != null)
                {
                    page = 1;
                }
                else
                {

                    name = Filter_Value;// to filter the list by name across all pages

                }
                ViewBag.FilterValue = name;
              
            
            List<ROLE> data = db.ROLES.OrderBy(q => q.ROLE_NAME).ToList();

            if (!String.IsNullOrEmpty(name) && name.Length <= 20)
            {
                   data = data.Where(m => m.ROLE_NAME.ToLower().Contains(name.ToLower())).ToList(); //filter by role name
            }
            return View(data.ToPagedList(pagenum, pagesize)); //list to all roles ...ordred by name


        }

        [HttpGet]
        public ActionResult Create(string role_Name)
        {

                var Result = from b in db.PERMISSIONS
                             where b.PERMISSION_NAME != "Cases"
                             orderby (b.PERMISSION_NAME)
                             select new
                             {
                                 b.PERMISSION_ID,
                                 b.PERMISSION_DESCRIPTION,
                                 b.ACTION_NAME,
                                 b.CONTROLLER_NAME,
                                 b.PERMISSION_NAME
                             };//get Permetions

           
                var Permission = new List<PermissioncheckboxViewModel>();//make list of Permission

                rolesModel RoleModel = new rolesModel();

                if (!String.IsNullOrEmpty(role_Name))
                {
                    RoleModel.ROLE_NAME = role_Name;

                }


          
            foreach (var item in Result)  //assign every item in (Result) to the (Permission) list
            {
                //add every item in result query to the (Permission)
                Permission.Add(new PermissioncheckboxViewModel { PERMISSION_ID = item.PERMISSION_ID, PERMISSION_NAME = item.PERMISSION_NAME, PERMISSION_DESCRIPTION = item.PERMISSION_DESCRIPTION });
             
                RoleModel.PermissionList = Permission; //assign (Permission) items to the PermissionList;
            }

        return View(RoleModel);
                


        }

        [HttpPost]
        public ActionResult Save(rolesModel model)
        {
            try
            {
                if (ModelState.IsValid)
                { 

                    if(model.PermissionList.Count == 0)
                    {
                        TempData["Danger"] = "At least one permission needs to be selected";
                        return RedirectToAction("Create", new { model.ROLE_NAME});

                    }
                    ROLE role = new ROLE();
                    role.ROLE_NAME = model.ROLE_NAME;
                    role.CREATED_DATE = DateTime.Now;
                    role.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.ROLES.Add(role);
                    db.SaveChanges();
                    decimal id = role.ROLE_ID; // current role id

                    //add the role permissions
                    foreach (var item in model.PermissionList)
                    {
                            var checkid = db.PERMISSIONS.Where(x => x.PERMISSION_ID == item.PERMISSION_ID).Any();
                            if (item.Checked && checkid)//if checkbox is checked => we add the permission to the role permission table
                            {
                                //insert
                                db.ROLE_PERMISSIONS.Add(new ROLE_PERMISSIONS() { PERMISSION_ID = item.PERMISSION_ID, ROLE_ID = id });
                                db.SaveChanges();
                            }

                    }

                    TempData["Success"] = role.ROLE_NAME + " has been saved successfully";//display a message to inform that (The role is added successfully)

                    return RedirectToAction("Index");
                }
                else
                {
                 
                    TempData["Danger"] = "Please enter all required fields";
                    return RedirectToAction("Create", new {model.ROLE_NAME});//if model is not valid  //we return action
                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
               
                    TempData["Danger"] = "Role is already exists";
                    return RedirectToAction("Index");
            }

           


    }

        [HttpGet]
        public ActionResult Edit(int id)
        {

            bool Found = db.ROLES.Where(Models => Models.ROLE_ID == id).Any();
            if (Found)
            {
                return View(_getRoles(id));

            }
            return RedirectToAction("Index", "Error");


        }

        [HttpPost]
        public ActionResult editRole(rolesModel model)
        {
            try
            {
                //check if role is exists
                ROLE data = db.ROLES.Where(Models => Models.ROLE_ID == model.ROLE_ID).FirstOrDefault();

                if (ModelState.IsValid && data != null)
                {

                    //save role info
                    data.ROLE_NAME = model.ROLE_NAME;
                    data.CHANGED_DATE = DateTime.Now;
                    data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.SaveChanges();

                    //delete all role id in role_Permission 
                    foreach (var item in db.ROLE_PERMISSIONS)
                    {

                        if (item.ROLE_ID == model.ROLE_ID)

                        {
                            db.Entry(item).State = System.Data.EntityState.Deleted;

                        }

                    }
                    //add the role permissions 
                    foreach (var item in model.PermissionList)
                    {

                        var checkid = db.PERMISSIONS.Where(x => x.PERMISSION_ID == item.PERMISSION_ID).Any();
                        if (item.Checked && checkid)//if checkbox is checked => we add the permission to the role permission table
                        {
                            db.ROLE_PERMISSIONS.Add(new ROLE_PERMISSIONS() { PERMISSION_ID = item.PERMISSION_ID, ROLE_ID = model.ROLE_ID });
                            db.SaveChanges();

                        }
  
                    }
             
                    TempData["Success"] = "The changes are saved successfully"; //display a message to inform that  (changes are saved successfully)
                    return RedirectToAction("Index");


                }
                else
                {
                    TempData["Danger"] = "Please enter all required fields";//if model is not valid//we return action
                    return RedirectToAction("Edit", new { model.ROLE_ID});
                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                TempData["Danger"] = "Role is already exists";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }

        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            bool Found = db.ROLES.Where(Models => Models.ROLE_ID == id).Any();
            if (Found)
            {
                return View(_getRoles(id));
            }
            return RedirectToAction("Index", "Error");


        }

        [HttpGet]
        public ActionResult GetID3(int id)
        {
       
                ROLE model = db.ROLES.Where(Models => Models.ROLE_ID == id).FirstOrDefault();//get all related data to the current region Id  
               
                var checkInUserTable = db.USER_ROLE.Where(x => x.ROLE_ID == id).FirstOrDefault(); //check if (id) used in users role  table to delete or not

                if (model != null && checkInUserTable == null)
                {

    
                    return PartialView(_getRoles(id));
                }
              
                    return PartialView("DeleteUsed"); //not allowed to delete
   

        }

        [HttpPost]
        public ActionResult deleteRole(rolesModel model)
        {
            try
            {
                ROLE data = db.ROLES.Where(Models => Models.ROLE_ID == model.ROLE_ID).FirstOrDefault();

            if(data != null)
            {
                    foreach (var item in db.ROLE_PERMISSIONS) //delete all role id in role_Permission 
                    {

                        if (item.ROLE_ID == model.ROLE_ID)
                        {
                            db.Entry(item).State = System.Data.EntityState.Deleted;

                        }

                    }

                    db.ROLES.Remove(data);  //delete
                    db.SaveChanges();
                    TempData["Success"] = "Role deleted successfully";  //display a message to inform that (The role is deleted successfully)
                    return RedirectToAction("Index");
            }
                else 
                {
                    return RedirectToAction("Index", "Error");

                }


            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-02292"))
            {
                TempData["Danger"] = "You cannot delete this role, since it has been used before";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }
        }

        private rolesModel _getRoles(int id)
        {
           
            var model = db.ROLES.FirstOrDefault(p => p.ROLE_ID == id);
             rolesModel roles = new rolesModel();
            roles.ROLE_ID = model.ROLE_ID;
            roles.ROLE_NAME = model.ROLE_NAME;
  
            //get permission
            var Result = from b in db.PERMISSIONS
                         where b.PERMISSION_NAME != "Cases"
                         orderby (b.PERMISSION_NAME)
                         select new
                         {
                             b.PERMISSION_ID,
                             b.PERMISSION_DESCRIPTION,
                             b.PERMISSION_NAME,
                             Checked = ((from ab in db.ROLE_PERMISSIONS
                                         where (ab.ROLE_ID == id) & (ab.PERMISSION_ID == b.PERMISSION_ID)
                                         select ab).Count() > 0) //subquery to check if the permission is assigned to the current role id or not 

                         };

            //make list of Permissions
            var Permissions = new List<PermissioncheckboxViewModel>();

            foreach (var item in Result)
            {

                //add every item in result query to the (Permission)
                Permissions.Add(new PermissioncheckboxViewModel
                {
                    PERMISSION_ID = item.PERMISSION_ID,
                    PERMISSION_DESCRIPTION = item.PERMISSION_DESCRIPTION,
                    PERMISSION_NAME = item.PERMISSION_NAME,
                    Checked = item.Checked
                });


            }

            //assign (Permission) items to the PermissionList;
            roles.PermissionList = Permissions;
            return roles;
        }



    

    }
}