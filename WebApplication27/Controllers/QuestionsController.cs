using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication27.Models;
using System.Web.Mvc;
using System.Data;
using PagedList;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{

    [CustomAuthenticationFilter]
    [CustomAuthorize("Manage Questions")]

    public class QuestionsController : Controller
    {
        private DB_Model db = new DB_Model();
        // GET: Questions
        [HttpGet]
        public ActionResult Index(int? page ,string name, string Filter_Value)
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

                 List<QUESTION> data = db.QUESTIONS.OrderBy(q => q.CATEGORy.CATEGORY_NAME).ThenBy(x=>x.QUESTION1).ToList();  //list to all questions ...ordred by name

                if (!String.IsNullOrEmpty(name) && name.Length <= 250)
                {
                   data  = data.Where(m => m.QUESTION1.ToLower().Contains(name.ToLower())).ToList();//filter by the question name
                }
                

                     return View(data.ToPagedList(pagenum, pagesize));
        }
   
        [HttpGet]
        public ActionResult create()
        {
            List<CATEGORy> list = db.CATEGORIES.Where(x => x.VSIBILITY_STATUS == "Visible").OrderBy(x => x.CATEGORY_NAME).ToList();

            var model = new questionsModel() //list all visible categories(grouped by category) to bind it in view

            {
                CategoriesList = new SelectList(list, "CATEGORY_ID", "CATEGORY_NAME", "CATEGORY_TYPE", null, null)
            };

            return View("create", model);

            
        }

        [HttpPost]
        public ActionResult Save(questionsModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //insert new question
                    QUESTION que = new QUESTION();
                    que.QUESTION1 = model.QUESTION1;
                    que.CATEGORY_ID = model.CATEGORY_ID;
                    que.ANSWER_TYPE = model.ANSWER_TYPE;
                    que.VISIBILITY_STATUS = model.VISIBILITY_STATUS;
                    que.DEFAULT_QUESTION = model.DEFAULT_QUESTION;
                    que.DISPLAY_ORDER = model.DISPLAY_ORDER;
                    que.CREATED_DATE = DateTime.Now;
                    que.CREATED_BY = Convert.ToDecimal(User.Identity.Name);
                    db.QUESTIONS.Add(que);
                    db.SaveChanges();

                    TempData["Success"] = que.QUESTION1 + " has been saved successfully"; //display a message to inform that (The question is added successfully)
                    return RedirectToAction("Index");

                }
                else
                {

                  
                    List<CATEGORy> list = db.CATEGORIES.Where(x => x.VSIBILITY_STATUS == "Visible").OrderBy(x => x.CATEGORY_NAME).ToList();
                    model.CategoriesList = new SelectList(list, "CATEGORY_ID", "CATEGORY_NAME", "CATEGORY_TYPE", null, null);
                    return View("create", model);  //if model is not valid  //return the view with all validation message and category list

                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
                {
                    TempData["Danger"] = "Question is already exists";
                    return RedirectToAction("Index");
                }
         
                    TempData["Danger"] = "Sorry, something went wrong";
                    return RedirectToAction("Index");
                

            }

        }

        [HttpGet]
        public ActionResult details(decimal id)
        {
            bool Found = db.QUESTIONS.Where(x => x.QUESTION_ID == id).Any();
            if (Found)
            {
                return View(_getQuestion(id));

            }
            return RedirectToAction("Index", "Error");

        }

        [HttpGet]
        public ActionResult GetID2(decimal id)
        {
            bool Found=  db.QUESTIONS.Where(x=>x.QUESTION_ID == id).Any();
            if (Found)
            {
                string check = CheckQuestionId(id);
                switch (check)
                {
                    case "Used In Completed Case":

                        return View("EditUsed", _getQuestion(id));

                    case "Used In Pending Case":

                        TempData["Danger"] = "You cannot edit this question since it has been used in pending case";
                        return RedirectToAction("Index");

                    case "Not Used at all in any case":

                        return View("Edit", _getQuestion(id));

                    default:

                        return RedirectToAction("Index");


                }
            }
            return RedirectToAction("Index", "Error");

        }

        [HttpPost]
        public ActionResult edit(questionsModel model)
        {
            try
            {

            if (ModelState.IsValid)
            {
                       
                    QUESTION data = db.QUESTIONS.FirstOrDefault(Models => Models.QUESTION_ID == model.QUESTION_ID);

                    if (data != null)
                    {
                        //update all data
                        data.QUESTION1 = model.QUESTION1;
                        data.CATEGORY_ID = model.CATEGORY_ID;
                        data.ANSWER_TYPE = model.ANSWER_TYPE;
                        data.VISIBILITY_STATUS = model.VISIBILITY_STATUS;
                        data.DEFAULT_QUESTION = model.DEFAULT_QUESTION;
                        data.DISPLAY_ORDER = model.DISPLAY_ORDER;
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
                 
                    List<CATEGORy> list = db.CATEGORIES.Where(x => x.VSIBILITY_STATUS == "Visible").OrderBy(x => x.CATEGORY_NAME).ToList();
                    model.CategoriesList = new SelectList(list, "CATEGORY_ID", "CATEGORY_NAME", "CATEGORY_TYPE", null, null);
                    return View("Edit", model); //if model is not valid  //we return the page with all validation message and category list
            }
            }

            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-00001"))
            {
                TempData["Danger"] = "Question already exist";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }




        }

        [HttpPost]
        public ActionResult editUsed(questionsModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    QUESTION data = db.QUESTIONS.FirstOrDefault(Models => Models.QUESTION_ID == model.QUESTION_ID);//get all releted data to the cruuent ID

                    if (data != null)
                    {
                        //update the status 
                        data.VISIBILITY_STATUS = model.VISIBILITY_STATUS;
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
        public ActionResult GetID3(int QUESTION_ID)
        {

            bool Found = db.QUESTIONS.Where(x => x.QUESTION_ID == QUESTION_ID).Any();
            bool checkChoiceTable = db.CASE_QUESTIONS.Where(x => x.QUESTION_ID == QUESTION_ID).Any();
            bool checkCaseQuestionTable = db.CASE_QUESTIONS.Where(x => x.QUESTION_ID == QUESTION_ID).Any();

            if (Found && (checkChoiceTable || checkCaseQuestionTable))
            {
                return PartialView("DeleteUsed");
            }
            else if(Found && (checkChoiceTable == false || checkCaseQuestionTable == false))
            {
                return PartialView(_getQuestion(QUESTION_ID));

            }

            return RedirectToAction("Index", "Error");





        }

        [HttpPost]
        public ActionResult deleteQuestion(questionsModel model)
        {
            try
            {
                QUESTION data = db.QUESTIONS.Where(Models => Models.QUESTION_ID == model.QUESTION_ID).FirstOrDefault();

                if(data != null) 
                { 
                //delete question
                db.QUESTIONS.Remove(data);
                db.SaveChanges();
                TempData["Success"] = "Question deleted successfully";//display a message to inform that (The Question is deleted successfully)
                    return RedirectToAction("Index");

                }

                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex) when (ex.InnerException.InnerException.Message.Contains("ORA-02292"))
            {
                TempData["Danger"] = "You cannot delete this question since it has been used before";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");

            }

        }

        private string CheckQuestionId(decimal id)
        {
            //check if used in case questions table  to update all data or only the status 
            var check = db.CASE_QUESTIONS.Where(x => x.QUESTION_ID == id).FirstOrDefault();

            //check if used in case question and the case still pending to prevent the edit 
            var Result = from b in db.CASE_QUESTIONS
                         from c in db.CASES
                         where c.CASE_STATUS == "Pending" && b.QUESTION_ID == id && b.CASE_ID == c.CASE_ID
                         select new
                         { b };
            var s = Result.FirstOrDefault();

 


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

        private questionsModel _getQuestion(decimal id)
        {
            QUESTION model = db.QUESTIONS.Where(Models => Models.QUESTION_ID == id).FirstOrDefault();//get all related data to question id e.g (name, status, etc)

            questionsModel q = new questionsModel();
            q.QUESTION_ID = model.QUESTION_ID;
            q.QUESTION1 = model.QUESTION1;
            q.CATEGORY_ID = model.CATEGORY_ID;
            q.ANSWER_TYPE = model.ANSWER_TYPE;
            q.DEFAULT_QUESTION = model.DEFAULT_QUESTION;
            q.DISPLAY_ORDER = model.DISPLAY_ORDER;
            q.VISIBILITY_STATUS = model.VISIBILITY_STATUS;

             List<CATEGORy> list = db.CATEGORIES.Where(x => x.VSIBILITY_STATUS == "Visible").OrderBy(x => x.CATEGORY_NAME).ToList();
              q.CategoriesList = new SelectList(list, "CATEGORY_ID", "CATEGORY_NAME", "CATEGORY_TYPE", null, null);
              return q;

        }




    }
}