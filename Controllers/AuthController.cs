using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginApi.Data;
using LoginApi.Models;

namespace LoginApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly ApplicationDbContext _context;

    public AuthController(
        IConfiguration configuration, 
        ILogger<AuthController> logger,
        ApplicationDbContext context)
    {
        _configuration = configuration;
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// 使用者註冊
    /// </summary>
    /// <param name="request">註冊請求，包含使用者名稱、密碼和電子郵件</param>
    /// <returns>註冊結果</returns>
    /// <response code="200">註冊成功</response>
    /// <response code="422">驗證失敗</response>
    /// <response code="409">使用者名稱或電子郵件已存在</response>
    /// <response code="500">伺服器內部錯誤</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // 模型驗證由 ValidationErrorFilter 自動處理

            // 檢查使用者名稱是否已存在
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);
            
            if (existingUser != null)
            {
                _logger.LogWarning($"註冊失敗：使用者名稱 {request.Username} 已存在");
                return Conflict(new ApiResponse
                {
                    StatusCode = 409,
                    Success = false,
                    Message = "使用者名稱已存在"
                });
            }

            // 檢查電子郵件是否已存在（如果提供）
            if (!string.IsNullOrEmpty(request.Email))
            {
                var existingEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);
                
                if (existingEmail != null)
                {
                    _logger.LogWarning($"註冊失敗：電子郵件 {request.Email} 已存在");
                    return Conflict(new ApiResponse
                    {
                        StatusCode = 409,
                        Success = false,
                        Message = "電子郵件已被使用"
                    });
                }
            }

            // 建立新使用者
            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Email = request.Email,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 生成 JWT Token
            var token = GenerateJwtToken(user.Username);

            _logger.LogInformation($"使用者 {user.Username} 註冊成功");

            return Ok(new ApiResponse<LoginResponse>
            {
                StatusCode = 200,
                Success = true,
                Message = "註冊成功",
                Data = new LoginResponse
                {
                    Success = true,
                    Token = token,
                    Message = "註冊成功",
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "註冊過程中發生錯誤");
            return StatusCode(500, new ApiResponse
            {
                StatusCode = 500,
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="request">登入請求，包含使用者名稱和密碼</param>
    /// <returns>登入結果，包含 JWT Token</returns>
    /// <response code="200">登入成功，返回 JWT Token</response>
    /// <response code="422">驗證失敗</response>
    /// <response code="401">使用者名稱或密碼錯誤</response>
    /// <response code="500">伺服器內部錯誤</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // 模型驗證由 ValidationErrorFilter 自動處理

            // 從資料庫查詢使用者
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                _logger.LogWarning($"登入失敗：使用者名稱 {request.Username} 不存在");
                return Unauthorized(new ApiResponse
                {
                    StatusCode = 401,
                    Success = false,
                    Message = "使用者名稱或密碼錯誤"
                });
            }

            // 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning($"登入失敗：使用者名稱 {request.Username} 密碼錯誤");
                return Unauthorized(new ApiResponse
                {
                    StatusCode = 401,
                    Success = false,
                    Message = "使用者名稱或密碼錯誤"
                });
            }

            // 更新最後更新時間
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // 生成 JWT Token
            var token = GenerateJwtToken(user.Username);

            _logger.LogInformation($"使用者 {user.Username} 登入成功");

            return Ok(new ApiResponse<LoginResponse>
            {
                StatusCode = 200,
                Success = true,
                Message = "登入成功",
                Data = new LoginResponse
                {
                    Success = true,
                    Token = token,
                    Message = "登入成功",
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入過程中發生錯誤");
            return StatusCode(500, new ApiResponse
            {
                StatusCode = 500,
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// OAuth2 Token 端點（用於 Swagger OAuth2 認證）
    /// </summary>
    /// <param name="request">Token 請求</param>
    /// <returns>Access Token</returns>
    /// <response code="200">成功獲取 Token</response>
    /// <response code="400">請求參數錯誤</response>
    /// <response code="401">使用者名稱或密碼錯誤</response>
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Token([FromForm] TokenRequest request)
    {
        try
        {
            // 驗證 grant_type
            if (request.grant_type != "password")
            {
                return BadRequest(new { error = "unsupported_grant_type", error_description = "只支援 password grant type" });
            }

            // 從資料庫查詢使用者
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.username);

            if (user == null)
            {
                _logger.LogWarning($"Token 請求失敗：使用者名稱 {request.username} 不存在");
                return Unauthorized(new { error = "invalid_grant", error_description = "使用者名稱或密碼錯誤" });
            }

            // 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(request.password, user.PasswordHash))
            {
                _logger.LogWarning($"Token 請求失敗：使用者名稱 {request.username} 密碼錯誤");
                return Unauthorized(new { error = "invalid_grant", error_description = "使用者名稱或密碼錯誤" });
            }

            // 生成 JWT Token
            var token = GenerateJwtToken(request.username);
            var expirationMinutes = int.Parse(_configuration.GetSection("JwtSettings")["ExpirationMinutes"] ?? "60");

            _logger.LogInformation($"使用者 {request.username} 通過 OAuth2 獲取 Token 成功");

            return Ok(new TokenResponse
            {
                access_token = token,
                token_type = "Bearer",
                expires_in = expirationMinutes * 60, // 轉換為秒
                username = request.username
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token 請求過程中發生錯誤");
            return StatusCode(500, new { error = "server_error", error_description = "伺服器內部錯誤" });
        }
    }

    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";
        var issuer = jwtSettings["Issuer"] ?? "LoginApi";
        var audience = jwtSettings["Audience"] ?? "LoginApiUsers";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

