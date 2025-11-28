using Microsoft.AspNetCore.Mvc;
using FinancyAPI.Services;
using FinancyAPI.DTOs;

namespace FinancyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = _userService.Login(request);
                if (user != null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Login successful!",
                        user = new
                        {
                            user.UserId,
                            user.FirstName,
                            user.LastName,
                            user.Email,
                            user.StudentId,
                            user.MonthlyAllowance,
                            user.Role,
                            user.FullName
                        }
                    });
                }
                return Ok(new { success = false, message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                bool success = _userService.Register(request);
                if (success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Registration successful! You can now login."
                    });
                }
                return Ok(new { success = false, message = "Email already exists!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPut("update-allowance")]
        public IActionResult UpdateAllowance([FromBody] UpdateAllowanceRequest request)
        {
            try
            {
                bool success = _userService.UpdateMonthlyAllowance(request.UserId, request.MonthlyAllowance);
                if (success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Monthly allowance updated successfully!"
                    });
                }
                return Ok(new { success = false, message = "Failed to update allowance" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class UpdateAllowanceRequest
    {
        public int UserId { get; set; }
        public decimal MonthlyAllowance { get; set; }
    
    }
}