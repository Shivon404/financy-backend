namespace FinancyAPI.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties (not stored in DB, just for display)
        public string? CategoryName { get; set; }
        public string? CategoryIcon { get; set; }
    }
}