using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaDiCore.Entities
{
    public class SaleTransaction
    {
        // aqui uso el invoice y no sale porque yo que se va a mostrar al final es la factura
        // osea lo que se imprime es la factura y no una venta como tal
        public Invoice InvoiceMaster { get; set; }
        public List<InvoiceDetails> InvoiceDetails { get; set; }
    }
}
