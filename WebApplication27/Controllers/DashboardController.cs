using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebApplication27.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using WebApplication27.Models.Infrastructure;

namespace WebApplication27.Controllers
{
    [CustomAuthenticationFilter]

    [CustomAuthorize("Access Dashboard")]
    public class DashboardController : Controller
    {
        private DB_Model db = new DB_Model();

        // GET: Dashboard


        public ActionResult Index()
        {

            //count pending cases 
            var pendingCases = db.CASES.Where(e => e.CASE_STATUS == "Pending").Count();
            ViewBag.pendingCases = pendingCases;


            //count completed cases 
            var completedCases = db.CASES.Where(e => e.CASE_STATUS == "Completed").Count();
            ViewBag.completedCases = completedCases;


            // count urgent cases 
            var UrgentCases = db.CASES.Where(e => e.URGENCY == "Urgent").Where(e => e.CASE_STATUS == "Pending").Count();
            ViewBag.UrgentCases = UrgentCases;

            //Deparments
            var listdep = db.REQUESTERS;
            List<int> repetations_11 = new List<int>();
            var departemnts = listdep.Select(x => x.DEPARTMENT.DEPARTMENT_NAME).Distinct();


            foreach (var item in departemnts)
            {
                repetations_11.Add(listdep.Count(x => x.DEPARTMENT.DEPARTMENT_NAME == item));

            }
            var rep = repetations_11;
            ViewBag.depName = departemnts;
            ViewBag.rep = repetations_11.ToList();
            //Urgency Cases
            var CountUrgencyCases = db.CASES.Where(e=>e.CASE_STATUS == "Completed").Count(x => x.URGENCY == "Urgent");
            var TotalMinutesForUrgencyCases = db.CASES.Where(e => e.CASE_STATUS == "Completed").Where(x => x.URGENCY == "Urgent").Sum(x => x.TIME_SPAN_MINUTES);
            if (CountUrgencyCases != 0)
            {
          
                    var divied = TotalMinutesForUrgencyCases / CountUrgencyCases;
                    int m = Convert.ToInt32(divied);
                    ViewBag.m = m;
             }
            else { ViewBag.m = "No Data Found";}

            //Reqular Cases
            var CountReqularCase = db.CASES.Where(e=> e.CASE_STATUS == "Completed").Count(x => x.URGENCY == "Regular");
            var TotalHoursForRegularCases = db.CASES.Where(e => e.CASE_STATUS == "Completed").Where(x => x.URGENCY == "Regular").Sum(x => x.TIME_SPAN_HOUR);
            if (CountReqularCase != 0)
            {
                var divied2 = TotalHoursForRegularCases / CountReqularCase;
                int h = Convert.ToInt32(divied2);
                ViewBag.h = h;

            }
            else { ViewBag.h = "No Data Found"; }



            return View();

        }

        [Authorize]
        [CustomAuthorize("Access Dashboard")]
        public ActionResult AllChart(DashboardModel data)
        {

            if (ModelState.IsValid)
            {

                var Filter = db.CASES.Where(e => e.CASE_START_DATE >= data.Start_date && e.CASE_START_DATE <= data.End_date && e.CASE_STATUS == "Completed").Any();

                if (Filter)
                {
                    if (data.ChartType == "Cases Overview PDF")
                    {
                        DateTime start = data.Start_date;
                        DateTime end = data.End_date;
                        return RedirectToAction("CasesOverviewPDF", new { start, end });

                    }
                    ViewBag.Option = data.ChartType;

                    //case per employee
                    var cases = db.CASES.Where(e => e.CASE_START_DATE >= data.Start_date && e.CASE_START_DATE <= data.End_date && e.CASE_STATUS == "Completed").GroupBy(r => r.USER.FULL_NAME).Select(r => new CasesPerEmployee()
                    {
                        Name = r.Key,
                        Count = r.Count()
                    });

                    var model = new GroupedUserCountViewModel
                    {
                        Items = cases.OrderByDescending(x => x.Count).ToList()
                    };


                    //refrenses 
                    var listRefrenses = from c in db.CASES
                                        join cc in db.CASE_REFERENCES on c.CASE_ID equals cc.CASE_ID
                                        where (c.CASE_STATUS == "Completed" && c.CASE_START_DATE >= data.Start_date && c.CASE_START_DATE <= data.End_date)
                                        select cc;
                    List<int> repetations_2 = new List<int>();
                    var refrenses = listRefrenses.Select(x => x.REFERENCE_NAME).Distinct();


                    foreach (var item in refrenses)
                    {
                        repetations_2.Add(listRefrenses.Count(x => x.REFERENCE_NAME == item));

                    }
                    var rep_2 = repetations_2;
                    ViewBag.refrensesName = refrenses;
                    ViewBag.rep_2 = repetations_2.ToList();

                    //General categories  

                    var listGeneralcategories = from c in db.CASES
                                                join cc in db.CASE_CATEGORIES on c.CASE_ID equals cc.CASE_ID
                                                join ccc in db.CATEGORIES on cc.CATEGORY_ID equals ccc.CATEGORY_ID
                                                where (c.CASE_STATUS == "Completed" && c.CASE_START_DATE >= data.Start_date && c.CASE_START_DATE <= data.End_date)
                                                where ccc.CATEGORY_TYPE == "General Questions"
                                                select cc;

                    List<int> repetations_3 = new List<int>();
                    var generalCategories = listGeneralcategories.Select(x => x.CATEGORY_NAME).Distinct();


                    foreach (var item in generalCategories)
                    {
                        repetations_3.Add(listGeneralcategories.Count(x => x.CATEGORY_NAME == item));

                    }
                    var rep_3 = repetations_3;
                    ViewBag.categoriesName = generalCategories;
                    ViewBag.rep_3 = repetations_3.ToList();


                    //Specific categories  
                    var listSpecificCategories = from c in db.CASES
                                                 join cc in db.CASE_CATEGORIES on c.CASE_ID equals cc.CASE_ID
                                                 join ccc in db.CATEGORIES on cc.CATEGORY_ID equals ccc.CATEGORY_ID
                                                 where (c.CASE_STATUS == "Completed" && c.CASE_START_DATE >= data.Start_date && c.CASE_START_DATE <= data.End_date)
                                                 where ccc.CATEGORY_TYPE == "Patient Specific Question"
                                                 select cc;

                    List<int> repetations_4 = new List<int>();
                    var SpecificCategories = listSpecificCategories.Select(x => x.CATEGORY_NAME).Distinct();


                    foreach (var item in SpecificCategories)

                    {
                        repetations_4.Add(listSpecificCategories.Count(x => x.CATEGORY_NAME == item));

                    }
                    var rep_4 = repetations_4;
                    ViewBag.specificCategoriesName = SpecificCategories;
                    ViewBag.rep_4 = repetations_4.ToList();




                    //Inquiry source
                    var listForInquiry = db.REQUESTERS;
                    List<int> repetations_5 = new List<int>();
                    var Inquirysource = listForInquiry.Select(x => x.CALLING_FROM).Distinct();


                    foreach (var item in Inquirysource)
                    {
                        repetations_5.Add(listForInquiry.Count(x => x.CALLING_FROM == item));

                    }
                    var rep_5 = repetations_5;
                    ViewBag.Inquirysource = Inquirysource;
                    ViewBag.rep_5 = repetations_5.ToList();



                    //Deparments
                    var listdep = db.REQUESTERS;
                    List<int> repetations_11 = new List<int>();
                    var departemnts = listdep.Select(x => x.DEPARTMENT.DEPARTMENT_NAME).Distinct();


                    foreach (var item in departemnts)
                    {
                        repetations_11.Add(listdep.Count(x => x.DEPARTMENT.DEPARTMENT_NAME == item));

                    }
                    var rep = repetations_11;
                    ViewBag.depName = departemnts;
                    ViewBag.rep = repetations_11.ToList();



                    //Is Differ

                    var listForDiffer = db.CASES.Where(e => e.CASE_START_DATE >= data.Start_date && e.CASE_START_DATE <= data.End_date && e.CASE_STATUS == "Completed");
                    List<int> repetations_6 = new List<int>();
                    var IsDiffer = listForDiffer.Select(x => x.IS_DIFFER).Distinct();


                    foreach (var item in IsDiffer)
                    {
                        repetations_6.Add(listForDiffer.Count(x => x.IS_DIFFER == item));

                    }
                    var rep_6 = repetations_6;
                    ViewBag.IsDiffer = IsDiffer;
                    ViewBag.rep_6 = repetations_6.ToList();



                    //ultimate category
                    var listForUltimateCategory = db.CASES.Where(e => e.CASE_START_DATE >= data.Start_date && e.CASE_START_DATE <= data.End_date && e.CASE_STATUS == "Completed");
                    List<int> repetations_7 = new List<int>();
                    var ultimateCategory = listForUltimateCategory.Select(x => x.ULTIMATE_CATEGORY).Distinct();

                    foreach (var item in ultimateCategory)
                    {
                        repetations_7.Add(listForUltimateCategory.Count(x => x.ULTIMATE_CATEGORY == item));
                    }
                    var rep_7 = repetations_7;
                    ViewBag.ultimateCategory = ultimateCategory;
                    ViewBag.rep_7 = repetations_7.ToList();

                    //Pregnancy Cases
                    var Pregnancy = db.CASES.Where(e => e.CASE_START_DATE >= data.Start_date && e.CASE_START_DATE <= data.End_date && e.CASE_STATUS == "Completed").Count(x => x.PREGNANT_OR_NOT == "Yes").ToString();
                    ViewBag.Pregnancy = Pregnancy;




                    //Urgency Cases
                    var CountUrgencyCases = db.CASES.Where(e => e.CASE_START_DATE >= data.Start_date && e.CASE_START_DATE <= data.End_date && e.CASE_STATUS == "Completed").Count(x => x.URGENCY == "Urgent");
                    var TotalMinutesForUrgencyCases = db.CASES.Where(e => e.CASE_START_DATE >= data.Start_date && e.CASE_START_DATE <= data.End_date && e.CASE_STATUS == "Completed").Where(x => x.URGENCY == "Urgent").Sum(x => x.TIME_SPAN_MINUTES);
                    if (CountUrgencyCases != 0)
                    {
                        var divied = TotalMinutesForUrgencyCases / CountUrgencyCases;
                        int m = Convert.ToInt32(divied);
                        ViewBag.m = m;
                    }
                    ViewBag.m = "No Data Found";

                    //Reqular Cases
                    var CountReqularCase = db.CASES.Where(e => e.CASE_START_DATE >= data.Start_date && e.CASE_START_DATE <= data.End_date && e.CASE_STATUS == "Completed").Count(x => x.URGENCY == "Regular");
                    var TotalHoursForRegularCases = db.CASES.Where(e => e.CASE_START_DATE >= data.Start_date && e.CASE_START_DATE <= data.End_date && e.CASE_STATUS == "Completed").Where(x => x.URGENCY == "Regular").Sum(x => x.TIME_SPAN_HOUR);
                    if (CountReqularCase != 0)
                    {
                        var divied2 = TotalHoursForRegularCases / CountReqularCase;
                        ViewBag.h = divied2;
                    }
                    ViewBag.h = "No Data Found";



                    return View("View",model);
                }

                else
                {
                    TempData["No_data"] = "No data found";
                    return RedirectToAction("Index");
                }


            }
            else
            {
                return View("View", data);


            }


        }


        public ActionResult CasesOverviewPDF(DateTime start, DateTime end)
        {
           
                var Filter = db.CASES.Where(e => e.CASE_START_DATE >= start && e.CASE_START_DATE <= end && e.CASE_STATUS == "Completed").Any();

                if (Filter)
                {

                    using (MemoryStream stream = new MemoryStream())
                    {    //تالت واحد التوب
                        //تاني واحد الرايقت
                       // last one is the bottom
                        Document PdfFile = new Document(PageSize.A4, 20f, 5f, 90f, 40f);
                        PdfWriter writer = PdfWriter.GetInstance(PdfFile, stream);
                        writer.PageEvent = new PDFBackgroundHelper();
                        PdfFile.Open();


                        //Query Section
                        var GetCaseData = from e in db.CASES
                                          where (e.CASE_START_DATE >= start && e.CASE_START_DATE <= end && e.CASE_STATUS == "Completed")
                                          select new { e };

                        var cases = db.CASES.Where(e => e.CASE_START_DATE >= start && e.CASE_START_DATE <= end && e.CASE_STATUS == "Completed").GroupBy(r => r.USER.FULL_NAME).Select(r => new CasesPerEmployee()
                        {
                            Name = r.Key,
                            Count = r.Count()
                        });


                        //PDF Table Settings
                        PdfPTable table = new PdfPTable(2);
                        table.DefaultCell.PaddingBottom = 10;
                        table.DefaultCell.PaddingTop = 5;
                        table.WidthPercentage = 100;
                        table.SpacingBefore =  40;
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                
                   
                        //Font Settings -> It can be used in any cell
                        var boldFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 11);
                        var Font = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 11);
                        table.AddCell(new Phrase("Name", boldFont));
                        table.AddCell(new Phrase("Cases", boldFont));
                        foreach (var item in cases.OrderBy(x=>x.Count))
                        {
                         
                                table.AddCell(new Phrase(item.Name, Font));
                                table.AddCell(new Phrase(item.Count.ToString(), Font));
                        }

                    //Pregnancy Cases
                    var Pregnancy = db.CASES.Where(e=> e.CASE_START_DATE >= start && e.CASE_START_DATE <= end && e.CASE_STATUS == "Completed").Count(x => x.PREGNANT_OR_NOT == "Yes").ToString();


                    //Urgency Cases
                    var CountUrgencyCases = db.CASES.Where(e => e.CASE_START_DATE >= start && e.CASE_START_DATE <= end && e.CASE_STATUS == "Completed").Count(x => x.URGENCY == "Urgent");
                    var TotalMinutesForUrgencyCases = db.CASES.Where(e => e.CASE_START_DATE >= start && e.CASE_START_DATE <= end && e.CASE_STATUS == "Completed").Where(x => x.URGENCY == "Urgent").Sum(x => x.TIME_SPAN_MINUTES);
                    string Urgency;

                    if (CountUrgencyCases != 0)
                    {
                        var divied = TotalMinutesForUrgencyCases / CountUrgencyCases;
                        int m = Convert.ToInt32(divied);
                         Urgency = m.ToString();

                    }
                    else { Urgency = "No Data Found"; }

                    //Reqular Cases
                    var CountReqularCase = db.CASES.Where(e => e.CASE_START_DATE >= start && e.CASE_START_DATE <= end && e.CASE_STATUS == "Completed").Count(x => x.URGENCY == "Regular");
                    var TotalHoursForRegularCases = db.CASES.Where(e => e.CASE_START_DATE >= start && e.CASE_START_DATE <= end && e.CASE_STATUS == "Completed").Where(x => x.URGENCY == "Regular").Sum(x => x.TIME_SPAN_HOUR);
                    string Reqular;
                    if (CountReqularCase != 0)
                    {
                        var divied2 = TotalHoursForRegularCases / CountReqularCase;
                         Reqular = divied2.ToString();
                    }
                    else { Reqular = "No Data Found"; }

                    Paragraph phrase = new Paragraph("Pregnancy-Related Cases:" + Pregnancy,Font);
                    phrase.SpacingBefore = 10;

                    Paragraph phrase2 = new Paragraph("Average Time Per Urgent Cases (Minutes):"+ Urgency,Font);
                    phrase2.SpacingBefore = 10;

                    Paragraph phrase3 = new Paragraph("Average Time Per Regular Cases (Hours) :" + Reqular,Font);


                    PdfFile.Add(table);
                    PdfFile.Add(phrase);
                    PdfFile.Add(phrase2);
                    PdfFile.Add(phrase3);




                    PdfFile.Close();
                        return File(stream.ToArray(), "application/pdf", "Cases Overview.pdf");
                    }


                }


                else{

                    return View("Index");

                }


        }

        public ActionResult CaesOverView()
        {

            //count pending cases 
            var pendingCases = db.CASES.Where(e => e.CASE_STATUS == "Pending").Count();
            ViewBag.pendingCases = pendingCases;


            //count completed cases 
            var completedCases = db.CASES.Where(e => e.CASE_STATUS == "Completed").Count();
            ViewBag.completedCases = completedCases;


            // count urgent cases 
            var UrgentCases = db.CASES.Where(e => e.URGENCY == "Urgent").Count();
            ViewBag.UrgentCases = UrgentCases;

            //Deparments
            var listdep = db.REQUESTERS;
            List<int> repetations_11 = new List<int>();
            var departemnts = listdep.Select(x => x.DEPARTMENT.DEPARTMENT_NAME).Distinct();


            foreach (var item in departemnts)
            {
                repetations_11.Add(listdep.Count(x => x.DEPARTMENT.DEPARTMENT_NAME == item));

            }
            var rep = repetations_11;
            ViewBag.depName = departemnts;
            ViewBag.rep = repetations_11.ToList();


            //case per employee
            var cases = db.CASES.Where(e => e.CASE_STATUS == "Completed").GroupBy(r => r.USER.FULL_NAME).Select(r => new CasesPerEmployee()
            {
                Name = r.Key,
                Count = r.Count()
            });

            var model = new GroupedUserCountViewModel
            {
                Items = cases.OrderByDescending(x => x.Count).ToList()
            };



            return PartialView(model);

        }

    }



    }
