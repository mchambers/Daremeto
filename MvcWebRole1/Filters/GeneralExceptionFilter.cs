using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Net.Http;
using DareyaAPI.Models;

namespace DareyaAPI.Filters
{
    

    public class GeneralExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is HttpResponseException)
            {
                //actionExecutedContext.Result = actionExecutedContext.Request.CreateResponse();
                actionExecutedContext.Result.Content = actionExecutedContext.Result.CreateContent<DaremetoResponseError>(new DaremetoResponseError { Message = actionExecutedContext.Exception.Message });
            }
            
            base.OnException(actionExecutedContext);
        }
    }
}