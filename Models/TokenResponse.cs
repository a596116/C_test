namespace LoginApi.Models;

/// <summary>
/// OAuth2 Token 回應模型
/// </summary>
public class TokenResponse
{
    /// <summary>
    /// Access Token
    /// </summary>
    public string access_token { get; set; } = string.Empty;

    /// <summary>
    /// Token 類型（通常是 "Bearer"）
    /// </summary>
    public string token_type { get; set; } = "Bearer";

    /// <summary>
    /// Token 有效期限（秒）
    /// </summary>
    public int expires_in { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? username { get; set; }
}

