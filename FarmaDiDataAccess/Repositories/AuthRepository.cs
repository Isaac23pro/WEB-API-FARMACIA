using FarmaDiCore.Common;
using FarmaDiCore.Entities;
using FarmaDiDataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaDiDataAccess.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;
        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<RepositoryResponse<Users>> RegisterAsync(Users user)
        {
            var response = new Users();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_RegisterUser", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@UserLastName", user.UserLastName);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@Mail", user.Mail);
                    cmd.Parameters.AddWithValue("UserPhone", user.UserPhone);
                    cmd.Parameters.AddWithValue("IsActive", user.IsActive);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;


                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            response.UserName = reader["UserName"].ToString()!;
                            response.UserLastName = reader["UserLastName"].ToString()!;
                            response.PasswordHash = reader["UserPassword"].ToString()!;
                            response.Mail = reader["Mail"].ToString()!;
                            response.UserPhone = reader["UserPhone"].ToString()!;
                            response.IsActive = (bool)reader["Isactive"];

                        }
                    }

                    var returmedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);
                    return new RepositoryResponse<Users>
                    {
                        Data = response,
                        OperationStatusCode = returmedValue
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Users>
                {
                    Data = null,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };

            }

        }
        public async Task<RepositoryResponse<Users>> GetByEmailAsync(string mail)
        {
            var user = new Users();
            var response = new RepositoryResponse<Users>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetUserByEmail", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Mail", mail);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user.UserId = (int)reader["UserId"];
                            user.UserName = reader["UserName"].ToString()!;
                            user.UserLastName = reader["UserLastName"].ToString()!;
                            user.PasswordHash = reader["PasswordHash"].ToString()!;
                            user.Mail = reader["Mail"].ToString()!;
                            user.UserPhone = reader["UserPhone"].ToString()!;
                            user.IsActive = (bool)reader["IsActive"];
                        }
                    }
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = user;
                    response.OperationStatusCode = returnedValue;

                    return response;

                }
                catch (SqlException ex)
                {
                    response.Data = null;
                    response.OperationStatusCode = ex.Number;
                    return response;
                }



            }
        }
        public async Task<RepositoryResponse<Users>> GetByUserNameAsync(string name)
        {
            var user = new Users();
            var response = new RepositoryResponse<Users>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetUserByName", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", name);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user.UserId = (int)reader["UserId"];
                            user.UserName = reader["UserName"].ToString()!;
                            user.UserLastName = reader["UserLastName"].ToString()!;
                            user.Mail = reader["Mail"].ToString()!;
                            user.PasswordHash = reader["UserPassword"].ToString()!;
                            user.UserPhone = reader["UserPhone"].ToString()!;
                            user.IsActive = (bool)reader["IsActive"];
                            
                        }
                    }
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = user;
                    response.OperationStatusCode = returnedValue;

                    return response;
                }
                catch (SqlException ex)
                {
                    response.Data = null;
                    response.OperationStatusCode = ex.Number;
                    return response;
                }
            }
        }

        public async Task<RepositoryResponse<IEnumerable<string>>> GetRolesByUserIdAsync(int userId)
        {
            var roles = new List<string>();
            var response = new RepositoryResponse<IEnumerable<string>>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetUserRolesByUserId", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            roles.Add(reader["RolName"].ToString()!);
                        }
                    }
                    var returnedValue = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value);

                    response.Data = roles;
                    response.OperationStatusCode = returnedValue;
                    response.Message = "Operacion existosa";

                    
                }
                catch (SqlException ex)
                {
                    response.Data = null;
                    response.OperationStatusCode = ex.Number;
                    response.Message = ex.Message;

                    
                }
                catch (Exception ex)
                {
                    response.Data = null;
                    response.OperationStatusCode = -1;
                    response.Message = ex.Message;

                    return response;
                }

                return response;
            }
        }


        public async Task<RepositoryResponse<bool>> SetRecoveryTokenAsync(int userId, string token, DateTime expiry)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("USP_SetRecoveryToken", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Token", token);
                        cmd.Parameters.AddWithValue("@Expiry", expiry);
                        var returnParameter = cmd.Parameters.Add("@ReturnValue", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();

                        var returnedValue = (int)returnParameter.Value;

                        return new RepositoryResponse<bool>
                        {
                            Data = returnedValue == 0, // Será TRUE solo si devolvió 0
                            OperationStatusCode = returnedValue
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<bool>
                {
                    Data = false,
                    OperationStatusCode = ex.Number,
                    Message = ex.Message
                };
            }
        }
        public async Task<RepositoryResponse<Users>> GetByRecoveryTokenAsync(string token)
        {
            var user = new Users();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_GetUserByRecoveryToken", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Token", token);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user.UserId = (int)reader["UserId"];
                            user.UserName = reader["UserName"].ToString()!;
                            user.UserLastName = reader["UserLastName"].ToString()!;
                            user.PasswordHash = reader["UserPassword"].ToString()!;
                            user.Mail = reader["Mail"].ToString()!;
                            user.UserPhone = reader["UserPhone"].ToString()!;
                            user.IsActive = (bool)reader["IsActive"];

                            return new RepositoryResponse<Users> { Data = user, OperationStatusCode = 0 };
                        }
                    }
                    return new RepositoryResponse<Users>
                    {
                        Data = null,
                        OperationStatusCode = 50009,
                        Message = "Token inválido o expirado."
                    };
                }
            }
            catch (SqlException ex)
            {
                return new RepositoryResponse<Users>
                {
                    Data = null,
                    OperationStatusCode = -1,
                    Message = ex.Message
                };
            }
        }
        public async Task<RepositoryResponse<bool>> UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand cmd = new SqlCommand("USP_UpdatePassword", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);

                    await cmd.ExecuteNonQueryAsync();
                    return new RepositoryResponse<bool>
                    {
                        Data = true,
                        OperationStatusCode = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return new RepositoryResponse<bool> { Data = false, OperationStatusCode = -1, Message = ex.Message };
            }
        }


    }
}
