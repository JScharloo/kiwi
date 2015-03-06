using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Lisa.Kiwi.Web.Models
{
    public class PhotoLoaderContext : DbContext
    {
        public PhotoLoaderContext()
            : base("PhotoLoaderContext")
        {
        }

        public DbSet<Photo> Photos { get; set; }

        public System.Data.Entity.DbSet<Lisa.Kiwi.Web.ViewModels.ViewPhoto> ViewPhotoes { get; set; }
    }
}