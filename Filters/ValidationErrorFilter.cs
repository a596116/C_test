using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using LoginApi.Models;

namespace LoginApi.Filters;

/// <summary>
/// 驗證錯誤過濾器，統一處理模型驗證錯誤
/// </summary>
public class ValidationErrorFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var keyValuePair in context.ModelState)
            {
                var key = keyValuePair.Key;
                var errorsList = keyValuePair.Value.Errors
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                if (errorsList.Length > 0)
                {
                    errors[key] = errorsList;
                }
            }

            var response = new ApiResponse
            {
                StatusCode = 422,
                Success = false,
                Message = "驗證失敗",
                Errors = errors
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = 422
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // 不需要處理
    }
}

