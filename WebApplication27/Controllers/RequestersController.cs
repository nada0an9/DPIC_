using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication27.Models;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize("Manage Requesters")]
    public class RequestersController : Controller
    {
        private DB_Model db = new DB_Model();
        // GET: Requesters

        [HttpGet]
        public ActionResult Index(int? page,string Search_data, string Filter_Value)
        {
                const int pagesize = 5;//number of items or rows per each page
                int pagenum = (page ?? 1); //defult page
                if (Search_data != null)
                {
                    page = 1;
                }
                else
                {

                    Search_data = Filter_Value;// to filter the list by name  or id across all pages

                }
                ViewBag.FilterValue = Search_data;

                List<REQUESTER> data = db.REQUESTERS.OrderBy(x => x.REQUESTER_NAME).ToList(); //list to all requesters ...ordred by name


                if (!String.IsNullOrEmpty(Search_data) && Search_data.All(char.IsDigit) && Search_data.Length <= 10)//id filter
                {
                    decimal RequesterID = Convert.ToDecimal(Search_data);
                    data = data.Where(m => m.REQUESTER_ID == RequesterID).ToList();
                }
                if (!String.IsNullOrEmpty(Search_data) && Search_data.All(char.IsDigit) == false)//requester name filter

                {
                     data = data.Where(m => m.REQUESTER_NAME.ToLower().Contains(Search_data.ToLower())).ToList();

                }
                return View(data.ToPagedList(pagenum, pagesize));
            

     
        }

        [HttpGet]
        public ActionResult Create()
        {
                List<DEPARTMENT> list = db.DEPARTMENTS.Where(x => x.VSIBILITY_STATUS == "Visible").ToList();

                var model = new requesterModel()
                {
                    CityList = new SelectList(list, "DEPARTMENT_ID", "DEPARTMENT_NAME", "REGION.REGION_NAME", null, null)  //departments list ... groubed by region name

                };
                return View(model);
            
        
        }
        [HttpPost]
        public ActionResult Add(requesterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //inser requester
                    REQUESTER Req = new REQUESTER();
                    Req.REQUESTER_ID = model.REQUESTER_ID;
                    Req.REQUESTER_NAME = model.REQUESTER_NAME;
                    Req.CONTACT = model.CONTACT;
                    Req.CALLING_FROM = model.CALLING_FROM;
                    Req.DEPARTMENT_ID = model.DEPARTMENT_ID;
                    Req.CREATED_DATE = DateTime.Now;
                    Req.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.REQUESTERS.Add(Req);
                    db.SaveChanges();
                    TempData["Success"] = model.REQUESTER_NAME+" has been saved successfully";//display a message to inform that (The requester is added successfully)

                    return RedirectToAction("Index");

                }
                else
                {
                    //to reload the departemnts in the drop down list
                    List<DEPARTMENT> list = db.DEPARTMENTS.Where(x => x.VSIBILITY_STATUS == "Visible").ToList();
                    model.CityList = new SelectList(list, "DEPARTMENT_ID", "DEPARTMENT_NAME", "REGION.REGION_NAME", null, null);
                    return View("Create", model);

                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
                {
                    TempData["Danger"] = "Requester is already exists";
                    return RedirectToAction("Index");
                }

                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");


            }

        }

        [HttpGet]
        public ActionResult GetID(decimal id)
        {

                bool Found = db.REQUESTERS.Where(x => x.REQUESTER_ID == id).Any();

                if (Found)
                {
                    return View(_getRequester(id));
                }

                return RedirectToAction("Index");
            
        }

        [HttpPost]
        public ActionResult Edit(requesterModel model)
        {
            try
            {
                //get all related date to the crruent requester's id
                REQUESTER data = db.REQUESTERS.FirstOrDefault(Models => Models.REQUESTER_ID == model.REQUESTER_ID);

                if (ModelState.IsValid)
                {
                    //update 
                    data.REQUESTER_ID = model.REQUESTER_ID;
                    data.REQUESTER_NAME = model.REQUESTER_NAME;
                    data.CONTACT = model.CONTACT;
                    data.CALLING_FROM = model.CALLING_FROM;
                    data.DEPARTMENT_ID = model.DEPARTMENT_ID;
                    data.CHANGED_DATE = DateTime.Now;
                    data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.SaveChanges();
                    TempData["Success"] = "The changes are saved successfully";//display a message to inform that  (changes are saved successfully)
                    return RedirectToAction("Index");

                }
                else
                {
                    List<DEPARTMENT> list = db.DEPARTMENTS.Where(x => x.VSIBILITY_STATUS == "Visible").ToList();
                    model.CityList = new SelectList(list, "DEPARTMENT_ID", "DEPARTMENT_NAME", "REGION.REGION_NAME", null, null); //to reload the data in the drop down list

                    return View("GetID", model);//if model is not valid //return the view with all validation messages 
                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                TempData["Danger"] = "Requester is already exists";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }

        }
 
        [HttpGet]
        public ActionResult GetID2(decimal REQUESTER_ID)
        {
            REQUESTER model = db.REQUESTERS.Where(Models => Models.REQUESTER_ID == REQUESTER_ID).FirstOrDefault();
            var check = db.CASES.Where(x => x.REQUESTER_ID == REQUESTER_ID).FirstOrDefault(); //check if (id) used in case table to delete or not


            if (check == null && model != null)
            {
                //allowed to delete
                return PartialView(_getRequester(REQUESTER_ID));
            }
            //not allowed to delete
            return PartialView("DeleteUsed");

        }
     
        [HttpPost]
        public ActionResult delete(decimal REQUESTER_ID)
        {

            var check = db.CASES.Where(x => x.REQUESTER_ID == REQUESTER_ID).FirstOrDefault();//Check if the requester is used in any case or not


            REQUESTER model = db.REQUESTERS.Where(Models => Models.REQUESTER_ID == REQUESTER_ID).FirstOrDefault();//get requester data from db



            if (check == null && model != null)
            {
                //delete
                db.REQUESTERS.Remove(model);
                db.SaveChanges();
                TempData["Success"] = "Requester deleted successfully"; //display a message to inform that (The Requester is deleted successfully)

                return RedirectToAction("Index");
            }

                return RedirectToAction("Index");

        }

        private requesterModel _getRequester(decimal id)
        {

            REQUESTER model = db.REQUESTERS.Where(x => x.REQUESTER_ID == id).FirstOrDefault();

            //get all related date to the crruent requester's id
            requesterModel data = new requesterModel();
            data.REQUESTER_ID = model.REQUESTER_ID;
            data.REQUESTER_NAME = model.REQUESTER_NAME;
            data.CONTACT = model.CONTACT;
            data.CALLING_FROM = model.CALLING_FROM;
            data.DEPARTMENT_ID = model.DEPARTMENT_ID;

            List<DEPARTMENT> list = db.DEPARTMENTS.Where(x => x.VSIBILITY_STATUS == "Visible").ToList();
            data.CityList = new SelectList(list, "DEPARTMENT_ID", "DEPARTMENT_NAME", "REGION.REGION_NAME", null, null); //departments list ... groubed by region name


            return data;

        }


    }




}