namespace FinancyAPI.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public decimal BudgetLimit { get; set; }
        public DateTime Month { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Computed properties
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount => BudgetLimit - SpentAmount;
        public double SpentPercentage => BudgetLimit > 0 ? (double)(SpentAmount / BudgetLimit * 100) : 0;
        public string Status
        {
            get
            {
                if (SpentPercentage >= 100) return "Over Budget";
                if (SpentPercentage >= 80) return "Warning";
                return "On Track";
            }
        }
        
        // Navigation properties
        public string? CategoryName { get; set; }
        public string? CategoryIcon { get; set; }
    }
}