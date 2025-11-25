using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaDiBusiness.DTOs.SaleDto
{
    public class SalesDetailsResponseDto
    {
        public int ProductId { get; set; }

        // --- NUEVOS CAMPOS (Para imprimir el ticket bonito) ---
        public string ProductTradeName { get; set; }   // Ej: "Panadol"
        public string ProductGenericName { get; set; } // Ej: "Paracetamol"
        // ------------------------------------------------------

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }


    }
}
