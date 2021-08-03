using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AdHocTestingEnvironments.Model.Entities;

namespace AdHocTestingEnvironments.Data
{
    public class AdHocTestingEnvironmentsContext : DbContext
    {
        public AdHocTestingEnvironmentsContext (DbContextOptions<AdHocTestingEnvironmentsContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationInfoEntity> InfoEntities { get; set; }
    }
}
