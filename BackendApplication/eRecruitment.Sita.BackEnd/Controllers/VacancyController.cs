﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eRecruitment.BusinessDomain.DAL.Entities.AppModels;
using eRecruitment.Sita.BackEnd.App_Data.Entities.DAL;
using eRecruitment.Sita.BackEnd.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Data;
using System.Text;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using SITA.Notifications;
//using OfficeOpenXml.Style;


namespace eRecruitment.Sita.BackEnd.Controllers
{
    public class VacancyController : Controller
    {
        readonly eRecruitment.BusinessDomain.DAL.DataAccess _dal = new BusinessDomain.DAL.DataAccess();
        eRecruitmentDataClassesDataContext _db = new eRecruitmentDataClassesDataContext();
        //Notification notify = new Notification();
        NotificationBL notify = new NotificationBL(new Notification());


        // GET: Vacancy
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            try {

                string userid = User.Identity.GetUserId();
                int cid = _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x => x.OrganisationID).FirstOrDefault();
                int totalVacancy = 0;
                int totalApproved = 0;
                int totalRejected = 0;
                int totalWithdrawn = 0;

                if (User.IsInRole("Recruiter"))
                {
                    //totalVacancy = _db.tblVacancies.Where(x => x.UserID == userid).Count();
                    totalVacancy = (from a in _db.tblVacancies
                                    from b in _db.tblFinYears
                                    where a.OrganisationID == cid && a.RecruiterUserId == userid
                                    && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.RecruiterUserId == userid
                                    select a.ID).Count();

                    //totalApproved = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 3).Count();
                    totalApproved = (from a in _db.tblVacancies
                                     from b in _db.tblFinYears
                                     where a.OrganisationID == cid && a.RecruiterUserId == userid
                                     && a.StatusID == 3
                                     && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.RecruiterUserId == userid
                                     select a.ID).Count();

                    //totalRejected = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 4).Count();
                    totalRejected = (from a in _db.tblVacancies
                                     from b in _db.tblFinYears
                                     where a.OrganisationID == cid && a.RecruiterUserId == userid
                                     && a.StatusID == 4
                                     && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.RecruiterUserId == userid
                                     select a.ID).Count();

                    //totalWithdrawn = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 5).Count();
                    totalWithdrawn = (from a in _db.tblVacancies
                                      from b in _db.tblFinYears
                                      where a.OrganisationID == cid && a.RecruiterUserId == userid
                                      && a.StatusID == 5
                                      && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.RecruiterUserId == userid
                                      select a.ID).Count();
                }

                else if (User.IsInRole("Recruiter Admin"))
                {
                    //totalVacancy = _db.tblVacancies.Where(x => x.UserID == userid).Count();
                    totalVacancy = (from a in _db.tblVacancies
                                    from b in _db.tblFinYears
                                    where a.OrganisationID == cid && a.UserID == userid
                                    && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate
                                    select a.ID).Count();

                    //totalApproved = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 3).Count();
                    totalApproved = (from a in _db.tblVacancies
                                     from b in _db.tblFinYears
                                     where a.OrganisationID == cid && a.UserID == userid
                                     && a.StatusID == 3
                                     && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate
                                     select a.ID).Count();

                    //totalRejected = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 4).Count();
                    totalRejected = (from a in _db.tblVacancies
                                     from b in _db.tblFinYears
                                     where a.OrganisationID == cid && a.UserID == userid
                                     && a.StatusID == 4
                                     && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate
                                     select a.ID).Count();

                    //totalWithdrawn = _db.tblVacancies.Where(x => x.UserID == userid && x.StatusID == 5).Count();
                    totalWithdrawn = (from a in _db.tblVacancies
                                      from b in _db.tblFinYears
                                      where a.OrganisationID == cid && a.UserID == userid
                                      && a.StatusID == 5
                                      && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate
                                      select a.ID).Count();
                }

                else if (User.IsInRole("Approver"))
                {
                    //totalVacancy = _db.tblVacancies.Where(x => x.OrganisationID == cid).Count();
                    totalVacancy = (from a in _db.tblVacancies
                                    from b in _db.tblFinYears
                                    where a.OrganisationID == cid
                                    && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.Manager == userid
                                    select a.ID).Count();

                    //totalApproved = _db.tblVacancies.Where(x => x.OrganisationID == cid && x.StatusID == 3).Count();
                    totalApproved = (from a in _db.tblVacancies
                                     from b in _db.tblFinYears
                                     where a.OrganisationID == cid && a.StatusID == 3
                                     && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.Manager == userid
                                     select a.ID).Count();

                    //totalRejected = _db.tblVacancies.Where(x => x.OrganisationID == cid && x.StatusID == 4).Count();
                    totalRejected = (from a in _db.tblVacancies
                                     from b in _db.tblFinYears
                                     where a.OrganisationID == cid && a.StatusID == 4
                                     && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.Manager == userid
                                     select a.ID).Count();

                    //totalWithdrawn = _db.tblVacancies.Where(x => x.OrganisationID == cid && x.StatusID == 5).Count();
                    totalWithdrawn = (from a in _db.tblVacancies
                                      from b in _db.tblFinYears
                                      where a.OrganisationID == cid && a.StatusID == 5
                                      && a.CreatedDate >= b.StartDate && a.CreatedDate <= b.EndDate && a.Manager == userid
                                      select a.ID).Count();
                }
                ViewBag.TotalVacancy = totalVacancy;
                ViewBag.TotalApproved = totalApproved;
                ViewBag.TotalRejected = totalRejected;
                ViewBag.TotalWithdrawn = totalWithdrawn;

            } catch (Exception ex) {


                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");



            }
           

            return View();
        }
        public ActionResult VacancyList()
        {
            try
            {
                string userid = User.Identity.GetUserId();
                ViewBag.VacancyList = _dal.GetVancyList(userid);
                ViewBag.RetractList = _dal.GetRetractReasonList();
                ViewBag.WithdrawalList = _dal.GetWithdrawalReasonList();
            }
            catch (Exception ex)
            {

                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");

            }

          
            return View();
        }

        public ActionResult ViewVacancyAdList()
        {
          

            try
            {
                string userid = User.Identity.GetUserId();
                ViewBag.VacancyList = _dal.GetVacancyListForView(userid);
            }
            catch (Exception ex)
            {

                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");

            }
            return View();
        }

        public ActionResult ViewVacancy(int id)
        {
            List<VacancyModels> p;

            try
            {
                string userid = User.Identity.GetUserId();
                var orgID = _dal.GetOrganisationID(userid);

                ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
                ViewBag.Organisation = _dal.GetOrganisationList(userid);
                ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
                ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
                ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
                ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
                ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

                ViewBag.Division = _dal.GetDivisionList(userid);
                ViewBag.SitaDepartment = _dal.GetDepartment(userid);
                ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
                ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
                ViewBag.VacancyType = _dal.GetVacancyTypeList();
                ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
                ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
                ViewBag.SkillsList = _dal.GetSelectedSkillsPerCatergiryList(id);
                ViewBag.RejectReasonList = _dal.GetRejectReasonList();
                ViewBag.ApproverList = _dal.GetListOfApprovers();
                ViewBag.PublishDays = _dal.GetPublishDaysList();
                ViewBag.Location = _dal.GetLocationList(orgID);

                ViewBag.Gender = _dal.GetGenderList();
                ViewBag.Race = _dal.GetRaceList();
                ViewBag.SalaryType = _dal.GetSalaryTypeList();
                ViewBag.RecruiterList = _dal.GetListOfRecruiters();

                 p = _dal.GetVacancyForApproval(id);

                ViewBag.Vacancy = p;
            }
            catch (Exception ex)
            {

                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");

            }
            return View(p);
        }

        public ActionResult DownloadVacancyAd(int id)
        {


            try
            {
                string userID = User.Identity.GetUserId();
                var data = _db.sp_GetVacancyAdDetail(id).FirstOrDefault();
                //var data = await candidateMyProfileWebAPI.DownloadVacancyAd(id);

                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {

                    Document document = new Document(PageSize.A4, 10, 10, 10, 10);
                    //Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                    Font courier = new Font(Font.FontFamily.HELVETICA, 9f);

                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();
                    //document.Add(new Paragraph("\n"));

                    //var logo = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/dist/img/sita.png"));

                    var logo = Image.GetInstance((byte[])data.FileData.ToArray());
                    logo.Alignment = Element.ALIGN_CENTER;
                    logo.ScaleToFit(120f, 80f);

                    document.Add(logo);
                    //document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                    BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                    ////Insert Image
                    //string imagepath = Server.MapPath("img");
                    //Image gif = Image.GetInstance(imagepath + "/eclogoAIM.png");
                    //gif.Alignment = Element.ALIGN_LEFT;
                    ////Resize image depend upon your need
                    //gif.ScaleToFit(250f, 200f);
                    ////Give space before image
                    //gif.SpacingBefore = 10f;
                    ////Give some space after the image
                    //gif.SpacingAfter = 1f;
                    //document.Add(gif);
                    ////End Image

                    Paragraph paraHead = new Paragraph("VACANCY ADVERTISEMENT");
                    paraHead.Alignment = Element.ALIGN_CENTER;
                    paraHead.Font = FontFactory.GetFont("dax-black", 10, Font.BOLD);
                    //cellVacancyPurpose.BackgroundColor = BaseColor.LIGHT_GRAY;

                    document.Add(paraHead);
                    document.Add(new Paragraph("\n"));

                    PdfPTable table = new PdfPTable(2);

                    //PdfPCell cell = new PdfPCell(new Phrase("Vacancy Information Download"));
                    PdfPCell cell = new PdfPCell(new Phrase("Vacancy Information Download", font));
                    cell.Colspan = 2;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.Border = 0; //Added this for testing
                    table.AddCell(cell);

                    PdfPCell emptyCell = new PdfPCell(new Phrase(" ", font));
                    emptyCell.Colspan = 2;
                    emptyCell.Border = 0;
                    table.AddCell(emptyCell);

                    //Reference Number
                    //table.AddCell("Reference Number:");
                    //table.AddCell(data.ReferenceNo);

                    PdfPCell cellBPSVacancyNo = new PdfPCell(new Phrase("BPS Vacancy Number:", font));
                    cellBPSVacancyNo.Colspan = 1;
                    cellBPSVacancyNo.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cellReferenceNo.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellBPSVacancyNo);

                    PdfPCell cellBPSVacancyNoContent = new PdfPCell(new Phrase(data.VacancyNo.ToString(), font));
                    cellBPSVacancyNoContent.Colspan = 1;
                    cellBPSVacancyNoContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellBPSVacancyNoContent);

                    PdfPCell cellReferenceNo = new PdfPCell(new Phrase("Reference Number:", font));
                    cellReferenceNo.Colspan = 1;
                    cellReferenceNo.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cellReferenceNo.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellReferenceNo);

                    PdfPCell cellReferenceNoContent = new PdfPCell(new Phrase(data.ReferenceNo.ToString(), font));
                    cellReferenceNoContent.Colspan = 1;
                    cellReferenceNoContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellReferenceNoContent);



                    //Job Title
                    PdfPCell cellJobTitle = new PdfPCell(new Phrase("Job Title:", font));
                    cellJobTitle.Colspan = 1;
                    cellJobTitle.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellJobTitle);

                    PdfPCell cellJobTitleContent = new PdfPCell(new Phrase(data.JobTitle.ToString(), font));
                    cellJobTitleContent.Colspan = 1;
                    cellJobTitleContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellJobTitleContent);


                    //Job Level
                    PdfPCell cellJobLevel = new PdfPCell(new Phrase("Job Level:", font));
                    cellJobLevel.Colspan = 1;
                    cellJobLevel.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellJobLevel);

                    PdfPCell cellJobLevelContent = new PdfPCell(new Phrase(data.JobLevel.ToString(), font));
                    cellJobLevelContent.Colspan = 1;
                    cellJobLevelContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellJobLevelContent);


                    //Vacancy Type
                    PdfPCell cellVacancyType = new PdfPCell(new Phrase("Vacancy Type:", font));
                    cellVacancyType.Colspan = 1;
                    cellVacancyType.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellVacancyType);

                    PdfPCell cellVacancyTypeContent = new PdfPCell(new Phrase(data.VacancyTypeName.ToString(), font));
                    cellVacancyTypeContent.Colspan = 1;
                    cellVacancyTypeContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellVacancyTypeContent);

                    //Salary Range
                    PdfPCell cellSalaryRange = new PdfPCell(new Phrase("Salary Range:", font));
                    cellSalaryRange.Colspan = 1;
                    cellSalaryRange.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellSalaryRange);

                    PdfPCell cellSalaryRangeContent = new PdfPCell(new Phrase(data.Salary.ToString(), font));
                    cellSalaryRangeContent.Colspan = 1;
                    cellSalaryRangeContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellSalaryRangeContent);

                    //Organisation
                    PdfPCell cellOrganisationName = new PdfPCell(new Phrase("Organisation Name:", font));
                    cellOrganisationName.Colspan = 1;
                    cellOrganisationName.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellOrganisationName);

                    PdfPCell cellOrganisationNameContent = new PdfPCell(new Phrase(data.OrganisationName.ToString(), font));
                    cellOrganisationNameContent.Colspan = 1;
                    cellOrganisationNameContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellOrganisationNameContent);

                    //table.AddCell("Report To: ");
                    //table.AddCell(data.Manager);

                    //Division
                    PdfPCell cellDivisionName = new PdfPCell(new Phrase("Division:", font));
                    cellDivisionName.Colspan = 1;
                    cellDivisionName.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellDivisionName);

                    PdfPCell cellDivisionNameContent = new PdfPCell(new Phrase(data.DivisionName.ToString(), font));
                    cellDivisionNameContent.Colspan = 1;
                    cellDivisionNameContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellDivisionNameContent);

                    //Department
                    PdfPCell cellDepartmentName = new PdfPCell(new Phrase("Department:", font));
                    cellDepartmentName.Colspan = 1;
                    cellDepartmentName.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellDepartmentName);

                    PdfPCell cellDepartmentNameContent = new PdfPCell(new Phrase(data.DepartmentName.ToString(), font));
                    cellDepartmentNameContent.Colspan = 1;
                    cellDepartmentNameContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellDepartmentNameContent);

                    //Employment Type
                    PdfPCell cellEmploymentType = new PdfPCell(new Phrase("Employment Type:", font));
                    cellEmploymentType.Colspan = 1;
                    cellEmploymentType.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellEmploymentType);

                    PdfPCell cellEmploymentTypeContent = new PdfPCell(new Phrase(data.EmploymentType.ToString(), font));
                    cellEmploymentTypeContent.Colspan = 1;
                    cellEmploymentTypeContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellEmploymentTypeContent);


                    //Contract Duiration
                    if (data.EmploymentType != null)
                    {
                        if (data.EmploymentType != "Permanent")
                        {
                            PdfPCell cellContractDuration = new PdfPCell(new Phrase("Contract Duration:", font));
                            cellContractDuration.Colspan = 1;
                            cellContractDuration.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.AddCell(cellContractDuration);

                            PdfPCell cellContractDurationContent = new PdfPCell(new Phrase(data.ContractDuration.ToString(), font));
                            cellContractDurationContent.Colspan = 1;
                            cellContractDurationContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                            table.AddCell(cellContractDurationContent);
                        }
                    }

                    //Location
                    PdfPCell cellLocation = new PdfPCell(new Phrase("Location:", font));
                    cellLocation.Colspan = 1;
                    cellLocation.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellLocation);

                    PdfPCell cellLocationContent = new PdfPCell(new Phrase(data.Location.ToString(), font));
                    cellLocationContent.Colspan = 1;
                    cellLocationContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellLocationContent);

                    //table.AddCell("Recruiter: ");
                    //table.AddCell(data.Recruiter);

                    //table.AddCell("Reply To: ");
                    //table.AddCell(data.ReplyTo);

                    //Number or Opening
                    PdfPCell cellNumberOfOpenings = new PdfPCell(new Phrase("Number Of Openings:", font));
                    cellNumberOfOpenings.Colspan = 1;
                    cellNumberOfOpenings.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(cellNumberOfOpenings);

                    PdfPCell cellNumberOfOpeningsContent = new PdfPCell(new Phrase(data.NumberOfOpenings.ToString(), font));
                    cellNumberOfOpeningsContent.Colspan = 1;
                    cellNumberOfOpeningsContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellNumberOfOpeningsContent);

                    table.AddCell(emptyCell);

                    //Vacancy Purpose
                    PdfPCell cellVacancyPurpose = new PdfPCell(new Phrase("Purpose of Job:", font));
                    cellVacancyPurpose.Colspan = 2;
                    cellVacancyPurpose.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellVacancyPurpose.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellVacancyPurpose);

                    PdfPCell cellVacancyPurposeContent = new PdfPCell(new Phrase(data.VacancyPurpose, font));
                    cellVacancyPurposeContent.Colspan = 2;
                    cellVacancyPurposeContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellVacancyPurposeContent);
                    //End Vacancy Purpose

                    table.AddCell(emptyCell);

                    //Responsibilities
                    PdfPCell cellResponsibilities = new PdfPCell(new Phrase("Responsibilities:", font));
                    cellResponsibilities.Colspan = 2;
                    cellResponsibilities.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellResponsibilities.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellResponsibilities);

                    PdfPCell cellResponsibilitiesContent = new PdfPCell(new Phrase(data.Responsibility, font));
                    cellResponsibilitiesContent.Colspan = 2;
                    cellResponsibilitiesContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellResponsibilitiesContent);
                    //End Responsibilities

                    //table.AddCell("Responsibilities: ");
                    //table.AddCell(data.Responsibility);

                    table.AddCell(emptyCell);

                    //Qualifications and Experience
                    PdfPCell cellQualificationsandExperience = new PdfPCell(new Phrase("Qualifications and Experience:", font));
                    cellQualificationsandExperience.Colspan = 2;
                    cellQualificationsandExperience.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellQualificationsandExperience.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellQualificationsandExperience);

                    PdfPCell cellQualificationsandExperienceContent = new PdfPCell(new Phrase(data.QualificationsandExperience, font));
                    cellQualificationsandExperienceContent.Colspan = 2;
                    cellQualificationsandExperienceContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellQualificationsandExperienceContent);

                    //table.AddCell("Qualifications and Experience: ");
                    //table.AddCell(data.QualificationsandExperience);
                    //End Qualifications and Experience
                    table.AddCell(emptyCell);

                    //Knowledge
                    PdfPCell cellKnowledge = new PdfPCell(new Phrase("Knowledge:", font));
                    cellKnowledge.Colspan = 2;
                    cellKnowledge.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellKnowledge.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellKnowledge);

                    PdfPCell cellKnowledgeContent = new PdfPCell(new Phrase(data.Knowledge, font));
                    cellKnowledgeContent.Colspan = 2;
                    cellKnowledgeContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellKnowledgeContent);
                    //table.AddCell("Knowledge: ");
                    //table.AddCell(data.Knowledge);
                    //End Knowledge


                    table.AddCell(emptyCell);
                    //Technical Competency
                    StringBuilder technicalCompBuilder = new StringBuilder();
                    int technicalCompCounter = 0;
                    var technicalComp = _dal.GetTechnicalCompList((int)data.jobprofileid);

                    foreach (var item in technicalComp)
                    {
                        technicalCompCounter += 1;
                        technicalCompBuilder.AppendLine(technicalCompCounter.ToString() + ". " + item.TechnicalComp + " - " + item.TechnicalCompDesc);
                    }

                    PdfPCell cellTechnicalComp = new PdfPCell(new Phrase("Technical Competencies", font));
                    cellTechnicalComp.Colspan = 2;
                    cellTechnicalComp.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellTechnicalComp.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellTechnicalComp);

                    PdfPCell cellTechnicalCompContent = new PdfPCell(new Phrase(technicalCompBuilder.ToString(), font));
                    cellTechnicalCompContent.Colspan = 2;
                    cellTechnicalCompContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellTechnicalCompContent);

                    //Leadership Competency
                    if (data.JobLevelID >= 17) //From the tblJob Level We sorted the Job Level with ID and 17 is the D4 and Above
                    {
                        StringBuilder leadershipCompBuilder = new StringBuilder();
                        int leadershipCompCounter = 0;
                        var leadershipComp = _dal.GetLeadershipCompList((int)data.jobprofileid);

                        foreach (var item in leadershipComp)
                        {
                            leadershipCompCounter += 1;
                            leadershipCompBuilder.AppendLine(leadershipCompCounter.ToString() + ". " + item.LeadershipComp + " - " + item.LeadershipCompDesc);
                        }

                        PdfPCell cellLeadershipComp = new PdfPCell(new Phrase("Leadership Competencies", font));
                        cellLeadershipComp.Colspan = 2;
                        cellLeadershipComp.HorizontalAlignment = Element.ALIGN_LEFT;
                        cellLeadershipComp.BackgroundColor = BaseColor.LIGHT_GRAY;
                        table.AddCell(cellLeadershipComp);

                        PdfPCell cellLeadershipCompContent = new PdfPCell(new Phrase(leadershipCompBuilder.ToString(), font));
                        cellLeadershipCompContent.Colspan = 2;
                        cellLeadershipCompContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                        table.AddCell(cellLeadershipCompContent);
                    }


                    //Behavioural Competency
                    StringBuilder behaviouralCompBuilder = new StringBuilder();
                    int behaviouralCompCounter = 0;
                    var behaviouralComp = _dal.GetBehaviouralCompList((int)data.jobprofileid);

                    foreach (var item in behaviouralComp)
                    {
                        behaviouralCompCounter += 1;
                        behaviouralCompBuilder.AppendLine(behaviouralCompCounter.ToString() + ". " + item.BehaviouralComp + " - " + item.BehaviouralCompDesc);
                    }

                    table.AddCell(emptyCell);
                    PdfPCell cellBehaviouralComp = new PdfPCell(new Phrase("Interpersonal and Behavioural Competencies", font));
                    cellBehaviouralComp.Colspan = 2;
                    cellBehaviouralComp.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellBehaviouralComp.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellBehaviouralComp);

                    PdfPCell cellBehaviouralCompContent = new PdfPCell(new Phrase(behaviouralCompBuilder.ToString(), font));
                    cellBehaviouralCompContent.Colspan = 2;
                    cellBehaviouralCompContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellBehaviouralCompContent);



                    //var skillsCategoryList = _dal.GetSelectedSkillsPerCatergiryListForJobProfile((int)data.JobProfileID);
                    //foreach (var item in skillsCategoryList)
                    //{
                    //  table.AddCell("Skills: ");
                    //  table.AddCell(item.skillName);
                    //}
                    table.AddCell(emptyCell);
                    //Additional Requirements
                    PdfPCell cellOtherSpecialRequirements = new PdfPCell(new Phrase("Additional Requirements", font));
                    cellOtherSpecialRequirements.Colspan = 2;
                    cellOtherSpecialRequirements.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellOtherSpecialRequirements.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellOtherSpecialRequirements);

                    PdfPCell cellOtherSpecialRequirementsContent = new PdfPCell(new Phrase(data.OtherSpecialRequirements, font));
                    cellOtherSpecialRequirementsContent.Colspan = 2;
                    cellOtherSpecialRequirementsContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellOtherSpecialRequirementsContent);

                    //table.AddCell("Additional Requirements: ");
                    //table.AddCell(data.OtherSpecialRequirements);
                    //End Additional Requirements

                    //table.AddCell("Recruiter ");
                    //table.AddCell(data.Recruiter);

                    //table.AddCell("Email your query to ");
                    //table.AddCell(data.RecruiterEmail);
                    table.AddCell(emptyCell);

                    PdfPCell cellHowToApply = new PdfPCell(new Phrase("How to apply", font));
                    cellHowToApply.Colspan = 2;
                    cellHowToApply.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellHowToApply.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellHowToApply);

                    Font LinkFont = FontFactory.GetFont(FontFactory.COURIER, 9, iTextSharp.text.Font.UNDERLINE, BaseColor.BLUE);

                    // var anchorMalebo = new Anchor("Malebo.pudikabekwa@sita.co.za", LinkFont);
                    //var malebo = anchorMalebo.Reference = "mailto:Malebo.pudikabekwa@sita.co.za";

                    // var anchorPrudence = new Anchor("Malebo.pudikabekwa@sita.co.za", LinkFont);
                    // var prudence = anchorPrudence.Reference = "mailto:Prudence.masola@sita.co.za";

                    // var anchorZanele = new Anchor("Zanele.sompini@sita.co.za", LinkFont);
                    // var zanele = anchorZanele.Reference = "mailto:Zanele.sompini@sita.co.za";

                    //var support = from a in _db.SupportStaffs
                    //              join b in _db.AspNetUsers on a.UserID equals b.Id
                    //              where a.IsActive == true
                    //              select b.Email;

                    //var result = (dynamic)null;
                    //List<string> list = new List<string>();

                    //foreach (var sf in support)
                    //{
                    //    //result = String.Join("; ", sf.ToString());

                    //    list.Add(sf.ToString());

                    //}
                    //String[] str = list.ToArray();
                    //result = string.Join(" , ", str);


                    PdfPCell cellcellHowToApplyContent = new PdfPCell(new Phrase("To apply please log onto the e-Government Portal: "
                                                        + new Uri(System.Configuration.ConfigurationManager.AppSettings["LogOutURL"])
                                                        + " and follow the following process;"
                                                        + Environment.NewLine + "1. Register using your ID and personal information;"
                                                        + Environment.NewLine + "2. Use received one-time pin to complete the registration;"
                                                        + Environment.NewLine + "3. Log in using your username and password;"
                                                        + Environment.NewLine + "4. Click on “Employment & Labour”; "
                                                        + Environment.NewLine + "5. Click on “Recruitment Citizen” to create profile, update profile, browse and apply for jobs;"
                                                        + Environment.NewLine
                                                        + Environment.NewLine + "Or, if candidate has registered on eservices portal, access "
                                                        + new Uri(System.Configuration.ConfigurationManager.AppSettings["LogOutURL"])
                                                        + ", then follow the below steps:"
                                                        + Environment.NewLine + "1. Click on “Employment & Labour”;"
                                                        + Environment.NewLine + "2. Click on “Recruitment Citizen”;"
                                                        + Environment.NewLine + "3. Log in using your username and password;"
                                                        + Environment.NewLine + "4. Click on “Recruitment Citizen” to create profile, update profile, browse and apply for jobs; "
                                                        + Environment.NewLine + Environment.NewLine
                                                        + "For support, please send an email to: " + System.Configuration.ConfigurationManager.AppSettings["SupportEmailAddress"], font));


                    cellcellHowToApplyContent.Colspan = 2;
                    cellcellHowToApplyContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellcellHowToApplyContent);


                    //        table.AddCell("HOW TO APPLY ");
                    //table.AddCell(System.Configuration.ConfigurationManager.AppSettings["LogOutURL"]);

                    table.AddCell(emptyCell);
                    //Closing Date
                    PdfPCell cellClosingDate = new PdfPCell(new Phrase("Closing Date : " + Convert.ToDateTime(data.ClosingDate).ToString("dd MMM yyyy"), font));
                    cellClosingDate.Colspan = 2;
                    cellClosingDate.HorizontalAlignment = Element.ALIGN_LEFT;

                    cellClosingDate.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cellClosingDate.Border = 0; //Added this for testing
                    table.AddCell(cellClosingDate);
                    //End Closing Date

                    table.AddCell(emptyCell);

                    char[] delimiterChars = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                    StringBuilder rbuilder = new StringBuilder();

                    string text = data.Disclaimer;

                    //System.Diagnostics.Debug.WriteLine($"Original text: '{text}'");

                    string[] words = text.Split(delimiterChars);
                    //System.Diagnostics.Debug.WriteLine($"{words.Length} words in text:");
                    int myVal = 0;
                    foreach (var word in words)
                    {
                        if (word != null || word != "")
                        {
                            myVal += 1;
                            if (myVal - 1 == 0)
                            {
                                rbuilder.Append(word + Environment.NewLine);
                                //rbuilder.AppendLine();
                                //System.Diagnostics.Debug.WriteLine($"<{word + Environment.NewLine}>");
                            }
                            else
                            {
                                rbuilder.Append(myVal - 1 + word + Environment.NewLine);
                                //System.Diagnostics.Debug.WriteLine($"<{myVal - 1 + word}>"); 
                            }
                        }
                    }

                    string disclaimerData = rbuilder.ToString();

                    PdfPCell cellDisclaimer = new PdfPCell(new Phrase("Disclaimer", font));
                    cellDisclaimer.Colspan = 2;
                    cellDisclaimer.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellDisclaimer.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellDisclaimer);

                    //string myDisclaimer =  string.Join(Environment.NewLine, data.Disclaimer.ToCharArray().Where(Char.IsDigit));

                    //PdfPCell cellDisclaimerContent = new PdfPCell(new Phrase(myDisclaimer));
                    //PdfPCell cellDisclaimerContent = new PdfPCell(new Phrase(data.Disclaimer));
                    PdfPCell cellDisclaimerContent = new PdfPCell(new Phrase(disclaimerData, font));
                    cellDisclaimerContent.Colspan = 2;
                    cellDisclaimerContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    table.AddCell(cellDisclaimerContent);

                    //table.DefaultCell.Border = Rectangle.NO_BORDER;
                    document.Add(table);

                    document.Add(new Paragraph("\n"));


                    //paragraph.Add(text);
                    Paragraph paraEnd = new Paragraph("******NB: EMAILED CV'S WILL NOT BE ACCEPTED******", font);
                    paraEnd.Alignment = Element.ALIGN_CENTER;
                    paraHead.Font = FontFactory.GetFont("dax-black", 10, Font.ITALIC);
                    document.Add(paraEnd);

                    document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                    document.Close();
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    Response.Clear();
                    Response.ContentType = "application/pdf";

                    string pdfName = data.ReferenceNo.Replace("/", "_");
                    string jobTitle = data.JobTitle.Replace(":", " ") + ".pdf";
                    string fileName = data.ReferenceNo.Replace("/", "_") + ".pdf";

                    //Response.AddHeader("content-disposition", "attachment;filename=" + string.Format("{}_{}", jobTitle,fileName));
                    Response.AddHeader("content-disposition", "attachment;filename=" + jobTitle);

                    //Response.AddHeader("Content-Disposition", @"attachment; filename=" + pdfName + ".pdf");
                    Response.ContentType = "application/pdf";
                    Response.Buffer = true;
                    Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                    Response.BinaryWrite(bytes);
                    Response.End();
                    Response.Close();

                }
            }
            catch (Exception ex)
            {

                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");

            }




           

            return View();
        }

        //Upload Vacancy
        [Authorize]
        [HttpGet]
        public ActionResult AddVacancy()
        {

            try
            {
                string userid = User.Identity.GetUserId();
                var orgID = _dal.GetOrganisationID(userid);

                ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
                ViewBag.Organisation = _dal.GetOrganisationList(userid);
                ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
                ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
                ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
                ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
                ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

                ViewBag.Location = _dal.GetLocationList(orgID);
                ViewBag.Division = _dal.GetDivisionList(userid);
                ViewBag.SitaDepartment = _dal.GetDepartment(userid);
                ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
                //ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();

                ViewBag.VacancyType = _dal.GetVacancyTypeList();
                ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
                ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
                ViewBag.PublishDays = _dal.GetPublishDaysList();
                ViewBag.ApproverList = _dal.GetListOfApprovers();

                ViewBag.Gender = _dal.GetGenderList();
                ViewBag.Race = _dal.GetRaceList();
                ViewBag.SalaryType = _dal.GetSalaryTypeList();
                ViewBag.RecruiterList = _dal.GetListOfRecruiters();

                var data = _dal.GetAllJobTitleListFromVacancyProfile(userid);
                int jobTitleID = data.Select(a => a.JobTitleID).FirstOrDefault();
                var model = _dal.GetVacancyProfileID(jobTitleID).FirstOrDefault();

                var skillsCategoryList = _dal.GetSelectedSkillsPerCatergiryListForJobProfile(model.VacancyProfileID);
                ViewBag.SkillsCategoryList = skillsCategoryList;
            }
            catch (Exception ex)
            {

                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");
            }
            

            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddVacancy(HttpPostedFileBase postedFile, HttpPostedFileBase postedBusinessCase, VacancyModels model, FormCollection fc, IEnumerable<HttpPostedFileBase> files, string button)
        {


            try {
                string Organisation = fc["Organisation"];
                string Recruiter = fc["FullName"];
                string RecruiterEmail = fc["EmailAddress"];
                string ds = fc["EmailAddress"];
                //String str = model.BPSVacancyNo;
                //char[] spearator = {','};
                //String[] strlist = str.Split(spearator);

                //Check files Extensions


                foreach (var file in files)
                {
                    if (file.ContentLength > 0 && file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);

                        var filesize = 5;
                        if (fileExt != "pdf" && fileExt != "tiff" && fileExt != "png" && fileExt != "gif" && fileExt != "jpeg" && fileExt != "jpg")
                        {
                            ModelState.AddModelError("", "Please check uploaded file extension, there seems to be an invalid extension/s - Only Upload Pdf/Images File");
                        }
                        else if (file.ContentLength > (filesize * 1024))
                        {
                            //ModelState.AddModelError("", "File size Should Be UpTo " + filesize + "KB");
                            //ModelState.AddModelError("", "File size Should Be UpTo 5MB");
                        }
                    }
                }

                string userid = User.Identity.GetUserId();
                var orgID = _dal.GetOrganisationID(userid);
                var VacancyProfileID = _dal.GetVacancyProfileID(model.JobTitleID);
                ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
                ViewBag.Organisation = _dal.GetOrganisationList(userid);
                ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
                ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
                ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
                ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
                ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

                ViewBag.Location = _dal.GetLocationList(orgID);
                ViewBag.Division = _dal.GetDivisionList(userid);
                ViewBag.SitaDepartment = _dal.GetDepartment(userid);
                ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
                ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
                ViewBag.VacancyType = _dal.GetVacancyTypeList();
                ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
                ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
                ViewBag.ApproverList = _dal.GetListOfApprovers();
                ViewBag.PublishDays = _dal.GetPublishDaysList();

                ViewBag.Gender = _dal.GetGenderList();
                ViewBag.Race = _dal.GetRaceList();
                ViewBag.SalaryType = _dal.GetSalaryTypeList();
                ViewBag.RecruiterList = _dal.GetListOfRecruiters();

                /*
                 * Author:khutso mabelane
                 * The following validations are handled on the view with javascript 
                 * **/

                //var data = _dal.CheckIfVacancyExists(model.DivisionID, model.DepartmentID, model.JobTitleID, model.OrganisationID);
                //var BPSVacancyNo = _dal.CheckIfVacancyNumberExists(model.BPSVacancyNo);


                //if (data > 0)
                //{
                //    ModelState.AddModelError(" ", "Record already Exist");
                //}
                //if (BPSVacancyNo > 0)
                //{
                //    ModelState.AddModelError(" ", "BPS Vacany Number already Exist");
                //}
                //if (model.VacancyQuestionID == null)
                //{
                //    ModelState.AddModelError(" ", "Please select at least one vacancy question");
                //}
                //if (model.DepartmentID == 0)
                //{
                //    ModelState.AddModelError(" ", "Please select department");
                //}

                if (ModelState.IsValid)
                {

                    //=========================Peter - 20230112========================
                    //if (button.ToLower() == "add")
                    //{
                    //    if (model.JobSpecificQuestions != null)
                    //    {
                    //        _dal.InsertUpdateLutJobSpecificQuestion((int)model.JobTitleID, model.JobSpecificQuestions, DateTime.Now, User.Identity.GetUserId(), null, null);
                    //        return View(model);
                    //    }
                    //}
                    //else if (button.ToLower() == "save")
                    //{
                    //    //code
                    //}
                    //=================================================================


                    string DeligationReasons = string.Empty;
                    string AdditonalRequirements = string.Empty;
                    string VacancyPurpose = string.Empty;
                    string QualificationAndExperience = string.Empty;
                    string TechComps = string.Empty;
                    string Disclaimer = string.Empty;
                    string Responsibility = string.Empty;
                    string Knowledge = string.Empty;
                    string LeadComps = string.Empty;
                    string BehaveComps = string.Empty;

                    if (model.DeligationReasons != null) { DeligationReasons = this.RemoveSpecialCharacters(model.DeligationReasons); } else { DeligationReasons = string.Empty; }
                    if (model.AdditonalRequirements != null) { AdditonalRequirements = this.RemoveSpecialCharacters(model.AdditonalRequirements); } else { AdditonalRequirements = string.Empty; }
                    if (model.VacancyPurpose != null) { VacancyPurpose = this.RemoveSpecialCharacters(model.VacancyPurpose); } else { VacancyPurpose = string.Empty; }
                    if (model.QualificationAndExperience != null) { QualificationAndExperience = this.RemoveSpecialCharacters(model.QualificationAndExperience); } else { QualificationAndExperience = string.Empty; }
                    if (model.TechComps != null) { TechComps = this.RemoveSpecialCharacters(model.TechComps); } else { TechComps = string.Empty; }
                    if (model.Disclaimer != null) { Disclaimer = this.RemoveSpecialCharacters(model.Disclaimer); } else { Disclaimer = string.Empty; }
                    if (model.Responsibility != null) { Responsibility = this.RemoveSpecialCharacters(model.Responsibility); } else { Responsibility = string.Empty; }
                    if (model.Knowledge != null) { Knowledge = this.RemoveSpecialCharacters(model.Knowledge); } else { Knowledge = string.Empty; }
                    if (model.LeadComps != null) { LeadComps = this.RemoveSpecialCharacters(model.LeadComps); } else { LeadComps = string.Empty; }
                    if (model.BehaveComps != null) { BehaveComps = this.RemoveSpecialCharacters(model.BehaveComps); } else { BehaveComps = string.Empty; }

                    int? vacancyid = _dal.InsertVacancy(userid, Convert.ToInt32(Organisation), model.BPSVacancyNo, model.DivisionID, model.DepartmentID, model.JobTitleID,
                                                       model.SalaryTypeID, Recruiter, RecruiterEmail, model.RecruiterTel, model.RecruiterUserId, model.Manager, model.GenderID, model.RaceID,
                                                       model.EmploymentTypeID, model.ContractDuration, Convert.ToDateTime(model.ClosingDate), model.NumberOfOpenings, model.VancyTypeID,
                                                       DeligationReasons, model.Location, AdditonalRequirements, VacancyProfileID[0].VacancyProfileID,
                                                       VacancyPurpose, QualificationAndExperience, TechComps, Disclaimer, Responsibility,
                                                       Knowledge, LeadComps, BehaveComps);

                    if (vacancyid != null)
                    {
                        if (model.VacancyQuestionID != null)
                        {
                            string vqid = null;
                            vqid = string.Join(";", model.VacancyQuestionID);
                            _dal.InsertUpdateVacancyQuestion((int)vacancyid, Convert.ToString(vqid));

                        }

                        if (model.BPSVacancyNo != null)
                        {
                            String BPSNumber = model.BPSVacancyNo;

                            _dal.InsertUpdateVacancyBPSNumber((int)vacancyid, Convert.ToString(BPSNumber));

                        }

                        // create a loop that will filter any thing inside fc that starts with sk_
                        List<string> listOfSkills = new List<string>();
                        for (var i = 0; i < fc.Count; i++)
                        {
                            var name = fc.Keys[i];
                            if (name.Contains("sk_"))
                            {
                                listOfSkills.Add(fc[i]);
                            }
                        }

                        var newList = listOfSkills;
                        string fileExt = string.Empty;
                        if (model.CategoryID > 0 && listOfSkills != null)
                        {

                            string vqid = null;
                            vqid = string.Join(";", listOfSkills);
                            _dal.InsertUpdateVacancySkill((int)vacancyid, Convert.ToInt32(model.CategoryID), Convert.ToString(vqid));

                        }

                        //Loop through all the specified files
                        foreach (var file in files)
                        {
                            if (file.ContentLength > 0)
                            {
                                //var fileName = Path.GetFileName(file.FileName);
                                var fileName = Path.GetFileNameWithoutExtension(file.FileName);

                                byte[] bytes;
                                using (BinaryReader br = new BinaryReader(file.InputStream))
                                {
                                    bytes = br.ReadBytes(file.ContentLength);
                                }
                                //string filePath = Path.GetFileName(postedFile.FileName);
                                //string filePath = "Requisition";
                                string filePath = fileName;

                                string ContentType = file.ContentType;

                                var count = _db.tblVacancyDocuments.Where(x => x.fkVacancyID == (int)vacancyid && x.sFileName == filePath).Count();
                                fileExt = string.Empty;
                                fileExt = System.IO.Path.GetExtension(file.FileName);
                                //_dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                                if (count == 0)
                                {
                                    _dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);
                                }
                                else
                                {
                                    _dal.UpdateVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                                }

                            }
                        }

                        var ReferenceNo = _dal.GetVacancyRefNo((int)vacancyid);

                        TempData["message"] = "You have successfully Uploaded the Vacancy, Here is Your Reference No:" + "(" + ReferenceNo[0].ReferenceNo + ")";

                    }

                    return RedirectToAction("VacancyList", new { id = userid });
                }



            }
            catch (Exception ex)
            {
                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");



            }
            //string[] QuestionID = null;
            //QuestionID = model.VacancyQuestionID.ToArray();
           
            return View(model);
        }


        //==================PETER TRY AGAIN THIS METHOD ON 2023/13/01=================================================
        //[HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //public ActionResult AddLutJobSpecificQuestion(HttpPostedFileBase postedFile, HttpPostedFileBase postedBusinessCase, VacancyModels model, FormCollection fc, IEnumerable<HttpPostedFileBase> files)
        //{

        //    Session["sessAddQuestion"] = "Doski";
        //    //AddVacancy(postedFile, postedBusinessCase, model, fc, files);

        //    return null;

        //}
        public JsonResult AddJobSpecQuest(JobJobSpecificQuestionModel objJobSpecificQuestionModel)
        {

            //if (!ModelState.IsValid)
            //{
            //    var errorList = (from item in ModelState
            //                     where item.Value.ErrorMessage.ToList());
            //    return Json(new { success = false, Message = "Some problem in validation", ErrorList = errorList });
            //}
            

            return Json( "", JsonRequestBehavior.AllowGet);

        }



        //=========================================================TRY THIS - END ========================================================

        //============Peter - Add Job Specific Question 20231101============================
        //public ActionResult AddLutJobSpecificQuestion(HttpPostedFileBase postedFile, HttpPostedFileBase postedBusinessCase, VacancyModels model, FormCollection fc, IEnumerable<HttpPostedFileBase> files)
        //{

        //    if (model.JobSpecificQuestions != null)
        //    {
        //        _dal.InsertUpdateLutJobSpecificQuestion((int)model.JobTitleID, model.JobSpecificQuestions, DateTime.Now, User.Identity.GetUserId(), null, null);

        //    }
        //    return View(model);
        //}
        //===================================================================================

        //Edit Vacancy 
        [Authorize]
        [HttpGet]
        public ActionResult EditVacancy(int id)
        {
            List<VacancyModels> p;
            try {

                string userid = User.Identity.GetUserId();
                var orgID = _dal.GetOrganisationID(userid);

                ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
                ViewBag.Organisation = _dal.GetOrganisationList(userid);
                ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
                ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
                ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
                ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
                ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

                ViewBag.Location = _dal.GetLocationList(orgID);
                ViewBag.Division = _dal.GetDivisionList(userid);
                ViewBag.SitaDepartment = _dal.GetDepartment(userid);
                ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
                ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
                ViewBag.VacancyType = _dal.GetVacancyTypeList();
                ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
                ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
                ViewBag.SkillsList = _dal.GetSelectedSkillsPerCatergiryList(id);
                ViewBag.ApproverList = _dal.GetListOfApprovers();
                ViewBag.PublishDays = _dal.GetPublishDaysList();

                ViewBag.Gender = _dal.GetGenderList();
                ViewBag.Race = _dal.GetRaceList();
                ViewBag.SalaryType = _dal.GetSalaryTypeList();
                ViewBag.RecruiterList = _dal.GetListOfRecruiters();

                p = _dal.GetVacancyListForEditByID(id);

            }
            catch (Exception ex)
            {


                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");


            }



            // convert the closeDate inside the "p" object to a string then reformat it to a html format
            //var newDate = p[0].ClosingDate.ToString().Substring(0, 9);
            //p[0].ClosingDate = newDate;

            return View(p);
        }

        //Upload Edit Vacancy
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditVacancy(HttpPostedFileBase postedFile, HttpPostedFileBase postedBusinessCase, VacancyModels item, FormCollection fc, int id, string[] VacancyQuestionID, IEnumerable<HttpPostedFileBase> files)
        {

            try {

                string Organisation = fc["Organisation"];
                string Recruiter = fc["FullName"];
                string RecruiterEmail = fc["EmailAddress"];
                string userid = User.Identity.GetUserId();
                Session["sessUserID"] = userid;


                //if (postedFile != null && postedFile.ContentLength > 00)
                //{
                //    var fileExt = System.IO.Path.GetExtension(postedFile.FileName).Substring(1);

                //    var filesize = 5;
                //    if (fileExt != "pdf" && fileExt != "tiff" && fileExt != "png" && fileExt != "gif" && fileExt != "jpeg" && fileExt != "jpg")
                //    {
                //        ModelState.AddModelError("", "File Extension Is InValid - Only Upload Pdf/Images File");
                //    }
                //    else if (postedFile.ContentLength > (filesize * 1024))
                //    {
                //        //ModelState.AddModelError("", "File size Should Be UpTo " + filesize + "KB");
                //        //ModelState.AddModelError("", "File size Should Be UpTo 5MB");
                //    }
                //}
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            if (file.ContentLength > 0 && file != null)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);

                                var filesize = 5;
                                if (fileExt != "pdf" && fileExt != "tiff" && fileExt != "png" && fileExt != "gif" && fileExt != "jpeg" && fileExt != "jpg")
                                {
                                    ModelState.AddModelError("", "Please check uploaded file extension, there seems to be an invalid extension/s - Only Upload Pdf/Images File");
                                }
                                else if (file.ContentLength > (filesize * 1024))
                                {
                                    //ModelState.AddModelError("", "File size Should Be UpTo " + filesize + "KB");
                                    //ModelState.AddModelError("", "File size Should Be UpTo 5MB");
                                }
                            }
                        }

                    }
                }



                //if (Convert.ToDateTime(item.ClosingDate) <= DateTime.Now)
                //{
                //    ModelState.AddModelError("", "Closing date cannot be less or equals to Today's Date");
                //    TempData["Warning"] = "Closing date cannot be less or equals to Today's Date";
                //    return RedirectToAction("VacancyList", new { id = userid });
                //}

                ViewBag.ClosingDate = item.ClosingDate;

                var orgID = _dal.GetOrganisationID(userid);
                var VacancyProfileID = _dal.GetVacancyProfileID(item.JobTitleID);
                ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
                ViewBag.Organisation = _dal.GetOrganisationList(userid);
                ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
                ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
                ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
                ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
                ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

                ViewBag.Location = _dal.GetLocationList(orgID);
                ViewBag.Division = _dal.GetDivisionList(userid);
                ViewBag.SitaDepartment = _dal.GetDepartment(userid);
                ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
                ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
                ViewBag.VacancyType = _dal.GetVacancyTypeList();
                ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
                ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
                ViewBag.SkillsList = _dal.GetSelectedSkillsPerCatergiryList(id);
                ViewBag.ApproverList = _dal.GetListOfApprovers();
                ViewBag.PublishDays = _dal.GetPublishDaysList();

                ViewBag.Gender = _dal.GetGenderList();
                ViewBag.Race = _dal.GetRaceList();
                ViewBag.SalaryType = _dal.GetSalaryTypeList();
                ViewBag.RecruiterList = _dal.GetListOfRecruiters();

                //if (VacancyQuestionID == null)
                //{
                //    ModelState.AddModelError(" ", "Please select at least one vacancy question");
                //}

                if (ModelState.IsValid)
                {
                    string DeligationReasons = string.Empty;
                    string AdditonalRequirements = string.Empty;
                    string VacancyPurpose = string.Empty;
                    string QualificationAndExperience = string.Empty;
                    string TechComps = string.Empty;
                    string Disclaimer = string.Empty;
                    string Responsibility = string.Empty;
                    string Knowledge = string.Empty;
                    string LeadComps = string.Empty;
                    string BehaveComps = string.Empty;

                    if (item.DeligationReasons != null) { DeligationReasons = this.RemoveSpecialCharacters(item.DeligationReasons); } else { DeligationReasons = string.Empty; }
                    if (item.AdditonalRequirements != null) { AdditonalRequirements = this.RemoveSpecialCharacters(item.AdditonalRequirements); } else { AdditonalRequirements = string.Empty; }
                    if (item.VacancyPurpose != null) { VacancyPurpose = this.RemoveSpecialCharacters(item.VacancyPurpose); } else { VacancyPurpose = string.Empty; }
                    if (item.QualificationAndExperience != null) { QualificationAndExperience = this.RemoveSpecialCharacters(item.QualificationAndExperience); } else { QualificationAndExperience = string.Empty; }
                    if (item.TechComps != null) { TechComps = this.RemoveSpecialCharacters(item.TechComps); } else { TechComps = string.Empty; }
                    if (item.Disclaimer != null) { Disclaimer = this.RemoveSpecialCharacters(item.Disclaimer); } else { Disclaimer = string.Empty; }
                    if (item.Responsibility != null) { Responsibility = this.RemoveSpecialCharacters(item.Responsibility); } else { Responsibility = string.Empty; }
                    if (item.Knowledge != null) { Knowledge = this.RemoveSpecialCharacters(item.Knowledge); } else { Knowledge = string.Empty; }
                    if (item.LeadComps != null) { LeadComps = this.RemoveSpecialCharacters(item.LeadComps); } else { LeadComps = string.Empty; }
                    if (item.BehaveComps != null) { BehaveComps = this.RemoveSpecialCharacters(item.BehaveComps); } else { BehaveComps = string.Empty; }

                    //if (item.DeligationReasons != null) { DeligationReasons = item.DeligationReasons; } else { DeligationReasons = string.Empty; }
                    //if (item.AdditonalRequirements != null) { AdditonalRequirements = item.AdditonalRequirements; } else { AdditonalRequirements = string.Empty; }
                    //if (item.VacancyPurpose != null) { VacancyPurpose = item.VacancyPurpose; } else { VacancyPurpose = string.Empty; }
                    //if (item.QualificationAndExperience != null) { QualificationAndExperience = item.QualificationAndExperience; } else { QualificationAndExperience = string.Empty; }
                    //if (item.TechComps != null) { TechComps = item.TechComps; } else { TechComps = string.Empty; }
                    //if (item.Disclaimer != null) { Disclaimer = item.Disclaimer; } else { Disclaimer = string.Empty; }
                    //if (item.Responsibility != null) { Responsibility = item.Responsibility; } else { Responsibility = string.Empty; }
                    //if (item.Knowledge != null) { Knowledge = item.Knowledge; } else { Knowledge = string.Empty; }
                    //if (item.LeadComps != null) { LeadComps = item.LeadComps; } else { LeadComps = string.Empty; }
                    //if (item.BehaveComps != null) { BehaveComps = item.BehaveComps; } else { BehaveComps = string.Empty; }

                    int? vacancyid = _dal.UpdateVacancy(id, userid, Convert.ToInt32(Organisation), item.BPSVacancyNo, item.DivisionID, item.DepartmentID, item.JobTitleID,
                                                        item.SalaryTypeID, item.Recruiter, item.RecruiterEmail, item.RecruiterTel, item.RecruiterUserId, item.Manager, item.GenderID, item.RaceID,
                                                        item.EmploymentTypeID, item.ContractDuration, Convert.ToDateTime(item.ClosingDate), item.NumberOfOpenings
                                                        , item.VancyTypeID, DeligationReasons,
                                                        item.Location, AdditonalRequirements, VacancyProfileID[0].VacancyProfileID,
                                                        VacancyPurpose, QualificationAndExperience, TechComps
                                                        , Disclaimer, Responsibility, Knowledge, LeadComps, BehaveComps);

                    if (vacancyid != null)
                    {
                        if (vacancyid != null)
                        {
                            if (VacancyQuestionID != null)
                            {
                                string vqid = null;
                                vqid = string.Join(";", VacancyQuestionID);
                                _dal.InsertUpdateVacancyQuestion((int)vacancyid, Convert.ToString(vqid));

                            }
                            _dal.UpdateVacancyStatus(7, Convert.ToInt32(id));
                        }

                        if (item.BPSVacancyNo != null)
                        {
                            String BPSNumber = item.BPSVacancyNo;

                            _dal.InsertUpdateVacancyBPSNumber((int)vacancyid, Convert.ToString(BPSNumber));

                        }

                        // create a loop that will filter any thing inside fc that starts with sk_
                        List<string> listOfSkills = new List<string>();
                        for (var i = 0; i < fc.Count; i++)
                        {
                            var name = fc.Keys[i];
                            if (name.Contains("sk_"))
                            {
                                listOfSkills.Add(fc[i]);
                            }
                        }

                        var newList = listOfSkills;
                        string fileExt = string.Empty;

                        if (item.CategoryID > 0 && listOfSkills != null)
                        {

                            string vqid = null;
                            vqid = string.Join(";", listOfSkills);
                            _dal.InsertUpdateVacancySkill((int)vacancyid, Convert.ToInt32(item.CategoryID), Convert.ToString(vqid));

                        }

                        //Loop through all the specified files
                        foreach (var file in files)
                        {
                            if (file != null)
                            {

                                if (file.ContentLength > 0)
                                {
                                    //var fileName = Path.GetFileName(file.FileName);
                                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);

                                    byte[] bytes;
                                    using (BinaryReader br = new BinaryReader(file.InputStream))
                                    {
                                        bytes = br.ReadBytes(file.ContentLength);
                                    }
                                    //string filePath = Path.GetFileName(postedFile.FileName);
                                    //string filePath = "Requisition";
                                    string filePath = fileName;

                                    string ContentType = file.ContentType;

                                    var count = _db.tblVacancyDocuments.Where(x => x.fkVacancyID == (int)vacancyid && x.sFileName == filePath).Count();
                                    fileExt = string.Empty;
                                    fileExt = System.IO.Path.GetExtension(file.FileName);
                                    //_dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                                    if (count == 0)
                                    {
                                        _dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);
                                    }
                                    else
                                    {
                                        _dal.UpdateVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                                    }

                                }
                            }

                        }

                        //if (postedFile != null && postedFile.ContentLength > 00)
                        //{
                        //    byte[] bytes;
                        //    using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                        //    {
                        //        bytes = br.ReadBytes(postedFile.ContentLength);
                        //    }
                        //    //string filePath = Path.GetFileName(postedFile.FileName);
                        //    string filePath = "Requisition";
                        //    fileExt = string.Empty;
                        //    fileExt = System.IO.Path.GetExtension(postedFile.FileName);
                        //    string ContentType = postedFile.ContentType;
                        //    var count = _db.tblVacancyDocuments.Where(x => x.fkVacancyID == id && x.sFileName == filePath).Count();
                        //    if (count == 0)
                        //    {
                        //        _dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);
                        //    }
                        //    else
                        //    {
                        //        _dal.UpdateVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                        //    }
                        //}

                        //if (postedBusinessCase != null && postedBusinessCase.ContentLength > 00)
                        //{
                        //    byte[] bytes;
                        //    using (BinaryReader br = new BinaryReader(postedBusinessCase.InputStream))
                        //    {
                        //        bytes = br.ReadBytes(postedBusinessCase.ContentLength);
                        //    }
                        //    //string filePath = Path.GetFileName(postedBusinessCase.FileName);
                        //    string filePath = "Business Case";
                        //    fileExt = string.Empty;
                        //    fileExt = System.IO.Path.GetExtension(postedBusinessCase.FileName);
                        //    string ContentType = postedBusinessCase.ContentType;
                        //    var count = _db.tblVacancyDocuments.Where(x => x.fkVacancyID == id && x.sFileName == filePath).Count();
                        //    if (count == 0)
                        //    {
                        //        _dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);
                        //    }
                        //    else
                        //    {
                        //        _dal.UpdateVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                        //    }
                        //}

                        TempData["message"] = "Vacancy Updated Successfully";

                    }

                    return RedirectToAction("VacancyList", new { id = userid });
                }
            } 
            catch (Exception ex) {


                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");

            }
           
            return View();
        }


        [Authorize]
        [HttpGet]
        public ActionResult ReAdvertiseVacancy(int id)
        {
            List<VacancyModels> p;
            try
            {

                //HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["BPSTrackerAPIURL"] + "VAC04731/20");
                //myRequest.Timeout= 5000;
                //HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
                //response.GetResponseStream();

                //if (response.StatusCode == HttpStatusCode.OK)
                //{
                //    Stream dataStream = response.GetResponseStream();
                //    // Open the stream using a StreamReader for easy access.
                //    StreamReader reader = new StreamReader(dataStream);
                //    // Read the content.
                //    string responseFromServer = reader.ReadToEnd();

                //    dynamic data = JsonConvert.DeserializeObject(responseFromServer);
                //}
                    //set status to re-advertise
                    //post model to save
                    string userid = User.Identity.GetUserId();
                var orgID = _dal.GetOrganisationID(userid);

                ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
                ViewBag.Organisation = _dal.GetOrganisationList(userid);
                ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
                ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
                ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
                ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
                ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

                ViewBag.Location = _dal.GetLocationList(orgID);
                ViewBag.Division = _dal.GetDivisionList(userid);
                ViewBag.SitaDepartment = _dal.GetDepartment(userid);
                ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
                ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
                ViewBag.VacancyType = _dal.GetVacancyTypeList();
                ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
                ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
                ViewBag.SkillsList = _dal.GetSelectedSkillsPerCatergiryList(id);
                ViewBag.ApproverList = _dal.GetListOfApprovers();
                ViewBag.PublishDays = _dal.GetPublishDaysList();

                ViewBag.Gender = _dal.GetGenderList();
                ViewBag.Race = _dal.GetRaceList();
                ViewBag.SalaryType = _dal.GetSalaryTypeList();
                ViewBag.RecruiterList = _dal.GetListOfRecruiters();
                ViewBag.VacancyReAdvertiseNO = _dal.getVacancyReAdvertiseNo(id);

                p = _dal.GetVacancyListForEditByID(id);
                

            }
            catch (Exception ex)
            {


                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");
            }
          
            // convert the closeDate inside the "p" object to a string then reformat it to a html format
            //var newDate = p[0].ClosingDate.ToString().Substring(0, 9);
            //p[0].ClosingDate = newDate;

            return View(p);
        }

        //Upload re adverstise Vacancy
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ReAdvertiseVacancy(HttpPostedFileBase postedFile, HttpPostedFileBase postedBusinessCase, VacancyModels item, FormCollection fc, int id, string[] VacancyQuestionID, IEnumerable<HttpPostedFileBase> files)
        {

            try
            {

                string Organisation = fc["Organisation"];
                string Recruiter = fc["FullName"];
                string RecruiterEmail = fc["EmailAddress"];
                string userid = User.Identity.GetUserId();
                Session["sessUserID"] = userid;


                //if (postedFile != null && postedFile.ContentLength > 00)
                //{
                //    var fileExt = System.IO.Path.GetExtension(postedFile.FileName).Substring(1);

                //    var filesize = 5;
                //    if (fileExt != "pdf" && fileExt != "tiff" && fileExt != "png" && fileExt != "gif" && fileExt != "jpeg" && fileExt != "jpg")
                //    {
                //        ModelState.AddModelError("", "File Extension Is InValid - Only Upload Pdf/Images File");
                //    }
                //    else if (postedFile.ContentLength > (filesize * 1024))
                //    {
                //        //ModelState.AddModelError("", "File size Should Be UpTo " + filesize + "KB");
                //        //ModelState.AddModelError("", "File size Should Be UpTo 5MB");
                //    }
                //}
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        if (file != null)
                        {
                            if (file.ContentLength > 0 && file != null)
                            {
                                var fileName = Path.GetFileName(file.FileName);
                                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);

                                var filesize = 5;
                                if (fileExt != "pdf" && fileExt != "tiff" && fileExt != "png" && fileExt != "gif" && fileExt != "jpeg" && fileExt != "jpg")
                                {
                                    ModelState.AddModelError("", "Please check uploaded file extension, there seems to be an invalid extension/s - Only Upload Pdf/Images File");
                                }
                                else if (file.ContentLength > (filesize * 1024))
                                {
                                    //ModelState.AddModelError("", "File size Should Be UpTo " + filesize + "KB");
                                    //ModelState.AddModelError("", "File size Should Be UpTo 5MB");
                                }
                            }
                        }

                    }
                }



                //if (Convert.ToDateTime(item.ClosingDate) <= DateTime.Now)
                //{
                //    ModelState.AddModelError("", "Closing date cannot be less or equals to Today's Date");
                //    TempData["Warning"] = "Closing date cannot be less or equals to Today's Date";
                //    return RedirectToAction("VacancyList", new { id = userid });
                //}

                ViewBag.ClosingDate = item.ClosingDate;

                var orgID = _dal.GetOrganisationID(userid);
                var VacancyProfileID = _dal.GetVacancyProfileID(item.JobTitleID);
                ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
                ViewBag.Organisation = _dal.GetOrganisationList(userid);
                ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
                ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
                ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
                ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
                ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

                ViewBag.Location = _dal.GetLocationList(orgID);
                ViewBag.Division = _dal.GetDivisionList(userid);
                ViewBag.SitaDepartment = _dal.GetDepartment(userid);
                ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
                ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
                ViewBag.VacancyType = _dal.GetVacancyTypeList();
                ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
                ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
                ViewBag.SkillsList = _dal.GetSelectedSkillsPerCatergiryList(id);
                ViewBag.ApproverList = _dal.GetListOfApprovers();
                ViewBag.PublishDays = _dal.GetPublishDaysList();

                ViewBag.Gender = _dal.GetGenderList();
                ViewBag.Race = _dal.GetRaceList();
                ViewBag.SalaryType = _dal.GetSalaryTypeList();
                ViewBag.RecruiterList = _dal.GetListOfRecruiters();

                //if (VacancyQuestionID == null)
                //{
                //    ModelState.AddModelError(" ", "Please select at least one vacancy question");
                //}

                if (ModelState.IsValid)
                {
                    string DeligationReasons = string.Empty;
                    string AdditonalRequirements = string.Empty;
                    string VacancyPurpose = string.Empty;
                    string QualificationAndExperience = string.Empty;
                    string TechComps = string.Empty;
                    string Disclaimer = string.Empty;
                    string Responsibility = string.Empty;
                    string Knowledge = string.Empty;
                    string LeadComps = string.Empty;
                    string BehaveComps = string.Empty;

                    if (item.DeligationReasons != null) { DeligationReasons = this.RemoveSpecialCharacters(item.DeligationReasons); } else { DeligationReasons = string.Empty; }
                    if (item.AdditonalRequirements != null) { AdditonalRequirements = this.RemoveSpecialCharacters(item.AdditonalRequirements); } else { AdditonalRequirements = string.Empty; }
                    if (item.VacancyPurpose != null) { VacancyPurpose = this.RemoveSpecialCharacters(item.VacancyPurpose); } else { VacancyPurpose = string.Empty; }
                    if (item.QualificationAndExperience != null) { QualificationAndExperience = this.RemoveSpecialCharacters(item.QualificationAndExperience); } else { QualificationAndExperience = string.Empty; }
                    if (item.TechComps != null) { TechComps = this.RemoveSpecialCharacters(item.TechComps); } else { TechComps = string.Empty; }
                    if (item.Disclaimer != null) { Disclaimer = this.RemoveSpecialCharacters(item.Disclaimer); } else { Disclaimer = string.Empty; }
                    if (item.Responsibility != null) { Responsibility = this.RemoveSpecialCharacters(item.Responsibility); } else { Responsibility = string.Empty; }
                    if (item.Knowledge != null) { Knowledge = this.RemoveSpecialCharacters(item.Knowledge); } else { Knowledge = string.Empty; }
                    if (item.LeadComps != null) { LeadComps = this.RemoveSpecialCharacters(item.LeadComps); } else { LeadComps = string.Empty; }
                    if (item.BehaveComps != null) { BehaveComps = this.RemoveSpecialCharacters(item.BehaveComps); } else { BehaveComps = string.Empty; }

                    //if (item.DeligationReasons != null) { DeligationReasons = item.DeligationReasons; } else { DeligationReasons = string.Empty; }
                    //if (item.AdditonalRequirements != null) { AdditonalRequirements = item.AdditonalRequirements; } else { AdditonalRequirements = string.Empty; }
                    //if (item.VacancyPurpose != null) { VacancyPurpose = item.VacancyPurpose; } else { VacancyPurpose = string.Empty; }
                    //if (item.QualificationAndExperience != null) { QualificationAndExperience = item.QualificationAndExperience; } else { QualificationAndExperience = string.Empty; }
                    //if (item.TechComps != null) { TechComps = item.TechComps; } else { TechComps = string.Empty; }
                    //if (item.Disclaimer != null) { Disclaimer = item.Disclaimer; } else { Disclaimer = string.Empty; }
                    //if (item.Responsibility != null) { Responsibility = item.Responsibility; } else { Responsibility = string.Empty; }
                    //if (item.Knowledge != null) { Knowledge = item.Knowledge; } else { Knowledge = string.Empty; }
                    //if (item.LeadComps != null) { LeadComps = item.LeadComps; } else { LeadComps = string.Empty; }
                    //if (item.BehaveComps != null) { BehaveComps = item.BehaveComps; } else { BehaveComps = string.Empty; }

                    int? vacancyid = _dal.UpdateVacancy(id, userid, Convert.ToInt32(Organisation), item.BPSVacancyNo, item.DivisionID, item.DepartmentID, item.JobTitleID,
                                                        item.SalaryTypeID, item.Recruiter, item.RecruiterEmail, item.RecruiterTel, item.RecruiterUserId, item.Manager, item.GenderID, item.RaceID,
                                                        item.EmploymentTypeID, item.ContractDuration, Convert.ToDateTime(item.ClosingDate), item.NumberOfOpenings
                                                        , item.VancyTypeID, DeligationReasons,
                                                        item.Location, AdditonalRequirements, VacancyProfileID[0].VacancyProfileID,
                                                        VacancyPurpose, QualificationAndExperience, TechComps
                                                        , Disclaimer, Responsibility, Knowledge, LeadComps, BehaveComps);

                    if (vacancyid != null)
                    {
                        if (vacancyid != null)
                        {
                           
                            if (VacancyQuestionID != null)
                            {
                                string vqid = null;
                                vqid = string.Join(";", VacancyQuestionID);
                                _dal.InsertUpdateVacancyQuestion((int)vacancyid, Convert.ToString(vqid));

                            }
                            //else {

                            //    _dal.DeleteVacancyQuestion(Convert.ToInt32(vacancyid));
                            //}

                            
                            tblVacancyReAdvertise vacancyReAdvertise = new tblVacancyReAdvertise();

                            vacancyReAdvertise.ReAdvertiseID = Guid.NewGuid();

                            vacancyReAdvertise.BPSNumber = item.BPSVacancyNo;

                            vacancyReAdvertise.VacancyID = int.Parse(vacancyid.ToString());
                            _dal.UpdateVacancyStatus(1, Convert.ToInt32(id));
                            _dal.VacancyReAdvertise(vacancyReAdvertise);
                            
                        }

                        if (item.BPSVacancyNo != null)
                        {
                            String BPSNumber = item.BPSVacancyNo;

                            _dal.InsertUpdateVacancyBPSNumber((int)vacancyid, Convert.ToString(BPSNumber));

                        }

                        // create a loop that will filter any thing inside fc that starts with sk_
                        List<string> listOfSkills = new List<string>();
                        for (var i = 0; i < fc.Count; i++)
                        {
                            var name = fc.Keys[i];
                            if (name.Contains("sk_"))
                            {
                                listOfSkills.Add(fc[i]);
                            }
                        }

                        var newList = listOfSkills;
                        string fileExt = string.Empty;

                        if (item.CategoryID > 0 && listOfSkills != null)
                        {

                            string vqid = null;
                            vqid = string.Join(";", listOfSkills);
                            _dal.InsertUpdateVacancySkill((int)vacancyid, Convert.ToInt32(item.CategoryID), Convert.ToString(vqid));

                        }

                        //Loop through all the specified files
                        foreach (var file in files)
                        {
                            if (file != null)
                            {

                                if (file.ContentLength > 0)
                                {
                                    //var fileName = Path.GetFileName(file.FileName);
                                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);

                                    byte[] bytes;
                                    using (BinaryReader br = new BinaryReader(file.InputStream))
                                    {
                                        bytes = br.ReadBytes(file.ContentLength);
                                    }
                                    //string filePath = Path.GetFileName(postedFile.FileName);
                                    //string filePath = "Requisition";
                                    string filePath = fileName;

                                    string ContentType = file.ContentType;

                                    var count = _db.tblVacancyDocuments.Where(x => x.fkVacancyID == (int)vacancyid && x.sFileName == filePath).Count();
                                    fileExt = string.Empty;
                                    fileExt = System.IO.Path.GetExtension(file.FileName);
                                    //_dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                                    if (count == 0)
                                    {
                                        _dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);
                                    }
                                    else
                                    {
                                        _dal.UpdateVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                                    }

                                }
                            }

                        }

                        //if (postedFile != null && postedFile.ContentLength > 00)
                        //{
                        //    byte[] bytes;
                        //    using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                        //    {
                        //        bytes = br.ReadBytes(postedFile.ContentLength);
                        //    }
                        //    //string filePath = Path.GetFileName(postedFile.FileName);
                        //    string filePath = "Requisition";
                        //    fileExt = string.Empty;
                        //    fileExt = System.IO.Path.GetExtension(postedFile.FileName);
                        //    string ContentType = postedFile.ContentType;
                        //    var count = _db.tblVacancyDocuments.Where(x => x.fkVacancyID == id && x.sFileName == filePath).Count();
                        //    if (count == 0)
                        //    {
                        //        _dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);
                        //    }
                        //    else
                        //    {
                        //        _dal.UpdateVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                        //    }
                        //}

                        //if (postedBusinessCase != null && postedBusinessCase.ContentLength > 00)
                        //{
                        //    byte[] bytes;
                        //    using (BinaryReader br = new BinaryReader(postedBusinessCase.InputStream))
                        //    {
                        //        bytes = br.ReadBytes(postedBusinessCase.ContentLength);
                        //    }
                        //    //string filePath = Path.GetFileName(postedBusinessCase.FileName);
                        //    string filePath = "Business Case";
                        //    fileExt = string.Empty;
                        //    fileExt = System.IO.Path.GetExtension(postedBusinessCase.FileName);
                        //    string ContentType = postedBusinessCase.ContentType;
                        //    var count = _db.tblVacancyDocuments.Where(x => x.fkVacancyID == id && x.sFileName == filePath).Count();
                        //    if (count == 0)
                        //    {
                        //        _dal.InsertVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);
                        //    }
                        //    else
                        //    {
                        //        _dal.UpdateVacancyDocument((int)vacancyid, filePath, bytes, ContentType, fileExt);

                        //    }
                        //}

                        TempData["message"] = "Vacancy Updated Successfully";

                    }

                    return RedirectToAction("VacancyList", new { id = userid });
                }
            }
            catch (Exception ex)
            {


                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");

            }


            return View();
        }




        //Department By Division
        [AllowAnonymous]
        [HttpPost]
        public ActionResult GetDepartmentList(int id)
        {
            return Json(_dal.GetDepartmentListByDivision(id), JsonRequestBehavior.AllowGet);
        }

        //Get Job Profile
        [Authorize]
        [HttpPost]
        public ActionResult GetJobProfileList(int id)
        {
            return Json(_dal.GetJobProfileList(id), JsonRequestBehavior.AllowGet);
        }


        //public JsonResult GetJobProfileList(int id)
        //{
        //    return Json(_dal.GetJobProfileList(id), JsonRequestBehavior.AllowGet);
        //}



        //Get Skills Per Catergory
        [Authorize]
        [HttpPost]
        public ActionResult GetSkillPerCatergory(int id)
        {
            return Json(_dal.GetSkillsPerCatergiryList(id));
        }

        //Get All Approved Vacancy
        [Authorize]
        public ActionResult ApproveVacancy()
        {
            string userid = User.Identity.GetUserId();
            ViewBag.ApprovedVacancy = _dal.GetApprovedVacancyList(userid);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ViewForApproval(int id)
        {
            string userid = User.Identity.GetUserId();
            var orgID = _dal.GetOrganisationID(userid);

            ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
            ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
            ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
            ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
            ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

            ViewBag.Division = _dal.GetDivisionList(userid);
            ViewBag.SitaDepartment = _dal.GetDepartment(userid);
            ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            ViewBag.VacancyType = _dal.GetVacancyTypeList();
            ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
            ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
            ViewBag.SkillsList = _dal.GetSelectedSkillsPerCatergiryList(id);
            ViewBag.RejectReasonList = _dal.GetRejectReasonList();
            ViewBag.ApproverList = _dal.GetListOfApprovers();
            ViewBag.PublishDays = _dal.GetPublishDaysList();
            ViewBag.Location = _dal.GetLocationList(orgID);

            ViewBag.Gender = _dal.GetGenderList();
            ViewBag.Race = _dal.GetRaceList();
            ViewBag.SalaryType = _dal.GetSalaryTypeList();
            ViewBag.RecruiterList = _dal.GetListOfRecruiters();

            var p = _dal.GetVacancyForApproval(id);

            ViewBag.Vacancy = p;
            return View(p);
        }

        //Approve Vacancy
        [HttpPost, ValidateInput(false)]
        public ActionResult Approve(VacancyModels item, FormCollection fc, int id)
        {

            try
            {
                string Organisation = fc["Organisation"];
                var VacancyProfileID = _dal.GetVacancyProfileID(item.JobTitleID);
                var ReferenceNo = _dal.GetVacancyRefNo(id);
                //3 is Approved

                StringBuilder rbuilder = new StringBuilder();

                var RecruiterEmail = _db.tblVacancies.Where(x => x.ID == id).Select(x => x.RecruiterEmail).FirstOrDefault();
                var managerEmail = _db.tblVacancies.Where(x => x.ID == id).Select(x => x.Manager).FirstOrDefault();
                var UserID = _db.tblVacancies.Where(x => x.ID == id).Select(x => x.UserID).FirstOrDefault();

                var info = (from a in _db.tblVacancies
                            join b in _db.tblProfiles on a.UserID equals b.UserID
                            //join c in _db.tblVacancyHistories on a.UserID equals c.UserID
                            join d in _db.lutJobTitles on a.JobTitleID equals d.JobTitleID

                            where a.ID == id
                            select new
                            {
                                b.EmailAddress
                                    ,
                                b.Surname
                                    ,
                                b.FirstName
                                    ,
                                a.ReferenceNo
                                    ,
                                d.JobTitle
                            }).FirstOrDefault();

                var recruiterinfo = (from a in _db.tblVacancies
                                     join b in _db.tblProfiles on a.UserID equals b.UserID
                                     //join c in _db.tblVacancyHistories on a.UserID equals c.UserID
                                     join d in _db.lutJobTitles on a.JobTitleID equals d.JobTitleID
                                     join e in _db.lutSalaryStructures on d.JobTitleID equals e.JobTitleID
                                     join f in _db.lutJobLevels on e.JobLevelID equals f.JobLevelID
                                     where a.ID == id
                                     select new
                                     {
                                         b.EmailAddress
                                             ,
                                         b.Surname
                                             ,
                                         b.FirstName
                                             ,
                                         a.ReferenceNo
                                             ,
                                         d.JobTitle
                                             ,
                                         f.JobLevelName
                                     }).FirstOrDefault();

                _dal.UpdateVacancyStatus(3, Convert.ToInt32(id));
                /* int StatusID = 3;

                 _dal.InsertUpdateVacancyHistory(id, UserID, ReferenceNo[0].ReferenceNo, Convert.ToInt32(Organisation), item.BPSVacancyNo, item.DivisionID, item.DepartmentID, item.JobTitleID, 
                                                    item.SalaryTypeID, item.CategoryDescr, item.Descr, item.JobLevelName, item.MinValue, item.MaxValue, item.Recruiter,
                                                    item.RecruiterEmail, item.RecruiterTel, item.RecruiterUserId, item.Manager, item.GenderID, item.RaceID, item.EmploymentTypeID, item.ContractDuration, 
                                                    Convert.ToDateTime(item.ClosingDate), item.NumberOfOpenings, item.VancyTypeID, item.DeligationReasons, item.Location, StatusID, VacancyProfileID[0].VacancyProfileID, item.VacancyPurpose, 
                                                    item.Responsibility,  item.QualificationAndExperience, item.TechnicalCompetenciesDescription, item.AdditonalRequirements, item.Disclaimer);*/
                //if (info != null)
                //{
                //    rbuilder = new StringBuilder();
                //    rbuilder.Append("Dear : <b>" + info.FirstName + " " + info.Surname + "</b>" + "<br/>");
                //    rbuilder.AppendLine("Vacancy Name: " + info.JobTitle + ", Reference No: " + info.ReferenceNo + "<br/>");
                //    rbuilder.Append(" has been approved and it is now available for applicants.");
                //    rbuilder.Append("");
                //    rbuilder.Append("<br/>");
                //    rbuilder.Append("<br/>Please log on to " + System.Configuration.ConfigurationManager.AppSettings["LogOutURL"] + " to access the e-Recruitment system and view the published job advertisement.");
                //    rbuilder.Append("<br/><b><i>Please note: This e-mail was sent from a notification-only address that cannot accept incoming e-mail. Please do not reply to this message.</i></b>");
                //    rbuilder.Append("<br/>Kind Regards<br/>E-Recruitment Team");
                //    string aprovedemail = rbuilder.ToString();
                //    notify.SendEmail(RecruiterEmail, "e-Recruitment Notification", aprovedemail);
                //}
                if (recruiterinfo != null)
                {
                    rbuilder = new StringBuilder();
                    rbuilder.Append("Dear : <b>" + recruiterinfo.FirstName + " " + recruiterinfo.Surname + "</b>" + "<br/>");
                    rbuilder.AppendLine("Vacancy for the position of " + recruiterinfo.JobTitle + ", " + recruiterinfo.JobLevelName + ", Reference No: " + recruiterinfo.ReferenceNo);
                    rbuilder.Append(" has been approved and it is now available for applicants.");
                    rbuilder.Append("");
                    rbuilder.Append("<br/>");
                    rbuilder.Append("<br/>Please log on to " + System.Configuration.ConfigurationManager.AppSettings["LogOutURL"] + " to access the e-Recruitment system and view the published job advertisement. <br/>");
                    rbuilder.Append("<br/><b><i>Please note: This e-mail was sent from a notification-only address that cannot accept incoming e-mail. Please do not reply to this message.</i></b>");
                    rbuilder.Append("<br/>Kind Regards<br/>E-Recruitment Team");
                    string recruitermail = rbuilder.ToString();
                    notify.SendEmail(recruiterinfo.EmailAddress, "e-Recruitment Notification", recruitermail);
                }

                //string cell = "0739495155";
                //notify.SendSMS(cell, "test");
                TempData["message"] = "Vacancy Approved Successfully";
            }
            catch (Exception ex)
            {

                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");
            }
           
            return RedirectToAction("ApproveVacancy", new { id = User.Identity.GetUserId() });
        }

        //Submit For Approvals
        public ActionResult SubmitForApprovals(string id)
        {
            try
            {
                string uid = User.Identity.GetUserId();

                //var UsID = _db.AspNetUserRoles.Where(s => s.RoleId == "2").ToList().Select(x => x.UserId).FirstOrDefault();
                //var Aemail = _db.AspNetUsers.Where(h => h.Id == UsID).ToList().Select(f => f.Email).FirstOrDefault();

                //string ApproverEmail = Aemail;

                //2 is for Submit For Approval
                //_dal.UpdateVacancyStatus(2, Convert.ToInt32(id));

                if (_dal.UpdateVacancyStatus(2, Convert.ToInt32(id)) == true)
                {
                    StringBuilder rbuilder = new StringBuilder();
                    string FullName = null;
                    string VacancyUploaded = null;
                    string ReferenceNo = null;
                    string email = null;
                    string JobLevelName = string.Empty;

                    var Approver = from a in _db.tblVacancies
                                   join b in _db.AspNetUsers on a.Manager equals b.Id
                                   join c in _db.tblProfiles on a.Manager equals c.UserID
                                   join d in _db.AspNetUserRoles on a.Manager equals d.UserId
                                   join e in _db.lutJobTitles on a.JobTitleID equals e.JobTitleID
                                   join f in _db.lutSalaryStructures on e.JobTitleID equals f.JobTitleID
                                   join g in _db.lutJobLevels on f.JobLevelID equals g.JobLevelID
                                   where a.ID == Convert.ToInt32(id)
                                   select new
                                   {
                                       FullName = c.FirstName + " " + c.Surname,
                                       email = b.Email,
                                       a.ReferenceNo,
                                       VacancyUploaded = e.JobTitle,
                                       JobLevelName = g.JobLevelName
                                   };
                    foreach (var d in Approver)
                    {
                        FullName = d.FullName;
                        VacancyUploaded = d.VacancyUploaded;
                        ReferenceNo = d.ReferenceNo;
                        email = d.email;
                        JobLevelName = d.JobLevelName;
                    }

                    rbuilder.Append("Dear : <b>" + FullName + "</b>" + "<br/>");
                    rbuilder.AppendLine();
                    rbuilder.Append("<br/>Vacancy for <b>" + string.Format("{0},{1},{2}", VacancyUploaded, JobLevelName, ReferenceNo) + "</b> requires your action.");
                    rbuilder.Append("<br/>");
                    rbuilder.Append("<br/>Please log on to " + System.Configuration.ConfigurationManager.AppSettings["LogOutURL"] + " to access the e-Recruitment system and view for further details.");
                    rbuilder.Append("<br/><br/><b><i>Please note: This e-mail was sent from a notification-only address that cannot accept incoming e-mail. Please do not reply to this message.</i></b>");
                    rbuilder.Append("<br/><br/>Kind Regards<br/>E-Recruitment Team");
                    string candidateEmail = rbuilder.ToString();

                    notify.SendEmail(email, "e-Recruitment Notification", candidateEmail);

                    TempData["message"] = "Vacancy Successfully Submitted For Approval";

                }
            }
            catch (Exception ex)
            {

                Error_Log error_log = new Error_Log();
                error_log.setFunctionName(this.ControllerContext);

                ErrorLog errorLog = new ErrorLog()
                {
                    ID = Guid.NewGuid(),
                    Function_Name = error_log.functionName,
                    DateSubmitted = DateTime.Now,
                    Message = ex.Message,
                    Application_Type = System.Configuration.ConfigurationManager.AppSettings["ApplicationType"],
                    IS_API = false

                };

                _dal.saveErrorLog(errorLog);
                return View("Error");
            }
           
            return RedirectToAction("VacancyList", new { id = User.Identity.GetUserId() });
        }

        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
            var doc = _db.tblVacancyDocuments.Where(x => x.id == id).FirstOrDefault();
            return File(doc.FileData.ToArray(), doc.ContentType, doc.sFileName + doc.FileExtension);
        }

        public ActionResult DeleteFile(int id, int VacancyId)
        {
            _db.sp_DeleteUploadedDocument(id);
            return RedirectToAction("EditVacancy", new { id = VacancyId });
        }

        //Reject Vacancy
        public ActionResult RejectVacancy(FormCollection fc)
        {
            string userid = User.Identity.GetUserId();
            string reason = fc["RejectReason"];
            int id = Convert.ToInt32(fc["hdnVacancyID"]);
            int rejectReasonID = Convert.ToInt32(fc["RejectReasonID"]);

            if (reason == "" || reason == " " || reason == null)
            {
                var newReason = (from a in _db.lutRejectReasons

                                 where a.RejectReasonID == rejectReasonID
                                 select new
                                 {
                                     a.RejectReason

                                 }).FirstOrDefault();
                reason = newReason.RejectReason;
            }

            _dal.UpdateRejectionReason(reason, id, userid, rejectReasonID);

            StringBuilder rbuilder = new StringBuilder();

            var data = _db.tblVacancies.Where(x => x.ID == id).FirstOrDefault();
            var info = (from a in _db.tblVacancies
                        join b in _db.tblProfiles on a.UserID equals b.UserID
                        join c in _db.lutJobTitles on a.JobTitleID equals c.JobTitleID
                        join d in _db.lutSalaryStructures on c.JobTitleID equals d.JobTitleID
                        join e in _db.lutJobLevels on d.JobLevelID equals e.JobLevelID

                        where a.ID == id
                        select new
                        {
                            b.EmailAddress
                                ,
                            b.Surname
                                ,
                            b.FirstName
                                ,
                            a.ReferenceNo
                                ,
                            c.JobTitle,
                            e.JobLevelName
                        }).FirstOrDefault();


            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append("<ul><li>" + reason + "</li></ul>");

            string rejectreason = sBuilder.ToString();

            _dal.UpdateVacancyStatus(4, Convert.ToInt32(id));

            rbuilder.Append("Dear : <b>" + info.FirstName + " " + info.Surname + "</b>" + "<br/>");

            rbuilder.AppendLine();

            rbuilder.Append("Your Vacancy " + string.Format("{0},{1},{2}", info.JobTitle, info.JobLevelName, info.ReferenceNo) + " has been rejected with the following reason: <br/><br/>" + rejectreason + "<br/>");

            rbuilder.Append("<br/>Please log on to " + System.Configuration.ConfigurationManager.AppSettings["LogOutURL"] + " to access the e-Recruitment system and view the published job advertisement.");
            rbuilder.Append("<br/><b><i>Please note: This e-mail was sent from a notification-only address that cannot accept incoming e-mail. Please do not reply to this message.</i></b>");
            rbuilder.Append("<br/>Kind Regards<br/>E-Recruitment Team");
            string aprovedemail = rbuilder.ToString();

            notify.SendEmail(data.RecruiterEmail, "e-Recruitment Notification", aprovedemail);

            TempData["message"] = "Vacancy Successfully Rejected";

            return RedirectToAction("ApproveVacancy", new { id = User.Identity.GetUserId() });
        }

        private string RemoveSpecialCharacters(string value)
        {
            //var data = _db.SpecialCharacters.ToList();
            //foreach (var d in data)
            //{
            //    value = Convert.ToString(MvcHtmlString.Create(HttpUtility.HtmlEncode(Regex.Replace(value, d.SC_Unicode, d.SC_UnicodeReplacementValue, RegexOptions.IgnoreCase))));
            //}
            //return value;
            //return Regex.Replace(value, @"[^0-9A-Za-z ,]", " ").Replace("&#61623", ".").Replace("  61623", ".").Replace("&#61553", ".").Replace("  61553", ".").Replace(" ", " ").Trim();
            //return Regex.Replace(value, @"[^0-9A-Za-z ,]", ".").Replace("\u0095", ".").Replace("\u0092", "'").Trim();
            return value.Replace("\u0095", ".").Replace("\u0092", "'").Replace("\u0096", "-").Replace("•", ".").Replace("amp;amp;amp;", "").Replace("&#39;", "'").Replace("&amp;amp;amp;", "&").Replace("‘", "'").Replace("’", "'").Replace("#39", "'").Replace("&quot;", "\"").Replace("&lt", "<").Replace("&gt", ">").Replace("&amp", "&").Trim();
        }


        //View Rejection Reason
        [Authorize]
        [HttpGet]
        public ActionResult ViewRejectionReason(int id)
        {
            string userid = User.Identity.GetUserId();
            var orgID = _dal.GetOrganisationID(userid);

            ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
            ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
            ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
            ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
            ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

            ViewBag.Division = _dal.GetDivisionList(userid);
            ViewBag.SitaDepartment = _dal.GetDepartment(userid);
            ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            ViewBag.VacancyType = _dal.GetVacancyTypeList();
            ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
            ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
            ViewBag.SkillsList = _dal.GetSelectedSkillsPerCatergiryList(id);
            ViewBag.RejectReasonList = _dal.GetRejectReasonList();
            ViewBag.ApproverList = _dal.GetListOfApprovers();
            ViewBag.PublishDays = _dal.GetPublishDaysList();
            ViewBag.Location = _dal.GetLocationList(orgID);

            ViewBag.Gender = _dal.GetGenderList();
            ViewBag.Race = _dal.GetRaceList();
            ViewBag.SalaryType = _dal.GetSalaryTypeList();
            ViewBag.RecruiterList = _dal.GetListOfRecruiters();

            var p = _dal.GetVacancyForApproval(id);
            ViewBag.Vacancy = p;
            return View(p);
        }
        //Withdraw vacancy
        public ActionResult WithdrawVacancy(FormCollection fc)
        {
            int withdrawalReasonID = int.Parse(fc["WithdrawalReasonID"]);
            string reason = fc["WithdrawalReason"];
            int id = Convert.ToInt32(fc["hdnVacancyID"]);
            _dal.UpdateWithdrawalReason(reason, id, withdrawalReasonID);
            return RedirectToAction("VacancyList", new { id = User.Identity.GetUserId() });
        }

        //Retract vacancy
        public ActionResult RetractVacancy(FormCollection hc)
        {
            StringBuilder rbuilder = new StringBuilder();
            int retractReasonID = int.Parse(hc["RetractReasonID"]);
            string reason = hc["RetractReason"];
            int id = Convert.ToInt32(hc["hdnVacancyIDFor"]);
            _dal.UpdateRetractReason(reason, id, retractReasonID);


            var candidateInfo = (from a in _db.tblCandidateVacancyApplications
                                 join b in _db.tblProfiles on a.UserID equals b.UserID
                                 join c in _db.tblVacancies on a.VacancyID equals c.ID
                                 join d in _db.lutJobTitles on c.JobTitleID equals d.JobTitleID
                                 join e in _db.lutSalaryStructures on d.JobTitleID equals e.JobTitleID
                                 join f in _db.lutJobLevels on e.JobLevelID equals f.JobLevelID
                                 where a.VacancyID == id
                                 select new { a.UserID, b.EmailAddress, b.FirstName, b.Surname, b.CellNo, d.JobTitle, c.ReferenceNo, f.JobLevelName }).ToList();

            foreach (var data in candidateInfo)
            {

                rbuilder = new StringBuilder();
                rbuilder.Append("Dear : <b>" + data.FirstName + " " + data.Surname + "</b>" + "<br/>");
                rbuilder.AppendLine("Please note the position " + data.JobTitle + ", " + data.JobLevelName + ", Reference No: " + data.ReferenceNo + "<br/>");
                rbuilder.Append(" has been retracted and should the position be re-advertised your application will be considered.");
                rbuilder.Append("");
                rbuilder.Append("<br/>");
                rbuilder.Append("<br/><b><i>Please note: This e-mail was sent from a notification-only address that cannot accept incoming e-mail. Please do not reply to this message.</i></b>");
                rbuilder.Append("<br/>Kind Regards<br/>E-Recruitment Team");
                string candidateEmailText = rbuilder.ToString();
                notify.SendEmail(data.EmailAddress, "e-Recruitment Notification", candidateEmailText);

            }
            var recruiterInfo = (from a in _db.AspNetUserRoles
                                 join b in _db.lutOrganisations on a.OrganisationID equals b.OrganisationID
                                 join c in _db.AspNetRoles on a.RoleId equals c.Id
                                 join d in _db.tblProfiles on a.UserId equals d.UserID
                                 join e in _db.AssignedDivisionDepartments on a.UserId equals e.UserId
                                 join f in _db.tblVacancies on d.UserID equals f.Manager
                                 join g in _db.lutJobTitles on f.JobTitleID equals g.JobTitleID
                                 join h in _db.lutSalaryStructures on g.JobTitleID equals h.JobTitleID
                                 join i in _db.lutJobLevels on h.JobLevelID equals i.JobLevelID
                                 where Convert.ToInt32(c.Id) == 2 && f.ID == id
                                 orderby d.FirstName ascending
                                 select new
                                 {
                                     RoleName = c.Name,
                                     ApproverUserIdUserId = d.UserID,
                                     Fullname = d.FirstName + " " + d.Surname,
                                     JobTitle = g.JobTitle,
                                     JobLevelName = i.JobLevelName,
                                     ReferenceNo = f.ReferenceNo,
                                     EmailAddress = d.EmailAddress
                                 }).Distinct().OrderBy(x => x.Fullname).ToList();

            foreach (var rInfo in recruiterInfo)
            {


                rbuilder = new StringBuilder();
                rbuilder.Append("Dear : <b>" + rInfo.Fullname + "</b>" + "<br/>");
                rbuilder.AppendLine("Please note the position " + rInfo.JobTitle + ", " + rInfo.JobLevelName + ", Reference No: " + rInfo.ReferenceNo + "<br/>");
                rbuilder.Append(" has been retracted and should the position be re-advertised your application will be considered.");
                rbuilder.Append("");
                rbuilder.Append("<br/>");
                rbuilder.Append("<br/><b><i>Please note: This e-mail was sent from a notification-only address that cannot accept incoming e-mail. Please do not reply to this message.</i></b>");
                rbuilder.Append("<br/>Kind Regards<br/>E-Recruitment Team");
                string approverEmailText = rbuilder.ToString();
                notify.SendEmail(rInfo.EmailAddress, "e-Recruitment Notification", approverEmailText);

            }


            return RedirectToAction("VacancyList", new { id = User.Identity.GetUserId() });

        }

        //[HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExtendVacancyClosingDate(FormCollection hc, int id, VacancyModels model)
        //{
        //    //ViewBag.Vacancy = _dal.GetVacancyListForEditByID(Convert.ToInt32(hc["hdnVacancyIDFor"])).FirstOrDefault();

        //    DateTime closingDate = DateTime.Parse(model.ClosingDate);
        //    if (Convert.ToDateTime(closingDate) <= DateTime.Now)
        //    {
        //        ModelState.AddModelError("", "Closing date cannot be less or equals to Today's Date");
        //        TempData["Warning"] = "Closing date cannot be less or equals to Today's Date";
        //        return RedirectToAction("VacancyList", new { id = User.Identity.GetUserId()});
        //    }



        //    //string reason = hc["RetractReason"];
        //    //int id = Convert.
        //    (hc["VacancyID"]);
        //    _db.sp_ExtendClosingate(id, closingDate);
        //    return RedirectToAction("VacancyList", new { id = User.Identity.GetUserId() });

        //}

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ExtendVacancyClosingDate(HttpPostedFileBase postedFile, HttpPostedFileBase postedBusinessCase, VacancyModels item, FormCollection fc, int id, string[] VacancyQuestionID, IEnumerable<HttpPostedFileBase> files)
        {
            string Organisation = fc["Organisation"];
            string Recruiter = fc["FullName"];
            string RecruiterEmail = fc["EmailAddress"];
            string userid = User.Identity.GetUserId();




            if (Convert.ToDateTime(item.ClosingDate) <= DateTime.Now)
            {
                ModelState.AddModelError("", "Closing date cannot be less or equals to Today's Date");
                TempData["Warning"] = "Closing date cannot be less or equals to Today's Date";
                return RedirectToAction("VacancyList", new { id = userid });
            }

            ViewBag.ClosingDate = item.ClosingDate;



            _db.sp_ExtendClosingate(id, Convert.ToDateTime(item.ClosingDate));


            TempData["message"] = "Date Updated Successfully";

            return RedirectToAction("VacancyList", new { id = userid });

        }


        [Authorize]
        [HttpGet]
        public ActionResult ExtendVacancyClosingDate(int id)
        {
            //ViewBag.Vacancy = _dal.GetVacancyListForEditByID(Convert.ToInt32(hc["hdnVacancyIDFor"])).FirstOrDefault();

            string userid = User.Identity.GetUserId();
            var orgID = _dal.GetOrganisationID(userid);

            ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
            ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
            ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
            ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
            ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

            ViewBag.Location = _dal.GetLocationList(orgID);
            ViewBag.Division = _dal.GetDivisionList(userid);
            ViewBag.SitaDepartment = _dal.GetDepartment(userid);
            ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            ViewBag.VacancyType = _dal.GetVacancyTypeList();
            ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
            ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
            ViewBag.SkillsList = _dal.GetSelectedSkillsPerCatergiryList(id);
            ViewBag.ApproverList = _dal.GetListOfApprovers();
            ViewBag.PublishDays = _dal.GetPublishDaysList();

            ViewBag.Gender = _dal.GetGenderList();
            ViewBag.Race = _dal.GetRaceList();
            ViewBag.SalaryType = _dal.GetSalaryTypeList();
            ViewBag.RecruiterList = _dal.GetListOfRecruiters();

            var p = _dal.GetVacancyListForEditByID(id);

            // convert the closeDate inside the "p" object to a string then reformat it to a html format
            //var newDate = p[0].ClosingDate.ToString().Substring(0, 9);
            //p[0].ClosingDate = newDate;

            return View(p);

        }

        //Vacancy screening
        [HttpGet]
        public ActionResult CandidateScreening()
        {
            string userid = User.Identity.GetUserId();

            ViewBag.ProvinceList = _dal.GetProvinceList();
            ViewBag.GenderList = _dal.GetGenderList();
            ViewBag.RaceList = _dal.GetRaceList();
            ViewBag.Vacancy = _dal.GetVacancyListForScreening(userid);
            return View();
        }

        public ActionResult ExportToExel(FormCollection fc, string[] VacancyQuestionID)
        {
            //if (VacancyQuestionID == null)
            //{
            //    ViewBag.ErrorMessage = "You must select one of the Question Banks provided!";
            //    ModelState.AddModelError("", "You must select one of the Question Banks provided");
            //}

            string VacancyQuestionBank = null;
            string vacancyID = fc["VacancyID"];
            string provinceID = fc["ProvinceID"];
            int ageRange = int.Parse(fc["AgeRange"]);
            string genderID = fc["GenderID"];
            string raceID = fc["RaceID"];
            int professioonallyRegisteredID = fc["professioonallyRegistered"] != null ? 1 : 0;
            int previouslyEmployedPSID = fc["previouslyEmployedPS"] != null ? 1 : 0;
            int attachedCV = fc["chkWithAttachedCV"] != null ? 1 : 0;
            int attachedID = fc["chkWithAttachedID"] != null ? 1 : 0;
            int withDisabilities = fc["chkWithDisabilities"] != null ? 1 : 0;
            int matricCompleted = fc["chkMatricCompleted"] != null ? 1 : 0;
            int driversLicence = fc["chkDriversLicence"] != null ? 1 : 0;

            // create a loop that will filter any thing inside fc that starts with sk_
            List<string> listVacancyQuestion = new List<string>();
            for (var i = 0; i < fc.Count; i++)
            {
                var name = fc.Keys[i];
                if (name.Contains("sk_"))
                {
                    listVacancyQuestion.Add(fc[i]);
                }
            }

            var newList = listVacancyQuestion;

            if (listVacancyQuestion != null)
            {
                VacancyQuestionBank = string.Join(";", listVacancyQuestion);
            }
            else
            {
                VacancyQuestionBank = null;
            }

            if (ModelState.IsValid)
            {

            }
            int id = int.Parse(vacancyID);

            DataTable dt = new DataTable();
            string vQuestions = string.Empty;

            dt = _dal.GeteRecruitmentScreenedCandidateList(vacancyID, provinceID, genderID
                    , raceID, withDisabilities, attachedCV, attachedID, ageRange
                    , VacancyQuestionBank, professioonallyRegisteredID, previouslyEmployedPSID, matricCompleted, driversLicence);
            
           

            vQuestions = _dal.GetKillerQuestionByVIDQBanks(Convert.ToInt32(vacancyID), VacancyQuestionBank);

            var vName = (from a in _db.tblVacancies
                         join c in _db.lutJobTitles on a.JobTitleID equals c.JobTitleID
                         where a.ID == id
                         select new
                         {
                             VacancyName = c.JobTitle + " - (" + a.ReferenceNo + ")",
                         }).FirstOrDefault();

            //_dal.GetScreenedCandidateList(vacancyID, provinceID, genderID, raceID, withDisabilities, attachedCV, attachedID, ageRange, VacancyQuestionBank, professioonallyRegisteredID, previouslyEmployedPSID);


            string provinceName = null;
            string AgeRange = null;
            string gender = null;
            string race = null;
            string professioonallyRegistered = null;
            string previouslyEmployedPS = null;
            string AttachedCV = null;
            string AttachedID = null;
            string WithDisabilities = null;
            string WithMatric = null;
            string WithDriversLicence = null;

            //var vName = _db.tblVacancies.Where(x => x.ID == vid).FirstOrDefault();
            //string VancancyName = vName.VacancyName + " - ( " + vName.ReferenceNo + ")";

            switch (provinceID)
            {
                case "10":
                    provinceName = "All";
                    break;
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    int pid = int.Parse(provinceID);
                    provinceName = _db.lutProvinces.Where(x => x.ProvinceID == pid).Select(x => x.ProvinceName).FirstOrDefault();
                    break;
                default:
                    break;
            }

            switch (genderID)
            {
                case "3":
                    gender = "All";
                    break;
                case "1":
                case "2":
                    int pid = int.Parse(genderID);
                    gender = _db.lutGenders.Where(x => x.GenderID == pid).Select(x => x.Gender).FirstOrDefault();
                    break;
                default:
                    break;
            }

            switch (raceID)
            {
                case "6":
                    race = "All";
                    break;
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                    int pid = int.Parse(raceID);
                    race = _db.lutRaces.Where(x => x.RaceID == pid).Select(x => x.RaceName).FirstOrDefault();
                    break;
                default:
                    break;
            }

            switch (professioonallyRegisteredID)
            {
                case 0:
                    professioonallyRegistered = "Not Applicable";
                    break;
                case 1:
                    professioonallyRegistered = "Yes";
                    break;
                default:
                    break;
            }

            switch (previouslyEmployedPSID)
            {
                case 0:
                    previouslyEmployedPS = "Not Applicable";
                    break;
                case 1:
                    previouslyEmployedPS = "Yes";
                    break;
                default:
                    break;

                    //case "1":
                    //    previouslyEmployedPS = "Yes";
                    //    break;
                    //case "2":
                    //    previouslyEmployedPS = "No";
                    //    break;
                    //default:
                    //    break;
            }

            switch (ageRange)
            {
                case 1:
                    AgeRange = "All";
                    break;
                case 2:
                    AgeRange = "18 - 25";
                    break;
                case 3:
                    AgeRange = "25 - 35";
                    break;
                case 4:
                    AgeRange = "18 - 59";
                    break;
                default:
                    break;
            }

            switch (attachedCV)
            {
                case 0:
                    AttachedCV = "Not Applicable";
                    break;
                case 1:
                    AttachedCV = "Yes";
                    break;
                default:
                    break;
            }

            switch (attachedID)
            {
                case 0:
                    AttachedID = "Not Applicable";
                    break;
                case 1:
                    AttachedID = "Yes";
                    break;
                default:
                    break;
            }


            switch (withDisabilities)
            {
                case 0:
                    WithDisabilities = "Not Applicable";
                    break;
                case 1:
                    WithDisabilities = "Yes";
                    break;
                default:
                    break;

            }

            switch (matricCompleted)
            {
                case 0:
                    WithMatric = "Not Applicable";
                    break;
                case 1:
                    WithMatric = "Yes";
                    break;
                default:
                    break;
            }

            switch (driversLicence)
            {
                case 0:
                    WithDriversLicence = "Not Applicable";
                    break;
                case 1:
                    WithDriversLicence = "Yes";
                    break;
                default:
                    break;
            }

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ScreenedCandidateList");

            //ws.Cell(6, 1).Value = "Application";
            ws.Cell(3, 2).Value = "Gender";
            ws.Cell(3, 3).Value = "Race";
            ws.Cell(3, 4).Value = "Province";
            ws.Cell(3, 5).Value = "Age Range";
            ws.Cell(3, 6).Value = "Disability";
            ws.Cell(3, 7).Value = "Attached CV";
            ws.Cell(3, 8).Value = "Attached ID";
            ws.Cell(3, 9).Value = "Professionally registered";
            ws.Cell(3, 10).Value = "Previously employed in the Public Service";
            ws.Cell(3, 11).Value = "Matric or equivalent";
            ws.Cell(3, 12).Value = "Valid Driver's Licence";

            ws.Cell(4, 1).Value = "Selected Criteria";
            ws.Cell(5, 3).Value = "Selected Vacancy Question/s";

            //Insert
            ws.Cell(2, 3).Value = vName.VacancyName;
            ws.Cell(4, 2).Value = gender;
            ws.Cell(4, 3).Value = race;
            ws.Cell(4, 4).Value = provinceName;
            ws.Cell(4, 5).Value = AgeRange;
            ws.Cell(4, 6).Value = WithDisabilities;
            ws.Cell(4, 7).Value = AttachedCV;
            ws.Cell(4, 8).Value = AttachedID;
            ws.Cell(4, 9).Value = professioonallyRegistered;
            ws.Cell(4, 10).Value = previouslyEmployedPS;
            ws.Cell(4, 11).Value = WithMatric;
            ws.Cell(4, 12).Value = WithDriversLicence;

            ws.Cell(5, 5).Value = vQuestions;

            //*****************************************************************

            //Populate the Data Dynamiccaly, Start with Column Headings
            //I am not sure how many columns will be returned as the query generates them dynamiccally
            int colCnt = dt.Columns.Count;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string colName = string.Empty;
                string NewColName = string.Empty;

                if (dt.Columns[i].ColumnName.ToString().Contains("WorkHistory"))
                {
                    int ColNumber = ExtractNumbersFromString(dt.Columns[i].ColumnName.ToString());
                    int newWHColNum = ResetWorkHistoryID(ColNumber);
                    if (ColNumber % 2 == 0)
                    {
                        //NewColName = "WorkPeriod" + ColNumber;
                        NewColName = "WorkPeriod";
                    }
                    else
                    {
                        NewColName = "Work History" + newWHColNum;
                    }
                    colName = NewColName;
                }
                else
                {
                    colName = dt.Columns[i].ColumnName.ToString();
                }
                ws.Cell(7, i + 1).Value = colName.ToString();
            }

            ws.Range(2, 3, 2, 9).Merge().AddToNamed("Titles");
            ws.Range(5, 3, 5, 4).Merge().AddToNamed("Titles");

            ws.Range(5, 5, 5, 12).Merge();
            ws.Range(7, 1, 7, colCnt).AddToNamed("Titles");
            ws.Range(3, 1, 3, 10).AddToNamed("Titles");
            ws.Range(2, 3, 2, 9).AddToNamed("Titles");

            // From a Database
            //var data = _dal.GetCandidateListForExport(id);
            var data = dt; //Data i got from Datatable
            var rangeWithData = ws.Cell(8, 1).InsertData(data); //Pasting the entire Data into spreadsheet

            var lasColUsed = ws.RowsUsed().CellsUsed().Max(cell => cell.Address.RowNumber);

            // Prepare the style for the titles
            var titlesStyle = wb.Style;
            titlesStyle.Font.Bold = true;
            titlesStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titlesStyle.Fill.BackgroundColor = XLColor.LightBlue;

            // Format all titles in one shot
            wb.NamedRanges.NamedRange("Titles").Ranges.Style = titlesStyle;

            ws.Columns().AdjustToContents();

            // wb.SaveAs("InsertingData.xlsx");

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", "attachment;filename=\"Screened_Candidates_" + DateTime.Now.ToShortDateString() + ".xlsx\"");
            Response.AddHeader("content-disposition", "attachment;filename=\"Screened_Candidates_" + DateTime.Now + ".xlsx\"");

            // Flush the workbook to the Response.OutputStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                memoryStream.Close();
            }

            Response.End();
            return View();
        }

        private int ResetWorkHistoryID(int value)
        {
            int ColNum = 0;
            switch (value)
            {
                case 1:
                    ColNum = 1;
                    break;
                case 3:
                    ColNum = 2;
                    break;
                case 5:
                    ColNum = 3;
                    break;
                case 7:
                    ColNum = 4;
                    break;
                case 9:
                    ColNum = 5;
                    break;
                case 11:
                    ColNum = 6;
                    break;
                case 13:
                    ColNum = 7;
                    break;
                case 15:
                    ColNum = 8;
                    break;
                case 17:
                    ColNum = 9;
                    break;
                case 19:
                    ColNum = 10;
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
            return ColNum;
        }

        private int ExtractNumbersFromString(string value)
        {
            string a = value;
            string b = string.Empty;
            int val = 0;

            for (int i = 0; i < a.Length; i++)
            {
                if (Char.IsDigit(a[i]))
                    b += a[i];
            }

            if (b.Length > 0)
                val = int.Parse(b);

            return val;
        }


        //Get Skills Per Catergory
        [Authorize]
        [HttpPost]
        public ActionResult GetQuestionBanksPerVacancy(int id)
        {
            return Json(_dal.GetQuestionBanksPerVacancy(id));
        }

        [HttpGet]
        public ActionResult
            CandidateCV()
        {
            return View();

        }

        [HttpPost]
        public FileResult DownloadCandidateCVitae(int profileID)
        {
            var data = _db.tblProfiles.Where(x => x.pkProfileID == profileID).FirstOrDefault();
            string fullname = data.FirstName + "_" + data.Surname;
            var doc = _db.Attachments.Where(x => x.ProfileID == profileID).FirstOrDefault();
            return File(doc.fileData.ToArray(), doc.contentType, fullname + "_" + doc.fileName.Replace(" ", ""));
            //return File(doc.fileData.ToArray(), doc.contentType, doc.fileName);
        }

        [HttpGet]
        public ActionResult CandidateProfile(int id)
        {
            string userid = User.Identity.GetUserId();
            int OrganisationID = _db.AspNetUserRoles.Where(x => x.UserId == userid).Select(x => x.OrganisationID).FirstOrDefault();

            ViewBag.Vacancy = _dal.GetVacancyListForScreening(userid);
            var Candidates = _dal.GetCandidateListUsingVacancyID(id);
            ViewBag.CandidateList = Candidates;
            var data = _dal.GetCandidateList(userid);
            return View(data);
        }

        [HttpPost]
        public ActionResult SeearchCandidateProfile(ScreenedCandidateModel data)
        {
            VacancyModels vacancy = new VacancyModels();

            string userid = User.Identity.GetUserId();
            int VacancyID = data.VacancyID;
            var Candidates = _dal.GetCandidateListUsingVacancyID(VacancyID);
            ViewBag.Vacancy = _dal.GetVacancyListForScreening(userid);
            vacancy = _dal.GetVacancyRef(VacancyID);

            if (Candidates.Count() == 0)
            {
                TempData["Warning"] = "No Candidates have applied for " + vacancy.ReferenceNo + " as yet.";
            }
            else
            {
                TempData["Message"] = "Search Results For " + vacancy.ReferenceNo + ".";

                return RedirectToAction("CandidateProfile", "Vacancy", new { id = VacancyID });
            }
            return RedirectToAction("CandidateProfile", "Vacancy", new { id = VacancyID });
        }

        //Candidate Profile
        [HttpGet]
        public ActionResult ViewCandidateProfile(int ID)
        {
            string userid = User.Identity.GetUserId();

            var Idnum = (from a in _db.tblCandidateVacancyApplications

                         where a.ApplicationID == ID
                         select new
                         {
                             a.IDNumber

                         }).FirstOrDefault();
            string id = Idnum.IDNumber;

            ViewBag.Citizenship = _dal.GetYesorNoList();
            ViewBag.SkillsProf = _dal.SkillsProf(userid);
            ViewBag.CommunicationMethod = _dal.GetMethodOfCommunication();

            ViewBag.Race = _dal.GetRaceList();
            ViewBag.Gender = _dal.GetGenderList();
            ViewBag.Province = _dal.GetProvinceList();
            ViewBag.YesorNo = _dal.GetYesorNoList();

            ViewBag.Country = _dal.GetCountryList();
            ViewBag.Disability = _dal.GetDisabilityList();
            ViewBag.QualificationType = _dal.GetQualificationTypeList();
            ViewBag.Language = _dal.GetLanguageList();
            ViewBag.LanguageProficiency = _dal.GetLanguageProficiencyList();
            ViewBag.SkillProficiency = _dal.GetSkillProficiencyList();
            ViewBag.Skills = _dal.GetSkillsList();

            ViewBag.CandidateEducationList = _dal.GetEducationList(ID);

            // var workHistory = new List<WorkHistoryModel>();
            var workHistory = _dal.GetWorkHistoryList(ID);


            List<WorkHistoryModel> w = new List<WorkHistoryModel>();
            //var data = _db.SpecialCharacters.ToList();

            //foreach (var a in workHistory)
            //{
            //    WorkHistoryModel h = new WorkHistoryModel();
            //    string text = a.positionHeld;
            //    //foreach(var b in data)
            //    //{
            //    //    string positionheld = Convert.ToString(MvcHtmlString.Create(HttpUtility.HtmlEncode(text.Replace(b.SC_Unicode, b.SC_UnicodeReplacementValue))));
            //    //    h.positionHeld = positionheld;
            //    //}

            //    string positionheld = Convert.ToString(MvcHtmlString.Create(HttpUtility.HtmlEncode(text.Replace("•", ".").Replace("’", "'").Replace("“", "\"").Replace("”", "\"").Replace("–", "-"))));
            //    h.positionHeld = this.RemoveSpecialCharacters(positionheld);
            //    h.jobTitle = a.jobTitle;
            //    h.previouslyEmployedDepartment = a.previouslyEmployedDepartment;
            //    h.previouslyEmployedPS = a.previouslyEmployedPS;
            //    h.reasonForLeaving = a.reasonForLeaving;
            //    h.startDate = a.startDate;
            //    h.companyName = a.companyName;
            //    h.department = a.department;
            //    h.endDate = a.endDate;
            //    h.reEmployment = a.reEmployment;
            //    h.workHistoryID = a.workHistoryID;

            //    w.Add(h);

            //}

            ViewBag.CandidateWorkHistoryList = workHistory;
            //‘~`!@#$%^&*()_-=+//*<>.”;:+-.

            ViewBag.CandidateSkillList = _dal.GetCandidateSkillList(ID);
            ViewBag.CandidateLanguageList = _dal.GetCandidateLanguageList(ID);
            ViewBag.CandidateReferenceList = _dal.GetReferenceList(ID);
            ViewBag.CandidateAttachmentList = _dal.GetCandidateAttachmentList(ID);

            var p = _dal.GetCandidateProfileInfo(ID);

            return View(p);
        }

        [HttpGet]
        public FileResult DownLoadCandidateAttachment(int attachmentID)
        {
            var data = (from a in _db.tblProfiles
                        join b in _db.Attachments
                        on a.pkProfileID equals b.ProfileID
                        where b.AttachmentID == attachmentID
                        select new { FullName = a.FirstName + '_' + a.Surname }).FirstOrDefault();

            var doc = _db.Attachments.Where(x => x.AttachmentID == attachmentID).FirstOrDefault();
            return File(doc.fileData.ToArray(), doc.contentType, data.FullName + '_' + doc.fileName.Replace(" ", "_") + doc.FileExtension);
        }

        [HttpGet]
        public ActionResult DownloadCandidateProfile(string id)
        {
            int appID = Convert.ToInt32(Request["appid"]);
            int profileId = _db.tblProfiles.Where(x => x.UserID == id).Select(x => x.pkProfileID).FirstOrDefault();
            var profileData = _dal.GetCandidateProfileInfo(appID).FirstOrDefault();
            var educationData = _dal.GetEducationList(appID).ToList();
            var workHistoryData = _dal.GetWorkHistoryList(appID);
            var skillsData = _dal.GetCandidateSkillList(appID);
            var languageData = _dal.GetCandidateLanguageList(appID);
            var referenceData = _dal.GetReferenceList(appID);

            //var data = _db.sp_GetVacancyAdDetail(id).FirstOrDefault();

            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {

                //Document document = new Document(PageSize.A4, 10, 10, 10, 10);
                Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                //Document document = new Document(new Rectangle(288f, 144f), 10, 10, 10, 10);
                //document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                Font courier = new Font(Font.FontFamily.HELVETICA, 9f);

                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                //document.Add(new Paragraph("\n"));

                Paragraph paraHead = new Paragraph("Candidate Profile of ".ToUpper() + string.Format("{0} {1}", profileData.Surname, profileData.FirstName).ToUpper());
                paraHead.Alignment = Element.ALIGN_CENTER;
                paraHead.Font = FontFactory.GetFont("dax-black", 20, Font.BOLD);
                //cellVacancyPurpose.BackgroundColor = BaseColor.LIGHT_GRAY;

                document.Add(paraHead);
                document.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(2);

                //PdfPCell cell = new PdfPCell(new Phrase("Vacancy Information Download"));
                PdfPCell cell = new PdfPCell(new Phrase("Personal Information"));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Border = 0; //Added this for testing
                table.AddCell(cell);

                PdfPCell emptyCell = new PdfPCell(new Phrase(" "));
                emptyCell.Colspan = 2;
                emptyCell.Border = 0;

                table.AddCell(emptyCell);
                table.SetWidths(new int[] { 1, 2 });
                PdfPCell cellP = null;

                cellP = new PdfPCell(new Phrase("ID Number: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.IDNumber);

                cellP = new PdfPCell(new Phrase("Passport Number: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.PassportNumber);

                cellP = new PdfPCell(new Phrase("Surname: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.Surname);

                cellP = new PdfPCell(new Phrase("First Name: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.FirstName);

                cellP = new PdfPCell(new Phrase("Date of Birth: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.DateOfBirth);

                cellP = new PdfPCell(new Phrase("Race: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.Race);

                cellP = new PdfPCell(new Phrase("Gender: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.Gender);

                cellP = new PdfPCell(new Phrase("Contact No: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.CellNo);

                cellP = new PdfPCell(new Phrase("Alternative No: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.AlternativeNo);

                cellP = new PdfPCell(new Phrase("Email Address: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.EmailAddress);

                cellP = new PdfPCell(new Phrase("Matric: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.Matric);

                cellP = new PdfPCell(new Phrase("Drivers License: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.DriversLicense);

                cellP = new PdfPCell(new Phrase("Unit No: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.UnitNo);

                cellP = new PdfPCell(new Phrase("Complex Name: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.ComplexName);

                cellP = new PdfPCell(new Phrase("Street No: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.StreetNo);

                cellP = new PdfPCell(new Phrase("Street Name: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.StreetName);

                cellP = new PdfPCell(new Phrase("Suburb Name: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.SuburbName);

                cellP = new PdfPCell(new Phrase("City: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.City);

                cellP = new PdfPCell(new Phrase("Province: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.Province);

                cellP = new PdfPCell(new Phrase("ID Number: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.PostalCode);

                if (profileData.ProfessionallyRegisteredID == 1)
                {
                    cellP = new PdfPCell(new Phrase("Are You Professionally Registered?"));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell("Are You Professionally Registered? ");
                    table.AddCell("Yes");

                    cellP = new PdfPCell(new Phrase("Registration Date: "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);

                    if (profileData.RegistrationDate.ToString().Contains("0001/01/01"))
                    {
                        table.AddCell(string.Empty);
                    }
                    else { table.AddCell(profileData.RegistrationDate.ToString()); }

                    cellP = new PdfPCell(new Phrase("Registration Number: "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    if (profileData.RegistrationNumber != null) { table.AddCell(profileData.RegistrationNumber.ToString()); } else { table.AddCell(string.Empty); }

                    cellP = new PdfPCell(new Phrase("Registration Body: "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    if (profileData.RegistrationBody != null) { table.AddCell(profileData.RegistrationBody.ToString()); } else { table.AddCell(string.Empty); }
                    //table.AddCell(profileData.RegistrationBody.ToString());
                }
                else
                {
                    cellP = new PdfPCell(new Phrase("Are You Professionally Registered? "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell("No");
                }

                cellP = new PdfPCell(new Phrase("Were you previously employed in the Public Service? "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                if (profileData.PreviouslyEmployedPS == 1)
                {
                    table.AddCell("Yes");
                }
                else
                {
                    table.AddCell("No");
                }

                if (profileData.ConditionsThatPreventsReEmploymentID == 1)
                {
                    cellP = new PdfPCell(new Phrase("Are there any conditions that prevents your re-employment?"));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell("Yes");

                    cellP = new PdfPCell(new Phrase("If Yes please specify:"));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell(profileData.ReEmployment);

                    cellP = new PdfPCell(new Phrase("Provide the name of the previous employing department:"));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell(profileData.PreviouslyEmployedDepartment);
                }
                else
                {
                    cellP = new PdfPCell(new Phrase("Are there any conditions that prevents your re-employment?"));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell("No");
                }

                if (profileData.fkDisabilityID == 1)
                {
                    cellP = new PdfPCell(new Phrase("Disability:"));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell("Yes");

                    cellP = new PdfPCell(new Phrase("Disability:"));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell(profileData.Disability);

                    //if (profileData.NatureOfDisability == 2)
                    //{
                    //    table.AddCell("Nature of Disability: ");
                    //    table.AddCell(profileData.OtherNatureOfDisability);
                    //}

                    cellP = new PdfPCell(new Phrase("Nature of Disability: "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell(profileData.OtherNatureOfDisability);

                }
                else
                {
                    cellP = new PdfPCell(new Phrase("Disability: "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell("No");
                }

                if (profileData.SACitizen == 1)
                {
                    cellP = new PdfPCell(new Phrase("RSA Citizen: "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell("Yes");
                }
                else
                {
                    cellP = new PdfPCell(new Phrase("RSA Citizen: "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell("No");
                }

                if (profileData.pkCriminalOffenseID == 1)
                {
                    cellP = new PdfPCell(new Phrase("Criminal Offence: "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell("Yes");
                }
                else
                {
                    cellP = new PdfPCell(new Phrase("Criminal Offence: "));
                    cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cellP);
                    table.AddCell("No");
                }

                string lang = _db.lutLanguages.Where(x => x.languageID == profileData.fkLanguageForCorrespondenceID).Select(x => x.LanguageName).FirstOrDefault();
                cellP = new PdfPCell(new Phrase("Preferred Language For Correspondence: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(lang);

                cellP = new PdfPCell(new Phrase("Telephone No During Working Hours: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(profileData.TelNoDuringWorkingHours);

                string commMethod = _db.lutMethodOfCommunications.Where(x => x.MethodID == profileData.MethodOfCommunicationID).Select(x => x.MethodOfCommunication).FirstOrDefault();
                cellP = new PdfPCell(new Phrase("Preferred Way Of Correspondence: "));
                cellP.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cellP);
                table.AddCell(commMethod);

                //table.AddCell("Correspondence Details: ");
                //table.AddCell(profileData.CorrespondanceDetails);


                //any  conditions exists that prevents your re-employment
                //table.AddCell("Division: ");
                //table.AddCell(data.DivisionName);

                //table.AddCell("Department: ");
                //table.AddCell(data.DepartmentName);

                //table.AddCell("Employment Type: ");
                //table.AddCell(data.EmploymentType);

                //if (data.EmploymentType != null)
                //{
                //    if (data.EmploymentType != "Permanent")
                //    {
                //        table.AddCell("Contract Duration: ");
                //        table.AddCell(data.ContractDuration);
                //    }
                //}


                //table.AddCell("Location: ");
                //table.AddCell(data.Location);

                //table.AddCell("Number Of Openings: ");
                //table.AddCell(Convert.ToString(data.NumberOfOpenings));

                table.AddCell(emptyCell);

                PdfPTable tableEducation = new PdfPTable(6);
                PdfPCell cellEducationHistory = new PdfPCell(new Phrase("Education History"));
                cellEducationHistory.Colspan = 6;
                cellEducationHistory.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                cellEducationHistory.BackgroundColor = BaseColor.GRAY;
                tableEducation.AddCell(cellEducationHistory);


                PdfPCell cellQualificationName = new PdfPCell(new Phrase("Qualification Name"));
                cellQualificationName.HorizontalAlignment = Element.ALIGN_LEFT;
                cellQualificationName.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableEducation.AddCell(cellQualificationName);

                PdfPCell cellInstitutionName = new PdfPCell(new Phrase("Institution Name"));
                cellInstitutionName.HorizontalAlignment = Element.ALIGN_LEFT;
                cellInstitutionName.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableEducation.AddCell(cellInstitutionName);

                PdfPCell cellQualificationTypeName = new PdfPCell(new Phrase("Qualification Type"));
                cellQualificationTypeName.HorizontalAlignment = Element.ALIGN_LEFT;
                cellQualificationTypeName.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableEducation.AddCell(cellQualificationTypeName);

                PdfPCell cellCertificateNumber = new PdfPCell(new Phrase("Certificate Number"));
                cellCertificateNumber.HorizontalAlignment = Element.ALIGN_LEFT;
                cellCertificateNumber.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableEducation.AddCell(cellCertificateNumber);

                PdfPCell cellEducationStartDate = new PdfPCell(new Phrase("Start Date"));
                cellEducationStartDate.HorizontalAlignment = Element.ALIGN_LEFT;
                cellEducationStartDate.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableEducation.AddCell(cellEducationStartDate);


                PdfPCell cellEducationEndDate = new PdfPCell(new Phrase("End Date"));
                cellEducationEndDate.HorizontalAlignment = Element.ALIGN_LEFT;
                cellEducationEndDate.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableEducation.AddCell(cellEducationEndDate);

                foreach (var d in educationData)
                {
                    tableEducation.AddCell(d.qualificationName);
                    tableEducation.AddCell(d.institutionName);
                    tableEducation.AddCell(d.QualificationTypeName);
                    tableEducation.AddCell(d.certificateNumber);
                    tableEducation.AddCell(d.startDate);
                    tableEducation.AddCell(d.endDate);
                }

                //table.AddCell(emptyCell);//Add Empty Cell
                //document.Add(new Paragraph("\n"));

                //Work History
                PdfPTable tableWorkHistory = new PdfPTable(6);
                //tableWorkHistory.SetWidths(new int[] { 1, 1, 2, 1, 1, 1, 1 });
                PdfPCell cellWorkHistory = new PdfPCell(new Phrase("Work History"));
                cellWorkHistory.Colspan = 6;
                cellWorkHistory.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                cellWorkHistory.BackgroundColor = BaseColor.GRAY;
                tableWorkHistory.AddCell(cellWorkHistory);
                //tableWorkHistory.AddCell(emptyCell);

                //tableWorkHistory.AddCell("Company Name");
                //tableWorkHistory.AddCell("Job Title");
                //tableWorkHistory.AddCell("Position Held");
                //tableWorkHistory.AddCell("Department");
                //tableWorkHistory.AddCell("Start Date");
                //tableWorkHistory.AddCell("End Date");
                //tableWorkHistory.AddCell("Reason For Leaving");


                PdfPCell cellCompanyName = new PdfPCell(new Phrase("Company Name"));
                cellCompanyName.HorizontalAlignment = Element.ALIGN_LEFT;
                cellCompanyName.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableWorkHistory.AddCell(cellCompanyName);

                PdfPCell cellJobTitle = new PdfPCell(new Phrase("Job Title"));
                cellJobTitle.HorizontalAlignment = Element.ALIGN_LEFT;
                cellJobTitle.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableWorkHistory.AddCell(cellJobTitle);

                //PdfPCell cellPositionHeld = new PdfPCell(new Phrase("Duties"));
                //cellPositionHeld.HorizontalAlignment = Element.ALIGN_LEFT;
                //cellPositionHeld.BackgroundColor = BaseColor.LIGHT_GRAY;
                //tableWorkHistory.AddCell(cellPositionHeld);

                PdfPCell cellDepartment = new PdfPCell(new Phrase("Department"));
                cellDepartment.HorizontalAlignment = Element.ALIGN_LEFT;
                cellDepartment.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableWorkHistory.AddCell(cellDepartment);

                PdfPCell cellStartDate = new PdfPCell(new Phrase("Start Date"));
                cellStartDate.HorizontalAlignment = Element.ALIGN_LEFT;
                cellStartDate.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableWorkHistory.AddCell(cellStartDate);

                PdfPCell cellEndDate = new PdfPCell(new Phrase("End Date"));
                cellEndDate.HorizontalAlignment = Element.ALIGN_LEFT;
                cellEndDate.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableWorkHistory.AddCell(cellEndDate);

                PdfPCell cellReasonForLeaving = new PdfPCell(new Phrase("Reason For Leaving"));
                cellReasonForLeaving.HorizontalAlignment = Element.ALIGN_LEFT;
                cellReasonForLeaving.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableWorkHistory.AddCell(cellReasonForLeaving);

                foreach (var d in workHistoryData)
                {
                    tableWorkHistory.AddCell(d.companyName);
                    tableWorkHistory.AddCell(d.jobTitle);
                    //tableWorkHistory.AddCell(d.positionHeld);
                    tableWorkHistory.AddCell(d.department);
                    tableWorkHistory.AddCell(d.startDate);
                    if (d.reasonForLeaving == "current") { tableWorkHistory.AddCell(string.Empty); } else { tableWorkHistory.AddCell(d.endDate); }
                    if (d.reasonForLeaving == "current") { tableWorkHistory.AddCell(string.Empty); } else { tableWorkHistory.AddCell(d.reasonForLeaving); }

                    PdfPCell cellDutiesHeading = new PdfPCell(new Phrase("Duties"));
                    cellDutiesHeading.Colspan = 6;
                    cellDutiesHeading.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    cellDutiesHeading.BackgroundColor = BaseColor.LIGHT_GRAY;
                    tableWorkHistory.AddCell(cellDutiesHeading);


                    //PdfPCell cellDuties = new PdfPCell(new Phrase(d.positionHeld));
                    //cellDuties.Colspan = 6;
                    //cellDuties.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    ////cellDuties.BackgroundColor = BaseColor.GRAY;
                    //tableWorkHistory.AddCell(cellDuties);

                    if (d.positionHeld != null && d.positionHeld != "")
                    {
                        // char[] delimiterChars = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '?' };
                        //char[] delimiterChars = { '.' };
                        string text = d.positionHeld;
                        //string[] duties = Regex.Split(d.positionHeld, @"(?<=[\.!\?])\s+");
                        //string[] duties = text.Split(delimiterChars);
                        string duties1 = this.RemoveSpecialCharacters(text);

                        //StringBuilder rbuilder = new StringBuilder();
                        //int myVal = 0;
                        //foreach (var word in duties)
                        //{
                        //    if (word != null || word != "")
                        //    {
                        //        myVal += 1;
                        //        if (myVal - 1 == 0)
                        //        {
                        //            rbuilder.Append(word + Environment.NewLine);
                        //            //rbuilder.AppendLine();
                        //            //System.Diagnostics.Debug.WriteLine($"<{word + Environment.NewLine}>");
                        //        }
                        //        else
                        //        {
                        //            //rbuilder.Append(myVal - 1 + word + Environment.NewLine);
                        //            rbuilder.Append(". " + word + Environment.NewLine);
                        //            //System.Diagnostics.Debug.WriteLine($"<{myVal - 1 + word}>"); 
                        //        }
                        //    }
                        //}

                        //string duties1 = rbuilder.ToString();

                        PdfPCell cellDuties = new PdfPCell(new Phrase(duties1));
                        cellDuties.Colspan = 6;
                        cellDuties.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                                                            //cellDuties.BackgroundColor = BaseColor.GRAY;
                                                            //cellDuties.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                        tableWorkHistory.AddCell(cellDuties);
                    }
                    else
                    {
                        PdfPCell cellDuties = new PdfPCell(new Phrase("No duties specified."));
                        cellDuties.Colspan = 6;
                        cellDuties.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                                                            //cellDuties.BackgroundColor = BaseColor.GRAY;
                        tableWorkHistory.AddCell(cellDuties);
                    }


                }

                //Candidate Skills
                PdfPTable tableSkills = new PdfPTable(2);
                PdfPCell cellSkills = new PdfPCell(new Phrase("Skills"));
                cellSkills.Colspan = 2;
                cellSkills.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                cellSkills.BackgroundColor = BaseColor.GRAY;
                tableSkills.AddCell(cellSkills);

                //tableSkills.AddCell(emptyCell);

                //tableSkills.AddCell("Skill Name");
                //tableSkills.AddCell("Skill Proficiency");

                PdfPCell cellSkillName = new PdfPCell(new Phrase("Skill Name"));
                cellSkillName.HorizontalAlignment = Element.ALIGN_LEFT;
                cellSkillName.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableSkills.AddCell(cellSkillName);

                PdfPCell cellSkillProficiency = new PdfPCell(new Phrase("Skill Proficiency"));
                cellSkillProficiency.HorizontalAlignment = Element.ALIGN_LEFT;
                cellSkillProficiency.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableSkills.AddCell(cellSkillProficiency);

                foreach (var d in skillsData)
                {
                    tableSkills.AddCell(d.skillName);
                    tableSkills.AddCell(d.SkillProficiency);

                }

                //Candidate Languages
                PdfPTable tableLanguage = new PdfPTable(2);
                PdfPCell cellLanguage = new PdfPCell(new Phrase("Languages"));
                cellLanguage.Colspan = 2;
                cellLanguage.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                cellLanguage.BackgroundColor = BaseColor.GRAY;
                tableLanguage.AddCell(cellLanguage);

                //tableLanguage.AddCell(emptyCell);

                //tableLanguage.AddCell("Language Name");
                //tableLanguage.AddCell("Language Proficiency");

                PdfPCell cellLanguageName = new PdfPCell(new Phrase("Language Name"));
                cellLanguageName.HorizontalAlignment = Element.ALIGN_LEFT;
                cellLanguageName.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableLanguage.AddCell(cellLanguageName);

                PdfPCell cellLanguageProficiency = new PdfPCell(new Phrase("Language Proficiency"));
                cellLanguageProficiency.HorizontalAlignment = Element.ALIGN_LEFT;
                cellLanguageProficiency.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableLanguage.AddCell(cellLanguageProficiency);

                foreach (var d in languageData)
                {
                    tableLanguage.AddCell(d.LanguageName);
                    tableLanguage.AddCell(d.LanguageProficiency);
                }

                //Candidate Reference
                PdfPTable tableReference = new PdfPTable(5);
                PdfPCell cellReference = new PdfPCell(new Phrase("References"));
                cellReference.Colspan = 5;
                cellReference.HorizontalAlignment = 1; //0=Left, 1=Centre, 2=Right
                cellReference.BackgroundColor = BaseColor.GRAY;
                tableReference.AddCell(cellReference);

                //tableReference.AddCell("Full Names");
                //tableReference.AddCell("Company Name");
                //tableReference.AddCell("Position Held");
                //tableReference.AddCell("Telephone No");
                //tableReference.AddCell("Email Address");


                PdfPCell cellFullNames = new PdfPCell(new Phrase("Full Names"));
                cellFullNames.HorizontalAlignment = Element.ALIGN_LEFT;
                cellFullNames.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableReference.AddCell(cellFullNames);

                PdfPCell cellRefCompanyName = new PdfPCell(new Phrase("Company Name"));
                cellRefCompanyName.HorizontalAlignment = Element.ALIGN_LEFT;
                cellRefCompanyName.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableReference.AddCell(cellRefCompanyName);

                PdfPCell cellRefPositionHeld = new PdfPCell(new Phrase("Position Held"));
                cellRefPositionHeld.HorizontalAlignment = Element.ALIGN_LEFT;
                cellRefPositionHeld.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableReference.AddCell(cellRefPositionHeld);

                PdfPCell cellTelephoneNo = new PdfPCell(new Phrase("Telephone No"));
                cellTelephoneNo.HorizontalAlignment = Element.ALIGN_LEFT;
                cellTelephoneNo.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableReference.AddCell(cellTelephoneNo);

                PdfPCell cellEmailAddress = new PdfPCell(new Phrase("Email Address"));
                cellEmailAddress.HorizontalAlignment = Element.ALIGN_LEFT;
                cellEmailAddress.BackgroundColor = BaseColor.LIGHT_GRAY;
                tableReference.AddCell(cellEmailAddress);

                foreach (var d in referenceData)
                {
                    tableReference.AddCell(d.refName);
                    tableReference.AddCell(d.companyName);
                    tableReference.AddCell(d.positionHeld);
                    tableReference.AddCell(d.telNo);
                    tableReference.AddCell(d.emailAddress);
                }


                document.Add(table);
                document.Add(tableEducation);
                document.Add(new Paragraph("\n"));
                document.Add(tableWorkHistory);
                document.Add(new Paragraph("\n"));
                document.Add(tableSkills);
                document.Add(new Paragraph("\n"));
                document.Add(tableLanguage);
                document.Add(new Paragraph("\n"));
                document.Add(tableReference);

                //Vacancy Purpose
                //PdfPCell cellEducationHistory = new PdfPCell(new Phrase("Education History:"));
                //cellEducationHistory.Colspan = 6;
                //cellEducationHistory.HorizontalAlignment = Element.ALIGN_LEFT;
                //cellEducationHistory.BackgroundColor = BaseColor.LIGHT_GRAY;
                //table.AddCell(cellEducationHistory);

                //table.AddCell(emptyCell);






                //PdfPCell cellVacancyPurposeContent = new PdfPCell(new Phrase(data.VacancyPurpose));
                //cellVacancyPurposeContent.Colspan = 2;
                //cellVacancyPurposeContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                //table.AddCell(cellVacancyPurposeContent);
                //End Vacancy Purpose

                //table.AddCell(emptyCell);

                //Responsibilities
                //PdfPCell cellResponsibilities = new PdfPCell(new Phrase("Responsibilities:"));
                //cellResponsibilities.Colspan = 2;
                //cellResponsibilities.HorizontalAlignment = Element.ALIGN_LEFT;
                //cellResponsibilities.BackgroundColor = BaseColor.LIGHT_GRAY;
                //table.AddCell(cellResponsibilities);

                //PdfPCell cellResponsibilitiesContent = new PdfPCell(new Phrase(data.Responsibility));
                //cellResponsibilitiesContent.Colspan = 2;
                //cellResponsibilitiesContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                //table.AddCell(cellResponsibilitiesContent);
                //End Responsibilities

                //table.AddCell("Responsibilities: ");
                //table.AddCell(data.Responsibility);

                //table.AddCell(emptyCell);

                //Qualifications and Experience
                //PdfPCell cellQualificationsandExperience = new PdfPCell(new Phrase("Qualifications and Experience:"));
                //cellQualificationsandExperience.Colspan = 2;
                //cellQualificationsandExperience.HorizontalAlignment = Element.ALIGN_LEFT;
                //cellQualificationsandExperience.BackgroundColor = BaseColor.LIGHT_GRAY;
                //table.AddCell(cellQualificationsandExperience);

                //PdfPCell cellQualificationsandExperienceContent = new PdfPCell(new Phrase(data.QualificationsandExperience));
                //cellQualificationsandExperienceContent.Colspan = 2;
                //cellQualificationsandExperienceContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                //table.AddCell(cellQualificationsandExperienceContent);

                //table.AddCell("Qualifications and Experience: ");
                //table.AddCell(data.QualificationsandExperience);
                //End Qualifications and Experience
                //table.AddCell(emptyCell);

                //Knowledge
                //PdfPCell cellKnowledge = new PdfPCell(new Phrase("Knowledge:"));
                //cellKnowledge.Colspan = 2;
                //cellKnowledge.HorizontalAlignment = Element.ALIGN_LEFT;
                //cellKnowledge.BackgroundColor = BaseColor.LIGHT_GRAY;
                //table.AddCell(cellKnowledge);

                //PdfPCell cellKnowledgeContent = new PdfPCell(new Phrase(data.Knowledge));
                //cellKnowledgeContent.Colspan = 2;
                //cellKnowledgeContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                //table.AddCell(cellKnowledgeContent);
                //table.AddCell("Knowledge: ");
                //table.AddCell(data.Knowledge);
                //End Knowledge


                //table.AddCell(emptyCell);
                //Technical Competency
                //StringBuilder technicalCompBuilder = new StringBuilder();
                //int technicalCompCounter = 0;
                //var technicalComp = _dal.GetTechnicalCompList((int)data.JobProfileID);

                //foreach (var item in technicalComp)
                //{
                //    technicalCompCounter += 1;
                //    technicalCompBuilder.AppendLine(technicalCompCounter.ToString() + ". " + item.TechnicalComp + " - " + item.TechnicalCompDesc);
                //}

                //PdfPCell cellTechnicalComp = new PdfPCell(new Phrase("Technical Competencies"));
                //cellTechnicalComp.Colspan = 2;
                //cellTechnicalComp.HorizontalAlignment = Element.ALIGN_LEFT;
                //cellTechnicalComp.BackgroundColor = BaseColor.LIGHT_GRAY;
                //table.AddCell(cellTechnicalComp);

                //PdfPCell cellTechnicalCompContent = new PdfPCell(new Phrase(technicalCompBuilder.ToString()));
                //cellTechnicalCompContent.Colspan = 2;
                //cellTechnicalCompContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                //table.AddCell(cellTechnicalCompContent);





                //Behavioural Competency
                //StringBuilder behaviouralCompBuilder = new StringBuilder();
                //int behaviouralCompCounter = 0;
                //var behaviouralComp = _dal.GetBehaviouralCompList((int)data.JobProfileID);

                //foreach (var item in behaviouralComp)
                //{
                //    behaviouralCompCounter += 1;
                //    behaviouralCompBuilder.AppendLine(behaviouralCompCounter.ToString() + ". " + item.BehaviouralComp + " - " + item.BehaviouralCompDesc);
                //}

                //PdfPCell cellBehaviouralComp = new PdfPCell(new Phrase("Interpersonal and Behavioural Competencies"));
                //cellBehaviouralComp.Colspan = 2;
                //cellBehaviouralComp.HorizontalAlignment = Element.ALIGN_LEFT;
                //cellBehaviouralComp.BackgroundColor = BaseColor.LIGHT_GRAY;
                //table.AddCell(cellBehaviouralComp);

                //PdfPCell cellBehaviouralCompContent = new PdfPCell(new Phrase(behaviouralCompBuilder.ToString()));
                //cellBehaviouralCompContent.Colspan = 2;
                //cellBehaviouralCompContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                //table.AddCell(cellBehaviouralCompContent);



                //var skillsCategoryList = _dal.GetSelectedSkillsPerCatergiryListForJobProfile((int)data.JobProfileID);
                //foreach (var item in skillsCategoryList)
                //{
                //  table.AddCell("Skills: ");
                //  table.AddCell(item.skillName);
                //}
                //table.AddCell(emptyCell);
                //Additional Requirements


                //table.AddCell("Additional Requirements: ");
                //table.AddCell(data.OtherSpecialRequirements);
                //End Additional Requirements

                //table.AddCell("Recruiter ");
                //table.AddCell(data.Recruiter);

                //table.AddCell("Email your query to ");
                //table.AddCell(data.RecruiterEmail);
                //table.AddCell(emptyCell);

                //PdfPCell cellHowToApply = new PdfPCell(new Phrase("How to apply"));
                //cellHowToApply.Colspan = 2;
                //cellHowToApply.HorizontalAlignment = Element.ALIGN_LEFT;
                //cellHowToApply.BackgroundColor = BaseColor.LIGHT_GRAY;
                //table.AddCell(cellHowToApply);

                //PdfPCell cellcellHowToApplyContent = new PdfPCell(new Phrase("Please log into eRecruitment system using the following url: "
                //                                    + new Uri(System.Configuration.ConfigurationManager.AppSettings["LogOutURL"])
                //                                    + " and navigate through Recruitment Jobs link. Click on Recruitment Citizen"));

                //cellcellHowToApplyContent.Colspan = 2;
                //cellcellHowToApplyContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                //table.AddCell(cellcellHowToApplyContent);
                //        table.AddCell("HOW TO APPLY ");
                //table.AddCell(System.Configuration.ConfigurationManager.AppSettings["LogOutURL"]);

                //table.AddCell(emptyCell);
                //Closing Date
                //PdfPCell cellClosingDate = new PdfPCell(new Phrase("Closing Date : " + Convert.ToDateTime(data.ClosingDate).ToString("dd MMM yyyy")));
                //cellClosingDate.Colspan = 2;
                //cellClosingDate.HorizontalAlignment = Element.ALIGN_LEFT;

                //cellClosingDate.BackgroundColor = BaseColor.LIGHT_GRAY;
                //cellClosingDate.Border = 0; //Added this for testing
                //table.AddCell(cellClosingDate);
                //End Closing Date

                //table.AddCell(emptyCell);
                //PdfPCell cellDisclaimer = new PdfPCell(new Phrase("Disclaimer"));
                //cellDisclaimer.Colspan = 2;
                //cellDisclaimer.HorizontalAlignment = Element.ALIGN_LEFT;
                //cellDisclaimer.BackgroundColor = BaseColor.LIGHT_GRAY;
                //table.AddCell(cellDisclaimer);

                //PdfPCell cellDisclaimerContent = new PdfPCell(new Phrase(data.Disclaimer));
                //cellDisclaimerContent.Colspan = 2;
                //cellDisclaimerContent.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                //table.AddCell(cellDisclaimerContent);

                //table.DefaultCell.Border = Rectangle.NO_BORDER;


                //document.Add(new Paragraph("\n"));


                //paragraph.Add(text);
                Paragraph paraEnd = new Paragraph("******NB: End of Candidate Profile******");
                paraEnd.Alignment = Element.ALIGN_CENTER;
                paraHead.Font = FontFactory.GetFont("dax-black", 20, Font.ITALIC);
                document.Add(paraEnd);

                document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                Response.Clear();

                Response.ContentType = "application/pdf";

                //string pdfName = data.FirstName.Replace("/", "_");
                string fileName = String.Format("{0}_{1}_Profile", profileData.FirstName.Trim(), profileData.Surname.Trim()) + ".pdf";
                //string fileName = "_Profile.pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);

                //Response.AddHeader("Content-Disposition", @"attachment; filename=" + pdfName + ".pdf");
                Response.ContentType = "application/pdf";
                Response.Buffer = true;
                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.BinaryWrite(bytes);
                Response.End();
                Response.Close();

            }

            return View();
        }

        //Department By Division
        [Authorize]
        [HttpPost]
        public ActionResult GetApproverByDepartmentList(int id)
        {
            var r = new object[] { _dal.GetApproverByDepartmentList(id), _dal.GetListOfRecruitersByDepartmentList(id) };


            return Json(r, JsonRequestBehavior.AllowGet);
        }
        /**
         * Author khutso
         */
        //Check if the vacancy exist 
        [Authorize]
        [HttpPost]
        public ActionResult checkVacancy(ValidateVacany VacancyJson)
        {
            var results = 0;
            if (!(VacancyJson == null))
            {
                results = _dal.CheckIfVacancyExists(VacancyJson.DivisionID, VacancyJson.DepartmentID, VacancyJson.JobTitleID, VacancyJson.OrganisationID);

            }


            return Json(results, JsonRequestBehavior.AllowGet);

        }
        /**
         * Author khutso
         */
        //Check the bps
        [Authorize]
        [HttpPost]
        public ActionResult checkVacancyBPSNumber(string BPSVacancyNo)
        {

            //HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["BPSTrackerAPIURL"] + BPSVacancyNo);
            //myRequest.Timeout = 5000;
            //HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();

            string userid = User.Identity.GetUserId();
            int orgID = _dal.GetOrganisationID(userid);

            var results = _dal.CheckIfVacancyNumberExists(BPSVacancyNo,orgID);
            return Json(results, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [HttpGet]
        public ActionResult VacancyReAdvertise(int id)
        {
            //ViewBag.Vacancy = _dal.GetVacancyListForEditByID(Convert.ToInt32(hc["hdnVacancyIDFor"])).FirstOrDefault();



            string userid = User.Identity.GetUserId();
            var orgID = _dal.GetOrganisationID(userid);

            ViewBag.EmploymentType = _dal.GetEmploymentTypeList();
            ViewBag.Organisation = _dal.GetOrganisationList(userid);
            ViewBag.QuetionBanks = _dal.GetGeneralQuestionsList(orgID);
            ViewBag.QuetionExperienceCat = _dal.GetGeneralQuestionsExperienceList(orgID);
            ViewBag.QuetionCertificationCat = _dal.GetGeneralQuestionsCertificationList(orgID);
            ViewBag.QuetionAnnualSalaryCat = _dal.GetGeneralQuestionsAnnualSalaryList(orgID);
            ViewBag.QuetionNoticePeriodCat = _dal.GetGeneralQuestionsNoticePeriodList(orgID);

            ViewBag.Location = _dal.GetLocationList(orgID);
            ViewBag.Division = _dal.GetDivisionList(userid);
            ViewBag.SitaDepartment = _dal.GetDepartment(userid);
            ViewBag.VacancyProfile = _dal.GetVacancyProfile(orgID);
            ViewBag.SkillsCategoryList = _dal.GetSkillsCategoryList();
            ViewBag.VacancyType = _dal.GetVacancyTypeList();
            ViewBag.RecruitersInfo = _dal.GetRecruiterInfo(userid);
            ViewBag.JobTitle = _dal.GetAllJobTitleListFromVacancyProfile(userid);
            ViewBag.SkillsList = _dal.GetSelectedSkillsPerCatergiryList(id);
            ViewBag.ApproverList = _dal.GetListOfApprovers();
            ViewBag.PublishDays = _dal.GetPublishDaysList();

            ViewBag.Gender = _dal.GetGenderList();
            ViewBag.Race = _dal.GetRaceList();
            ViewBag.SalaryType = _dal.GetSalaryTypeList();
            ViewBag.RecruiterList = _dal.GetListOfRecruiters();

            var p = _dal.GetVacancyListForEditByID(id);

            // convert the closeDate inside the "p" object to a string then reformat it to a html format
            //var newDate = p[0].ClosingDate.ToString().Substring(0, 9);
            //p[0].ClosingDate = newDate;

            return View(p);

        }
    }
}