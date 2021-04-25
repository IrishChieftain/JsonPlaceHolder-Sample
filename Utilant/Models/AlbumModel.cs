using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utilant.Models
{
    public class AlbumModel
    {
        public int UserId { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }
    }
}