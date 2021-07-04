using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication27.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Mvc;
using System.IO;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize("Generate Reports")]


    public class ReportController : Controller
    {
        private DB_Model db = new DB_Model();

        // GET: Report
   
        [HttpGet]
        public ActionResult Index()
        {
            List<USER> listUser = db.USERS.ToList();
            ViewBag.userList = new SelectList(listUser, "USER_ID", "FULL_NAME");


            return View();
        }
 
        public ActionResult Validate(ReportModel model)
        {
            if (ModelState.IsValid)
            {

                bool AllUsers = model.allUserOrOne;
                var GetCaseDataCheck1 = db.CASES.Where(e => e.CASE_START_DATE >= model.Start_date && e.CASE_START_DATE <= model.End_date && e.CASE_STATUS == "Completed").Any();
                var GetCaseDataCheck = db.CASES.Where(e => e.CASE_START_DATE >= model.Start_date && e.CASE_START_DATE <= model.End_date && e.CASE_STATUS == "Completed" && e.USER_ID == model.USER_ID).Any();

                DateTime start = model.Start_date;
                DateTime end = model.End_date;
                string user_id = model.USER_ID.ToString();

                if (AllUsers == true && GetCaseDataCheck1)
                {
             
                    return RedirectToAction("ReportToAll", new { start, end, user_id, AllUsers });
                }
                if (AllUsers == false && GetCaseDataCheck)
                {
                    return RedirectToAction("ReportToAll", new { start, end, user_id, AllUsers });

                }
               
                    TempData["No_data"] = "No data found";
                    return RedirectToAction("Index");
                
            }
            else
            {

                return View("Index", model);//validation in view 
            }

        }

        public ActionResult ReportToAll(DateTime start, DateTime end, string user_id, bool AllUsers)
        {


            using (MemoryStream stream = new System.IO.MemoryStream())
            {  
                Document PdfFile = new Document(PageSize.A4, 20f, 5f, 90f, 40f);//4-> bottom 3->top 2->right 1->left
                PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
                writer.PageEvent = new PDFBackgroundHelper();
                PdfFile.Open();


                //Query Section
                var GetCaseData = from e in db.CASES
                                  where (e.CASE_START_DATE >= start && e.CASE_START_DATE <= end && e.CASE_STATUS == "Completed")
                                  select new { e };

                if (AllUsers == false)
                {
                    var User = Convert.ToDecimal(user_id);

                    GetCaseData = GetCaseData.Where(x => x.e.USER_ID == User);
                }
     



                int RowNo = 1; //value to be incremented For each case


                foreach (var item in GetCaseData)
                {
                    //PDF Hidden Table Settings
                    PdfPTable table = new PdfPTable(2);
                    table.DefaultCell.Border = 0;
                    table.DefaultCell.PaddingBottom = 10;
                    table.DefaultCell.PaddingTop = 5;
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = Element.ALIGN_LEFT;



                    //Font Settings -> It can be used in any cell
                    var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 11);
                    var Font = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11);

                    if (RowNo != 0)
                    {
                        table.AddCell(new Phrase("Case Description Stage", boldFont));
                        table.AddCell(new Phrase("", boldFont));
                        //get first stage information
                        table.AddCell(new Phrase("Case Number : " + RowNo++.ToString(), boldFont));
                        table.AddCell("");
                        table.AddCell(new Phrase("The Following Case was Submitted by", boldFont));
                        table.AddCell(new Phrase(item.e.USER.FULL_NAME, Font));
                        table.AddCell(new Phrase("Case Start Date:", boldFont));
                        table.AddCell(new Phrase(item.e.CASE_START_DATE.ToString(), Font));

                        table.AddCell(new Phrase("Case End Date:", boldFont));
                        table.AddCell(new Phrase(item.e.CASE_END_DATE.ToString(), Font));
                        table.AddCell(new Phrase("Requester ID", boldFont));
                        table.AddCell(new Phrase(item.e.REQUESTER_ID.ToString(), Font));
                        table.AddCell(new Phrase("Case Type :", boldFont));
                        table.AddCell(new Phrase(item.e.CASE_TYPE, Font));
                        table.AddCell(new Phrase("Case Description", boldFont));
                        table.AddCell(new Phrase("", boldFont));

                        PdfPCell CaseDescrption = new PdfPCell(new Phrase(item.e.CASE_DESCRIBTION, Font));
                        CaseDescrption.Colspan = 2;
                        CaseDescrption.Border = 0;
                        CaseDescrption.FixedHeight = 50f;


                        CaseDescrption.PaddingBottom = 10;
                        CaseDescrption.PaddingTop = 5;
                        //Merege two cells in on cell 
                        table.AddCell(CaseDescrption);




                        PdfPCell CellLine = new PdfPCell();
                        CellLine.BorderColorBottom = BaseColor.GRAY;
                        CellLine.BorderColorLeft = BaseColor.WHITE;
                        CellLine.BorderColorRight = BaseColor.WHITE;
                        CellLine.BorderColorTop = BaseColor.WHITE;
                        CellLine.PaddingTop = 15;
                        CellLine.PaddingBottom = 15;



                        table.AddCell(CellLine);
                        table.AddCell(CellLine);
                        PdfFile.NewPage();
                        //get patient information stage if the case type was specific
                        if (item.e.CASE_TYPE == "Patient Specific Question")
                        {

                            table.AddCell(new Phrase("Patient Information Stage", boldFont));
                            table.AddCell(new Phrase("", boldFont));
                            if (item.e.HAS_MRN_OR_NOT == "Yes")
                            {
                                table.AddCell(new Phrase("MRN", boldFont));
                                table.AddCell(new Phrase(item.e.MRN, Font));

                            }

                            table.AddCell(new Phrase("Name", boldFont));
                            table.AddCell(new Phrase(item.e.NAME, Font));
                            if (item.e.HAS_MRN_OR_NOT == "Yes")
                            {
                                table.AddCell(new Phrase(item.e.MRN, Font));
                            }

                            table.AddCell(new Phrase("Age", boldFont));
                            table.AddCell(new Phrase("Gender", boldFont));
                            table.AddCell(new Phrase(item.e.AGE, Font));
                            table.AddCell(new Phrase(item.e.GENDER, Font));
                            if (item.e.GENDER == "Female")
                            {
                                table.AddCell(new Phrase("Pregnant", boldFont));
                            }
                            if (item.e.PREGNANT_OR_NOT == "Yes")
                            {
                                table.AddCell(new Phrase("Trimester / Week", boldFont));
                            }
                            if (item.e.GENDER == "Female")
                            {
                                table.AddCell(new Phrase(item.e.PREGNANT_OR_NOT, Font));
                            }
                            if (item.e.PREGNANT_OR_NOT == "Yes")
                            {
                                table.AddCell(new Phrase(item.e.PREGNANT_WEEK, Font));
                            }
                            table.AddCell(new Phrase("Allergy History", boldFont));
                            table.AddCell(new Phrase("Weight (Kg)", boldFont));
                            table.AddCell(new Phrase(item.e.ALLERGY_HISTORY, Font));
                            table.AddCell(new Phrase(item.e.WEIGHT.ToString(), Font));

                            table.AddCell(new Phrase("Hight", boldFont));
                            table.AddCell(new Phrase("Diagnosis", boldFont));
                            table.AddCell(new Phrase(item.e.HEIGHT.ToString(), Font));
                            table.AddCell(new Phrase(item.e.DIAGNOSIS, Font));

                            table.AddCell(new Phrase("Serum Creatinine", boldFont));
                            table.AddCell(new Phrase("Liver Function", boldFont));
                            table.AddCell(new Phrase(item.e.SERUM_CREATININE, Font));
                            table.AddCell(new Phrase(item.e.LIVER_FUNCTION, Font));

                            table.AddCell(new Phrase("Electrolytes (Ca, Mg, K, P)", boldFont));
                            table.AddCell(new Phrase("Other Relevant Laboratory Result", boldFont));
                            table.AddCell(new Phrase(item.e.ELECTROLYTES, Font));
                            table.AddCell(new Phrase(item.e.LABORATORY_RESULT, Font));

                            table.AddCell(new Phrase("Current / Past Medications", boldFont));
                            table.AddCell(new Phrase("Other Related Sings and Symptoms", boldFont));
                            table.AddCell(new Phrase(item.e.MEDICATIONS, Font));
                            table.AddCell(new Phrase(item.e.RELATED_SINGS, Font));
                            table.FooterRows = 0;


                            table.AddCell(CellLine);
                            table.AddCell(CellLine);

                            //Query to get the case comorbidities
                            table.AddCell(new Phrase("Case Comorbidities", boldFont));
                            table.AddCell(new Phrase(""));
                            var GetCasecomorbidity = db.CASE_COMORBIDITY.Where(x => x.CASE_ID == item.e.CASE_ID);

                            List list5 = new List(List.UNORDERED, 10f);
                            list5.SetListSymbol("\u2022");
                            list5.IndentationLeft = 30f;

                            list5.Add(new ListItem());
                            PdfPCell c6 = new PdfPCell();
                            c6.Border = 0;
                            c6.PaddingBottom = 10;
                            c6.PaddingTop = 5;

                            Phrase C= new Phrase();
                            C.Font = Font;
                            Phrase C2 = new Phrase();
                            C2.Font = Font;  


                            foreach (var item6 in GetCasecomorbidity)
                            {
                                list5.Add(new ListItem(item6.COMORBIDITY.COMORBIDITY_NAME, Font));
                                if (item6.COMORBIDITY.COMORBIDITY_NAME == "Other")
                                {
                                    C.Add("Other Comorbidities");
                                    C2.Add(item6.OTHER_COMORBIDITY);
                                }
                            }
                            c6.AddElement(list5);
                            table.AddCell(c6);
                            table.AddCell("");
                            table.AddCell(C);
                            table.AddCell("");
                            table.AddCell(C2);
                            table.AddCell("");

                            table.AddCell(CellLine);
                            table.AddCell(CellLine);


                        }



                        table.AddCell(new Phrase("Case Categories Stage", boldFont));
                        table.AddCell(new Phrase(""));

                        var GetCaseCategory = db.CASE_CATEGORIES.Where(x => x.CASE_ID == item.e.CASE_ID);
                        List list1 = new List(List.UNORDERED, 10f);
                        list1.SetListSymbol("\u2022");
                        list1.IndentationLeft = 30f;
                        PdfPCell c2 = new PdfPCell();
                        c2.Border = 0;
                        c2.PaddingBottom = 10;
                        c2.PaddingTop = 5;
                        foreach (var item2 in GetCaseCategory)
                        { list1.Add(new ListItem(item2.CATEGORy.CATEGORY_NAME, Font)); }
                        c2.AddElement(list1);
                        table.AddCell(c2);
                        table.AddCell("");

                        table.AddCell(CellLine);
                        table.AddCell(CellLine);


                        //Query to get case questions and thier answers
                        table.AddCell(new Phrase("Case Questions Stage", boldFont));
                        table.AddCell(new Phrase("", boldFont));
                        var GetCaseQuestions = db.CASE_QUESTIONS.Where(x => x.CASE_ID == item.e.CASE_ID);
                        table.AddCell(new Phrase("Questions", boldFont));
                        table.AddCell(new Phrase("Answers", boldFont));
                        foreach (var item3 in GetCaseQuestions)
                        {
                            if (item3.QUESTION.ANSWER_TYPE == "Multiple Choice")
                            {
                                table.AddCell(new Phrase(item3.QUESTION.QUESTION1, Font));
                                table.AddCell(new Phrase(item3.MULTIPLE_CHOICE.CHOICE_NAM, Font));

                            }
                            else
                            {
                                table.AddCell(new Phrase(item3.QUESTION.QUESTION1, Font));
                                table.AddCell(new Phrase(item3.ANSWER, Font));
                            }
                        }

                        table.AddCell(CellLine);
                        table.AddCell(CellLine);

                        //get  case detailes stage inforamation
                        table.AddCell(new Phrase("Case Details Stage", boldFont));
                        table.AddCell(new Phrase("", boldFont));

                        table.AddCell(new Phrase("Ultimate Question", boldFont));
                        table.AddCell(new Phrase(""));
                        table.AddCell(new Phrase(item.e.ULTIMATE_QUESTION, Font));
                        table.AddCell(new Phrase(""));
                        //need to merege
                        table.AddCell(new Phrase("Ultimate Category:", boldFont));
                        table.AddCell(new Phrase(item.e.ULTIMATE_CATEGORY, Font));

                        table.AddCell(new Phrase("Case Urgency:", boldFont));
                        table.AddCell(new Phrase(item.e.URGENCY, Font));

                        table.AddCell(CellLine);
                        table.AddCell(CellLine);

                        table.AddCell(new Phrase("Case Answer Stage", boldFont));
                        table.AddCell(new Phrase("", boldFont));

                        //need to merege

                        table.AddCell(new Phrase("Answer", boldFont));
                        table.AddCell(new Phrase(""));
                        table.AddCell(new Phrase(item.e.ANSWER, Font));
                        table.AddCell(new Phrase(""));


                        table.AddCell(new Phrase("Researched By", boldFont));
                        table.AddCell(new Phrase(""));
                        table.AddCell(new Phrase(item.e.RESEARCHER_NAME, Font));
                        table.AddCell("");



                        //Query to get the case refrenses
                        table.AddCell(new Phrase("Case References", boldFont));
                        table.AddCell(new Phrase(""));
                        var GetCaseRfrenses = db.CASE_REFERENCES.Where(x => x.CASE_ID == item.e.CASE_ID);
                        List list2 = new List(List.UNORDERED, 10f);
                        list2.SetListSymbol("\u2022");
                        list2.IndentationLeft = 30f;
                        PdfPCell c5 = new PdfPCell();
                        c5.Border = 0;
                        c5.PaddingBottom = 10;
                        c5.PaddingTop = 5;

                        Phrase p = new Phrase();
                        p.Font = Font;
                        Phrase p2 = new Phrase();
                        p2.Font = Font;

                        foreach (var item4 in GetCaseRfrenses)
                        {

                            list2.Add(new ListItem(item4.REFERENCE.REFERENCE_NAME, Font));
                            if (item4.REFERENCE.REFERENCE_NAME == "Other")
                            {
                                p.Add("Other Refrenses");
                                p2.Add(item4.OTHER_REFRENSES);
                            }
                        }
                        c5.AddElement(list2);
                        table.AddCell(c5);
                        table.AddCell("");
                        table.AddCell(p);
                        table.AddCell("");
                        table.AddCell(p2);
                        table.AddCell("");


                        table.AddCell(CellLine);
                        table.AddCell(CellLine);



                        //get record stage inforamation
                        table.AddCell(new Phrase("Case Record Stage", boldFont));
                        table.AddCell(new Phrase("", boldFont));

                        table.AddCell(new Phrase("Answer Given to The Same Requester", boldFont));
                        table.AddCell(new Phrase("", boldFont));
                        table.AddCell(new Phrase(item.e.ANSWER_GIVEN, Font));
                        table.AddCell(new Phrase("", boldFont));

                        table.AddCell(new Phrase("Attempts Made to Contact Requester", boldFont));
                        table.AddCell(new Phrase("", boldFont));

                        table.AddCell(new Phrase(item.e.CONTACT_ATTEMPT, Font));
                        table.AddCell(new Phrase("", boldFont));

                        table.AddCell(new Phrase("Was The Ultimate Question Different from Original Question ?", boldFont));
                        table.AddCell("");
                        table.AddCell(new Phrase(item.e.IS_DIFFER, Font));
                        table.AddCell("");
                        if (item.e.ANSWER_GIVEN == "NO")
                        {

                            table.AddCell(new Phrase("Receiver ID", boldFont));
                            table.AddCell(new Phrase("Receiver Name", boldFont));
                            table.AddCell(new Phrase(item.e.RECEIVER_ID.ToString(), Font));
                            table.AddCell(new Phrase(item.e.RECEIVER_NAME, Font));
                        }


                        if (item.e.ANSWER_GIVEN == "Not")
                        {

                            table.AddCell(new Phrase("Comment", boldFont));
                            table.AddCell(new Phrase(item.e.NOT_REACHABLE_COMMENT, Font));


                        }
                        table.AddCell(CellLine);
                        table.AddCell(CellLine);



                    }
                    PdfFile.Add(table);
                    PdfFile.NewPage();


                }


                PdfFile.Close();
                return File(stream.ToArray(), "application/pdf", "DPIC Report.pdf");
            }



        }



    }



}


 class  PDFBackgroundHelper : PdfPageEventHelper
{

    private PdfContentByte cb;
    private List<PdfTemplate> templates;
    private List<PdfTemplate> templates2;

    //constructor
    public PDFBackgroundHelper()
    {
        this.templates = new List<PdfTemplate>();
        this.templates2 = new List<PdfTemplate>();

    }

    public override void OnEndPage(PdfWriter writer, Document document)
    {
        base.OnEndPage(writer, document);

        cb = writer.DirectContentUnder;
        PdfTemplate templateM = cb.CreateTemplate(50, 50);

        PdfTemplate templateN = cb.CreateTemplate(50, 50);


        templates.Add(templateM);

        templates2.Add(templateN);
        //page number
        int pageN = writer.CurrentPageNumber;
        String pageText = "Page " + pageN.ToString();
        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        float len = bf.GetWidthPoint(pageText, 10);
        cb.BeginText();
        cb.SetFontAndSize(bf, 10);
        cb.SetTextMatrix(550, 10);
        cb.ShowText(pageText);
        cb.EndText();
        cb.AddTemplate(templateM, document.Bottom + len, document.PageSize.GetRight(document.RightMargin));


        //page Header
        String Header = "Drug & Poison Information Center";
        String Header2 = "Clinical Pharmacy Department";
        String Header3 = "Pharmaceutical Care Services";
        String Header4 = "Prince Mohamed bin Abdulaziz Hospital-Madinah";
        String Header5 = "";


        BaseFont bf1 = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        BaseFont bf2 = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        float len1 = bf1.GetWidthPoint(Header, 10);
        float len3 = bf2.GetWidthPoint(Header2, 8);
        cb.BeginText();
        cb.SetFontAndSize(bf1, 11);
        cb.SetTextMatrix(document.LeftMargin, document.PageSize.GetTop(document.TopMargin) + 60);
        cb.ShowText(Header);
        cb.SetFontAndSize(bf2, 9);
        cb.SetTextMatrix(document.LeftMargin, document.PageSize.GetTop(document.TopMargin) + 45);
        cb.ShowText(Header2);
        cb.SetTextMatrix(document.LeftMargin, document.PageSize.GetTop(document.TopMargin) + 35);
        cb.ShowText(Header3);
        cb.SetTextMatrix(document.LeftMargin, document.PageSize.GetTop(document.TopMargin) + 25);
        cb.ShowText(Header4);
        cb.SetTextMatrix(document.LeftMargin, document.PageSize.GetTop(document.TopMargin) + 20);
        cb.ShowText(Header5);

        cb.EndText();
        cb.AddTemplate(templateN, document.LeftMargin + len1, document.PageSize.GetTop(document.TopMargin));
        cb.AddTemplate(templateN, document.LeftMargin + len3, document.PageSize.GetTop(document.TopMargin));






    }

    public override void OnCloseDocument(PdfWriter writer, Document document)
    {
        base.OnCloseDocument(writer, document);
        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);


        foreach (PdfTemplate item in templates)
        {



            item.BeginText();
            item.SetFontAndSize(bf, 10);
            item.SetTextMatrix(0, 0);
            item.ShowText("");
            item.EndText();
        }


        foreach (PdfTemplate item in templates2)
        {



            item.BeginText();
            item.SetFontAndSize(bf, 10);
            item.SetTextMatrix(0, 0);
            item.ShowText("");
            item.ShowText("");
            item.ShowText("");
            item.ShowText("");
            item.ShowText("");

            item.EndText();



        }


    }
}
