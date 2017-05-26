using System.IO;
using System.Linq;
using System.Web.Mvc;
using ProfileSample.DAL;
using ProfileSample.Models;
using System.Web;
using System.Collections.Generic;

namespace ProfileSample.Controllers
{
	public class ImagesController : Controller
    {
		[HttpGet]
		public ActionResult Get(string searchPhrase)
		{
			IEnumerable<ImageModel> sources;
			using (var context = new ProfileSampleEntities())
			{
				sources = context.ImgSources.Where(item => item.Name.StartsWith(searchPhrase))
					.Select(item => new ImageModel()
					{
						Name = item.Name,
						Data = item.Data
					}).ToList();
			}

			return PartialView(sources);
		}

		[HttpGet]
		public ActionResult Convert()
        {
            var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg");

            using (var context = new ProfileSampleEntities())
            {
                foreach (var file in files)
                {
                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        byte[] buff = new byte[stream.Length];

                        stream.Read(buff, 0, (int) stream.Length);

                        var entity = new ImgSource()
                        {
                            Name = Path.GetFileName(file),
                            Data = buff,
                        };

                        context.ImgSources.Add(entity);
                        context.SaveChanges();
                    }
                } 
            }

            return RedirectToAction("Index");
        }

		[HttpGet]
		public ActionResult Upload()
		{
			ViewBag.Message = "Upload your file here";

			return View();
		}

		[HttpPost]
		public ActionResult Upload(HttpPostedFileBase upload)
		{
			if (upload == null)
			{
				return View("Error");
			}

			using (var context = new ProfileSampleEntities())
			{
				using (var stream = upload.InputStream)
				{
					byte[] buff = new byte[stream.Length];

					stream.Read(buff, 0, (int)stream.Length);

					var entity = new ImgSource()
					{
						Name = upload.FileName,
						Data = buff,
					};

					context.ImgSources.Add(entity);
					context.SaveChanges();
				}
			}

			return RedirectToAction("Index", "Home");
		}
    }
}