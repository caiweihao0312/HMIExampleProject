# MotionCtrlDemoApp

一个使用 C# 和 P/Invoke 调用运动控制卡 SDK 的完整示例项目，演示：

- 打开/关闭设备  
- 设置运动参量（速度、加速度）  
- 单轴绝对/相对运动和回零  
- 多轴同步启动  
- 轴状态监控（运动中/报警/限位）  
- I/O 控制（DI/DO）  

## 要求

- .NET 6.0 SDK 或更高  
- 控制卡厂商提供的 `MotionCardAPI.dll`（32/64 位需与应用匹配）  
- Windows 操作系统  

## 目录结构

```
MotionCtrlDemoApp/
├─ MotionCtrlDemoApp.csproj
├─ README.md
└─ Program.cs
```

## 构建与运行

1. 将厂商的 `MotionCardAPI.dll` 放到 `bin/Debug/net6.0/` 目录下。  
2. 在项目根目录执行：  
   ```
   dotnet build
   dotnet run
   ```  
3. 根据控制卡手册修改 `MotionController.cs` 中的 DLL 名称和函数签名即可。