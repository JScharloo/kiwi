﻿using System.Threading.Tasks;
using System.Web.Mvc;
using Lisa.Kiwi.WebApi;

namespace Lisa.Kiwi.Web
{
    public class ReportsController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var reports = await _proxy.GetAsync();
            return View(reports);
        }

        public async Task<ActionResult> Details(int id)
        {
            var report = await _proxy.GetAsync(id);
            return View(report);
        }

        private Proxy<Report> _proxy = new Proxy<Report>("http://localhost:20151/", "/reports");
    }
}