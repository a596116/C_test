# 專案啟動指南

## 步驟 1：安裝 .NET SDK

### macOS 安裝方式

#### 方法 1：使用 Homebrew（推薦）
```bash
brew install dotnet
```

#### 方法 2：官方安裝程式
1. 訪問 https://dotnet.microsoft.com/download
2. 下載 .NET 8.0 SDK for macOS
3. 執行安裝程式並完成安裝

#### 方法 3：使用安裝腳本
```bash
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0
```

### 驗證安裝
安裝完成後，在終端機執行：
```bash
dotnet --version
```
應該會顯示版本號（例如：8.0.xxx）

---

## 步驟 2：還原專案依賴

在專案目錄下執行：
```bash
dotnet restore
```

---

## 步驟 3：啟動專案

### 開發模式啟動
```bash
dotnet run
```

### 指定環境變數啟動
```bash
# 開發環境
ASPNETCORE_ENVIRONMENT=Development dotnet run

# 生產環境
ASPNETCORE_ENVIRONMENT=Production dotnet run
```

---

## 步驟 4：訪問 API

啟動成功後，您會看到類似以下的輸出：
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

### 訪問 Swagger UI
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

### API 端點
- **登入 API**: POST https://localhost:5001/api/auth/login
- **Swagger JSON**: https://localhost:5001/swagger/v1/swagger.json

---

## 步驟 5：測試 API

### 使用 Swagger UI（推薦）
1. 在瀏覽器打開 https://localhost:5001
2. 找到 `POST /api/auth/login` 端點
3. 點擊 "Try it out"
4. 輸入測試帳號：
   ```json
   {
     "username": "admin",
     "password": "admin123"
   }
   ```
5. 點擊 "Execute"
6. 複製返回的 Token
7. 點擊右上角 "Authorize" 按鈕
8. 輸入 `Bearer {你的token}` 格式的 Token

### 使用 curl 命令
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}' \
  -k
```

### 使用 PowerShell（Windows）
```powershell
Invoke-RestMethod -Uri "https://localhost:5001/api/auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body '{"username":"admin","password":"admin123"}' `
  -SkipCertificateCheck
```

---

## 常見問題

### 1. 端口已被佔用
如果 5000 或 5001 端口已被使用，可以修改 `Properties/launchSettings.json` 或使用：
```bash
dotnet run --urls "http://localhost:5002;https://localhost:5003"
```

### 2. HTTPS 憑證錯誤
開發環境中，瀏覽器可能會顯示憑證警告，這是正常的。點擊「進階」→「繼續前往」即可。

### 3. 找不到 dotnet 命令
- 確認已正確安裝 .NET SDK
- 重新啟動終端機
- 檢查 PATH 環境變數是否包含 .NET 安裝路徑

---

## 其他有用的命令

### 建置專案（不執行）
```bash
dotnet build
```

### 發佈專案
```bash
dotnet publish -c Release -o ./publish
```

### 執行測試（如果有測試專案）
```bash
dotnet test
```

### 查看專案資訊
```bash
dotnet --info
```

