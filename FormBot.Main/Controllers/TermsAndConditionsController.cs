using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormBot.Helper;
using FormBot.BAL;
using FormBot.BAL.Service;

namespace FormBot.Main.Controllers
{
    [Authorize]
    public class TermsAndConditionsController : Controller
    {
        #region Properties
        private readonly IUserBAL _UserTerm;
        #endregion

        public TermsAndConditionsController(IUserBAL userTerm)
        {
            this._UserTerm = userTerm;
        }

        // GET: TermsAndConditions
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult SCA()
        {
            FormBot.Entity.User user = new FormBot.Entity.User();
            if (ProjectSession.UserTypeId > 0 && ProjectSession.SolarCompanyId > 0)
            {
                var dsUserTerm = _UserTerm.GetDetailsForTermsAndCondition(ProjectSession.UserTypeId, ProjectSession.SolarCompanyId);
                List<FormBot.Entity.User> users = DBClient.DataTableToList<FormBot.Entity.User>(dsUserTerm.Tables[0]);
                FormBot.Entity.User userView = users.FirstOrDefault();

                
                if(userView != null)
                {
                    user.CompanyABN = userView.CompanyABN;
                    user.AccountName = userView.AccountName;
                    user.BSB = userView.BSB;
                    user.AccountNumber = userView.AccountNumber;
                    user.ResellerName = userView.ResellerName;
                    user.Logo = userView.Logo;
                }

            }
            return View(user);
            
        }
    }
}