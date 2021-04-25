using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Utilant.Models;
using PagedList;

namespace Utilant.Controllers
{
    public class AlbumsController : Controller
    {
        // GET: Albums
        public ActionResult GetAlbums(int? pageNumber)
        {
            // Spent much time troubleshooting albums data table sporadically coming up blank, and
            // eventually not at all, only to find that the https://github.com/typicode/jsonplaceholder/issues/160
            // was having 520 errors for two days. Their server on cloudflare was the issue. 

            try
            {
                return View(AlbumsModelBAL.GetAlbums().ToPagedList(pageNumber ?? 1, 10));
            }
            catch
            {
                return View("Error");
            }
        }


        [HttpPost]
        public ActionResult GetAlbums(int? pageNumber, string query)
        {
            try
            {
                string searchQuery = query.Trim();
                return View(AlbumsModelBAL.SearchAlbums(searchQuery).ToPagedList(pageNumber ?? 1, 10));
            }
            catch
            {
                return View("Error");
            }
        }
    }
}