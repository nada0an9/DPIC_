using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using WebApplication27.Models;
using System.Web.Mvc;
using PagedList;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize("Manage Categories")]
    public class CategoriesController : Controller
    {
        private DB_Model db = new DB_Model();
        // GET: Categories
   
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

                List<CATEGORy> data = db.CATEGORIES.OrderBy(x => x.CATEGORY_NAME).ToList();
                if (!String.IsNullOrEmpty(name) && name.Length <= 225)
                {
                   data = data.Where(m => m.CATEGORY_NAME.ToLower().Contains(name.ToLower())).ToList();//filter by the category name ... ordred by name

                }
          
            return View(data.ToPagedList(pagenum, pagesize));
                
            
 
        }

        public ActionResult create()
        {
            return View("create");

        }
     
        [HttpPost]
        public ActionResult Save(categoriesModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CATEGORy cate = new CATEGORy();
                    cate.CATEGORY_NAME = model.CATEGORY_NAME;
                    cate.CATEGORY_TYPE = model.CATEGORY_TYPE;
                    cate.VSIBILITY_STATUS = model.VSIBILITY_STATUS;
                    cate.DEFUALT = model.DEFUALT;
                    cate.DISPLAY_ORDER = model.DISPLAY_ORDER;
                    cate.CREATED_DATE = DateTime.Now;
                    cate.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.CATEGORIES.Add(cate); //insert the category information to category table
                    db.SaveChanges();

                    TempData["Success"] = cate.CATEGORY_NAME + " has been saved successfully";//display a message to inform that (The category is added successfully)
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("create", model);  //validation alert messages in view

                }
            }
            catch (Exception ex)
            {
                if(ex.InnerException.InnerException.Message.Contains("ORA-00001"))
                {
                    TempData["Danger"] = "Category is already exists";
                    return RedirectToAction("Index");
                }
        
                    TempData["Danger"] = "Sorry, something went wrong";
                    return RedirectToAction("Index");
                
         
            }
        }


        [HttpGet]
        public ActionResult GetID2(int id)
        {

            bool Found = db.CATEGORIES.Where(x => x.CATEGORY_ID == id).Any();
            if (Found)
            {
                string check = CheckCategoryId(id);

                switch (check)
                {
                    case "Used In Completed Case":

                        return View("EditUsed", _getCategory(id));

                    case "Used In Pending Case":

                        TempData["Danger"] = "You cannot edit this category since it has been used in pending case";
                        return RedirectToAction("Index");

                    case "Not Used at all in any case":

                        return View("Edit", _getCategory(id));

                    default:

                        return RedirectToAction("Index");


                }
            }
            return RedirectToAction("Index", "Error");

        }

        [HttpPost]
        public ActionResult edit(categoriesModel model)
        {
            try
            {
                CATEGORy data = db.CATEGORIES.FirstOrDefault(Models => Models.CATEGORY_ID == model.CATEGORY_ID);

                if (ModelState.IsValid)
                {

                    if (data != null)
                    {
                        data.VSIBILITY_STATUS = model.VSIBILITY_STATUS;
                        data.CATEGORY_NAME = model.CATEGORY_NAME;
                        data.CATEGORY_TYPE = model.CATEGORY_TYPE;
                        data.DEFUALT = model.DEFUALT;
                        data.DISPLAY_ORDER = model.DISPLAY_ORDER;
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                        db.SaveChanges();
                        //display a message to inform that (The changes are saved successfully)
                        TempData["Success"] = "The changes are saved successfully";
                        return RedirectToAction("Index");
                    }

                    return RedirectToAction("Index", "Error");
                    
                }
                else
                {
                    //returen the validation alert message 
                    return View("Edit", model);
                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                TempData["Danger"] = "Category already exist";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }

        }

        [HttpPost]
        public ActionResult editUsed(categoriesModel model)
        {
            try
            {
                CATEGORy data = db.CATEGORIES.FirstOrDefault(Models => Models.CATEGORY_ID == model.CATEGORY_ID);


                if (ModelState.IsValid)
                {
                    if (data != null)
                    {
                        //update status 
                        data.VSIBILITY_STATUS = model.VSIBILITY_STATUS;
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                        db.SaveChanges();
                        TempData["Success"] = "The changes are saved successfully"; //display a message to inform that (The changes are saved successfully)

                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index", "Error");


                }
                else
                {
                    //returen the validation alert message 
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
        public ActionResult GetID3(int CATEGORY_ID)
        {

                bool Found = db.CATEGORIES.Where(x => x.CATEGORY_ID == CATEGORY_ID).Any();
                bool check = db.QUESTIONS.Where(x => x.CATEGORY_ID == CATEGORY_ID).Any();
                bool check2 = db.CASE_CATEGORIES.Where(x => x.CATEGORY_ID == CATEGORY_ID).Any();

                if (Found && (check || check2))
                {
                    return PartialView("DeleteUsed");
                }
                else if (Found && (check == false || check2 == false))
                {
                    return PartialView(_getCategory(CATEGORY_ID));

                }

                return RedirectToAction("Index", "Error");
        }

        [HttpPost]
        public ActionResult delete(int CATEGORY_ID)
        {
            try
            {
                CATEGORy model = db.CATEGORIES.Where(Models => Models.CATEGORY_ID == CATEGORY_ID).FirstOrDefault();

                if(model != null)
                {
                    db.CATEGORIES.Remove(model); //delete current  category
                    db.SaveChanges();
                    TempData["Success"] = "Category deleted successfully";//display a message to inform that (The category is deleted successfully)
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index", "Error");


            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-02292"))
            {
                    TempData["Danger"] = "You cannot delete this category since it has been used before";
                    return RedirectToAction("Index");
            }
            catch (Exception)
            {
                    TempData["Danger"] = "Sorry, something went wrong";
                    return RedirectToAction("Index");
                
            }
        }

        private string CheckCategoryId(int id)
        {

            var check = db.CASE_CATEGORIES.Where(x => x.CATEGORY_ID == id).FirstOrDefault(); //check if used in case category table to update all data OR to update the status only

            var Result = from b in db.CASE_CATEGORIES
                         from c in db.CASES
                         where c.CASE_STATUS == "Pending" && b.CATEGORY_ID == id && b.CASE_ID == c.CASE_ID
                         select new
                         { b };
            var s = Result.FirstOrDefault(); //check if used in case category and the case still pending




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

        private categoriesModel _getCategory(int id)
        {

            CATEGORy model = db.CATEGORIES.Where(Models => Models.CATEGORY_ID == id).SingleOrDefault();//get all related data to the current category id  

            categoriesModel categories = new categoriesModel();
            categories.CATEGORY_ID = model.CATEGORY_ID;
            categories.CATEGORY_NAME = model.CATEGORY_NAME;
            categories.CATEGORY_TYPE = model.CATEGORY_TYPE;
            categories.VSIBILITY_STATUS = model.VSIBILITY_STATUS;
            categories.DEFUALT = model.DEFUALT;
            categories.DISPLAY_ORDER = model.DISPLAY_ORDER;
            return categories;

        }


    }
    }