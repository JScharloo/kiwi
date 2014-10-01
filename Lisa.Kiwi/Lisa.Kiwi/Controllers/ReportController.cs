﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using Lisa.Kiwi.Data.Models;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Lisa.Kiwi.WebApi.Controllers
{
    public class ReportController : ODataController
    {
        private KiwiContext db = new KiwiContext();
        private readonly CloudQueue _queue = new QueueConfig().BuildQueue();

        // GET odata/Report
        [EnableQuery]
        public IQueryable<Models.Report> GetReport()
        {
            var result =
                from s in db.Statuses
                group s by s.Report
                into g
                let latest = g.Max(s => s.Created)
                let r = g.Key
                let status = g.FirstOrDefault(s => s.Created == latest)
                select new Models.Report()
                {
                    Created = r.Created,
                    Description = r.Description,
                    Guid = r.Guid,
                    Ip = r.Ip,
                    Location = r.Location,
                    Time = r.Time,
                    UserAgent = r.UserAgent,
                    Status = status.Name
                };
            
            return result;
        }

        // GET odata/Report(5)
        [EnableQuery]
        public SingleResult<Report> GetReport([FromODataUri] int key)
        {
            return SingleResult.Create(db.Reports.Where(report => report.Id == key));
        }

        // PUT odata/Report(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != report.Id)
            {
                return BadRequest();
            }
          
            try
            {
                await _queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(report)));
            }
            catch (Exception)
            {   // TODO: Figure out possible exceptions!!
                if (!ReportExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(report);
        }

        // POST odata/Report
        public async Task<IHttpActionResult> Post(Report report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(report)));

            return Created(report);
        }

        // PATCH odata/Report(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Report> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Report report = await db.Reports.FindAsync(key);
            if (report == null)
            {
                return NotFound();
            }

            patch.Patch(report);

            try
            {
                await _queue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(report)));
            }
            catch (Exception)
            {   // TODO: Figure out possible exceptions!!
                if (!ReportExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(report);
        }

        // DELETE odata/Report(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Report report = await db.Reports.FindAsync(key);
            if (report == null)
            {
                return NotFound();
            }

            db.Reports.Remove(report);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET odata/Report(5)/Contact
        [EnableQuery]
        public IQueryable<Contact> GetContact([FromODataUri] int key)
        {
            return db.Reports.Where(m => m.Id == key).SelectMany(m => m.Contacts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReportExists(int key)
        {
            return db.Reports.Count(e => e.Id == key) > 0;
        }
    }
}
