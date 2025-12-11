#!/bin/bash

echo "=========================================="
echo "Login API 啟動腳本"
echo "=========================================="
echo ""

# 檢查 .NET SDK 是否安裝
if ! command -v dotnet &> /dev/null
then
    echo "❌ 錯誤：未找到 .NET SDK"
    echo ""
    echo "請先安裝 .NET SDK："
    echo "  macOS: brew install dotnet"
    echo "  或訪問: https://dotnet.microsoft.com/download"
    echo ""
    exit 1
fi

echo "✅ .NET SDK 版本："
dotnet --version
echo ""

# 還原依賴
echo "📦 還原專案依賴..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "❌ 依賴還原失敗"
    exit 1
fi
echo ""

# 建置專案
echo "🔨 建置專案..."
dotnet build
if [ $? -ne 0 ]; then
    echo "❌ 專案建置失敗"
    exit 1
fi
echo ""

# 啟動專案
echo "🚀 啟動 API 伺服器..."
echo ""
echo "=========================================="
echo "API 將在以下網址啟動："
echo "  HTTP:  http://localhost:5000"
echo "  HTTPS: https://localhost:5001"
echo ""
echo "Swagger UI: https://localhost:5001"
echo "=========================================="
echo ""
echo "按 Ctrl+C 停止伺服器"
echo ""

dotnet run

