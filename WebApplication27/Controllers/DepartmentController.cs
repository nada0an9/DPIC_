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
    [CustomAuthorize("Manage Departments")]
    public class DepartmentController : Controller
    {
        private DB_Model db = new DB_Model();

        // GET: Department


        [HttpGet]
        public ActionResult Index(int? page, string name, string Filter_Value)
        {

                const int pagesize = 5;//number of items per each page
                int pagenum = (page ?? 1); //defult page
                if (name != null)
                {
                    page = 1;
                }
                else
                {

                    name = Filter_Value;// to filter the list by name across all pages ... not only the first page

                }
                ViewBag.FilterValue = name;

                List<DEPARTMENT> data = db.DEPARTMENTS.OrderBy(q => q.DEPARTMENT_NAME).ToList(); //list to all department ...ordred by name


                if (!String.IsNullOrEmpty(name) && name.Length <= 150)
                {
                   data = data.Where(m => m.DEPARTMENT_NAME.ToLower().Contains(name.ToLower())).ToList();//filter by the name

                }

                return View(data.ToPagedList(pagenum, pagesize));

        }

        [HttpGet]
        public ActionResult create()
        {
                List<REGION> list = db.REGIONS.Where(x=>x.VISIBILITY_STATUS == "Visible").OrderBy(x=>x.REGION_NAME).ToList(); //regions list

                ViewBag.regions = new SelectList(list, "REGION_ID", "REGION_NAME");
                 return View("Create");
        }
 
        [HttpPost]
        public ActionResult Save(departmentModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DEPARTMENT dep = new DEPARTMENT();
                    dep.COST_CODE = model.COST_CODE;
                    dep.DEPARTMENT_NAME = model.DEPARTMENT_NAME;
                    dep.VSIBILITY_STATUS = model.VSIBILITY_STATUS;
                    dep.REGION_ID = model.REGION_ID;
                    dep.CREATED_DATE = DateTime.Now;
                    dep.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.DEPARTMENTS.Add(dep);//insert new department

                    db.SaveChanges();
                    TempData["Success"] = dep.DEPARTMENT_NAME + " has been saved successfully";//display a message to inform that (The department is added successfully)

                    return RedirectToAction("Index");

                }
                else
                {
                    List<REGION> list = db.REGIONS.Where(x => x.VISIBILITY_STATUS == "Visible").OrderBy(x => x.REGION_NAME).ToList();
                    ViewBag.regions = new SelectList(list, "REGION_ID", "REGION_NAME");
                    return View("Create", model); //if model is not valid //we return the page with all validation messages + regions list



                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                    TempData["Danger"] = "Department is already exists";
                    return RedirectToAction("Index");
            }
            catch (Exception)
            {

                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }
        }
        
        [HttpGet]
        public ActionResult GetID2(int id)
        {
     
                bool Found = db.DEPARTMENTS.Where(Models => Models.DEPARTMENT_ID == id).Any();//check if current department Id is exists

                List<REGION> list = db.REGIONS.Where(x => x.VISIBILITY_STATUS == "Visible").OrderBy(x => x.REGION_NAME).ToList();
                ViewBag.regions = new SelectList(list, "REGION_ID", "REGION_NAME");//region list



            if (Found)
            {
                if (CheckDepartmentId(id) == "Unused")
                {
                    return View("Edit", _getDepartment(id));


                }

                return View("EditUsed", _getDepartment(id));
            }
            return RedirectToAction("Index", "Error");
        }

        [HttpPost]
        public ActionResult edit(departmentModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    DEPARTMENT data = db.DEPARTMENTS.FirstOrDefault(Models => Models.DEPARTMENT_ID == model.DEPARTMENT_ID);//get all related data to the current department Id  

                    if (data != null)
                    {
                        data.DEPARTMENT_NAME = model.DEPARTMENT_NAME;
                        data.VSIBILITY_STATUS = model.VSIBILITY_STATUS;
                        data.COST_CODE = model.COST_CODE;
                        data.REGION_ID = model.REGION_ID;
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                        db.SaveChanges(); //update them


                        TempData["Success"] = "The changes are saved successfully";//display a message to inform that  (changes are saved successfully)
                        return RedirectToAction("Index");
                    }

                    return RedirectToAction("Index", "Error");
                }
                else
                {

                    List<REGION> list = db.REGIONS.Where(x => x.VISIBILITY_STATUS == "Visible").OrderBy(x => x.REGION_NAME).ToList();
                    ViewBag.regions = new SelectList(list, "REGION_ID", "REGION_NAME");
                    return View("Edit", model);//if model is not valid  //we return the page with all validation messages + regions list

                }

            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                TempData["Danger"] = "Department is already exists";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }
    }

      
        [HttpPost]
        public ActionResult editUsed(departmentModel model)
        {
            try
            {
                DEPARTMENT data = db.DEPARTMENTS.FirstOrDefault(Models => Models.DEPARTMENT_ID == model.DEPARTMENT_ID);//get all related data to the current department Id  


                if (ModelState.IsValid)
                {
                    if (data != null)
                    {
                        //update status
                        data.VSIBILITY_STATUS = model.VSIBILITY_STATUS;
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                        db.SaveChanges();
                        TempData["Success"] = "The changes are saved successfully"; //display a message to inform that (changes are saved successfully)

                        return RedirectToAction("Index");

                    }

                    return RedirectToAction("Index", "Error");
                }

                else
                {
                    return View("EditUsed", model); //if model is not valid //we return the page with all validation messages

                }
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }









        }

        [HttpGet]
        public ActionResult GetID3(int DEPARTMENT_ID)
        {
            DEPARTMENT model = db.DEPARTMENTS.Where(Models => Models.DEPARTMENT_ID == DEPARTMENT_ID).SingleOrDefault();


            if (model != null && CheckDepartmentId(DEPARTMENT_ID) == "Unused")
            { 
                
                return PartialView(_getDepartment(DEPARTMENT_ID));


            }
                    return PartialView("DeleteUsed");//not allowed to delete


        }

        [HttpPost]
        public ActionResult deleteDep(departmentModel model)
        {
            try
            {
                DEPARTMENT data = db.DEPARTMENTS.Where(Models => Models.DEPARTMENT_ID == model.DEPARTMENT_ID).FirstOrDefault(); //get current department Id --> to delete it


                if (data != null)
                {
                    db.DEPARTMENTS.Remove(data); //delete
                    db.SaveChanges();
                    TempData["Success"] = "Department deleted successfully"; //display a message to inform that (The department is deleted successfully)

                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index", "Error");

            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-02292"))
            {
                TempData["Danger"] = "You cannot delete this department, since it has been used before";
                return RedirectToAction("Index");
            }

            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }
        }

        private departmentModel _getDepartment(int id)
        {

            DEPARTMENT model = db.DEPARTMENTS.Where(Models => Models.DEPARTMENT_ID == id).FirstOrDefault();

            departmentModel departmentModel = new departmentModel();
            departmentModel.DEPARTMENT_ID = model.DEPARTMENT_ID;
            departmentModel.COST_CODE = model.COST_CODE;
            departmentModel.DEPARTMENT_NAME = model.DEPARTMENT_NAME;
            departmentModel.VSIBILITY_STATUS = model.VSIBILITY_STATUS;
            departmentModel.REGION_ID = model.REGION_ID;
            return departmentModel;

        }

        private string CheckDepartmentId(int id)
        {
            var check = db.REQUESTERS.Where(x => x.DEPARTMENT_ID == id).FirstOrDefault();

            string status;

            if (check != null)
            {

                status = "Used In Requester";
            }
            else 
            {

                status = "Unused";
            }

            return status;

        }

    }
}