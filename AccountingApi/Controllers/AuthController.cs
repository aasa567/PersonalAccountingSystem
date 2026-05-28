using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccountingApi.Models;

namespace AccountingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        // 修復後的建構函式
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("這個名稱已經有人用了喔！ (๑´ㅂ`๑)");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("註冊成功！可以去登入囉！");
        }

        // 登入方法
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("帳號或密碼錯誤喔！");
            }

            return Ok(new { message = "登入成功！", username = user.Username });
        }

        // 1. 取得所有使用者清單
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.Select(u => new { u.Id, u.Username }).ToListAsync();
            return Ok(users);
        }

        // 2. 重設特定使用者密碼
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return NotFound("找不到該使用者");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("0000") ; // 暫時重設為預設密碼
            await _context.SaveChangesAsync();
            return Ok($"{dto.Username} 的密碼已重設為 0000");
        }
    }

    // UserDto 放在 namespace 內，Controller 之外
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }  

    public class ResetPasswordDto
    {
        public string Username { get; set; }
    }
}