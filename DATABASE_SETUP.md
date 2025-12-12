# 資料庫設定說明

## 安裝 EF Core 工具（如果尚未安裝）

執行以下命令來安裝 EF Core 工具：

```bash
dotnet tool install --global dotnet-ef
```

## 建立資料庫遷移

執行以下命令來建立資料庫遷移：

```bash
dotnet ef migrations add InitialCreate
```

## 套用遷移到資料庫

執行以下命令來套用遷移並建立資料表：

```bash
dotnet ef database update
```

## 注意事項

1. 確保 MySQL 資料庫服務正在運行
2. 確認 `appsettings.json` 中的連接字串正確
3. 確保資料庫 `haodai-server-new` 已存在
4. 如果資料庫不存在，請先建立資料庫：
   ```sql
   CREATE DATABASE `haodai-server-new` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```

## API 端點

### 註冊
- **POST** `/api/auth/register`
- 請求體：
  ```json
  {
    "username": "testuser",
    "password": "password123",
    "confirmPassword": "password123",
    "email": "test@example.com"
  }
  ```

### 登入
- **POST** `/api/auth/login`
- 請求體：
  ```json
  {
    "username": "testuser",
    "password": "password123"
  }
  ```

