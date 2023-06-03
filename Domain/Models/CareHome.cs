using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CareHome
    {
        public int Id { get; set; }
        public string HomeName { get; set; }
        public string PostCode { get; set; }
        public DateTime? DateCreated { get; set; }

    }
}
