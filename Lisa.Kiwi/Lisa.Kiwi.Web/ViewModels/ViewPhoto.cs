using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lisa.Kiwi.Web.ViewModels
{
    public class ViewPhoto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContentLength { get; set; }
        public string ContentType { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
    }
}