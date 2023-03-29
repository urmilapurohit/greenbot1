using FormBot.BAL.Service;
using FormBot.Entity.Email;
using FormBot.Helper;
using FormBot.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class EmailTemplateController : Controller
    {
        #region Properties
        private readonly IEmailBAL _emailTemplateBAL;
        #endregion

        #region Constructor
        public EmailTemplateController(IEmailBAL emailBAL)
        {
            this._emailTemplateBAL = emailBAL;
        }
        #endregion

        #region Events

        /// <summary>
        /// Get Email Templates
        /// </summary>
        /// <returns>Returns List of Email Template</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get method for View Email Template
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Returns Single Email Template</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult View(string id)
        {
            EmailInfo emailInfo = new EmailInfo();
            var props = emailInfo.GetType().GetProperties().Where(w => w.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() == 0);
            var templateFields = from PropertyInfo prop in props
                                 select new { ID = prop.Name, Name = prop.Name };
            ViewData["TemplateFields"] = new SelectList(templateFields, "ID", "Name");

            EmailTemplate emailTemplate = new EmailTemplate();
            int templateID = 0;
            if (!string.IsNullOrEmpty(id))
            {
                int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out templateID);
            }

            emailTemplate = _emailTemplateBAL.GetEmailTemplateByID(templateID);
            return View(emailTemplate);
        }

        /// <summary>
        /// Get method for Create New Email Template
        /// </summary>
        /// <returns>Returns List of Email Template</returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult Create()
        {
            EmailInfo emailInfo = new EmailInfo();
            var props = emailInfo.GetType().GetProperties().Where(w => w.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() == 0);
            var templateFields = from PropertyInfo prop in props
                                 select new { ID = prop.Name, Name = prop.Name };
            ViewData["TemplateFields"] = new SelectList(templateFields, "ID", "Name");
            EmailTemplate emailTemplate = new EmailTemplate();
            return View(emailTemplate);
        }

        /// <summary>
        /// Post method for create Email Template.
        /// </summary>
        /// <param name="emailTemplate">The email template.</param>
        /// <returns>
        /// return Template listing page.
        /// </returns>
        [HttpPost]
        [UserAuthorization]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(FormBot.Entity.Email.EmailTemplate emailTemplate)
        {
            EmailInfo emailInfo = new EmailInfo();
            var props = emailInfo.GetType().GetProperties().Where(w => w.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() == 0);
            var templateFields = from PropertyInfo prop in props
                                 select new { ID = prop.Name, Name = prop.Name };
            ViewData["TemplateFields"] = new SelectList(templateFields, "ID", "Name");

            if (ModelState.IsValid)
            {
                try
                {
                    emailTemplate.CreatedBy = ProjectSession.LoggedInUserId;
                    int templateID = _emailTemplateBAL.CreateTemplate(emailTemplate);
                    if (templateID > 0)
                    {
                        this.ShowMessage(SystemEnums.MessageType.Success, "Email Template has been saved successfully.", true);
                        return RedirectToAction("Index", "EmailTemplate");
                    }
                    else
                    {
                        this.ShowMessage(SystemEnums.MessageType.Error, "Error occured while saving EmailTemplate.", true);
                        return View(emailTemplate);
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "Error occured while saving EmailTemplate.", true);
                    return View(emailTemplate);
                }
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return View(emailTemplate);
            }
        }

        /// <summary>
        /// Get method for edit Email Template.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// return Template listing page.
        /// </returns>
        [HttpGet]
        [UserAuthorization]
        public ActionResult Edit(string id)
        {
            EmailInfo emailInfo = new EmailInfo();
            var props = emailInfo.GetType().GetProperties().Where(w => w.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() == 0);
            var templateFields = from PropertyInfo prop in props
                                 select new { ID = prop.Name, Name = prop.Name };
            ViewData["TemplateFields"] = new SelectList(templateFields, "ID", "Name");
            EmailTemplate emailTemplate = new EmailTemplate();

            if (!string.IsNullOrEmpty(id))
            {
                int templateID = 0;
                if (!string.IsNullOrEmpty(id))
                {
                    int.TryParse(QueryString.GetValueFromQueryString(id, "id"), out templateID);
                }

                emailTemplate = _emailTemplateBAL.GetEmailTemplateByID(templateID);
            }

            return View(emailTemplate);
        }

        /// <summary>
        /// Post method for edit Email Template.
        /// </summary>
        /// <param name="emailTemplate">The email template.</param>
        /// <returns>
        /// return Template listing page.
        /// </returns>
        [HttpPost]
        [UserAuthorization]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(FormBot.Entity.Email.EmailTemplate emailTemplate)
        {
            EmailInfo emailInfo = new EmailInfo();
            var props = emailInfo.GetType().GetProperties().Where(w => w.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Count() == 0);
            var templateFields = from PropertyInfo prop in props
                                 select new { ID = prop.Name, Name = prop.Name };
            ViewData["TemplateFields"] = new SelectList(templateFields, "ID", "Name");

            if (ModelState.IsValid)
            {
                try
                {
                    emailTemplate.ModifiedBy = ProjectSession.LoggedInUserId;
                    int templateID = _emailTemplateBAL.CreateTemplate(emailTemplate);
                    if (templateID > 0)
                    {
                        this.ShowMessage(SystemEnums.MessageType.Success, "Email Template has been updated successfully.", true);
                        return RedirectToAction("Index", "EmailTemplate");
                    }
                    else
                    {
                        this.ShowMessage(SystemEnums.MessageType.Error, "Error occured while saving EmailTemplate.", true);
                        return View(emailTemplate);
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage(SystemEnums.MessageType.Error, "Error occured while saving EmailTemplate.", true);
                    return View(emailTemplate);
                }
            }
            else
            {
                this.ShowMessage(SystemEnums.MessageType.Error, String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception)), true);
                return View(emailTemplate);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// List All Email Template.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="subject">The subject.</param>
        public void GetEmailTemplateList(string templateName = "", string subject = "")
        {
            GridParam gridParam = Grid.ParseParams(HttpContext.Request);
            int pageNumber = Convert.ToInt32((gridParam.PageStart / gridParam.PageSize) + 1);
            IList<EmailTemplate> lstEmailTemplate = _emailTemplateBAL.GetEmailTemplateList(pageNumber, gridParam.PageSize, gridParam.SortCol, gridParam.SortDir, templateName, subject);
            if (lstEmailTemplate.Count > 0)
            {
                gridParam.TotalDisplayRecords = lstEmailTemplate.FirstOrDefault().TotalRecords;
                gridParam.TotalRecords = lstEmailTemplate.FirstOrDefault().TotalRecords;
            }

            HttpContext.Response.Write(Grid.PrepareDataSet(lstEmailTemplate, gridParam));
        }

        /// <summary>
        /// Delete Selected Email Template.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns>
        /// Returns test.
        /// </returns>
        [UserAuthorization]
        public ActionResult DeleteEmailTemplate(string templateId)
        {
            //try
            //{
            int tID = 0;
            if (!string.IsNullOrEmpty(templateId))
            {
                int.TryParse(QueryString.GetValueFromQueryString(templateId, "id"), out tID);
            }

            _emailTemplateBAL.DeleteEmailTemplate(tID);
            return this.Json(new { success = true });
            //}
            //catch (Exception ex)
            //{
            //    return this.Json(new { success = false, errormessage = ex.Message });
            //}
        }

        #endregion
    }
}