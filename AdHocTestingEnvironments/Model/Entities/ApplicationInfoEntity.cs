using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Model.Entities
{
    public class ApplicationInfoEntity
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string ContainerImage { get; set; }
        public virtual bool HasDatabase { get; set; }
        public virtual string InitSql { get; set; }
    }
}
