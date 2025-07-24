using System;
using System.Threading;
using MotionCtrlDemo;

namespace MotionCtrlDemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== 运动控制卡完整示例 ===");
            using var ctrl = new MotionController();

            if (!ctrl.Open())
            {
                Console.WriteLine("打开设备失败，请检查连接和 DLL 文件");
                return;
            }

            // 设置多轴参数
            int[] axes = { 0, 1 };
            for (int i = 0; i < axes.Length; i++)
                ctrl.SetParam(axes[i], velocity: 100.0, accel: 200.0);

            // 多轴同步绝对运动
            double[] targets = { 500.0, 300.0 };
            Console.WriteLine("发起多轴同步运动...");
            if (ctrl.GroupMoveAbs(axes, targets))
                Console.WriteLine("同步运动命令已发送");
            else
                Console.WriteLine("同步运动失败");

            // 轮询并打印状态
            for (int t = 0; t < 20; t++)
            {
                foreach (var axis in axes)
                {
                    Console.WriteLine(
                        $"轴 {axis}: Moving={ctrl.IsMoving(axis)}, " +
                        $"Alarm={ctrl.IsAlarm(axis)}, Limit={ctrl.IsLimit(axis)}");
                }
                Thread.Sleep(200);
            }

            // I/O 控制示例
            Console.WriteLine("设置 DO0=ON，读取 DI0...");
            ctrl.SetDO(0, true);
            ctrl.GetDI(0, out bool din);
            Console.WriteLine($"DI0 = {din}");

            // 停止所有轴
            foreach (var axis in axes)
                ctrl.Stop(axis);

            Console.WriteLine("示例结束，释放资源");
        }
    }
}