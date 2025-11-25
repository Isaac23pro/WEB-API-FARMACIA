using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaDiCore.Entities
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
        public DateTime RegisteredDate { get; set; }


    }
}
