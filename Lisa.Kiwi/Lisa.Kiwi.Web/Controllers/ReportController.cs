﻿using System;
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
using System.Configuration;

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
            report = await _reportProxy.PostAsync(report);

            // TODO: add error handling

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
            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

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
            var report = await GetCurrentReport();
            _modelFactory.Modify(report, viewModel);
            await _reportProxy.PatchAsync(report.Id, report);

            // TODO: add error handling

            return RedirectToAction("Done");
        }

        public ActionResult Done()
        {
            // TODO: show report

            return View();
        }

        private async Task<Report> GetCurrentReport()
        {
            var cookie = Request.Cookies["report"];
            int reportId = Int32.Parse(cookie.Value);
            return await _reportProxy.GetAsync(reportId);
        }

        private readonly Proxy<Report> _reportProxy = new Proxy<Report>("/reports/");
        private readonly ModelFactory _modelFactory = new ModelFactory();
	}
}