using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CTR_FLS_2.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Override the initialize method in order to get handles to the user and request
        /// objects in order to write to the user activity log.
        /// Can't do this in the constructor because the Authorization hasn't happened at 
        /// that time so now references to user and request are available
        /// </summary>
        /// <param name="requestContext"></param>
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            try
            {
                //if (!String.IsNullOrWhiteSpace(Common.Methods.GetUserLoginName()) && Request != null)
                //{
                //    using (ConfigcontrolEntities context = new ConfigcontrolEntities())
                //    {
                //        user_activity_log LogEntry = new user_activity_log
                //        {
                //            activity_date = DateTime.Now,
                //            user_name = Common.Methods.GetUserLoginName(),
                //            activity_desc = Request.FilePath
                //        };

                //        context.user_activity_log.Add(LogEntry);
                //        context.SaveChanges();
                //    }
                //}
            }
            catch (Exception Ex)
            {
                //CommonServices.LogError(Ex);
            }
        }

    }
}