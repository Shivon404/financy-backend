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
                        isAdmin = user.IsAdmin,
                        user = new
                        {
                            user.UserId,
                            user.FirstName,
                            user.LastName,
                            user.Email,
                            user.StudentId,
                            user.MonthlyAllowance,
                            user.FullName,
                            user.IsAdmin
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

        [HttpPut("update-profile")]
        public IActionResult UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                bool success = _userService.UpdateUserProfile(
                    request.UserId,
                    request.FirstName,
                    request.LastName,
                    request.StudentId
                );

                if (success)
                {
                    var updatedUser = _userService.GetUserById(request.UserId);

                    return Ok(new
                    {
                        success = true,
                        message = "Profile updated successfully!",
                        user = new
                        {
                            updatedUser.UserId,
                            updatedUser.FirstName,
                            updatedUser.LastName,
                            updatedUser.Email,
                            updatedUser.StudentId,
                            updatedUser.MonthlyAllowance,
                            updatedUser.FullName,
                            updatedUser.IsAdmin
                        }
                    });
                }

                return Ok(new { success = false, message = "Failed to update profile" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete-account/{userId}")]
        public IActionResult DeleteAccount(int userId)
        {
            try
            {
                bool success = _userService.DeleteUser(userId);

                if (success)
                {
                    return Ok(new { success = true, message = "Account deleted successfully" });
                }

                return Ok(new { success = false, message = "Failed to delete account" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class UpdateProfileRequest
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? StudentId { get; set; }
    }

    public class UpdateAllowanceRequest
    {
        public int UserId { get; set; }
        public decimal MonthlyAllowance { get; set; }
    }
}
