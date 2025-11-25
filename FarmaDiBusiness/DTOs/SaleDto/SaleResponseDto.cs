using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaDiBusiness.DTOs.SaleDto
{
    public class SaleResponseDto
    {
        public int InvoiceId { get; set; }
        public int UserId { get; set; }
        public string ClientName { get; set; }

        // Usamos DateTime para incluir la hora, vital para el orden de la cola
        public DateTime RegisteredDate { get; set; }

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; } // Coincide con 'Total' en BD
        public bool IsPrinted { get; set; } // <--- ¡NUEVO! Vital para tu cola

        // Opcional: Si quieres devolver el nombre del método de pago en lugar del ID
        // public string PaymentMethod { get; set; } 
        public int PaymentMethodId { get; set; }

        public List<SalesDetailsResponseDto> Details { get; set; }


    }
}
