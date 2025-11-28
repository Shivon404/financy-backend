using Microsoft.AspNetCore.Mvc;
using FinancyAPI.Services;
using FinancyAPI.DTOs;

namespace FinancyAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserService _userService;

        public AdminController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsers(); // returns list of users
                return Ok(new { success = true, users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/admin/users/{id}
        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                bool updated = _userService.UpdateUser(id, request);
                if (updated)
                    return Ok(new { success = true, message = "User updated successfully" });
                
                return Ok(new { success = false, message = "Failed to update user" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/admin/users/{id}/status
        [HttpPut("users/{id}/status")]
        public IActionResult ToggleUserStatus(int id)
        {
            try
            {
                bool toggled = _userService.ToggleUserStatus(id);
                if (toggled)
                    return Ok(new { success = true, message = "User status updated successfully" });

                return Ok(new { success = false, message = "Failed to update status" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

}

