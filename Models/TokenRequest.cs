using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models;

/// <summary>
/// OAuth2 Token 請求模型
/// </summary>
public class TokenRequest
{
    /// <summary>
    /// 授權類型（必須是 "password"）
    /// </summary>
    [Required]
    public string grant_type { get; set; } = "password";

    /// <summary>
    /// 使用者名稱
    /// </summary>
    [Required]
    public string username { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required]
    public string password { get; set; } = string.Empty;
}

