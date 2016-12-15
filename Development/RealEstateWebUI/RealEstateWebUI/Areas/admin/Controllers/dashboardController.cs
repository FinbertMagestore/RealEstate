using RealEstateWebUI.Areas.admin.Models;
using RealEstateWebUI.Areas.admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RealEstateWebUI.Areas.admin.Controllers
{
    public class dashboardController : Controller
    {
        private LogService tblLogService = new LogService();
        AccountService appUserService = new AccountService();
        [Authorize(Roles = "Admin")]
        //
        // GET: /admin/dashboard/
        public ActionResult index()
        {
            List<TblLog> lstTblLog = tblLogService.GetAll();
            List<TblLog> result = new List<TblLog>();
            if (lstTblLog != null && lstTblLog.Count > 0)
            {
                for (int i = 0; i < 10 && i < lstTblLog.Count; i++)
                {
                    var temp = lstTblLog[i];
                    temp.Href2Object = tblLogService.GetHref2ObjectID(temp.TableNameID, temp.ActionID, temp.ObjectID);
                    if (temp.ActionID != (int)Common.ActionID.Delete)
                    {
                        temp.ObjectValue = tblLogService.GetValueLink2ObjectID(temp.TableNameID, temp.ActionID, temp.ObjectID, temp.ObjectValue);
                    }
                    temp.AppUser = appUserService.GetByPrimaryKey(temp.UserID);
                    result.Add(temp);
                }
            }

            return View(result);
        }
    }
}