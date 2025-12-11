namespace LoginApi.Models;

/// <summary>
/// 登入回應模型
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// 是否登入成功
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// JWT Token（僅在登入成功時返回）
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string? Token { get; set; }

    /// <summary>
    /// 回應訊息
    /// </summary>
    /// <example>登入成功</example>
    public string? Message { get; set; }

    /// <summary>
    /// Token 過期時間（UTC）
    /// </summary>
    /// <example>2024-01-01T12:00:00Z</example>
    public DateTime? ExpiresAt { get; set; }
}

