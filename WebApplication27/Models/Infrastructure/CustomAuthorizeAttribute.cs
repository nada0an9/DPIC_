using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace WebApplication27.Models.Infrastructure
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string allowedroles;
        public CustomAuthorizeAttribute(string roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            var userId = Convert.ToDecimal(httpContext.User.Identity.Name);
            if (userId != 0) 

                using (var db = new DB_Model())
                {
                    var userPermission = (from u in db.USERS
                                          from rr in db.ROLES
                                          join r in db.USER_ROLE on rr.ROLE_ID equals r.ROLE_ID
                                          join rp in db.ROLE_PERMISSIONS on rr.ROLE_ID equals rp.ROLE_ID
                                          join p in db.PERMISSIONS on rp.PERMISSION_ID equals p.PERMISSION_ID
                                          where u.USER_ID == userId && p.PERMISSION_DESCRIPTION == allowedroles && r.USER_ID == userId
                                          select new
                                    {
                                        p.PERMISSION_DESCRIPTION
                                    }).FirstOrDefault();
               
                        if (userPermission == null) { return false; }

                        if (allowedroles == userPermission.PERMISSION_DESCRIPTION) { return true; }
                    
                }


            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Accounts" },
                    { "action", "UnAuthorized" }
               });
        }
    }
}

