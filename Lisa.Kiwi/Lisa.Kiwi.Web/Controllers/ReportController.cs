using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lisa.Kiwi.Web.Models;
using Lisa.Kiwi.WebApi;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Lisa.Kiwi.Web.ViewModels;
using System.Collections.Generic;

namespace Lisa.Kiwi.Web.Reporting.Controllers
{
	public class ReportController : Controller
	{

        public ActionResult viewPhoto()
        {
            var photos = _Db.Photos;
            List<ViewPhoto> results = new List<ViewPhoto>();

            foreach (var photo in photos)
            {
                var image = GetImage(photo);


                var viewPhoto = new ViewPhoto
                {
                    Id = photo.Id,
                    Name = photo.Name,
                    ContentLength = photo.ContentLength,
                    ContentType = photo.ContentType,
                    Key = photo.Key,
                    Url = image.Uri.ToString()
                };
                results.Add(viewPhoto);
            }
            return View(results);
        }
        public ReportController()
        {
            _StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            _BlobClient = _StorageAccount.CreateCloudBlobClient();

            _Container = _BlobClient.GetContainerReference("photos");
            _Container.CreateIfNotExists();
            _Container.SetPermissions(
                new BlobContainerPermissions
                {
                    PublicAccess =
                        BlobContainerPublicAccessType.Blob
                });

            _Db = new PhotoLoaderContext();
        }



        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Photo photoEntity, HttpPostedFileBase photo)
        {

            if (!IsImage(photo.ContentType))
            {
                ModelState.AddModelError("Image", "Only images alloud.");
            }

            // 1 000 000 bytes is 1 MB
            if (photo.ContentLength > 20000000)
            {
                ModelState.AddModelError("Image", "The image cannot be larger then 20MB.");
            }

            var cookie = Request.Cookies["report"];
            int reportId = Int32.Parse(cookie.Value);
            var extension = Path.GetExtension(photo.FileName);
            var blobname = string.Format("{0}/{1}{2}",reportId, i, extension);
            var blob = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString")).CreateCloudBlobClient().GetContainerReference("photos").GetBlockBlobReference(blobname);
            if (blob.Exists())
            {
                i = i + 1;
                var key = string.Format("{0}/{1}{2}", reportId, i, extension);
                if (photoEntity.Name == null)
                {
                    photoEntity.Name = photo.FileName;
                    photoEntity.Name = photoEntity.Name.Replace(extension, "");
                }

                photoEntity.ContentLength = photo.ContentLength;
                photoEntity.ContentType = photo.ContentType;
                photoEntity.Key = key;
            

                if (!ModelState.IsValid)
                {
                    // Make the fields empty, because they need a new image.
                    photoEntity = null;
                    return View(photoEntity);
                }

                CloudBlockBlob block = _Container.GetBlockBlobReference(key);
                block.Properties.ContentType = photo.ContentType;

                block.UploadFromStream(photo.InputStream);
                return RedirectToAction("Index");
            }
            else
            {
                var key = string.Format("{0}/{1}{2}", reportId, i, extension);
                if (photoEntity.Name == null)
                {
                    photoEntity.Name = photo.FileName;
                    photoEntity.Name = photoEntity.Name.Replace(extension, "");
                }

                photoEntity.ContentLength = photo.ContentLength;
                photoEntity.ContentType = photo.ContentType;
                photoEntity.Key = key;
   

                if (!ModelState.IsValid)
                {
                    // Make the fields empty, because they need a new image.
                    photoEntity = null;
                    return View(photoEntity);
                }

                CloudBlockBlob block = _Container.GetBlockBlobReference(key);
                block.Properties.ContentType = photo.ContentType;

                block.UploadFromStream(photo.InputStream);

                return RedirectToAction("Index");
            }



        }

        public ActionResult Delete(int? id)
        {
            var photo = _Db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }

            var image = GetImage(photo);

            var viewPhoto = CreateViewPhoto(photo, image);

            return View(viewPhoto);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var photo = _Db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }

            var image = GetImage(photo);

            _Db.Photos.Remove(photo);
            _Db.SaveChanges();

            image.Delete();

            return RedirectToAction("viewPhoto");
        }
        private static int i = 0;
        private CloudStorageAccount _StorageAccount;
        private CloudBlobClient _BlobClient;
        private CloudBlobContainer _Container;
        private PhotoLoaderContext _Db;

        private CloudBlockBlob GetImage(Photo photo)
        {
            var image = _Container.GetBlockBlobReference(photo.Key);
            return image;
        }

        private ViewPhoto CreateViewPhoto(Photo photo, CloudBlockBlob image = null)
        {
            var url = "";
            if (image != null)
            {
                url = image.Uri.ToString();
            }

            var viewPhoto = new ViewPhoto
            {
                Id = photo.Id,
                Name = photo.Name,
                ContentLength = photo.ContentLength,
                ContentType = photo.ContentType,
                Key = photo.Key,
                Url = url
            };

            return viewPhoto;
        }

        private bool IsImage(string contentType)
        {
            string[] alloudMime = new string[] { "image" };
            foreach (var mime in alloudMime)
            {
                if (contentType.Contains(mime))
                {
                    return true;
                }
            }
            return false;
        }


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

        // Fiddler version
        //private readonly Proxy<Report> _reportProxy = new Proxy<Report>("http://localhost.fiddler:20151/", "/reports/");

        // Normal version
        private readonly Proxy<Report> _reportProxy = new Proxy<Report>("http://localhost:20151/", "/reports/");

        private readonly ModelFactory _modelFactory = new ModelFactory();
	}
}