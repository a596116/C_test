# Login API 專案

這是一個使用 ASP.NET Core 8.0 建立的登入 API 專案。

## 功能特色

- JWT Token 認證
- RESTful API 設計
- Swagger UI 文檔
- 錯誤處理和日誌記錄

## 專案結構

```
LoginApi/
├── Controllers/
│   └── AuthController.cs    # 認證控制器
├── Models/
│   ├── LoginRequest.cs      # 登入請求模型
│   └── LoginResponse.cs     # 登入回應模型
├── Program.cs               # 應用程式入口點
├── appsettings.json         # 應用程式配置
└── LoginApi.csproj         # 專案文件
```

## 安裝與執行

### 前置需求

- .NET 8.0 SDK 或更高版本

### 安裝步驟

1. 還原 NuGet 套件：
```bash
dotnet restore
```

2. 執行應用程式：
```bash
dotnet run
```

3. 應用程式將在以下 URL 啟動：
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`

4. 開啟 Swagger UI：
   - `https://localhost:5001/swagger`

## API 端點

### POST /api/auth/login

登入 API，驗證使用者憑證並返回 JWT Token。

**請求範例：**
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**成功回應（200）：**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "登入成功",
  "expiresAt": "2024-01-01T12:00:00Z"
}
```

**失敗回應（401）：**
```json
{
  "success": false,
  "token": null,
  "message": "使用者名稱或密碼錯誤",
  "expiresAt": null
}
```

## 測試帳號

專案內建以下測試帳號：

- 使用者名稱: `admin`, 密碼: `admin123`
- 使用者名稱: `user`, 密碼: `user123`
- 使用者名稱: `test`, 密碼: `test123`

## 配置說明

JWT 設定可以在 `appsettings.json` 中修改：

```json
{
  "JwtSettings": {
    "SecretKey": "您的密鑰（至少32個字元）",
    "Issuer": "LoginApi",
    "Audience": "LoginApiUsers",
    "ExpirationMinutes": "60"
  }
}
```

## 注意事項

- 此專案使用簡單的記憶體字典來儲存用戶資料，實際應用中應該使用資料庫
- 密碼應該進行雜湊處理，不應以明文儲存
- 生產環境中應該使用更安全的密鑰管理方式
- 建議實作密碼重設、刷新 Token 等功能

