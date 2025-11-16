using Microsoft.AspNetCore.Mvc;
using FinancyAPI.Services;
using FinancyAPI.Models;

namespace FinancyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetController : ControllerBase
    {
        private readonly BudgetService _budgetService;

        public BudgetController(BudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet("{userId}")]
        public IActionResult GetBudgets(int userId, [FromQuery] string? month)
        {
            try
            {
                DateTime? targetMonth = string.IsNullOrEmpty(month) ? null : DateTime.Parse(month);
                var budgets = _budgetService.GetBudgetsByUserId(userId, targetMonth);
                return Ok(new { success = true, budgets });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult SetBudget([FromBody] Budget budget)
        {
            try
            {
                bool success = _budgetService.SetBudget(budget);
                if (success)
                    return Ok(new { success = true, message = "Budget set successfully!" });
                return Ok(new { success = false, message = "Failed to set budget" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{budgetId}/{userId}")]
        public IActionResult DeleteBudget(int budgetId, int userId)
        {
            try
            {
                bool success = _budgetService.DeleteBudget(budgetId, userId);
                if (success)
                    return Ok(new { success = true, message = "Budget deleted successfully!" });
                return Ok(new { success = false, message = "Failed to delete budget" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}