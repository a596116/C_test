namespace LoginApi.Models;

/// <summary>
/// 統一的 API 回應格式
/// </summary>
/// <typeparam name="T">資料類型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// HTTP 狀態碼
    /// </summary>
    /// <example>200</example>
    public int StatusCode { get; set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// 回應訊息
    /// </summary>
    /// <example>操作成功</example>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 回應資料
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 驗證錯誤（僅在驗證失敗時返回）
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }
}

/// <summary>
/// 統一的 API 回應格式（無資料）
/// </summary>
public class ApiResponse : ApiResponse<object>
{
}

