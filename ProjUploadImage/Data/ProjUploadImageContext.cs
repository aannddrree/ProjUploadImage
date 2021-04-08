using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjUploadImage.Models;

namespace ProjUploadImage.Data
{
    public class ProjUploadImageContext : DbContext
    {
        public ProjUploadImageContext (DbContextOptions<ProjUploadImageContext> options)
            : base(options)
        {
        }

        public DbSet<ProjUploadImage.Models.Product> Product { get; set; }
    }
}
