using System.Web.Mvc;

using Utilant.Models;


namespace Utilant.Controllers
{
    public class PostsController : Controller
    {
        // GET: Posts
        public ActionResult GetPosts(int userID)
        {
            try
            { 
                return View(AlbumsModelBAL.GetPosts(userID));
            }
            catch
            {
                return View("Error");
            }
        }
    }
}
