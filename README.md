# HMIExampleProject
上位机示例项目



   # SerialPortTool 串口调试工具

   一个基于 WinForms 的高级串口调试工具，支持多串口连接、灵活配置、多种发送/接收选项及日志管理，帮助开发者快速验证和调试串口设备。

   ## 核心功能

   - 点击“添加串口”按钮，为每个串口创建独立的 Tab 页  
   - 在新建的 Tab 中选择 COM 口、波特率、数据位、校验位、停止位、流控，再点击“打开”开始通信，点击“关闭”断开  
   - 接收区实时显示收到的数据，可勾选“HEX接收”以十六进制格式查看，可勾选“带时间戳”在每行前加上接收时间  
   - 在底部输入要发送的内容，勾选“HEX发送”可发送十六进制字节；支持“自动发送”并设置发送间隔（ms）  
   - 支持“清空接收”“清空发送”按钮；可一键“导出日志”保存当前接收窗口内容  
   - 底部状态栏显示本页已发送字节数和已接收字节数  

   ## 系统要求

   - .NET Framework 4.7.2 或更高  
   - Visual Studio 2017 或更高  

   ## 快速开始

   1. 克隆或下载仓库，使用 Visual Studio 打开 `SerialPortTool.sln`。  
   2. 构建并运行（Ctrl+Shift+B / F5）。  
   3. 点击 **“添加串口”**，在新标签页中选择串口设置，点击 **“打开”**。  
   4. 在“接收区”查看数据，可选项“HEX接收”“带时间戳”。  
   5. 在“发送区”输入要发送的内容，或勾选“自动发送”并设置间隔，点击 **“发送”**。  
   6. 使用 **“清空接收”、“清空发送”** 按钮分别清理窗口内容；点击 **“导出日志”** 保存接收内容。  
   7. 底部状态栏实时显示本页已发送/接收字节数。  

   ## 项目结构

   - SerialPortTool.csproj — 项目文件  
   - Program.cs — 应用入口，初始化 WinForms  
   - MainForm.Designer.cs — 主界面控件布局  
   - MainForm.cs — 业务逻辑，包括串口打开/关闭、数据收发处理

   ## 截图示例

   <img width="1404" height="1022" alt="image" src="https://github.com/user-attachments/assets/a305925a-c8f6-4d1d-b010-7be3cd88a445" />



# SaleAppDemo 项目说明

## 项目简介
SaleAppDemo 是一个基于 .NET Framework 4.7.2 的分层架构示例项目，主要用于演示商品销售相关的 Web API 设计与实现。项目采用 C# 7.3 进行开发，包含数据访问、业务逻辑、领域模型和 Web API 层，适合企业级应用的基础架构学习和二次开发。

## 项目结构
- **Sale.WebAPI**  
  提供 RESTful API 接口，负责与前端或第三方系统的数据交互。包含全局异常处理、路由配置、控制器等。
- **Sale.BLL**  
  业务逻辑层，封装核心业务规则和流程。
- **Sale.DAL**  
  数据访问层，负责与数据库的数据交互。
- **Sale.Domain**  
  领域模型层，定义实体对象和基础数据结构。

## 主要功能
- 商品管理（如 ProductsController 控制器）
- 全局异常处理（ExceptionHandlingFilter 过滤器，支持自定义跳过和详细日志记录）
- 标准化的 API 错误响应
- 分层架构，便于扩展和维护

## 关键技术
- ASP.NET Web API
- C# 7.3
- .NET Framework 4.7.2
- 分层架构（DAL/BLL/Domain/WebAPI）

## 快速开始
1. 使用 Visual Studio 2022 打开解决方案。
2. 配置数据库连接字符串（如有需要）。
3. 编译并运行 Sale.WebAPI 项目。
4. 通过 Postman 或浏览器访问 API 接口（如 `/api/products`）。

## 异常处理说明
- 全局异常由 `ExceptionHandlingFilter` 统一捕获并返回标准化 JSON 响应。
- 支持通过 `SkipExceptionHandlingAttribute` 跳过指定控制器或 Action 的全局异常处理。

## 贡献与反馈
如需贡献代码或反馈问题，请提交 Pull Request 或 Issue。

---
