using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Lisa.Kiwi.Web.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string Key { get; set; }
    }
}