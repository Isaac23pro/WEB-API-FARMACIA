using FarmaDiCore.Common;
using FarmaDiCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaDiDataAccess.Interfaces
{
    public interface ISalesRepository
    {
        Task<RepositoryResponse<SaleTransaction>> InsertAsync(Invoice master, IEnumerable<SaleDetails> details, int paymentMethodId);
        Task<RepositoryResponse<SaleTransaction>> GetInvoiceByIdAsync(int invoiceId);

        Task<RepositoryResponse<IEnumerable<Invoice>>> GetPendingInvoicesAsync();
    }
}
