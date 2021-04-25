using System.Web.Mvc;

using Utilant.Models;

namespace Utilant.Controllers
{
    public class UsersController : Controller
    {
        // GET: User
        public ActionResult GetUser(string email)
        {
            try
            {
                return View(AlbumsModelBAL.GetUser(email));
            }
            catch
            {
                return View("Error");
            }
        }
    }
}