using Microsoft.AspNetCore.Mvc;
using FinancyAPI.Services;

namespace FinancyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ExpenseService _expenseService;
        private readonly CategoryService _categoryService;

        public AdminController(UserService userService, ExpenseService expenseService, CategoryService categoryService)
        {
            _userService = userService;
            _expenseService = expenseService;
            _categoryService = categoryService;
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return Ok(new { success = true, users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("stats")]
        public IActionResult GetSystemStats()
        {
            try
            {
                var stats = _userService.GetSystemStats();
                return Ok(new { success = true, stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ===== CATEGORY MANAGEMENT ENDPOINTS =====

        [HttpGet("categories")]
        public IActionResult GetAllCategories()
        {
            try
            {
                var categories = _categoryService.GetAllCategoriesWithUsage();
                return Ok(new { success = true, categories });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("categories")]
        public IActionResult CreateCategory([FromBody] CategoryRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new { success = false, message = "Category name is required" });
                }

                if (string.IsNullOrWhiteSpace(request.Icon))
                {
                    return BadRequest(new { success = false, message = "Category icon is required" });
                }

                bool result = _categoryService.CreateCategory(request.Name.Trim(), request.Icon);
                
                if (result)
                    return Ok(new { success = true, message = "Category created successfully" });
                
                return StatusCode(500, new { success = false, message = "Failed to create category" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("categories/{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new { success = false, message = "Category name is required" });
                }

                if (string.IsNullOrWhiteSpace(request.Icon))
                {
                    return BadRequest(new { success = false, message = "Category icon is required" });
                }

                // Check if category exists
                var category = _categoryService.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound(new { success = false, message = "Category not found" });
                }

                bool result = _categoryService.UpdateCategory(id, request.Name.Trim(), request.Icon);
                
                if (result)
                    return Ok(new { success = true, message = "Category updated successfully" });
                
                return StatusCode(500, new { success = false, message = "Failed to update category" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("categories/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                // Check if category exists
                var category = _categoryService.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound(new { success = false, message = "Category not found" });
                }

                bool result = _categoryService.DeleteCategory(id);
                
                if (result)
                    return Ok(new { success = true, message = "Category deleted successfully" });
                
                return BadRequest(new { success = false, message = "Cannot delete category that is being used in expenses" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Helper class for category requests
        public class CategoryRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public bool Active { get; set; } = true;
        }
    }
}