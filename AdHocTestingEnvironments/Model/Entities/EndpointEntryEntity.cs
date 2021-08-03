using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Model.Entities
{
    public class EndpointEntryEntity
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Destination { get; set; }
    }
}
