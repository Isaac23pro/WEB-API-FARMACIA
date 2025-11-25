using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaDiBusiness.DTOs.SaleDto
{
    public class CreateSaleDto
    {
        [Required(ErrorMessage = "El id del usuario es obligatorio")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un método de pago válido")]
        public int PaymentMethodId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El descuento no puede ser negativo")]
        public decimal Discount { get; set; }

        [Required(ErrorMessage = "Por lo menos debe de haber un producto en la compra.")]
        [MinLength(1, ErrorMessage = "La lista de detalles no puede estar vacía")]
        public List<CreateSaleDetailDto> Details { get; set; }

    }
}
