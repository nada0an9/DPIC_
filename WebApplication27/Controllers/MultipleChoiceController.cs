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
    [CustomAuthorize("Manage Choices")]

    public class MultipleChoiceController : Controller
    {
        private DB_Model db = new DB_Model();

        // GET: MultipleChoice

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

                    name = Filter_Value;// to filter the list  by name across all page

                }
                ViewBag.FilterValue = name;

                List<MULTIPLE_CHOICE> data = db.MULTIPLE_CHOICE.OrderBy(q => q.QUESTION.QUESTION1).ThenBy(X=>X.CHOICE_NAM).ToList();

                if (!String.IsNullOrEmpty(name) && name.Length <= 150)
                {
                     data = data.Where(m => m.CHOICE_NAM.ToLower().Contains(name.ToLower())).OrderBy(x => x.CHOICE_NAM).ToList();//filter by the choice name ... ordred by name

                }
    
                    return View(data.ToPagedList(pagenum, pagesize)); //list to all choice ordered by their names


        }

        public ActionResult create()
        {

            List<QUESTION> list = db.QUESTIONS.Where(m => m.ANSWER_TYPE == "Multiple Choice").Where(x=>x.VISIBILITY_STATUS == "Visible").OrderBy(x=>x.QUESTION1).ToList();
            ViewBag.question = new SelectList(list, "QUESTION_ID", "QUESTION1");  //list all visible questions that have an answer type (Multiple Choice)     
            return View("Create");

            
        }
    
        public ActionResult Save(choiceModel model)
        {
            try
            {


                if (ModelState.IsValid)
                {
                    MULTIPLE_CHOICE ch = new MULTIPLE_CHOICE();
                    ch.CHOICE_NAM = model.CHOICE_NAM;
                    ch.QUESTION_ID = model.QUESTION_ID;
                    ch.DISPLAY_ORDER = model.DISPLAY_ORDER;
                    ch.VISIBILITY_STATUS = model.VISIBILITY_STATUS;
                    ch.CREATED_DATE = DateTime.Now;
                    ch.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.MULTIPLE_CHOICE.Add(ch); //insert new choice
                    db.SaveChanges();
                    TempData["Success"] =  ch.CHOICE_NAM+ " has been saved successfully";
                    return RedirectToAction("Index"); //display a message to inform that (The choice is added successfully)

                }
                else
                {

                    List<QUESTION> list = db.QUESTIONS.Where(m => m.ANSWER_TYPE == "Multiple Choice").Where(x => x.VISIBILITY_STATUS == "Visible").OrderBy(x => x.QUESTION1).ToList();
                    ViewBag.question = new SelectList(list, "QUESTION_ID", "QUESTION1");
                    return View("Create", model); //if model is not valid  //we return the view with all validation message and question list

                }
            }
            catch (Exception ex)when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
                {
                    TempData["Danger"] = "Choice is already exists";
                    return RedirectToAction("Index");
                }
            catch (Exception)
            {
                    TempData["Danger"] = "Sorry, something went wrong";
                    return RedirectToAction("Index");
            }


        }

        public ActionResult GetID2(int id)
        {

              MULTIPLE_CHOICE model = db.MULTIPLE_CHOICE.Where(Models => Models.CHOICE_ID == id).SingleOrDefault();

              List<QUESTION> list = db.QUESTIONS.Where(m => m.ANSWER_TYPE == "Multiple Choice").ToList();
              ViewBag.question = new SelectList(list, "QUESTION_ID", "QUESTION1"); //list all visible questions that have an answer type (Multiple Choice)        

                if (model != null && CheckId(id) == "Unused")
                {
                    return View("Edit", _getChoice(id));

                }
                if (model != null && CheckId(id) == "Used")
                {
                return View("EditUsed", _getChoice(id));

                }



            TempData["Danger"] = "You cannot edit this choice, since it has been used before";
            return RedirectToAction("Index");

                

        }
     
        public ActionResult edit(choiceModel model)
        {
            try
            {

                MULTIPLE_CHOICE data = db.MULTIPLE_CHOICE.FirstOrDefault(Models => Models.CHOICE_ID == model.CHOICE_ID);

                if (ModelState.IsValid)
                {
                   
                    if (data != null && CheckId(model.CHOICE_ID) == "Unused")
                    {
                        //update
                        data.QUESTION_ID = model.QUESTION_ID;
                        data.CHOICE_NAM = model.CHOICE_NAM;
                        data.DISPLAY_ORDER = model.DISPLAY_ORDER;
                        data.VISIBILITY_STATUS = model.VISIBILITY_STATUS;
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);

                        TempData["Success"] = "The changes are saved successfully";//display a message to inform that  (changes are saved successfully)

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    if (data != null && CheckId(model.CHOICE_ID) == "Used")
                    {
                        //update status only
                        data.VISIBILITY_STATUS = model.VISIBILITY_STATUS;
                        data.CHANGED_DATE = DateTime.Now;
                        data.CREATED_BY = Convert.ToDecimal(User.Identity.Name);

                        TempData["Success"] = "The changes are saved successfully";//display a message to inform that  (changes are saved successfully)

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else 
                { 

                    List<QUESTION> list = db.QUESTIONS.Where(m => m.ANSWER_TYPE == "Multiple Choice").Where(x => x.VISIBILITY_STATUS == "Visible").OrderBy(x => x.QUESTION1).ToList();
                    ViewBag.question = new SelectList(list, "QUESTION_ID", "QUESTION1");
                    return View("Edit", model);   //if model is not valid  //we return the page with all validation message and question list

                }
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                TempData["Danger"] = "Choice already exist";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }

            }

        public ActionResult GetID3(int CHOICE_ID)

        {
            MULTIPLE_CHOICE model = db.MULTIPLE_CHOICE.FirstOrDefault(Models => Models.CHOICE_ID == CHOICE_ID);

            if (model != null && CheckId(model.CHOICE_ID) == "Unused")
            {

                return PartialView(_getChoice(CHOICE_ID));


            }
            return PartialView("DeleteUsed");//not allowed to delete


        }

        public ActionResult Delete(int CHOICE_ID)
        {
            try
            {
                MULTIPLE_CHOICE model = db.MULTIPLE_CHOICE.FirstOrDefault(Models => Models.CHOICE_ID == CHOICE_ID);



                if (model != null && CheckId(model.QUESTION_ID) == "Unused")
                {
                    db.MULTIPLE_CHOICE.Remove(model);//delete the choice
                    db.SaveChanges();
                    TempData["Success"] = "Choice deleted successfully"; //display a message to inform that (The choice is deleted successfully)
                    return RedirectToAction("Index");
                }

                TempData["Danger"] = "You cannot delete this choice, since it has been used before";
                return RedirectToAction("Index");

                
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-02292"))
            {
                TempData["Danger"] = "You cannot delete this choice, since it has been used before";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }


        }

        private choiceModel _getChoice(int id)
        {

            MULTIPLE_CHOICE model = db.MULTIPLE_CHOICE.Where(Models => Models.CHOICE_ID == id).SingleOrDefault();

            choiceModel c = new choiceModel();
            c.CHOICE_ID = model.CHOICE_ID;
            c.QUESTION_ID = model.QUESTION_ID;
            c.CHOICE_NAM = model.CHOICE_NAM;
            c.VISIBILITY_STATUS = model.VISIBILITY_STATUS;
            c.DISPLAY_ORDER = model.DISPLAY_ORDER;
            return c;

        }

        private string CheckId(decimal id)
        {
            var check = db.CASE_QUESTIONS.Where(x => x.CHOICE_ID == id).FirstOrDefault();

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




    }
}