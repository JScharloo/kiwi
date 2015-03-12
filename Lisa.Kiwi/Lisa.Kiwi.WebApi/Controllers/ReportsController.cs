﻿using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using System.Web.Security;
using Newtonsoft.Json.Linq;

namespace Lisa.Kiwi.WebApi.Controllers
{
    [Authorize]
    public class ReportsController : ApiController
    {
        [EnableQuery]
        public IQueryable<Report> Get()
        {
            var reports = _db.Reports
                .Include("StatusChanges")
                .Include("Contact")
                .Include("Vehicle")
                .ToList()
                .Select(reportData => _modelFactory.Create(reportData))
                .AsQueryable();

            return reports;
        }

        public async Task<IHttpActionResult> Get(int? id)
        {
            var reportData = await _db.Reports
                .Include("StatusChanges")
                .Include("Contact")
                .Include("Vehicle")
                .SingleOrDefaultAsync(r => id == r.Id);

            if (reportData == null)
            {
                return NotFound();
            }

            var report = _modelFactory.Create(reportData);
            return Ok(report);
        }

        [AllowAnonymous]
        public async Task<IHttpActionResult> Post([FromBody] Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var reportData = _dataFactory.Create(report);
            var statusChangeData = new StatusChangeData
            {
                Created = DateTimeOffset.Now,
                IsVisible = report.IsVisible,
                Status = report.CurrentStatus.ToString()
            };
            reportData.StatusChanges.Add(statusChangeData);

            _db.Reports.Add(reportData);
            await _db.SaveChangesAsync();

            report = _modelFactory.Create(reportData);


            var anonymousToken = String.Format("{0}‼{1}", report.Id, DateTime.Now.AddMinutes(1));

            

            var purpose = "AnonymousToken";

            //var value = , purpose);
            var value = Encoding.UTF8.GetBytes(anonymousToken);

            value = MachineKey.Protect(Encoding.UTF8.GetBytes(anonymousToken));

            report.AnonymousToken = HttpServerUtility.UrlTokenEncode(value);



            var decryptedTokenByteArray = Convert.FromBase64String(report.AnonymousToken);

            decryptedTokenByteArray = MachineKey.Unprotect(decryptedTokenByteArray);

            var decryptedToken = Encoding.UTF8.GetString(decryptedTokenByteArray);

            var url = Url.Route("DefaultApi", new { controller = "reports", id = reportData.Id });
            return Created(url, report);
        }

        public async Task<IHttpActionResult> Patch(int? id, [FromBody] JToken json)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            if (claimsIdentity.HasClaim(ClaimTypes.Role, "anonymous"))
            {
                var reportIdFromClaim = Int32.Parse(claimsIdentity.Claims.FirstOrDefault(c => c.ValueType == "reportId").Value);

                if(id != reportIdFromClaim)
                {
                    return NotFound();
                }
            }

            var reportData = await _db.Reports.FindAsync(id);
            if (reportData == null)
            {
                return NotFound();
            }

            _dataFactory.Modify(reportData, json);

            await _db.SaveChangesAsync();

            var report = _modelFactory.Create(reportData);
            return Ok(report);
        }

        private readonly KiwiContext _db = new KiwiContext();
        private readonly ModelFactory _modelFactory = new ModelFactory();
        private readonly DataFactory _dataFactory = new DataFactory();
    }
}