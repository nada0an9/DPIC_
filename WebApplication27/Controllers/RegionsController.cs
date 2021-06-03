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
    [CustomAuthorize("Manage Regions")]
    public class RegionsController : Controller
    {
        // GET: Regions
        private DB_Model db = new DB_Model();

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

                List<REGION> data = db.REGIONS.OrderBy(q => q.REGION_NAME).ToList();//list to all regions ordered by their names

            if (!String.IsNullOrEmpty(name) && name.Length <= 150)
            {
                     data = data.Where(m => m.REGION_NAME.ToLower().Contains(name.ToLower())).ToList();//filter by the name
            }

                return View(data.ToPagedList(pagenum, pagesize));
        }

        public ActionResult Create()
        {

            return View();
        }
   
        [HttpPost]
        public ActionResult Save(RegionModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    REGION data = new REGION();
                    //insert new region
                    data.REGION_NAME = model.REGION_NAME;
                    data.VISIBILITY_STATUS = model.Vsibility_status;
                    data.CREATED_DATE = DateTime.Now;
                    data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.REGIONS.Add(data);
                    db.SaveChanges();
                    TempData["Success"] = data.REGION_NAME + " has been saved successfully";//display a message to inform that (The region is added successfully)
                    return RedirectToAction("Index");

                }
                else
                {

                    return View("Create", model); //if model is not valid //display all validation message

                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {

                TempData["Danger"] = "Region is already exists";
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

            REGION model = db.REGIONS.Where(Models => Models.REGION_ID == id).FirstOrDefault();  

            if (model != null)
            {
                string check = CheckRegionId(id);

                switch (check)
                {
                    case "Used":

                        return View("EditUsed", _getRegion(id));

                    case "Unused":

                        return View("Edit", _getRegion(id));

                    default:

                        return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index", "Error");




        }

        [HttpPost]
        public ActionResult edit(RegionModel model)
        {
            try
            {
                REGION data = db.REGIONS.Where(Models => Models.REGION_ID == model.REGION_ID).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    if (model != null)
                    {
                        //update
                        data.REGION_NAME = model.REGION_NAME;
                        data.VISIBILITY_STATUS = model.Vsibility_status;
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                        db.SaveChanges();
                        TempData["Success"] = "The changes are saved successfully"; //display a message to inform that  (changes are saved successfully)

                        return RedirectToAction("Index");

                    }
                    return RedirectToAction("Index", "Error");


                }
                else
                {
                    return View("Edit", model);  //if model is not valid, return the view with all validation message


                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                TempData["Danger"] = "Region is already exists";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                 TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult editUsed(RegionModel model)
        {
            try
            {
                REGION data = db.REGIONS.Where(Models => Models.REGION_ID == model.REGION_ID).FirstOrDefault();///get all related data to the current region Id  


                if (ModelState.IsValid)
                {
                    if (data != null)
                    {
                        //update status
                        data.VISIBILITY_STATUS = model.Vsibility_status;
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
                    //if model is not valid 
                    //we return the view with all validation messages
                    return View("Edit", model);
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

            REGION model = db.REGIONS.Where(Models => Models.REGION_ID == id).SingleOrDefault();//get all related data to the current region Id  

            if (model != null && CheckRegionId(id) == "Unused")
            {
                return PartialView(model);
            }

            //not allowed to delete
            return PartialView("DeleteUsed");


        }

        [HttpPost]
        public ActionResult deleteRegion(RegionModel model)
        {
            try
            {
                //get current region Id --> to delete it
                REGION data = db.REGIONS.Where(Models => Models.REGION_ID == model.REGION_ID).FirstOrDefault();

                if (data != null)
                {
                    db.REGIONS.Remove(data);//delete
                    db.SaveChanges();
                    TempData["Success"] = "Region deleted successfully"; //display a message to inform that (The region is deleted successfully)
                    return RedirectToAction("Index");

                }

                return RedirectToAction("Index");
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-02292"))
            {
                TempData["Danger"] = "You cannot delete this region, since it has been used before";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }
        }


        public string CheckRegionId(decimal id)
        {

            var check = db.DEPARTMENTS.Where(x => x.REGION_ID == id).FirstOrDefault();

            string status;

            if (check != null)
            {

                status = "Used";
            }
            else
            {

                status = "Unused";
            }

            return status;

        }

        private RegionModel _getRegion(int id)
        {

                REGION model = db.REGIONS.Where(Models => Models.REGION_ID == id).SingleOrDefault();  //get all related data to the current region Id  

                RegionModel RegionModel = new RegionModel();
                RegionModel.REGION_ID = model.REGION_ID;
                RegionModel.REGION_NAME = model.REGION_NAME;
                RegionModel.Vsibility_status = model.VISIBILITY_STATUS;
                return RegionModel;

        }


    }
}