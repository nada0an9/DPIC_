using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication27.Models;
using PagedList;
using System.Web.Mvc;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize("Manage Reference")]
    public class ReferencesController : Controller
    {
        // GET: References
        private DB_Model db = new DB_Model();

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

            List<REFERENCE> data = db.REFERENCES.OrderBy(q => q.REFERENCE_NAME).OrderBy(r => r.REFERENCE_NAME).ToList();

            if (!String.IsNullOrEmpty(name) && name.Length <= 200)
            {

                 data = data.Where(m => m.REFERENCE_NAME.ToLower().Contains(name.ToLower())).ToList();  //filter by the name


            }
            return View(data.ToPagedList(pagenum, pagesize)); //list to all references ...ordred by name

            



        }

        [HttpGet]
        public ActionResult Create()
        {

            return View();

        }

        [HttpPost]
        public ActionResult Save(referencesModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //insert new references
                    REFERENCE refrence = new REFERENCE();
                    refrence.REFERENCE_NAME = model.REFERENCE_NAME;
                    refrence.VISIBILIT_STATUS = model.VISIBILIT_STATUS;
                    refrence.DISPLAY_ORDER = model.DISPLAY_ORDER;
                    refrence.CREATED_DATE = DateTime.Now;
                    refrence.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.REFERENCES.Add(refrence);
                    db.SaveChanges();
                    TempData["Success"] = refrence.REFERENCE_NAME + " has been saved successfully";//display a message to inform that (The reference is added successfully)

                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Create", model);//if model is not valid  //we return the page with all validation messages


                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                    TempData["Danger"] = "Reference is already exists";
                    return RedirectToAction("Index");
            }
            catch (Exception )
            {
               TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");
            }
            

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
      
            REFERENCE model = db.REFERENCES.Where(Models => Models.REFERENCE_ID == id).FirstOrDefault(); //get all related data to the current references Id  

            if(model != null)
            {
                string check = CheckRefrenceId(id);

                switch (check)
                {
                    case "Used In Completed Case":

                        return View("EditUsed", _getRefrence(id));

                    case "Used In Pending Case":

                        TempData["Danger"] = "You cannot edit this reference since it has been used in pending case";
                        return RedirectToAction("Index");

                    case "Not Used at all in any case":

                        return View("Edit", _getRefrence(id));

                    default:

                        return RedirectToAction("Index");
                }
            }
      

                 return RedirectToAction("Index", "Error");


        }


        [HttpPost]
        public ActionResult editReferences(referencesModel model)
        {
            try 
            { 
            REFERENCE data = db.REFERENCES.FirstOrDefault(Models => Models.REFERENCE_ID == model.REFERENCE_ID); //get all related data to the current reference Id  

                if(data != null)
                {
                    if (ModelState.IsValid)
                    {
                        //update
                        data.REFERENCE_NAME = model.REFERENCE_NAME;
                        data.VISIBILIT_STATUS = model.VISIBILIT_STATUS;
                        data.DISPLAY_ORDER = model.DISPLAY_ORDER;
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                        db.SaveChanges();
                        TempData["Success"] = "The changes are saved successfully"; //display a message to inform that  (changes are saved successfully)
                        return RedirectToAction("Index");

                    }
                    else
                    {
                        //if model is not valid 
                        //we return the page with all validation messages
                        return View("Edit", model);
                    }
                }
                return RedirectToAction("Index", "Error");

            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
   
                    TempData["Danger"] = "Reference is already exists";
                    return RedirectToAction("Index");
            }
            catch (Exception)
            {
                    TempData["Danger"] = "Sorry, something went wrong";
                    return RedirectToAction("Index");
            }
        }
 
        [HttpPost]
        public ActionResult editUsedReferences(referencesModel model)
        {
            try 
            { 

            //get all related data to the current references Id  
            REFERENCE data = db.REFERENCES.FirstOrDefault(Models => Models.REFERENCE_ID == model.REFERENCE_ID);


            if (ModelState.IsValid && CheckRefrenceId(model.REFERENCE_ID) == "Used In Completed Case")
            {
                //update status
                data.VISIBILIT_STATUS = model.VISIBILIT_STATUS;
                data.CHANGED_DATE = DateTime.Now;
                data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                db.SaveChanges();

                //display a message to inform that  (changes are saved successfully)
                TempData["Success"] = "The changes are saved successfully";
                return RedirectToAction("Index");
            }
            else
            {
                //if model is not valid 
                //we return the page with all validation messages
                return View("EditUsed", model);
            }
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");
            }


        }

        [HttpGet]
        public ActionResult GetID3(int REFERENCE_ID)
        {
            try
            {

                REFERENCE model = db.REFERENCES.Where(Models => Models.REFERENCE_ID == REFERENCE_ID).SingleOrDefault(); //get all related data to the current reference Id  


            if (model != null && CheckRefrenceId(REFERENCE_ID) == "Not Used at all in any case")
            {
                return PartialView(model);//delete view ==> if the reference Id is "not" used in case references table

            }
        
                    //not allowed to delete
                    return PartialView("DeleteUsed");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");
            }

        }
 
        [HttpPost]
        public ActionResult deleteref(int REFERENCE_ID)
        {
            try
            {
                //get current reference Id  --> to delete it
                REFERENCE model = db.REFERENCES.Where(Models => Models.REFERENCE_ID == REFERENCE_ID).SingleOrDefault();

                if (CheckRefrenceId(REFERENCE_ID) == "Not Used at all in any case" && model !=null)
                {
                    //delete
                    db.REFERENCES.Remove(model);
                    db.SaveChanges();
                    TempData["Success"] = "Reference deleted successfully";//display a message to inform that (The reference is deleted successfully)
                    return RedirectToAction("Index");
                }


                return RedirectToAction("Index");

            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-02292"))
            {
                TempData["Danger"] = "You cannot delete this reference, since it has been used before";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }
            }

        public string CheckRefrenceId(decimal referencesId)
        {

            //check if used in case references table to update all data OR to update the status only
            var check = db.CASE_REFERENCES.Where(x => x.REFERENCE_ID == referencesId).FirstOrDefault();

            //check if used in case references and the case still pending
            var Result = from b in db.CASE_REFERENCES
                         from c in db.CASES
                         where c.CASE_STATUS == "Pending" && b.REFERENCE_ID == referencesId && b.CASE_ID == c.CASE_ID
                         select new
                         { b };
            var s = Result.FirstOrDefault();

            string status;
           
            if (check != null && s == null)
            {

                status = "Used In Completed Case";
            }
            else if(check != null && s != null)
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

        private referencesModel _getRefrence(int id)
        {

            REFERENCE model = db.REFERENCES.FirstOrDefault(Models => Models.REFERENCE_ID == id);

            referencesModel data = new referencesModel();
            data.REFERENCE_ID = model.REFERENCE_ID;
            data.REFERENCE_NAME = model.REFERENCE_NAME;
            data.DISPLAY_ORDER = model.DISPLAY_ORDER;
            data.VISIBILIT_STATUS = model.VISIBILIT_STATUS;

            return data;

        }

    }
}
