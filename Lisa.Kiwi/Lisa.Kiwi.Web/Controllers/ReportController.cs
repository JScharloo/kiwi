﻿using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lisa.Kiwi.WebApi;
using Lisa.Kiwi.WebApi.Access;
using System.Web.Configuration;

namespace Lisa.Kiwi.Web
{
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            return View(new CategoryViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Index(CategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // TODO: replace AnonymousToken in Report model by a custom HTTP header
            var report = _modelFactory.Create(viewModel);
            report = await _reportProxy.PostAsync(report);

            await EnsureReportAccess(report);

            return RedirectToAction("Location");
        }

        public ActionResult Location()
        {
            return View(new LocationViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Location(LocationViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            return RedirectToAction(report.Category);
        }

        public ActionResult FirstAid()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> FirstAid(FirstAidViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();

            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            // TODO: add error handling

            return RedirectToAction("Contact");
        }


        public ActionResult Theft()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Theft(TheftViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();

            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            return RedirectToAction("Vehicle");
        }

        public ActionResult Drugs()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Drugs(DrugsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();

            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            return RedirectToAction("Perpetrator");
        }

        public ActionResult Fight()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Fight(FightViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();

            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            // TODO: add error handling

            return RedirectToAction("Vehicle");
        }

        public ActionResult Weapons()
        {
            return View(new WeaponViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Weapons(WeaponViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();

            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            return RedirectToAction("Perpetrator");
        }

        public ActionResult Nuisance()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Nuisance(NuisanceViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();

            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            return RedirectToAction("Vehicle");
        }

        public ActionResult Bullying()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Bullying(BullyingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            if (viewModel.HasPerpetrator)
            {
                if(!viewModel.HasVictim)
                {
                    return RedirectToAction("Perpetrator");
                }
                
                return RedirectToAction("Perpetrator", routeValues: new { hasVictim = viewModel.HasVictim });
            }
            else if (!viewModel.HasPerpetrator && viewModel.HasVictim)
            {
                return RedirectToAction("Victim");
            }

            return RedirectToAction("Vehicle");
            
            switch(viewModel.HasPerpetrator)
        }

        public ActionResult Other()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Other(OtherViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            return RedirectToAction("Vehicle");
        }

        public ActionResult Perpetrator()
        {
            return View(new PerpetratorViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Perpetrator(PerpetratorViewModel viewModel, bool hasVictim)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            return RedirectToAction(hasVictim ? "Victim" : "Vehicle");
        }

        public ActionResult Victim()
        {
            return View(new VictimViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Victim(VictimViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            // TODO: add error handling
            return RedirectToAction("Vehicle");
        }

        public ActionResult Vehicle()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Vehicle(VehicleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();

            if (viewModel.HasVehicle)
            {
                _modelFactory.Modify(report, viewModel);
                await _reportProxy.PatchAsync(report.Id, report);
            }

            switch (report.Category)
            {
                case "Theft":
                case "Nuisance":
                case "Other":
                    return RedirectToAction("ContactRequired");

                default:
                    return RedirectToAction("Contact");
            }
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Contact(ContactViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            // TODO: add error handling

            return RedirectToAction("Done");
        }

        public ActionResult ContactRequired()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ContactRequired(ContactRequiredViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            // TODO: add error handling

            return RedirectToAction("Done");
        }

        public async Task<ActionResult> Done()
        {
            var report = await GetCurrentReport();
            return View(report);
        }

        [HttpPost]
        public ActionResult Done(string category)
        {
            switch (category)
            {
                case "Theft":
                    return View("Police");

                case "Bullying":
                    return View("Help");

                default:
                    return View("End");
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            _reportProxy = new Proxy<Report>(WebConfigurationManager.AppSettings["WebApiUrl"], "/reports/");

            var tokenCookie = Request.Cookies["token"];
            if (tokenCookie != null)
            {
                _reportProxy.Token = tokenCookie.Value;
            }

            base.OnActionExecuting(context);
        }

        private async Task EnsureReportAccess(Report report)
        {
            var loginProxy = new AuthenticationProxy(WebConfigurationManager.AppSettings["WebApiUrl"], "/api/oauth");
            var token = await loginProxy.LoginAnonymous(report.AnonymousToken);

            // TODO: add error handling
            var authCookie = new HttpCookie("token", token.Value)
            {
                // TODO: let the web api determine the expiration time of the token. Right now, this doesn't
                // work because both types of token (for the reporter and the dashboard) have the same expiration
                // value, which is set in Startup.cs.
                Expires = DateTime.Now.AddMinutes(10)
            };
            var cookie = new HttpCookie("report", report.Id.ToString());

            Response.Cookies.Add(cookie);
            Response.Cookies.Add(authCookie);
        }

        private async Task<Report> GetCurrentReport()
        {
            var cookie = Request.Cookies["report"];
            if (cookie == null)
            {
                return null;
            }
            var reportId = Int32.Parse(cookie.Value);
            return await _reportProxy.GetAsync(reportId);
        }

        private Proxy<Report> _reportProxy;

        private readonly ModelFactory _modelFactory = new ModelFactory();
    }
}
