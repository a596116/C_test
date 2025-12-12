using System.ComponentModel.DataAnnotations;

namespace LoginApi.Models;

/// <summary>
/// 註冊請求模型
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// 使用者名稱
    /// </summary>
    /// <example>newuser</example>
    [Required(ErrorMessage = "使用者名稱為必填欄位")]
    [MinLength(3, ErrorMessage = "使用者名稱長度至少需要 3 個字元")]
    [MaxLength(50, ErrorMessage = "使用者名稱長度不能超過 50 個字元")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "使用者名稱只能包含英文字母、數字和底線")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    /// <example>password123</example>
    [Required(ErrorMessage = "密碼為必填欄位")]
    [MinLength(6, ErrorMessage = "密碼長度至少需要 6 個字元")]
    [MaxLength(100, ErrorMessage = "密碼長度不能超過 100 個字元")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 確認密碼
    /// </summary>
    /// <example>password123</example>
    [Required(ErrorMessage = "確認密碼為必填欄位")]
    [Compare("Password", ErrorMessage = "密碼與確認密碼不相符")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件（可選）
    /// </summary>
    /// <example>user@example.com</example>
    [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
    [MaxLength(100, ErrorMessage = "電子郵件長度不能超過 100 個字元")]
    public string? Email { get; set; }
}

