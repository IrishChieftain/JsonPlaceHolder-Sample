using System.Web.Mvc;

using Utilant.Models;

namespace Utilant.Controllers
{
    public class PhotosController : Controller
    {
        // GET: Thumbnails by title
        public ActionResult GetThumbs(string title)
        {
            try
            {
                return View(AlbumsModelBAL.GetThumbs(title));
            }
            catch
            {
                return View("Error");
            }
        }
    }
}