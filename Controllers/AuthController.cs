using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginApi.Models;

namespace LoginApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    // 簡單的用戶資料庫（實際應用中應該使用真實的資料庫）
    private readonly Dictionary<string, string> _users = new()
    {
        { "admin", "admin123" },
        { "user", "user123" },
        { "test", "test123" }
    };

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
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
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            // 模型驗證由 ValidationErrorFilter 自動處理

            // 驗證用戶憑證（實際應用中應該從資料庫查詢）
            if (!_users.ContainsKey(request.Username) || _users[request.Username] != request.Password)
            {
                _logger.LogWarning($"登入失敗：使用者名稱 {request.Username}");
                return Unauthorized(new ApiResponse
                {
                    StatusCode = 401,
                    Success = false,
                    Message = "使用者名稱或密碼錯誤"
                });
            }

            // 生成 JWT Token
            var token = GenerateJwtToken(request.Username);

            _logger.LogInformation($"使用者 {request.Username} 登入成功");

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
    public IActionResult Token([FromForm] TokenRequest request)
    {
        try
        {
            // 驗證 grant_type
            if (request.grant_type != "password")
            {
                return BadRequest(new { error = "unsupported_grant_type", error_description = "只支援 password grant type" });
            }

            // 驗證用戶憑證
            if (!_users.ContainsKey(request.username) || _users[request.username] != request.password)
            {
                _logger.LogWarning($"Token 請求失敗：使用者名稱 {request.username}");
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

