using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.LookupModels
{
    public class Qualification
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public DateTime? DateCreated { get; set; }

    }
}
