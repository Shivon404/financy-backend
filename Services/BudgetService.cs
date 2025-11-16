using FinancyAPI.Models;
using MySql.Data.MySqlClient;

namespace FinancyAPI.Services
{
    public class BudgetService
    {
        private readonly DatabaseService _dbService;

        public BudgetService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public List<Budget> GetBudgetsByUserId(int userId, DateTime? month = null)
        {
            var budgets = new List<Budget>();
            var targetMonth = month ?? DateTime.Now;
            var firstDayOfMonth = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                
                // Get the latest budget for each category
                string query = @"SELECT b.*, c.category_name, c.icon,
                               COALESCE(SUM(e.amount), 0) as spent_amount
                               FROM budgets b
                               JOIN categories c ON b.category_id = c.category_id
                               LEFT JOIN expenses e ON e.category_id = b.category_id 
                                   AND e.user_id = b.user_id 
                                   AND YEAR(e.expense_date) = YEAR(@month) 
                                   AND MONTH(e.expense_date) = MONTH(@month)
                               WHERE b.user_id = @userId 
                               AND b.budget_id IN (
                                   SELECT MAX(budget_id) 
                                   FROM budgets 
                                   WHERE user_id = @userId 
                                   GROUP BY category_id
                               )
                               GROUP BY b.budget_id, b.user_id, b.category_id, b.budget_limit, 
                                        b.month, b.created_at, c.category_name, c.icon
                               ORDER BY c.category_name";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@month", firstDayOfMonth);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            budgets.Add(new Budget
                            {
                                BudgetId = reader.GetInt32("budget_id"),
                                UserId = reader.GetInt32("user_id"),
                                CategoryId = reader.GetInt32("category_id"),
                                BudgetLimit = reader.GetDecimal("budget_limit"),
                                Month = reader.GetDateTime("month"),
                                SpentAmount = reader.GetDecimal("spent_amount"),
                                CategoryName = reader.GetString("category_name"),
                                CategoryIcon = reader.GetString("icon"),
                                CreatedAt = reader.GetDateTime("created_at")
                            });
                        }
                    }
                }
            }
            return budgets;
        }

        public bool SetBudget(Budget budget)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                
                // Check if budget exists for this category and month
                string checkQuery = @"SELECT budget_id FROM budgets 
                                     WHERE user_id = @userId AND category_id = @categoryId AND month = @month";
                
                int? existingBudgetId = null;
                using (var checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@userId", budget.UserId);
                    checkCmd.Parameters.AddWithValue("@categoryId", budget.CategoryId);
                    checkCmd.Parameters.AddWithValue("@month", budget.Month);
                    
                    var result = checkCmd.ExecuteScalar();
                    if (result != null)
                        existingBudgetId = Convert.ToInt32(result);
                }
                
                if (existingBudgetId.HasValue)
                {
                    // Update existing budget
                    string updateQuery = "UPDATE budgets SET budget_limit = @limit WHERE budget_id = @budgetId";
                    using (var cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@budgetId", existingBudgetId.Value);
                        cmd.Parameters.AddWithValue("@limit", budget.BudgetLimit);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                else
                {
                    // Insert new budget
                    string insertQuery = @"INSERT INTO budgets (user_id, category_id, budget_limit, month) 
                                         VALUES (@userId, @categoryId, @limit, @month)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", budget.UserId);
                        cmd.Parameters.AddWithValue("@categoryId", budget.CategoryId);
                        cmd.Parameters.AddWithValue("@limit", budget.BudgetLimit);
                        cmd.Parameters.AddWithValue("@month", budget.Month);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
        }

        public bool DeleteBudget(int budgetId, int userId)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM budgets WHERE budget_id = @budgetId AND user_id = @userId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@budgetId", budgetId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}