using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace DareyaAPI.Controllers
{
    public class PhoneController : TwilioController
    {
        //
        // GET: /Sms/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Sms/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Sms/Create

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Sms(SmsRequest request)
        {
            var response = new TwilioResponse();

            string[] validNumbers = new string[] { "+18508032974", "+13237071329" };
            
            if (!validNumbers.Contains<string>(request.From))
            {
                response.Sms("Hi, I'm not allowed to talk to strangers. Please visit http://dareme.to.");
                return new TwiMLResult(response);
            }

            string[] parsed=request.Body.Split(new char[] { ' ' });
            if (parsed==null || parsed.Length < 2)
            {
                response.Sms("That's not a valid command. :-/");
                return new TwiMLResult(response);
            }

            DareManager dareMgr = new DareManager();

            switch (parsed[0].ToUpper())
            {
                case "MOD":
                    dareMgr.Takedown(Convert.ToInt64(parsed[1]));
                    break;
                default:
                    response.Sms("That's not a valid command. :-\\");
                    break;
            }

            return new TwiMLResult(response);
        }

        //
        // POST: /Sms/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Sms/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Sms/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Sms/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Sms/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
