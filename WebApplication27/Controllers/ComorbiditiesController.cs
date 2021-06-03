using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using WebApplication27.Models;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize("Manage Comorbidities")]
    public class ComorbiditiesController : Controller
    {
        private DB_Model db = new DB_Model();
        // GET: Comorbidities


        public ActionResult create()
        {

            return View("create");

        }

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
                name = Filter_Value;// to filter the list by name across all pages

            }
               ViewBag.FilterValue = name;


               List<COMORBIDITY> data = db.COMORBIDITies.OrderBy(q => q.COMORBIDITY_NAME).OrderBy(c => c.COMORBIDITY_NAME).ToList();

                if (!String.IsNullOrEmpty(name) && name.Length <= 220)
                {
                      data  = data.Where(m => m.COMORBIDITY_NAME.ToLower().Contains(name.ToLower())).ToList(); //filter by the name

                }

                return View(data.ToPagedList(pagenum, pagesize));  //list to all Comorbidities ...ordred by name



        }

        [HttpPost]
        public ActionResult Save(comorbidityModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //insert a comorbidity 
                    COMORBIDITY data = new COMORBIDITY();
                    data.COMORBIDITY_NAME = model.COMORBIDITY_NAME;
                    data.VISIBILIT_STATUS = model.VISIBILIT_STATUS;
                    data.DISPLAY_ORDER = model.DISPLAY_ORDER;
                    data.CREATED_DATE = DateTime.Now;
                    data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.COMORBIDITies.Add(data);
                    db.SaveChanges();
                    TempData["Success"] = data.COMORBIDITY_NAME + " has been saved successfully";//display a message to inform that (The comorbidity is added successfully)

                    return RedirectToAction("Index");

                }
                else
                {
                    return View("create", model);//if model is not valid  //return the view with all validation messages

                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
                {
                    TempData["Danger"] = "Comorbidity is already exists";
                    return RedirectToAction("Index");
                }
                    TempData["Danger"] = "Sorry, something went wrong";
                    return RedirectToAction("Index");
                

            }


        }
  
        [HttpGet]
        public ActionResult Edit(int id)
        {

                bool Found = db.COMORBIDITies.Where(x => x.COMORBIDITY_ID == id).Any();
                if (Found)
                {
                    string check = CheckComorbidityId(id);

                    switch (check)
                    {
                        case "Used In Completed Case":

                            return View("EditUsed", _getComorbidity(id));

                        case "Used In Pending Case":

                            TempData["Danger"] = "You cannot edit this comorbidity since it has been used in pending case";
                            return RedirectToAction("Index");

                        case "Not Used at all in any case":

                            return View("Edit", _getComorbidity(id));

                        default:

                            return RedirectToAction("Index");


                    }
                }
                return RedirectToAction("Index", "Error");

        }

        [HttpPost]
        public ActionResult editComorbidity(comorbidityModel model)
        {
            try
            {
              COMORBIDITY data = db.COMORBIDITies.FirstOrDefault(Models => Models.COMORBIDITY_ID == model.COMORBIDITY_ID);

                if (ModelState.IsValid)
                {

                     if (data != null)
                     { 
                        data.COMORBIDITY_NAME = model.COMORBIDITY_NAME;
                        data.VISIBILIT_STATUS = model.VISIBILIT_STATUS;
                        data.DISPLAY_ORDER = model.DISPLAY_ORDER;
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                        db.SaveChanges();

                        TempData["Success"] = "The changes are saved successfully";//display a message to inform that  (changes are saved successfully)
                         return RedirectToAction("Index");
                     }

                    return RedirectToAction("Index", "Error");

                }
                else
                {
                  return View("Edit", model);//if model is not valid //we return the page with all validation messages
 
                }
            }

            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                TempData["Danger"] = "comorbidity already exist";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }

        }
   
        [HttpPost]
        public ActionResult editUsedComorbidity(comorbidityModel model)
        {
            try
            {
                COMORBIDITY data = db.COMORBIDITies.FirstOrDefault(Models => Models.COMORBIDITY_ID == model.COMORBIDITY_ID); //get all related data to the current Comorbidity Id to update them

                if (ModelState.IsValid)
                {
                    if (data != null)
                    {
                        data.VISIBILIT_STATUS = model.VISIBILIT_STATUS;// update status
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                        db.SaveChanges();
                        TempData["Success"] = "The changes are saved successfully";//display a message to inform that  (changes are saved successfully)
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index", "Error");

                }
                else
                {
                    return View("EditUsed", model);//if model is not valid //we return the page with all validation messages

                }

            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }



        }

        [HttpGet]
        public ActionResult GetID3(int COMORBIDITY_ID)
        {
    
                COMORBIDITY model = db.COMORBIDITies.Where(Models => Models.COMORBIDITY_ID == COMORBIDITY_ID).FirstOrDefault();

                var check = db.CASE_COMORBIDITY.Where(x => x.COMORBIDITY_ID == COMORBIDITY_ID).Any();//check if used in case comorbidity table

                if (model != null && check == false)
                {
                    return PartialView(model);//delete if the Comorbidity Id not used in case comorbidity table

                }
                    //not allowed to delete
                    return PartialView("DeleteUsed");

               
        }
       
        [HttpPost]
        public ActionResult deleteComorbidity(int COMORBIDITY_ID)
        {
            try 
            {
                COMORBIDITY model = db.COMORBIDITies.Where(Models => Models.COMORBIDITY_ID == COMORBIDITY_ID).FirstOrDefault();

                var check = db.CASE_COMORBIDITY.Where(x => x.COMORBIDITY_ID == model.COMORBIDITY_ID).FirstOrDefault();//check if used in case comorbidity table


                if (model != null && check == null)
                {
                    //delete comorbidity
                    db.COMORBIDITies.Remove(model);
                    db.SaveChanges();
                    //display a message to inform that (The comorbidity is deleted successfully)
                    TempData["Success"] = "Comorbidity deleted successfully";
                    return RedirectToAction("Index");

                }
                return RedirectToAction("Index");




            }
            catch (Exception ex) when(ex.InnerException.InnerException.Message.Contains("ORA-02292"))
            {
                TempData["Danger"] = "You cannot delete this comorbidity since it has been used before";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");
            }

        }

        private string CheckComorbidityId(int id)
        {

            var check = db.CASE_COMORBIDITY.Where(x => x.COMORBIDITY_ID == id).FirstOrDefault();//check if used in case comorbidity table to update all data OR to update the status only

            var Result = from b in db.CASE_COMORBIDITY
                         from c in db.CASES
                         where c.CASE_STATUS == "Pending" && b.COMORBIDITY_ID == id && b.CASE_ID == c.CASE_ID
                         select new
                         { b };
            var s = Result.FirstOrDefault();//check if used in case comorbidity table and the case still pending


            string status;

            if (check != null && s == null)
            {

                status = "Used In Completed Case";
            }
            else if (check != null && s != null)
            {

                status = "Used In Pending Case";
            }
            else if (check == null && s == null)
            {

                status = "Not Used at all in any case";
            }
            else
            {
                status = "Nothing";

            }

            return status;

        }

        private comorbidityModel _getComorbidity(int id)
        {

            COMORBIDITY model = db.COMORBIDITies.FirstOrDefault(Models => Models.COMORBIDITY_ID == id);

            comorbidityModel c = new comorbidityModel();
            c.COMORBIDITY_ID = model.COMORBIDITY_ID;
            c.COMORBIDITY_NAME = model.COMORBIDITY_NAME;
            c.VISIBILIT_STATUS = model.VISIBILIT_STATUS;
            c.DISPLAY_ORDER = model.DISPLAY_ORDER;
            return c;

        }


    }
}