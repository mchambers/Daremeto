using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace DareyaAPI.Models
{
    public class DaremetoResponseError
    {
        public string Message { get; set; }
    }

    public class DaremetoResponseException : System.Web.Http.HttpResponseException
    {
        public DaremetoResponseException(string ErrorMessage, System.Net.HttpStatusCode Code)
            : base(Code)
        {
            this.Response.Content = this.Response.CreateContent<DaremetoResponseError>(new DaremetoResponseError { Message = ErrorMessage });
        }
    }
}