using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AccountingApi.Models;
// 🌟 1. 新增：引入 JWT 與安全性必備的命名空間
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AccountingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        // 🌟 2. 新增：宣告一個讀取設定檔的欄位
        private readonly IConfiguration _configuration;

        // 🌟 2. 修改：在建構函式中注入 IConfiguration
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; // 讓 Controller 拿得到 appsettings.json 的設定
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
                PasswordHash = passwordHash,
                Role = "User" // 🌟 順便確保新註冊的人預設角色都是一般 User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("註冊成功！可以去登入囉！");
        }

        // 🌟 3. 重點修改：改寫後的 Login 方法
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            // A. 先找出使用者
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            // B. 驗證帳號是否存在、密碼是否正確
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("帳號或密碼錯誤喔！");
            }

            // C. 帳密正確！開始現場製作 JWT 數位手環

            // 寫在手環上的公開貼紙（包含 Id, 帳號, 角色權限）
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User") // 把資料庫裡的 Role（Admin/User）塞進來
            };

            // 從 appsettings.json 讀取我們設定的秘密鑰匙
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 配置 Token 的詳細發行資訊
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), // 手環有效期限 1 天
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken); // 產生長字串 Token

            // D. 回傳黃金數位通行證給前端
            return Ok(new
            {
                Token = tokenString,
                Username = user.Username,
                Role = user.Role ?? "User"
            });
        }

        // 1. 取得所有使用者清單
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.Select(u => new { u.Id, u.Username, u.Role }).ToListAsync();
            return Ok(users);
        }

        // 2. 重設特定使用者密碼
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return NotFound("找不到該使用者");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("0000"); // 重設為預設密碼
            await _context.SaveChangesAsync();
            return Ok($"{dto.Username} 的密碼已重設為 0000");
        }
    }

    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string Username { get; set; } = string.Empty;
    }
}