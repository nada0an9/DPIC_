using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication27.Models;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]

    public class CompletedCasesController : Controller
    {    private DB_Model db = new DB_Model();

        // GET: CompletedCases
     
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CaseDescriptionSatge(decimal CASE_ID) 
        {
            //requesters list to bind it in the view 
            List<REQUESTER> list = db.REQUESTERS.ToList();
            ViewBag.Requester = new SelectList(list, "REQUESTER_ID", "REQUESTER_NAME");

                Case model = db.CASES.Where(Models => Models.CASE_ID == CASE_ID).Where(x => x.CASE_STATUS == "Completed").SingleOrDefault();

                if (model != null)
                {    //get case description stage info from (model)

                    caseDescriptionStage data = new caseDescriptionStage();
                    data.CASE_ID = model.CASE_ID;
                    data.CASE_START_DATE = model.CASE_START_DATE;
                    data.REQUESTER_ID = model.REQUESTER_ID;
                    data.CASE_DESCRIBTION = model.CASE_DESCRIBTION;
                    data.CASE_TYPE = model.CASE_TYPE;
                    data.URGENCY = model.URGENCY;

                    return View(data);

                }

                return View();


           



        }

        [HttpGet]
        public ActionResult getPatientInformation(decimal CASE_ID)
        {


            Case model = db.CASES.Where(Models => Models.CASE_ID == CASE_ID).SingleOrDefault();

            //get patient information from (model)
            if (model != null)
            {
            patientInformationStage data = new patientInformationStage();
            data.CASE_ID = model.CASE_ID;
            data.MRN = model.MRN;
            data.NAME = model.NAME;
            data.AGE = model.AGE;
            data.GENDER = model.GENDER;
            data.PREGNANT_OR_NOT = model.PREGNANT_OR_NOT;
            data.PREGNANT_WEEK = model.PREGNANT_WEEK;
            data.ALLERGY_HISTORY = model.ALLERGY_HISTORY;
            data.HEIGHT = model.HEIGHT;
            data.WEIGHT = model.WEIGHT;
            data.patientMRN = model.HAS_MRN_OR_NOT;
            data.CASE_START_DATE = model.CASE_START_DATE;
            data.DIAGNOSIS = model.DIAGNOSIS;
            data.ELECTROLYTES = model.ELECTROLYTES;
            data.LIVER_FUNCTION = model.LIVER_FUNCTION;
            data.LABORATORY_RESULT = model.LABORATORY_RESULT;
            data.SERUM_CREATININE = model.SERUM_CREATININE;
            data.MEDICATIONS = model.MEDICATIONS;
            data.RELATED_SINGS = model.RELATED_SINGS;



                //get comorbidity
                var Result = from b in db.CASE_COMORBIDITY
                             join c in db.COMORBIDITies on b.COMORBIDITY_ID equals c.COMORBIDITY_ID
                             where (b.CASE_ID == CASE_ID)
                             orderby(c.DISPLAY_ORDER)
                             select new
                             {
                                 b.COMORBIDITY_ID,
                                 b.COMORBIDITY_NAME,
                                 b.OTHER_COMORBIDITY

                             };


             //make list of comorbidities
             var ComorbidityCheckboxlist = new List<CheckBoxModel>();

            foreach (var item in Result)
            {
                    string result2 =
                                      (from c in db.CASE_COMORBIDITY
                                       where c.CASE_ID == CASE_ID && c.OTHER_COMORBIDITY != null
                                       select c.OTHER_COMORBIDITY).FirstOrDefault();//query to get other comorbidity if exists

                    //add every item in result query to the (ComorbidityCheckboxlist)
                    ComorbidityCheckboxlist.Add(new CheckBoxModel { id = item.COMORBIDITY_ID, name = item.COMORBIDITY_NAME, Checked = true, other =result2 });


            }

            data.ComorbidityList = ComorbidityCheckboxlist;
            return View(data);

            }
            return View();

        }
      
        [HttpGet]
        public ActionResult getListCategories(decimal CASE_ID)
        {
            Case model = db.CASES.Where(Models => Models.CASE_ID == CASE_ID).Where(x => x.CASE_STATUS == "Completed").SingleOrDefault();

            if (model != null)
            {
                //query to get case categories
                var Result =
                             from s in db.CASE_CATEGORIES
                             join c in db.CATEGORIES on s.CATEGORY_ID equals c.CATEGORY_ID
                             where (s.CASE_ID == CASE_ID)
                             orderby (c.DISPLAY_ORDER)
                             select new
                             {
                                 s.CATEGORY_ID,
                                 s.CATEGORY_NAME,

                             };

                var myviewmodel = new CaseViewModel();
                myviewmodel.CASE_ID = CASE_ID;
                myviewmodel.CASE_TYPE = model.CASE_TYPE;

                var categoryCheckboxlist = new List<categoryCheckboxViewModel>();
                //get categories list
                foreach (var item in Result)
                {
                    //add every item in result query to the (categoryCheckboxlist)
                    categoryCheckboxlist.Add(new categoryCheckboxViewModel { id = item.CATEGORY_ID, name = item.CATEGORY_NAME, Checked = true });


                }

                myviewmodel.CategoryList = categoryCheckboxlist;
                return View(myviewmodel);

            }
            return View();

        }

        [HttpGet]
        public ActionResult getListQuestions(decimal CASE_ID)
        {
            Case model = db.CASES.Where(Models => Models.CASE_ID == CASE_ID).Where(x => x.CASE_STATUS == "Completed").SingleOrDefault();

            if (model != null)
            {
                //query to get case questions
                var Result = from b in db.QUESTIONS
                             join c in db.CASE_QUESTIONS on b.QUESTION_ID equals c.QUESTION_ID
                             join f in db.CASE_CATEGORIES on b.CATEGORY_ID equals f.CATEGORY_ID
                             where (c.CASE_ID == CASE_ID) & (c.QUESTION_ID == b.QUESTION_ID) & (f.CASE_ID == CASE_ID)
                             orderby (b.DISPLAY_ORDER)
                             select new
                             {
                                 c.QUESTION_ID,
                                 c.QUESTION_NAME,
                                 b.ANSWER_TYPE,
                                 b.DEFAULT_QUESTION,
                                 b.CATEGORy.CATEGORY_NAME,
                                 c.ANSWER,
                                 c.CHOICE_ID

                             };

                var myviewmodel = new CaseViewModel();
                myviewmodel.CASE_ID = CASE_ID;

                //make list of questions
                var QuestionsCheckboxlist = new List<QuestionsCheckboxViewModel>();

                foreach (var item in Result)
                {

                    var Result2 = from b in db.MULTIPLE_CHOICE
                                  where (b.QUESTION_ID == item.QUESTION_ID)
                                  orderby (b.DISPLAY_ORDER)
                                  select b;
                    List<MULTIPLE_CHOICE> lst = Result2.ToList(); //get a list of related multiple-choice to the current question's id in (item)

                    //add every item in result query to the (QuestionsCheckboxlist)
                    QuestionsCheckboxlist.Add(new QuestionsCheckboxViewModel { id = item.QUESTION_ID, name = item.QUESTION_NAME, Answer = item.ANSWER, fieldType = item.ANSWER_TYPE, Choices = lst, defult = item.DEFAULT_QUESTION, Category = item.CATEGORY_NAME, Choice_Id =item.CHOICE_ID });
                }

                myviewmodel.QuestionsList = QuestionsCheckboxlist;
                return View(myviewmodel);

            }
            return View();

            




        }

        [HttpGet]
        public ActionResult getCasDetails(decimal CASE_ID)
        {

            Case model = db.CASES.Where(Models => Models.CASE_ID == CASE_ID).Where(x => x.CASE_STATUS == "Completed").SingleOrDefault();

            if (model != null)
            {
                // get case details stage info from (model)
                caseDetailsStage data = new caseDetailsStage();
                data.CASE_ID = model.CASE_ID;
                data.ULTIMATE_QUESTION = model.ULTIMATE_QUESTION;
                data.ULTIMATE_CATEGORY = model.ULTIMATE_CATEGORY;
                return View(data);
            }
            return View();
            

        }
   
        [HttpGet]
        public ActionResult getCaseAnswer(decimal CASE_ID)
        {

            Case model = db.CASES.Where(Models => Models.CASE_ID == CASE_ID).Where(x => x.CASE_STATUS == "Completed").SingleOrDefault();

            if (model != null)
            {
                // get answer stage info from (model)
                answersStage data = new answersStage();
                data.CASE_ID = model.CASE_ID;
                data.ANSWER = model.ANSWER;
                data.RESEARCHER_NAME = model.RESEARCHER_NAME;


                //get case reference
                var Result = from b in db.CASE_REFERENCES
                             where b.CASE_ID == CASE_ID
                             join c in db.REFERENCES on b.REFERENCE_ID equals c.REFERENCE_ID
                             where (b.CASE_ID == CASE_ID)
                             orderby (c.DISPLAY_ORDER)
                             select new
                             {
                                 b.REFERENCE_ID,
                                 b.REFERENCE_NAME,

                             };


                //make list of references
                var refrensesCheckboxlist = new List<CheckBoxModel>();

                foreach (var item in Result)
                {

                    string result2 =
                         (from c in db.CASE_REFERENCES
                          where c.CASE_ID == CASE_ID && c.OTHER_REFRENSES != null
                          select c.OTHER_REFRENSES).FirstOrDefault();

                    //add every item in result query to the (refrensesCheckboxlist)
                    refrensesCheckboxlist.Add(new CheckBoxModel { id = item.REFERENCE_ID, name = item.REFERENCE_NAME, Checked = true, other = result2 });


                }

                data.RefrensesList = refrensesCheckboxlist;

                return View(data);

            }
            return View();

        }

        [HttpGet]
        public ActionResult getCaseRecored(decimal CASE_ID)
        {


            Case model = db.CASES.Where(Models => Models.CASE_ID == CASE_ID).SingleOrDefault();

            if(model != null)
            {
             // get record stage info from (model)
             recordStage data = new recordStage();
            data.CASE_ID = model.CASE_ID;
            data.ANSWER_GIVEN = model.ANSWER_GIVEN;
            data.NOT_REACHABLE_COMMENT = model.NOT_REACHABLE_COMMENT;
            data.RECEIVER_ID = model.RECEIVER_ID;
            data.RECEIVER_NAME = model.RECEIVER_NAME;
            data.CONTACT_ATTEMPT = model.CONTACT_ATTEMPT;
            data.IS_DIFFER = model.IS_DIFFER;

            return View(data);
            }


            return View();


        }



    }
}