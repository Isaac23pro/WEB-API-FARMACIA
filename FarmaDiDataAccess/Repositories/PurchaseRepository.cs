using FarmaDiCore.Common;
using FarmaDiCore.Entities;
using FarmaDiDataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaDiDataAccess.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly string _ConnectionString;
        private const string StoredProcedureName = "USP_InsertPurchase";
        private const string UdttTypeName = "PurchaseDetailsType";

        public PurchaseRepository(IConfiguration configuration)
        {
            _ConnectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<RepositoryResponse<PurchaseTransaction>> InserAsync(Purchase master, IEnumerable<PurchaseDetails> details)
        {
            var transaction = new PurchaseTransaction();
            try
            {
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand(StoredProcedureName, connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    DateTime purchaseDate = master.RegisteredDate == default ? DateTime.Now : master.RegisteredDate;

                    // 2. Parámetros Maestros
                    cmd.Parameters.AddWithValue("@SupplierId", master.SupplierId);
                    cmd.Parameters.AddWithValue("@UserId", master.UserId);
                    cmd.Parameters.AddWithValue("@PurchaseDate", purchaseDate); // <-- AGREGADO
                    // Se usa "TotalAmount" o "Total" en la tabla, pero el SP usa un cálculo interno.
                    // El SP usa @Observations, no @Observation
                    cmd.Parameters.AddWithValue("@Observations", (object)master.Observation ?? DBNull.Value);
                   // cmd.Parameters.AddWithValue("@PurchaseNum", (object)master.PurchaseNum ?? DBNull.Value); // 


                   
                    var detailsTable = new DataTable();
                    detailsTable.Columns.Add("ProductId", typeof(int));
                    detailsTable.Columns.Add("Quantity", typeof(int));         
                    detailsTable.Columns.Add("UnitPrice", typeof(decimal));
                    detailsTable.Columns.Add("BatchNumber", typeof(string));   
                    detailsTable.Columns.Add("ManufacturingDate", typeof(DateTime)); 
                    detailsTable.Columns.Add("ExpirationDate", typeof(DateTime));   

                    // 4. Llenar el DataTable
                    foreach (var item in details)
                    {
                        detailsTable.Rows.Add(
                            item.ProductId,
                            item.Quantity,
                            item.UnitPrice,
                            item.BatchNumber,           
                            (object)item.ManufacturingDate ?? DBNull.Value,
                            item.ExpirationDate
                        );
                    }

             
                    SqlParameter detailParm = cmd.Parameters.AddWithValue("@PurchaseDetails", detailsTable); 

                    detailParm.SqlDbType = SqlDbType.Structured;
                    detailParm.TypeName = UdttTypeName;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        // LECTURA DEL MAESTRO (PRIMER RESULT SET)
                        if (await reader.ReadAsync())
                        {
                            transaction.Master = new Purchase
                            {
                           
                                PurchaseId = (int)reader["PurchaseId"],
                                SupplierId = (int)reader["SupplierId"],
                                UserId = (int)reader["UserId"],
                                Total = (decimal)reader["Total"],
                                Observation = reader["Observations"] is DBNull ? null : reader["Observations"].ToString(),
                                RegisteredDate = (DateTime)reader["RegisteredDate"],
                                PurchaseNum = reader["PurchaseNum"] is DBNull ? null : reader["PurchaseNum"].ToString()
                            };
                        }

                   
                        await reader.NextResultAsync();


                        var detailsList = new List<PurchaseDetails>();
                        while (await reader.ReadAsync())
                        {
                            detailsList.Add(new PurchaseDetails
                            {
                                Id = (int)reader["PurchaseDetailId"],
                                PurchaseId = (int)reader["PurchaseId"],
                                ProductId = (int)reader["ProductId"],
                                BatchId = (int)reader["BatchId"],
                                Quantity = (int)reader["Quantity"],
                                UnitPrice = (decimal)reader["UnitPrice"],
                                TotalPrice = (decimal)reader["TotalPrice"],
                                RegisteredDate = (DateTime)reader["RegisteredDate"],


                                BatchNumber = reader["BatchNumber"].ToString(),
                                ManufacturingDate = reader["ManufacturingDate"] is DBNull ? null : (DateTime?)reader["ManufacturingDate"],
                                ExpirationDate = (DateTime)reader["ExpirationDate"]
                            });
                        }

                        transaction.Details = detailsList;
                    }

                    return new RepositoryResponse<PurchaseTransaction>
                    {
                        Data = transaction,
                        OperationStatusCode = 0,
                        Message = "Operación exitosa"
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<PurchaseTransaction>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = $"Error de Base de Datos ({ex.Number}): {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<PurchaseTransaction>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = $"Error General: {ex.Message}",
                };
            }
        }
    }
}