using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eRecruitment.Sita.BackEnd.Models
{
    public class Error_Log
    {
        public Guid ID { get; set; }
        public string functionName { get; set; }

        private DateTime dateSubmitted { get; set; }
        public bool IS_API { get; set; }

        public string applicationType { get; set; }

        public void setFunctionName(ControllerContext controllercontext)
        {
            functionName = "Controller Name:" + "  "+ controllercontext.RouteData.Values["controller"].ToString() + " " + "," + "  "+ "Action Name:" + controllercontext.RouteData.Values["action"].ToString();
        }



    }
}