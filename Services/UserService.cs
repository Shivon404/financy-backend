using FinancyAPI.Models;
using FinancyAPI.DTOs;
using MySql.Data.MySqlClient;

namespace FinancyAPI.Services
{
    public class UserService
    {
        private readonly DatabaseService _dbService;

        public UserService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public User? Login(LoginRequest request)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM users WHERE email = @email AND password = @password";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", request.Email);
                    cmd.Parameters.AddWithValue("@password", request.Password);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = reader.GetInt32("user_id"),
                                FirstName = reader.GetString("first_name"),
                                LastName = reader.GetString("last_name"),
                                Email = reader.GetString("email"),
                                StudentId = reader.IsDBNull(reader.GetOrdinal("student_id")) ? null : reader.GetString("student_id"),
                                MonthlyAllowance = reader.GetDecimal("monthly_allowance"),
                                CreatedAt = reader.GetDateTime("created_at")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool Register(RegisterRequest request)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                
                // Check if email exists
                string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @email";
                using (var checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@email", request.Email);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0) return false;
                }
                
                // Insert new user
                string insertQuery = @"INSERT INTO users (first_name, last_name, email, password, student_id, monthly_allowance) 
                                       VALUES (@firstName, @lastName, @email, @password, @studentId, @allowance)";
                using (var cmd = new MySqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@firstName", request.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", request.LastName);
                    cmd.Parameters.AddWithValue("@email", request.Email);
                    cmd.Parameters.AddWithValue("@password", request.Password);
                    cmd.Parameters.AddWithValue("@studentId", (object?)request.StudentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@allowance", request.MonthlyAllowance);
                    
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public User? GetUserById(int userId)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM users WHERE user_id = @userId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = reader.GetInt32("user_id"),
                                FirstName = reader.GetString("first_name"),
                                LastName = reader.GetString("last_name"),
                                Email = reader.GetString("email"),
                                StudentId = reader.IsDBNull(reader.GetOrdinal("student_id")) ? null : reader.GetString("student_id"),
                                MonthlyAllowance = reader.GetDecimal("monthly_allowance"),
                                CreatedAt = reader.GetDateTime("created_at")
                            };
                        }
                    }
                }
            }
            return null;
        }
        public bool UpdateMonthlyAllowance(int userId, decimal newAllowance)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = "UPDATE users SET monthly_allowance = @allowance WHERE user_id = @userId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@allowance", newAllowance);
                    
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}