using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lisa.Kiwi.Web.Models;
using Lisa.Kiwi.WebApi;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Resources;

namespace Lisa.Kiwi.Web.Reporting.Controllers
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

            var report = _modelFactory.Create(viewModel);
            report = await PostReport(report);
            
            var cookie = new HttpCookie("report", report.Id.ToString());
            Response.SetCookie(cookie);

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
            await PatchReport(report.Id, report);

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
            await PatchReport(report.Id, report);

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
            await PatchReport(report.Id, report);

            return RedirectToAction("ContactRequired");
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
            await PatchReport(report.Id, report);

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
            await PatchReport(report.Id, report);

            // TODO: add error handling

            return RedirectToAction("Contact");
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
            await PatchReport(report.Id, report);

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
            await PatchReport(report.Id, report);

            return RedirectToAction("ContactRequired");
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
            await PatchReport(report.Id, report);

            return RedirectToAction("Perpetrator");
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
            await PatchReport(report.Id, report);

            return RedirectToAction("ContactRequired");
        }

        public ActionResult Perpetrator()
        {
            return View(new PerpetratorViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Perpetrator(PerpetratorViewModel viewModel)
        {
        	if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
        	
            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await PatchReport(report.Id, report);

            // TODO: add error handling
            return RedirectToAction("Contact");
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
            await PatchReport(report.Id, report);

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
            await PatchReport(report.Id, report);

            // TODO: add error handling

            return RedirectToAction("Done");
        }
        public ActionResult Done()
        {
            // TODO: show report

            return View();
        }

	    public ActionResult Error()
	    {
	        return View();
	    }

        private async Task<Report> GetCurrentReport()
        {
            var cookie = Request.Cookies["report"];
            if (cookie == null)
            {
                return null;
            }
            int reportId = Int32.Parse(cookie.Value);
            return await _reportProxy.GetAsync(reportId);
        }

        private async Task<Report> PostReport(Report report)
        {
            try
            {
                return await _reportProxy.PostAsync(report);
            }
            catch (Exception e)
            {
                RedirectToAction("Error", new { ErrorMessage = "Er is iets fout gegaan tijdens het opslaan van het report.", TechnicalErrorMessage = e.Message });
            }
            return null;
        }



        private async Task<Report> PatchReport(int id, Report report)
        {
            try
            {
                return await _reportProxy.PatchAsync(id, report);
            }
            catch (Exception e)
            {
                RedirectToAction("Error", new { ErrorMessage = "Er is iets fout gegaan tijdens het aanpassen van het report.", TechnicalErrorMessage = e.Message });
            }
            return null;
        }

        // Fiddler version
        //private readonly Proxy<Report> _reportProxy = new Proxy<Report>("http://localhost.fiddler:20151/", "/reports/");

        // Normal version
        private readonly Proxy<Report> _reportProxy = new Proxy<Report>("http://localhost:20151/", "/reports/");

        private readonly ModelFactory _modelFactory = new ModelFactory();
	}
}