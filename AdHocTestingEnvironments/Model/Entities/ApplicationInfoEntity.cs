using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Model.Entities
{
    public class ApplicationInfoEntity
    {
        public virtual int Id { get; set; }
        public string Name { get; set; }
        public string ContainerImage { get; set; }
        public bool HasDatabase { get; set; }
        public string InitSql { get; set; }
    }
}
