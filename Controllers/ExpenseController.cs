using Microsoft.AspNetCore.Mvc;
using FinancyAPI.Services;
using FinancyAPI.Models;

namespace FinancyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseService _expenseService;

        public ExpenseController(ExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet("{userId}")]
        public IActionResult GetExpenses(int userId, [FromQuery] int? categoryId, [FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            try
            {
                DateTime? start = string.IsNullOrEmpty(startDate) ? null : DateTime.Parse(startDate);
                DateTime? end = string.IsNullOrEmpty(endDate) ? null : DateTime.Parse(endDate);
                
                var expenses = _expenseService.GetExpensesByUserId(userId, categoryId, start, end);
                return Ok(new { success = true, expenses });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddExpense([FromBody] Expense expense)
        {
            try
            {
                bool success = _expenseService.AddExpense(expense);
                if (success)
                    return Ok(new { success = true, message = "Expense added successfully!" });
                return Ok(new { success = false, message = "Failed to add expense" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult UpdateExpense([FromBody] Expense expense)
        {
            try
            {
                bool success = _expenseService.UpdateExpense(expense);
                if (success)
                    return Ok(new { success = true, message = "Expense updated successfully!" });
                return Ok(new { success = false, message = "Failed to update expense" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{expenseId}/{userId}")]
        public IActionResult DeleteExpense(int expenseId, int userId)
        {
            try
            {
                bool success = _expenseService.DeleteExpense(expenseId, userId);
                if (success)
                    return Ok(new { success = true, message = "Expense deleted successfully!" });
                return Ok(new { success = false, message = "Failed to delete expense" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _expenseService.GetCategories();
                return Ok(new { success = true, categories });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}