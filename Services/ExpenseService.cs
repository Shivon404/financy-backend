using FinancyAPI.Models;
using MySql.Data.MySqlClient;

namespace FinancyAPI.Services
{
    public class ExpenseService
    {
        private readonly DatabaseService _dbService;

        public ExpenseService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public List<Expense> GetExpensesByUserId(int userId, int? categoryId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var expenses = new List<Expense>();
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = @"SELECT e.*, c.category_name, c.icon 
                               FROM expenses e 
                               JOIN categories c ON e.category_id = c.category_id 
                               WHERE e.user_id = @userId";
                
                if (categoryId.HasValue)
                    query += " AND e.category_id = @categoryId";
                if (startDate.HasValue)
                    query += " AND e.expense_date >= @startDate";
                if (endDate.HasValue)
                    query += " AND e.expense_date <= @endDate";
                    
                query += " ORDER BY e.expense_date DESC, e.created_at DESC";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    if (categoryId.HasValue)
                        cmd.Parameters.AddWithValue("@categoryId", categoryId.Value);
                    if (startDate.HasValue)
                        cmd.Parameters.AddWithValue("@startDate", startDate.Value);
                    if (endDate.HasValue)
                        cmd.Parameters.AddWithValue("@endDate", endDate.Value);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            expenses.Add(new Expense
                            {
                                ExpenseId = reader.GetInt32("expense_id"),
                                UserId = reader.GetInt32("user_id"),
                                CategoryId = reader.GetInt32("category_id"),
                                Amount = reader.GetDecimal("amount"),
                                ExpenseDate = reader.GetDateTime("expense_date"),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                                CategoryName = reader.GetString("category_name"),
                                CategoryIcon = reader.GetString("icon"),
                                CreatedAt = reader.GetDateTime("created_at")
                            });
                        }
                    }
                }
            }
            return expenses;
        }

        public bool AddExpense(Expense expense)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO expenses (user_id, category_id, amount, expense_date, description) 
                               VALUES (@userId, @categoryId, @amount, @date, @description)";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", expense.UserId);
                    cmd.Parameters.AddWithValue("@categoryId", expense.CategoryId);
                    cmd.Parameters.AddWithValue("@amount", expense.Amount);
                    cmd.Parameters.AddWithValue("@date", expense.ExpenseDate);
                    cmd.Parameters.AddWithValue("@description", (object?)expense.Description ?? DBNull.Value);
                    
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateExpense(Expense expense)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE expenses 
                               SET category_id = @categoryId, amount = @amount, 
                                   expense_date = @date, description = @description 
                               WHERE expense_id = @expenseId AND user_id = @userId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@expenseId", expense.ExpenseId);
                    cmd.Parameters.AddWithValue("@userId", expense.UserId);
                    cmd.Parameters.AddWithValue("@categoryId", expense.CategoryId);
                    cmd.Parameters.AddWithValue("@amount", expense.Amount);
                    cmd.Parameters.AddWithValue("@date", expense.ExpenseDate);
                    cmd.Parameters.AddWithValue("@description", (object?)expense.Description ?? DBNull.Value);
                    
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteExpense(int expenseId, int userId)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM expenses WHERE expense_id = @expenseId AND user_id = @userId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@expenseId", expenseId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Category> GetCategories()
        {
            var categories = new List<Category>();
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM categories";
                
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            CategoryId = reader.GetInt32("category_id"),
                            CategoryName = reader.GetString("category_name"),
                            Icon = reader.GetString("icon")
                        });
                    }
                }
            }
            return categories;
        }
    }
}