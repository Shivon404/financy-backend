using FinancyAPI.Models;
using MySql.Data.MySqlClient;

namespace FinancyAPI.Services
{
    public class CategoryService
    {
        private readonly DatabaseService _dbService;

        public CategoryService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        // Get all categories with usage count
        public List<object> GetAllCategoriesWithUsage()
        {
            var categories = new List<object>();
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT 
                        c.category_id,
                        c.category_name,
                        c.icon,
                        COUNT(e.expense_id) as usage_count
                    FROM categories c
                    LEFT JOIN expenses e ON c.category_id = e.category_id
                    GROUP BY c.category_id, c.category_name, c.icon
                    ORDER BY c.category_name";
                
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int usageCount = reader.GetInt32("usage_count");
                        categories.Add(new
                        {
                            id = reader.GetInt32("category_id"),
                            categoryId = reader.GetInt32("category_id"),
                            name = reader.GetString("category_name"),
                            categoryName = reader.GetString("category_name"),
                            icon = reader.GetString("icon"),
                            usage = usageCount,
                            count = usageCount,
                            active = usageCount > 0,  // Active if used, Inactive if not
                            isActive = usageCount > 0
                        });
                    }
                }
            }
            return categories;
        }

        // Create a new category
        public bool CreateCategory(string name, string icon)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO categories (category_name, icon) 
                               VALUES (@name, @icon)";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@icon", icon);
                    
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Update an existing category
        public bool UpdateCategory(int categoryId, string name, string icon)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE categories 
                               SET category_name = @name, icon = @icon 
                               WHERE category_id = @categoryId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@categoryId", categoryId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@icon", icon);
                    
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Delete a category
        public bool DeleteCategory(int categoryId)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                
                // Check if category is being used in expenses
                string checkQuery = "SELECT COUNT(*) FROM expenses WHERE category_id = @categoryId";
                using (var checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@categoryId", categoryId);
                    int usageCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                    
                    if (usageCount > 0)
                    {
                        // Category is in use, cannot delete
                        return false;
                    }
                }
                
                // Delete the category
                string deleteQuery = "DELETE FROM categories WHERE category_id = @categoryId";
                using (var cmd = new MySqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@categoryId", categoryId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Get category by ID
        public Category? GetCategoryById(int categoryId)
        {
            using (var conn = _dbService.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM categories WHERE category_id = @categoryId";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@categoryId", categoryId);
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Category
                            {
                                CategoryId = reader.GetInt32("category_id"),
                                CategoryName = reader.GetString("category_name"),
                                Icon = reader.GetString("icon")
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}