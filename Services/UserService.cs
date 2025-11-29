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

        // ===== ADMIN METHODS - NEW =====
        
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM users ORDER BY created_at DESC";
                
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = reader.GetInt32("user_id"),
                            FirstName = reader.GetString("first_name"),
                            LastName = reader.GetString("last_name"),
                            Email = reader.GetString("email"),
                            StudentId = reader.IsDBNull(reader.GetOrdinal("student_id")) ? null : reader.GetString("student_id"),
                            MonthlyAllowance = reader.GetDecimal("monthly_allowance"),
                            CreatedAt = reader.GetDateTime("created_at")
                        });
                    }
                }
            }
            return users;
        }

        public object GetSystemStats()
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                
                // Count total users (excluding admins)
                int totalUsers = 0;
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE email NOT LIKE '%@admin.com'", conn))
                {
                    totalUsers = Convert.ToInt32(cmd.ExecuteScalar());
                }
                
                // Count total expenses
                int totalExpenses = 0;
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM expenses", conn))
                {
                    totalExpenses = Convert.ToInt32(cmd.ExecuteScalar());
                }
                
                // Sum total amount spent
                decimal totalSpent = 0;
                using (var cmd = new MySqlCommand("SELECT COALESCE(SUM(amount), 0) FROM expenses", conn))
                {
                    totalSpent = Convert.ToDecimal(cmd.ExecuteScalar());
                }
                
                // Count total budgets
                int totalBudgets = 0;
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM budgets", conn))
                {
                    totalBudgets = Convert.ToInt32(cmd.ExecuteScalar());
                }
                
                return new
                {
                    totalUsers,
                    totalExpenses,
                    totalSpent,
                    totalBudgets
                };
            }
        }

        public bool UpdateUserProfile(int userId, string firstName, string lastName, string? studentId)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE users 
                               SET first_name = @firstName, 
                                   last_name = @lastName, 
                                   student_id = @studentId 
                               WHERE user_id = @userId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@studentId", (object?)studentId ?? DBNull.Value);
                    
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteUser(int userId)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM users WHERE user_id = @userId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}