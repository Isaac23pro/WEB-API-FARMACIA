using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaDiBusiness.DTOs.SaleDto
{
    public class InvoiceQueueDto
    {
        public int InvoiceId { get; set; }
        public string ClientName { get; set; }
        public DateTime RegisteredDate { get; set; }
        public decimal Total { get; set; }
        public string WaitTime { get; set; } // "Hace 5 min"
    }
}
