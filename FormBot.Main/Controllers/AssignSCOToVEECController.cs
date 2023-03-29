using FormBot.BAL.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FormBot.Main.Controllers
{
    public class AssignSCOToVEECController : Controller
    {

        #region Properties

        private readonly ICreateVeecBAL _veec;
        private readonly IUserBAL _userBAL;
        private readonly IEmailBAL _emailBAL;
        //private readonly ICreateVEECHistoryBAL _jobHistory;
        #endregion

        #region Constructor

        public AssignSCOToVEECController(ICreateVeecBAL veec, IUserBAL userBAL, IEmailBAL emailBAL, ICreateJobHistoryBAL jobHistory)
        {
            this._veec = veec;
            this._userBAL = userBAL;
            this._emailBAL = emailBAL;
            //this._jobHistory = jobHistory;
        }

        #endregion
        // GET: AssignSCOToVEEC
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetSCOUser()
        {
            List<SelectListItem> Items = _veec.GetSCOUser().Select(a => new SelectListItem { Text = a.FirstName + ' ' + a.LastName, Value = a.UserID.ToString() }).ToList();
            return Json(Items, JsonRequestBehavior.AllowGet);
        }
    }
}