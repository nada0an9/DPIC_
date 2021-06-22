using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication27.Models;
using PagedList;
using System.Data;
using System.Globalization;
using ClosedXML.Excel;
using System.IO;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize("Create New Case")]

    public class CaseController : Controller
    {
        // Case
        private DB_Model db = new DB_Model();

        public ActionResult Index()
        {
            //requesters list to bind it in the view 
            List<REQUESTER> list = db.REQUESTERS.ToList();
            ViewBag.Requester = new SelectList(list, "REQUESTER_ID", "REQUESTER_NAME");
            return View();
        }

        [HttpPost]
        public ActionResult saveFirstStage(caseDescriptionStage model)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    String now = DateTime.Now.ToString("MM/dd/yyyy HH:mm");//datetime format
                    DateTime dt = DateTime.Parse(now, new CultureInfo("en-CA"));
                    Case data = new Case();// get data from model
                    data.CASE_START_DATE = dt;
                    data.REQUESTER_ID = model.REQUESTER_ID;
                    data.CASE_DESCRIBTION = model.CASE_DESCRIBTION;
                    data.CASE_TYPE = model.CASE_TYPE;
                    data.CASE_STATUS = "Pending";
                    data.USER_ID = Convert.ToDecimal(User.Identity.Name);
                    data.CURRENT_STAGE = "Description stage";
                    data.URGENCY = model.URGENCY;
                    data.CASE_END_DATE = DateTime.Now;
                    db.CASES.Add(data); //insert the data
                    db.SaveChanges();
                    decimal id = data.CASE_ID;// get crrunt user id  
                    string CaseType = data.CASE_TYPE; // get case type

                    if (CaseType == "General Questions")
                    {

                        return RedirectToAction("getListCategories", new { id });  //if case with general type //we go to the categories stage
                    }

                    return RedirectToAction("getPatientInformation", new { id }); //if case with spesific type // we go to patient information stage
                }
                else
                {

                    List<REQUESTER> list = db.REQUESTERS.ToList();
                    ViewBag.Requester = new SelectList(list, "REQUESTER_ID", "REQUESTER_NAME");//requesters list to bind it in the view 
                    return View("Index", model);//if model is not valid  //we return the view with all validation messages


                }
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult getCaseDescription(decimal id)
        {
            //get all related data to the current case's Id 
            //to check case is pending 
            Case model = db.CASES.Where(Models => Models.CASE_ID == id).Where(models => models.CASE_STATUS == "Pending" || models.CASE_STATUS == "Pending for Approval").SingleOrDefault();

            //requester list to bind it in the view 
            List<REQUESTER> list = db.REQUESTERS.ToList();
            ViewBag.Requester = new SelectList(list, "REQUESTER_ID", "REQUESTER_NAME");
            if (model != null)
            {
                caseDescriptionStage data = new caseDescriptionStage();
                data.CASE_ID = model.CASE_ID;
                data.REQUESTER_ID = model.REQUESTER_ID;
                data.CASE_DESCRIBTION = model.CASE_DESCRIBTION;
                data.CASE_TYPE = model.CASE_TYPE;
                data.URGENCY = model.URGENCY;
                data.CASE_TYPE = model.CASE_TYPE;

                return View(data);
            }

            return View();


        }

        [HttpPost]
        public ActionResult updateCaseDescription(caseDescriptionStage model)
        {
            try
            {
                decimal id = model.CASE_ID;

                if (ModelState.IsValid)
                {

                    Case data = db.CASES.Where(Models => Models.CASE_ID == id).SingleOrDefault();
                    if (data != null)
                    {
                        data.REQUESTER_ID = model.REQUESTER_ID;
                        data.CASE_DESCRIBTION = model.CASE_DESCRIBTION;
                        data.CASE_TYPE = model.CASE_TYPE;
                        data.CASE_STATUS = "Pending";
                        data.USER_ID = Convert.ToDecimal(User.Identity.Name);
                        data.CURRENT_STAGE = "Description stage";
                        data.URGENCY = model.URGENCY;
                        data.CASE_END_DATE = DateTime.Now;

                        db.SaveChanges(); //save data
                        string CaseType = data.CASE_TYPE;

                        if (CaseType == "General Questions")
                        {
                            //go to categories stage
                            return RedirectToAction("getListCategories", new { id });
                        }

                        //go to patient information stage
                        return RedirectToAction("getPatientInformation", new { id });
                    }
                    return RedirectToAction("Index", "Error");


                }
                else
                {

                    List<REQUESTER> list = db.REQUESTERS.ToList();
                    ViewBag.Requester = new SelectList(list, "REQUESTER_ID", "REQUESTER_NAME"); //requester list to bind it in the view 

                    return View("getCaseDescription", model);//if model is not valid //we return the view with all validation messages

                }
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult getPatientInformation(decimal id)
        {

            Case model = db.CASES.Where(Models => Models.CASE_ID == id).Where(models => models.CASE_STATUS == "Pending" || models.CASE_STATUS == "Pending for Approval").SingleOrDefault();

            if (model != null)
            {
                //get patient Information
                patientInformationStage data = new patientInformationStage();
                data.CASE_ID = model.CASE_ID;
                data.patientMRN = model.HAS_MRN_OR_NOT;
                data.MRN = model.MRN;
                data.NAME = model.NAME;
                data.AGE = model.AGE;
                data.GENDER = model.GENDER;
                data.PREGNANT_OR_NOT = model.PREGNANT_OR_NOT;
                data.PREGNANT_WEEK = model.PREGNANT_WEEK;
                data.ALLERGY_HISTORY = model.ALLERGY_HISTORY;
                data.HEIGHT = model.HEIGHT;
                data.WEIGHT = model.WEIGHT;
                data.CASE_START_DATE = model.CASE_START_DATE;
                data.DIAGNOSIS = model.DIAGNOSIS;
                data.ELECTROLYTES = model.ELECTROLYTES;
                data.LIVER_FUNCTION = model.LIVER_FUNCTION;
                data.LABORATORY_RESULT = model.LABORATORY_RESULT;
                data.SERUM_CREATININE = model.SERUM_CREATININE;
                data.MEDICATIONS = model.MEDICATIONS;
                data.RELATED_SINGS = model.RELATED_SINGS;

                //get comorbidities of the case
                var Result = from b in db.COMORBIDITies
                             where (b.VISIBILIT_STATUS == "Visible")
                             orderby (b.DISPLAY_ORDER)
                             select new
                             {
                                 b.COMORBIDITY_ID,
                                 b.COMORBIDITY_NAME,
                                 Checked = ((from ab in db.CASE_COMORBIDITY
                                             where (ab.CASE_ID == id) & (ab.COMORBIDITY_ID == b.COMORBIDITY_ID)
                                             select ab).Count() > 0)//subquery to check if the comorbidity is assigned to the current case's id or not 
                             };

                //make list of comorbidities
                var ComorbidityCheckboxlist = new List<comorbidities_>();

                foreach (var item in Result)
                {

                    string result2 = (from c in db.CASE_COMORBIDITY
                                      where c.CASE_ID == model.CASE_ID && c.OTHER_COMORBIDITY != null
                                      select c.OTHER_COMORBIDITY).FirstOrDefault();//query to get other comorbidity if exists

                    //add every item in result query to the (ComorbidityCheckboxlist)
                    ComorbidityCheckboxlist.Add(new comorbidities_ { id = item.COMORBIDITY_ID, name = item.COMORBIDITY_NAME, Checked = item.Checked, other = result2 });


                }

                data.ComorbidityList = ComorbidityCheckboxlist;
                return View(data);
            }


            return View();

        }

        [HttpPost]
        public ActionResult UpdatePatientInformation(decimal CASE_ID, patientInformationStage model, string SaveAsPending)
        {
            decimal id = model.CASE_ID;

            if (ModelState.IsValid)
            {
                Case data = db.CASES.Where(Models => Models.CASE_ID == CASE_ID).SingleOrDefault();

                if (data != null)
                {
                    //save Patient information
                    data.MRN = model.MRN;
                    data.NAME = model.NAME;
                    data.HAS_MRN_OR_NOT = model.patientMRN;
                    data.AGE = model.AGE;
                    data.GENDER = model.GENDER;
                    data.PREGNANT_OR_NOT = model.PREGNANT_OR_NOT;
                    data.PREGNANT_WEEK = model.PREGNANT_WEEK;
                    data.ALLERGY_HISTORY = model.ALLERGY_HISTORY;
                    data.WEIGHT = model.WEIGHT;
                    data.HEIGHT = model.HEIGHT;
                    data.DIAGNOSIS = model.DIAGNOSIS;
                    data.SERUM_CREATININE = model.SERUM_CREATININE;
                    data.LIVER_FUNCTION = model.LIVER_FUNCTION;
                    data.ELECTROLYTES = model.ELECTROLYTES;
                    data.LABORATORY_RESULT = model.LABORATORY_RESULT;
                    data.MEDICATIONS = model.MEDICATIONS;
                    data.RELATED_SINGS = model.RELATED_SINGS;
                    data.USER_ID = Convert.ToDecimal(User.Identity.Name);
                    data.CASE_STATUS = "Pending";
                    data.CURRENT_STAGE = "Patient stage";
                    data.CASE_END_DATE = DateTime.Now;
                    db.SaveChanges();

                    //delete all comorbidities with the current case's id
                    foreach (var item in db.CASE_COMORBIDITY)
                    {

                        if (item.CASE_ID == CASE_ID)

                        {
                            db.Entry(item).State = System.Data.EntityState.Deleted;

                        }

                    }

                    //validate comorbidities list 
                    foreach (var item in model.ComorbidityList)
                    {
                        var comorbiditiesIdCheck = db.COMORBIDITies.Where(x => x.COMORBIDITY_ID == item.id).Any();
                        if (comorbiditiesIdCheck == false || (item.name != null && item.name.Length > 220) || (item.other != null && item.other.Length > 150))
                        {
                            TempData["Danger"] = "at least one comorbidity needs to be selected";
                            return RedirectToAction("getPatientInformation", new { id });
                        }


                    }

                    //add all checked comorbidities to the case comorbidities table
                    foreach (var item in model.ComorbidityList)
                    {
                        if (item.Checked)
                        {
                            db.CASE_COMORBIDITY.Add(new CASE_COMORBIDITY() { CASE_ID = model.CASE_ID, COMORBIDITY_ID = item.id, COMORBIDITY_NAME = item.name, OTHER_COMORBIDITY = item.other });
                            db.SaveChanges();

                        }
                    }

                    if (SaveAsPending != null)
                    {
                        return RedirectToAction("Filter"); //if "save as pending case" button is pressed

                    }

                    return RedirectToAction("getListCategories", new { id });
                }
                return RedirectToAction("Index", "Error");

            }
            else
            {    //if model is not valid 
                //we return the view with all validation messages
                return View("getPatientInformation", model);

            }



        }

        [HttpGet]
        public ActionResult getListCategories(decimal id)
        {
            Case model = db.CASES.Where(Models => Models.CASE_ID == id).Where(models => models.CASE_STATUS == "Pending" || models.CASE_STATUS == "Pending for Approval").SingleOrDefault();

            if (model != null)
            {
                var myviewmodel = new CaseViewModel();
                myviewmodel.CASE_ID = id;
                myviewmodel.CASE_TYPE = model.CASE_TYPE;


                //command to get a List of categories and subquery to check for each category it is assigned to the current case or not 
                var Result = from b in db.CATEGORIES
                             from c in db.CASES
                             where (c.CASE_ID == id) & (b.CATEGORY_TYPE == c.CASE_TYPE) & (b.VSIBILITY_STATUS == "Visible")
                             orderby (b.DISPLAY_ORDER)
                             select new
                             {
                                 b.CATEGORY_ID,
                                 b.CATEGORY_NAME,
                                 b.DEFUALT,
                                 Checked = ((from ab in db.CASE_CATEGORIES
                                             where (ab.CASE_ID == id) & (ab.CATEGORY_ID == b.CATEGORY_ID)
                                             select ab).Count() > 0) ////subquery to check if the category is assigned to the current case's id or not 
                             };

                var categoryCheckboxlist = new List<categories_>();

                //assign every item from the query to the "categoryCheckboxlist" list
                foreach (var item in Result)
                {
                    if (item.DEFUALT == "Yes")
                    {
                        categoryCheckboxlist.Add(new categories_ { id = item.CATEGORY_ID, name = item.CATEGORY_NAME, Checked = true, defult = item.DEFUALT });

                    }
                    if (item.DEFUALT == "No")
                    {
                        categoryCheckboxlist.Add(new categories_ { id = item.CATEGORY_ID, name = item.CATEGORY_NAME, Checked = item.Checked, defult = item.DEFUALT });

                    }

                }

                //return categoies list to the view
                myviewmodel.CategoryList = categoryCheckboxlist;
                return View(myviewmodel);
            }

            return View();

        }

        [HttpPost]
        public ActionResult SaveListCategories(CaseViewModel model, string SaveAsPending)
        {
            try
            {
                Case data = db.CASES.Where(models => models.CASE_STATUS == "Pending" || models.CASE_STATUS == "Pending for Approval").FirstOrDefault(Models => Models.CASE_ID == model.CASE_ID);

                if (data != null)
                {
                    decimal Id = model.CASE_ID;


                    //Update cruennt stage info
                    data.CURRENT_STAGE = "Categories stage";
                    data.USER_ID = Convert.ToDecimal(User.Identity.Name);
                    data.CASE_STATUS = "Pending";
                    data.CASE_END_DATE = DateTime.Now;
                    db.SaveChanges();

                    //delete all categories 
                    foreach (var item in db.CASE_CATEGORIES)
                    {


                        if (item.CASE_ID == model.CASE_ID)

                        {
                            db.Entry(item).State = System.Data.EntityState.Deleted;

                        }
                    }

                    //add categories
                    foreach (var item in model.CategoryList)
                    {
                        var categoryIdCheck = db.CATEGORIES.Where(x => x.CATEGORY_ID == item.id).Any();

                        if (item.Checked && categoryIdCheck && (item.name != null && item.name.Length <= 255))
                        {
                            //insert every item to case categories table
                            db.CASE_CATEGORIES.Add(new CASE_CATEGORIES() { CASE_ID = model.CASE_ID, CATEGORY_ID = item.id, CATEGORY_NAME = item.name });
                            db.SaveChanges();
                        }
                    }

                    if (SaveAsPending != null) //if the "save as pending" button clicked, it will go to the "filter" action
                    {

                        return RedirectToAction("Filter");
                    }


                    return RedirectToAction("getListQuestions", "Case", new { Id });

                }
                return RedirectToAction("Index", "Error");
            }
            catch (Exception)
            {
                TempData["Danger"] = "Sorry, something went wrong";
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public ActionResult caseQuestions(decimal id)
        {
            //list to save 
            Case model = db.CASES.Where(Models => Models.CASE_ID == id).Where(models => models.CASE_STATUS == "Pending" || models.CASE_STATUS == "Pending for Approval").SingleOrDefault();

            if (model != null)
            {
                //query to get a list of questions and check for each question (if it is assigned to the current case's id or not) 
                var Result = from b in db.QUESTIONS
                             join f in db.CASE_CATEGORIES on b.CATEGORY_ID equals f.CATEGORY_ID
                             where (f.CASE_ID == id) & (b.VISIBILITY_STATUS == "Visible")
                             orderby (b.DISPLAY_ORDER)
                             select new
                             {
                                 b.QUESTION_ID,
                                 b.QUESTION1,
                                 b.ANSWER_TYPE,
                                 b.DEFAULT_QUESTION,
                                 b.CATEGORy.CATEGORY_NAME,
                                 Checked = ((from ab in db.CASE_QUESTIONS
                                             where (ab.CASE_ID == id) & (ab.QUESTION_ID == b.QUESTION_ID)
                                             select ab).Count() > 0)
                             };


                var myviewmodel = new CaseViewModel();
                myviewmodel.CASE_ID = id;

                //make list of questions
                var QuestionsCheckboxlist = new List<questions_>();



                //assign every item in the query to the "QuestionsCheckboxlist" list
                foreach (var item in Result)
                {
                    if (item.DEFAULT_QUESTION == "Yes")
                    {
                        QuestionsCheckboxlist.Add(new questions_ { id = item.QUESTION_ID, Category = item.CATEGORY_NAME, name = item.QUESTION1, fieldType = item.ANSWER_TYPE, defult = item.DEFAULT_QUESTION, Checked = true });


                    }
                    if (item.DEFAULT_QUESTION == "No")
                    {
                        QuestionsCheckboxlist.Add(new questions_ { id = item.QUESTION_ID, Category = item.CATEGORY_NAME, name = item.QUESTION1, fieldType = item.ANSWER_TYPE, defult = item.DEFAULT_QUESTION, Checked = item.Checked });


                    }

                }



                myviewmodel.QuestionsList = QuestionsCheckboxlist;
                return View(myviewmodel);

            }

            return View();



        }

        [HttpGet]
        public ActionResult getListQuestions(decimal Id)
        {
            //just list
            Case model = db.CASES.Where(Models => Models.CASE_ID == Id).Where(models => models.CASE_STATUS == "Pending" || models.CASE_STATUS == "Pending for Approval").SingleOrDefault();

            if (model != null)
            {
                //query to get a list of case questions to the current case's id
                var Result = from b in db.QUESTIONS
                             join f in db.CASE_CATEGORIES on b.CATEGORY_ID equals f.CATEGORY_ID
                             join l in db.CASE_QUESTIONS on b.QUESTION_ID equals l.QUESTION_ID
                             where (f.CASE_ID == Id) & (b.VISIBILITY_STATUS == "Visible") & (l.CASE_ID == Id)
                             orderby b.CATEGORy.DISPLAY_ORDER, b.DISPLAY_ORDER
                             select new
                             {
                                 b.QUESTION_ID,
                                 b.QUESTION1,
                                 b.ANSWER_TYPE,
                                 b.DEFAULT_QUESTION,
                                 b.CATEGORy.CATEGORY_NAME,
                                 b.DISPLAY_ORDER,
                             };



                var myviewmodel = new CaseViewModel();
                myviewmodel.CASE_ID = Id;
                myviewmodel.CASE_TYPE = model.CASE_TYPE;

                var QuestionsCheckboxlist = new List<questions_>();

                //assign every item in the query to the "QuestionsCheckboxlist" list
                foreach (var item in Result)
                {

                    //if the answer type to (current question id) is multiple choice, we will get the choices for this question from the multiple choice table
                    var RelatedChoices = from b in db.MULTIPLE_CHOICE
                                         where (b.QUESTION_ID == item.QUESTION_ID) && (b.VISIBILITY_STATUS == "Visible")
                                         orderby (b.DISPLAY_ORDER)
                                         select b;
                    //then, make related choice as a list
                    List<MULTIPLE_CHOICE> lst = RelatedChoices.ToList();


                    //get related answer to the (current question id)
                    var RelatedAnswers = db.CASE_QUESTIONS
                                     .Where(p => p.CASE_ID == Id)
                                     .Where(p => p.QUESTION_ID == item.QUESTION_ID)
                                     .Select(p => p.ANSWER).FirstOrDefault();

                    //get related choice id to the (current question id) //may contains a null value
                    var RelatedChoiceId = db.CASE_QUESTIONS
                                     .Where(p => p.CASE_ID == Id)
                                     .Where(p => p.QUESTION_ID == item.QUESTION_ID)
                                     .Select(p => p.CHOICE_ID).FirstOrDefault();

                    QuestionsCheckboxlist.Add(new questions_ { id = item.QUESTION_ID, Category = item.CATEGORY_NAME, name = item.QUESTION1, Answer = RelatedAnswers, fieldType = item.ANSWER_TYPE, Choices = lst, defult = item.DEFAULT_QUESTION, Choice_Id = RelatedChoiceId });
                }


                myviewmodel.QuestionsList = QuestionsCheckboxlist;
                return View(myviewmodel);

            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveCaseQuestions(CaseViewModel model)
        {
            //save case question
            Case data = db.CASES.Where(models => models.CASE_STATUS == "Pending" || models.CASE_STATUS == "Pending for Approval").FirstOrDefault(Models => Models.CASE_ID == model.CASE_ID);
            decimal Id = model.CASE_ID;

            if (data != null)
            {

                //Update cruennt stage
                data.CURRENT_STAGE = "Questions stage";
                data.USER_ID = Convert.ToDecimal(User.Identity.Name);
                data.CASE_STATUS = "Pending";
                db.SaveChanges();

                //update case questions 
                foreach (var item in db.CASE_QUESTIONS)
                {

                    if (item.CASE_ID == model.CASE_ID)

                    {
                        db.Entry(item).State = System.Data.EntityState.Modified;

                    }
                }
                //validate all questions //need to be validated
                foreach (var item in model.QuestionsList)
                {

                    var QuestionIdCheck = db.QUESTIONS.Where(x => x.QUESTION_ID == item.id).Any();

                    if (QuestionIdCheck == false || (item.name != null && item.name.Length > 255))
                    {
                        TempData["Danger"] = "please enter all required fields";
                        return RedirectToAction("getListQuestions", new { Id });
                    }
                }

                //add Questions in case questions table
                foreach (var item in model.QuestionsList)
                {
                    var check = db.CASE_QUESTIONS.Where(x => x.CASE_ID == Id).Where(x => x.QUESTION_ID == item.id).FirstOrDefault();
                    if (item.Checked && check == null)
                    {
                        db.CASE_QUESTIONS.Add(new CASE_QUESTIONS() { CASE_ID = model.CASE_ID, QUESTION_ID = item.id, QUESTION_NAME = item.name });
                        db.SaveChanges();

                    }
                    if (item.Checked == false && check != null)
                    {
                        CASE_QUESTIONS deletedQuestion = db.CASE_QUESTIONS.Where(x => x.CASE_ID == Id).Where(x => x.QUESTION_ID == item.id).FirstOrDefault();
                        db.CASE_QUESTIONS.Remove(deletedQuestion);
                        db.SaveChanges();

                    }


                }
                return RedirectToAction("getListQuestions", "Case", new { Id });


            }

            TempData["Danger"] = "No Questions Found";
            return RedirectToAction("caseQuestions", "Case", new { Id });

        }

        [HttpPost]
        public ActionResult SaveListAnswers(CaseViewModel model, string SaveAsPending)
        {
            Case data = db.CASES.Where(models => models.CASE_STATUS == "Pending" || models.CASE_STATUS == "Pending for Approval").FirstOrDefault(Models => Models.CASE_ID == model.CASE_ID);
            decimal Id = model.CASE_ID;
            var checkcasehavequestions = db.CASE_QUESTIONS.Where(c => c.CASE_ID == Id).Any();

            if (checkcasehavequestions && data != null)
            {
                //Update cruennt stage
                data.CURRENT_STAGE = "Questions stage";
                data.USER_ID = Convert.ToDecimal(User.Identity.Name);
                data.CASE_STATUS = "Pending";
                data.CASE_END_DATE = DateTime.Now;
                db.SaveChanges();

                //validate answer 
                foreach (var item in model.QuestionsList)
                {

                    var QuestionIdCheck = db.QUESTIONS.Where(x => x.QUESTION_ID == item.id).Any();

                    if (QuestionIdCheck == false || (item.Answer != null && item.Answer.Length > 4000))
                    {
                        TempData["Danger"] = "The answer cannot exceed 4000 characters.";
                        return RedirectToAction("getListQuestions", new { Id });
                    }
                }
                //delete all questions with the current case's id
                foreach (var item in db.CASE_QUESTIONS)
                {

                    if (item.CASE_ID == Id)

                    {
                        db.Entry(item).State = System.Data.EntityState.Deleted;

                    }

                }
                //add all questions in list to the case questions table
                foreach (var item in model.QuestionsList)
                {

                    db.CASE_QUESTIONS.Add(new CASE_QUESTIONS() { CASE_ID = model.CASE_ID, QUESTION_ID = item.id, QUESTION_NAME = item.name, ANSWER = item.Answer, CHOICE_ID = item.Choice_Id });
                    db.SaveChanges();


                }




                if (SaveAsPending != null) //if the "save as pending" button clicked, it will go to the "filter" action
                {

                    return RedirectToAction("Filter");
                }

                return RedirectToAction("getCasDetails", "Case", new { Id });
            }

            else
            {
                TempData["Danger"] = "No Questions Found";
                return RedirectToAction("getListQuestions", "Case", new { Id });

            }
        }


        [HttpGet]
        public ActionResult getCasDetails(decimal id)
        {
            Case model = db.CASES.Where(Models => Models.CASE_ID == id).Where(x => x.CASE_STATUS == "Pending" || x.CASE_STATUS == "Pending for Approval").FirstOrDefault();

            //categories list to bind it in the view 
            List<CATEGORy> list = db.CATEGORIES.Where(x => x.VSIBILITY_STATUS == "Visible").OrderBy(x => x.DISPLAY_ORDER).ToList();
            ViewBag.catelist = new SelectList(list, "CATEGORY_NAME", "CATEGORY_NAME");

            if (model != null)
            {
                //get case details info 
                caseDetailsStage data = new caseDetailsStage();
                data.ULTIMATE_QUESTION = model.ULTIMATE_QUESTION;
                data.ULTIMATE_CATEGORY = model.ULTIMATE_CATEGORY;
                data.CASE_ID = model.CASE_ID;
                data.CASE_TYPE = model.CASE_TYPE;
                View(data);
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdateCaseDetails(decimal CASE_ID, caseDetailsStage model, string SaveAsPending)
        {

            if (ModelState.IsValid)
            {

                Case data = db.CASES.FirstOrDefault(Models => Models.CASE_ID == CASE_ID);
                if (data != null)
                {
                    data.CASE_STATUS = "Pending";
                    data.ULTIMATE_QUESTION = model.ULTIMATE_QUESTION;
                    data.ULTIMATE_CATEGORY = model.ULTIMATE_CATEGORY;
                    data.CURRENT_STAGE = "Details stage";
                    data.USER_ID = Convert.ToDecimal(User.Identity.Name);
                    data.CASE_END_DATE = DateTime.Now;
                    db.SaveChanges();
                }
                if (SaveAsPending != null)
                {

                    return RedirectToAction("Filter");
                }

                decimal Id = model.CASE_ID;
                return RedirectToAction("getCaseAnswer", "Case", new { Id });
            }
            else
            {
                List<CATEGORy> list = db.CATEGORIES.Where(x => x.VSIBILITY_STATUS == "Visible").OrderBy(x => x.DISPLAY_ORDER).ToList();
                ViewBag.catelist = new SelectList(list, "CATEGORY_ID", "CATEGORY_NAME");  //categories list to bind it in the view 


                return View("getCasDetails", model); //if model is not valid //we return the view with all validation messages

            }


        }

        [HttpGet]
        public ActionResult getCaseAnswer(decimal Id)
        {

            Case model = db.CASES.Where(Models => Models.CASE_ID == Id).Where(x => x.CASE_STATUS == "Pending" || x.CASE_STATUS == "Pending for Approval").SingleOrDefault();

            if (model != null)
            {
                //get answer information
                answersStage data = new answersStage();
                data.CASE_ID = model.CASE_ID;
                data.ANSWER = model.ANSWER;
                data.RESEARCHER_NAME = model.RESEARCHER_NAME;
                data.CASE_TYPE = model.CASE_TYPE;



                //get references of the case
                var Result = from b in db.REFERENCES
                             where (b.VISIBILIT_STATUS == "Visible")
                             orderby (b.DISPLAY_ORDER)
                             select new
                             {
                                 b.REFERENCE_ID,
                                 b.REFERENCE_NAME,
                                 Checked = ((from ab in db.CASE_REFERENCES
                                             where (ab.CASE_ID == Id) & (ab.REFERENCE_ID == b.REFERENCE_ID)
                                             select ab).Count() > 0)//subquery to check if the refrence is assigned to the current case's id or not 

                             };


                //make list of refrences
                var refrensesCheckboxlist = new List<references_>();

                foreach (var item in Result)
                {

                    string result2 =
                         (from c in db.CASE_REFERENCES
                          where c.CASE_ID == Id && c.OTHER_REFRENSES != null
                          select c.OTHER_REFRENSES).FirstOrDefault();//query to get other refrences if exist

                    //add every item in result query to the (refrensesCheckboxlist)
                    refrensesCheckboxlist.Add(new references_ { id = item.REFERENCE_ID, name = item.REFERENCE_NAME, Checked = item.Checked, other = result2 });


                }

                data.RefrensesList = refrensesCheckboxlist;

                return View(data);
            }

            else
            {
                return View();

            }



        }


        [HttpPost]
        public ActionResult updateCaseAnswer(decimal CASE_ID, answersStage model, string SaveAsPending)
        {
            Case data = db.CASES.Where(x => x.CASE_STATUS == "Pending" || x.CASE_STATUS == "Pending for Approval").FirstOrDefault(Models => Models.CASE_ID == CASE_ID);

            if (ModelState.IsValid)
            {
                if (data != null)
                {

                    data.ANSWER = model.ANSWER;
                    data.RESEARCHER_NAME = model.RESEARCHER_NAME;
                    data.CASE_STATUS = "Pending";
                    data.USER_ID = Convert.ToDecimal(User.Identity.Name);
                    data.CURRENT_STAGE = "Answer stage";
                    db.SaveChanges();//update case inforamtion



                    foreach (var item in db.CASE_REFERENCES)//delete all related references

                    {

                        if (item.CASE_ID == model.CASE_ID)
                        {
                            db.Entry(item).State = System.Data.EntityState.Deleted;

                        }

                    }

                    foreach (var item in model.RefrensesList)//validate references
                    {
                        var RefrensesIdCheck = db.REFERENCES.Where(x => x.REFERENCE_ID == item.id).Any();

                        if (RefrensesIdCheck == false || (item.name != null && item.name.Length > 200) || (item.other != null && item.other.Length > 150))
                        {
                            decimal id = model.CASE_ID;

                            return RedirectToAction("getCaseAnswer", new { id });
                        }

                    }
                    //add the case references
                    foreach (var item in model.RefrensesList)
                    {

                        if (item.Checked)
                        {
                            db.CASE_REFERENCES.Add(new CASE_REFERENCES() { CASE_ID = model.CASE_ID, REFERENCE_ID = item.id, OTHER_REFRENSES = item.other, REFERENCE_NAME = item.name });
                            db.SaveChanges();

                        }

                    }
                    var myviewmodel = new answersStage();
                    myviewmodel.CASE_ID = model.CASE_ID;
                    decimal Id = model.CASE_ID;

                    if (SaveAsPending != null)
                    {

                        return RedirectToAction("Filter"); //if the case saved as Pending

                    }

                    return RedirectToAction("getCaseRecored", "Case", new { Id }); //if the case saved and need to continue 



                }
                return RedirectToAction("Index", "Error");

            }
            else
            {
                return View("getCaseAnswer", model);//if model is not valid //we return the view with all validation messages


            }




        }

        [HttpGet]
        public ActionResult getCaseRecored(decimal id)
        {
            //get recored Information
            Case model = db.CASES.Where(Models => Models.CASE_ID == id).Where(x => x.CASE_STATUS == "Pending" || x.CASE_STATUS == "Pending for Approval").SingleOrDefault();

            if (model != null)
            {

                recordStage data = new recordStage();
                data.CASE_ID = model.CASE_ID;
                data.ANSWER_GIVEN = model.ANSWER_GIVEN;
                data.NOT_REACHABLE_COMMENT = model.NOT_REACHABLE_COMMENT;
                data.RECEIVER_ID = model.RECEIVER_ID;
                data.RECEIVER_NAME = model.RECEIVER_NAME;
                data.CONTACT_ATTEMPT = model.CONTACT_ATTEMPT;
                data.IS_DIFFER = model.IS_DIFFER;
                data.CASE_TYPE = model.CASE_TYPE;
                return View(data);
            }
            else
            {
                return View();
            }


        }


        [HttpPost]
        public ActionResult updateCaseRecored(recordStage model)
        {
            if (ModelState.IsValid == true)
            {

                Case data = db.CASES.FirstOrDefault(Models => Models.CASE_ID == model.CASE_ID);
                if (data != null)
                {
                    DateTime startTime = data.CASE_START_DATE; //get the start date to find the time span
                    DateTime EndTime = DateTime.Now;
                    TimeSpan span = EndTime.Subtract(startTime);
                    int minutes = span.Minutes;//min span
                    int days = span.Days;//day span
                    int countHours = CountHours(startTime, EndTime);//find the span of (hour)
                    string status = CheckCaseStatus(model.CASE_ID, Convert.ToDecimal(User.Identity.Name));//get case status
                    string stage = CheckCurrentStage(status); // get current stage

                    //update the case inforamtion
                    data.TIME_SPAN_MINUTES = minutes;
                    data.TIME_SPAN_HOUR = countHours;
                    data.TIME_SPAN_DAY = days;
                    data.ANSWER_GIVEN = model.ANSWER_GIVEN;
                    data.RECEIVER_ID = model.RECEIVER_ID;
                    data.RECEIVER_NAME = model.RECEIVER_NAME;
                    data.NOT_REACHABLE_COMMENT = model.NOT_REACHABLE_COMMENT;
                    data.CONTACT_ATTEMPT = model.CONTACT_ATTEMPT;
                    data.IS_DIFFER = model.IS_DIFFER;
                    data.USER_ID = Convert.ToDecimal(User.Identity.Name);
                    data.CASE_END_DATE = EndTime;
                    data.CURRENT_STAGE = stage;
                    data.CASE_STATUS = status;
                    db.SaveChanges();

                    //check case status
                    switch (status)
                    {
                        case "Completed":

                            TempData["Success"] = "The case completed successfully";
                            return RedirectToAction("Filter");

                        case "Pending":
                            TempData["warning"] = "This case is not completed, please review the case";
                            return RedirectToAction("Filter");

                        case "Pending for Approval":
                            TempData["alert"] = "This case require approval by the supervisor";
                            return RedirectToAction("Filter");

                    }

                    return RedirectToAction("Filter");
                }
                return RedirectToAction("Index", "Error");

            }

            else
            {
                return View("getCaseRecored", model);//if model's data are not valid  //we return the view with all validation messages


            }

        }
        public ActionResult Filter(string Start_date, string Filter_Value7, string Status,
                             string Requestrt_id, string Category_id, string Filter_Value2,
                             string Filter_Value3, string Filter_Value5, string User_id, string Filter_Value4,
                             string End_date, string Filter_Value6, int? Page_No, string ExportExcel)
        {
            const int pagesize = 7;
            int pagenum = (Page_No ?? 1);

            if (ExportExcel != null)
            {

                return RedirectToAction("ExportToExcel", new { User_id, Requestrt_id, Start_date, End_date, Category_id, Status });//export completed case table as excel file 

            }

            if (Requestrt_id != null || Start_date != null || Category_id != null || User_id != null || End_date != null || Status != null)
            {
                Page_No = 1;
            }
            else
            {  //to filter the list by name across all pages
                Requestrt_id = Filter_Value2;
                Start_date = Filter_Value3;
                Category_id = Filter_Value4;
                User_id = Filter_Value5;
                End_date = Filter_Value6;
                Status = Filter_Value7;


            }
            ViewBag.FilterValue2 = Requestrt_id;
            ViewBag.FilterValue3 = Start_date;
            ViewBag.FilterValue4 = Category_id;
            ViewBag.FilterValue5 = User_id;
            ViewBag.FilterValue6 = End_date;
            ViewBag.FilterValue7 = Status;



            List<CATEGORy> list = db.CATEGORIES.ToList();
            ViewBag.CategoryList = new SelectList(list, "CATEGORY_ID", "CATEGORY_NAME");//category list to bind it in the view 


            List<USER> listUser = db.USERS.ToList();
            ViewBag.userList = new SelectList(listUser, "USER_ID", "FULL_NAME");//user list to bind it in the view 


            List<REQUESTER> ListRequester = db.REQUESTERS.ToList();
            ViewBag.ListRequester = new SelectList(ListRequester, "REQUESTER_ID", "REQUESTER_NAME"); //requester list to bind it in the view 


            var Case = (from c in db.CASES
                        select c);

            if (!String.IsNullOrEmpty(Category_id) && decimal.TryParse(Category_id, out _)) //check if category id is entred to filter (etc..)
            {
                var categoryId = Convert.ToDecimal(Category_id);

                var Case2 = (from c in db.CASES
                             join ca in db.CASE_CATEGORIES on c.CASE_ID equals ca.CASE_ID
                             where ca.CATEGORY_ID == categoryId
                             select c);

                if (!String.IsNullOrEmpty(User_id) && decimal.TryParse(User_id, out _))
                {
                    var User = Convert.ToDecimal(User_id);
                    Case2 = Case2.Where(stu => stu.USER_ID == User);
                }

                if (!String.IsNullOrEmpty(Requestrt_id) && decimal.TryParse(Requestrt_id, out _))
                {
                    var requesterID = Convert.ToDecimal(Requestrt_id);
                    Case2 = Case2.Where(stu => stu.REQUESTER_ID == requesterID);
                }

                if (!String.IsNullOrEmpty(Status))
                {
                    Case2 = Case2.Where(stu => stu.CASE_STATUS == Status);
                }
                if (String.IsNullOrEmpty(Status))
                {
                    Case2 = Case2.Where(stu => stu.CASE_STATUS == "Pending");
                }

                if (!String.IsNullOrEmpty(Start_date))
                {
                    DateTime dateTime = DateTime.Parse(Start_date);
                    Case2 = Case2.Where(stu => stu.CASE_START_DATE >= dateTime);
                }
                if (!String.IsNullOrEmpty(End_date))
                {
                    DateTime dateTime = DateTime.Parse(End_date);
                    Case2 = Case2.Where(stu => stu.CASE_START_DATE <= dateTime);

                }

                switch (Status)//sort according to status
                {
                    case "Pending":
                        Case2 = Case2.OrderByDescending(s => s.CASE_START_DATE);
                        break;
                    case "Pending for Approval":
                        Case2 = Case2.OrderByDescending(s => s.CASE_START_DATE);
                        break;
                    case "Completed":
                        Case2 = Case2.OrderByDescending(s => s.CASE_END_DATE);
                        break;
                    default:
                        Case2 = Case2.OrderByDescending(s => s.CASE_START_DATE);
                        break;
                }
                return View(Case2.ToPagedList(pagenum, pagesize));


            }

            if (!String.IsNullOrEmpty(User_id) && decimal.TryParse(User_id, out _))
            {
                var User = Convert.ToDecimal(User_id);
                Case = Case.Where(stu => stu.USER_ID == User);
            }

            if (!String.IsNullOrEmpty(Requestrt_id) && decimal.TryParse(Requestrt_id, out _))
            {
                var requesterID = Convert.ToDecimal(Requestrt_id);
                Case = Case.Where(stu => stu.REQUESTER_ID == requesterID);
            }

            if (!String.IsNullOrEmpty(Status))
            {
                Case = Case.Where(stu => stu.CASE_STATUS == Status);
            }
            if (String.IsNullOrEmpty(Status))
            {
                Case = Case.Where(stu => stu.CASE_STATUS == "Pending");
            }

            if (!String.IsNullOrEmpty(Start_date))
            {
                DateTime dateTime = DateTime.Parse(Start_date);
                Case = Case.Where(stu => stu.CASE_START_DATE >= dateTime);
            }
            if (!String.IsNullOrEmpty(End_date))
            {
                DateTime dateTime = DateTime.Parse(End_date);
                Case = Case.Where(stu => stu.CASE_START_DATE <= dateTime);

            }

            switch (Status)//sort according to status
            {
                case "Pending":
                    Case = Case.OrderByDescending(s => s.CASE_START_DATE);
                    break;
                case "Pending for Approval":
                    Case = Case.OrderByDescending(s => s.CASE_START_DATE);
                    break;
                case "Completed":
                    Case = Case.OrderByDescending(s => s.CASE_END_DATE);
                    break;
                default:
                    Case = Case.OrderByDescending(s => s.CASE_START_DATE);
                    break;
            }



            return View(Case.ToPagedList(pagenum, pagesize));

        }

        public ActionResult Edit(decimal id)
        {
            //to check the stage of the current case's Id 
            var checkStage = db.CASES.Where(c => c.CASE_ID == id).Select(c => c.CURRENT_STAGE).FirstOrDefault();


            switch (checkStage.ToString())
            {
                case "Description stage":
                    return RedirectToAction("getCaseDescription", new { id });
                case "Patient stage":
                    return RedirectToAction("getPatientInformation", new { id });
                case "Categories stage":
                    return RedirectToAction("getListCategories", new { id });
                case "Questions stage":
                    return RedirectToAction("getListQuestions", new { id });
                case "Details stage":
                    return RedirectToAction("getCasDetails", new { id });
                case "Answer stage":
                    return RedirectToAction("getCaseAnswer", new { id });
                case "Record stage":
                    return RedirectToAction("getCaseRecored", new { id });
                default:
                    return RedirectToAction("getCaseDescription", new { id });

            }



        }

        public FileResult ExportToExcel(string User_id, string Requestrt_id, string Start_date, string End_date, string Category_id, string Status)
        {
            var result = (from c in db.CASES
                          select c);

            if (!String.IsNullOrEmpty(Category_id) && decimal.TryParse(Category_id, out _))
            {
                var categoryId = Convert.ToDecimal(Category_id);

                var Result2 = (from c in db.CASES
                               join ca in db.CASE_CATEGORIES on c.CASE_ID equals ca.CASE_ID
                               where ca.CATEGORY_ID == categoryId
                               select c);


                if (!String.IsNullOrEmpty(User_id) && decimal.TryParse(User_id, out _))
                {
                    var User = Convert.ToDecimal(User_id);
                    Result2 = Result2.Where(stu => stu.USER_ID == User);
                }
                if (!String.IsNullOrEmpty(Status) && Status.Length <= 20)
                {
                    Result2 = Result2.Where(stu => stu.CASE_STATUS == Status);
                }

                if (!String.IsNullOrEmpty(Requestrt_id) && decimal.TryParse(Requestrt_id, out _))
                {
                    var requesterID = Convert.ToDecimal(Requestrt_id);
                    Result2 = Result2.Where(stu => stu.REQUESTER_ID == requesterID);
                }
                DateTime parsedDateTime__;

                if (!String.IsNullOrEmpty(Start_date) && DateTime.TryParse(Start_date, out parsedDateTime__))
                {
                    DateTime dateTime = DateTime.Parse(Start_date);
                    Result2 = Result2.Where(stu => stu.CASE_START_DATE >= dateTime);
                }
                DateTime parsedDateTime2__;

                if (!String.IsNullOrEmpty(End_date) && DateTime.TryParse(End_date, out parsedDateTime2__))
                {

                    DateTime dateTime = DateTime.Parse(End_date);
                    Result2 = Result2.Where(stu => stu.CASE_START_DATE <= dateTime);
                }

                DataTable dt1 = new DataTable("CaseData");
                if (Status == "Pending" || Status == "Pending for Approval" || Status == "")
                {

                    dt1.Columns.AddRange(new DataColumn[3] {
                                            new DataColumn("Case Start Date"),
                                            new DataColumn("Status"),
                                            new DataColumn("Full Name"),
                                            });
                    foreach (var item in Result2)
                    {
                        dt1.Rows.Add(item.CASE_START_DATE, item.CASE_STATUS, item.USER.FULL_NAME);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt1);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DPIC Cases.xlsx");
                        }
                    }
                }

                dt1.Columns.AddRange(new DataColumn[4] { new DataColumn("Case Start Date"),
                                                        new DataColumn("Case End Date"),
                                                        new DataColumn("Status"),
                                                        new DataColumn("Full Name"),
                });
                foreach (var item in Result2)
                {
                    dt1.Rows.Add(item.CASE_START_DATE, item.CASE_END_DATE, item.CASE_STATUS, item.USER.FULL_NAME);
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt1);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DPIC Cases.xlsx");
                    }
                }


            }

            else
            {
                if (!String.IsNullOrEmpty(User_id) && decimal.TryParse(User_id, out _))
                {
                    var User = Convert.ToDecimal(User_id);
                    result = result.Where(stu => stu.USER_ID == User);
                }
                if (!String.IsNullOrEmpty(Status) && Status.Length <= 20)
                {
                    result = result.Where(stu => stu.CASE_STATUS == Status);
                }

                if (!String.IsNullOrEmpty(Requestrt_id) && decimal.TryParse(Requestrt_id, out _))
                {
                    var requesterID = Convert.ToDecimal(Requestrt_id);
                    result = result.Where(stu => stu.REQUESTER_ID == requesterID);
                }
                DateTime parsedDateTime;

                if (!String.IsNullOrEmpty(Start_date) && DateTime.TryParse(Start_date, out parsedDateTime))
                {
                    DateTime dateTime = DateTime.Parse(Start_date);
                    result = result.Where(stu => stu.CASE_START_DATE >= dateTime);
                }
                DateTime parsedDateTime2;

                if (!String.IsNullOrEmpty(End_date) && DateTime.TryParse(End_date, out parsedDateTime2))
                {

                    DateTime dateTime = DateTime.Parse(End_date);
                    result = result.Where(stu => stu.CASE_START_DATE <= dateTime);
                }

                DataTable dt = new DataTable("CaseData");
                if (Status == "Pending" || Status == "Pending for Approval" || Status == "")
                {

                    dt.Columns.AddRange(new DataColumn[3] {
                                            new DataColumn("Case Start Date"),
                                            new DataColumn("Status"),
                                            new DataColumn("Full Name"),
                                            });
                    foreach (var item in result)
                    {
                        dt.Rows.Add(item.CASE_START_DATE, item.CASE_STATUS, item.USER.FULL_NAME);
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DPIC Cases.xlsx");
                        }
                    }
                }


                dt.Columns.AddRange(new DataColumn[4] { new DataColumn("Case Start Date"),
                                                        new DataColumn("Case End Date"),
                                                        new DataColumn("Status"),
                                                        new DataColumn("Full Name"),
                });
                foreach (var item in result)
                {
                    dt.Rows.Add(item.CASE_START_DATE, item.CASE_END_DATE, item.CASE_STATUS, item.USER.FULL_NAME);
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DPIC Cases.xlsx");
                    }
                }

            }


        }


        private int CountHours(DateTime start, DateTime end)
        {

            int count = 0;
            TimeSpan span = start.Subtract(end);
            int hours = span.Hours;

            if (hours == 0) //if the hours is 0, it dosent count the hours

            {

                return count;


            }
            //count the hours (within work hours) and (without weekend days)
            for (var i = start; i < end; i = i.AddHours(1))
            {
                if (i.DayOfWeek != DayOfWeek.Friday && i.DayOfWeek != DayOfWeek.Saturday)
                {
                    if (i.TimeOfDay.Hours >= 8 && i.TimeOfDay.Hours <= 17)
                    {
                        count++;
                    }
                }
            }


            return count;

        }

        private string CheckCaseStatus(decimal caseID, decimal userId)
        {

            var CaseData =
                           (from b in db.CASES
                            where (b.CASE_ID == caseID) & (b.CASE_START_DATE != null) &
                            (b.CASE_DESCRIBTION != null) & (b.CASE_TYPE != null) & (b.ULTIMATE_QUESTION != null) &
                            (b.ULTIMATE_CATEGORY != null) & (b.URGENCY != null) & (b.ANSWER != null) & (b.USER_ID != null)
                            select new
                            { b }).Any();

            var UserIsAllowed =
                            (from u in db.USERS
                             from rr in db.ROLES
                             join r in db.USER_ROLE on rr.ROLE_ID equals r.ROLE_ID
                             join rp in db.ROLE_PERMISSIONS on rr.ROLE_ID equals rp.ROLE_ID
                             join p in db.PERMISSIONS on rp.PERMISSION_ID equals p.PERMISSION_ID
                             where u.USER_ID == userId && p.PERMISSION_DESCRIPTION == "Allow to submit the case" && r.USER_ID == userId
                             select new
                             { p.PERMISSION_DESCRIPTION }).Any();

            var checkCaseType = db.CASES.Where(x => x.CASE_ID == caseID).Select(x => x.CASE_TYPE).FirstOrDefault();
            var CaseHaveCategories = db.CASE_CATEGORIES.Where(x => x.CASE_ID == caseID).Any();
            var CaseHaveQuestions = db.CASE_QUESTIONS.Where(x => x.CASE_ID == caseID).Any();
            var CaseHasRefrences = db.CASE_REFERENCES.Where(x => x.CASE_ID == caseID).Any();
            var CaseHasComorbitdies = db.CASE_COMORBIDITY.Where(x => x.CASE_ID == caseID).Any();

            //switch
            string status;

            if (CaseData && CaseHaveCategories && CaseHaveQuestions && CaseHasRefrences && UserIsAllowed && checkCaseType == "Patient Specific Question" && CaseHasComorbitdies)
            {
                status = "Completed";
            }
            else if (CaseData && CaseHaveCategories && CaseHaveQuestions && CaseHasRefrences && UserIsAllowed && checkCaseType == "General Questions")
            {
                status = "Completed";

            }
            else if (CaseData && CaseHaveCategories && CaseHaveQuestions && CaseHasRefrences && UserIsAllowed == false)
            {

                status = "Pending for Approval";

            }

            else
            {
                status = "Pending";

            }

            return status;

        }


        private static string CheckCurrentStage(string status)
        {
            //check case status
            switch (status)
            {
                case "Completed":
                    return "Record stage";

                case "Pending":
                    return "Description stage";

                case "Pending for Approve":
                    return "Description stage";

                default:
                    return "Description stage";

            }

        }







    }
}