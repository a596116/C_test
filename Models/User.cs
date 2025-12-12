using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginApi.Models;

/// <summary>
/// 使用者模型
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// 使用者 ID（主鍵）
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// 使用者名稱（唯一）
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密碼雜湊值
    /// </summary>
    [Required]
    [MaxLength(255)]
    [Column(TypeName = "varchar(255)")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件（唯一，可選）
    /// </summary>
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string? Email { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最後更新時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }
}

