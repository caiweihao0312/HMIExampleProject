using System;
using System.Runtime.InteropServices;

namespace MotionCtrlDemo
{
    /// <summary>
    /// 提供运动控制卡的基本操作封装，包括设备管理、运动控制、I/O 控制等。
    /// </summary>
    public class MotionController : IDisposable
    {
        private const string DllName = "MotionCardAPI.dll";

        // 打开/关闭设备
        [DllImport(DllName, EntryPoint = "MC_OpenDevice", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_OpenDevice(int cardIndex);

        [DllImport(DllName, EntryPoint = "MC_CloseDevice", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_CloseDevice(int cardHandle);

        // 设置运动参数
        [DllImport(DllName, EntryPoint = "MC_SetMotionParam", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_SetMotionParam(int cardHandle, int axis, double velocity, double accel);

        // 单轴运动
        [DllImport(DllName, EntryPoint = "MC_MoveAbsolute", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_MoveAbsolute(int cardHandle, int axis, double position);

        [DllImport(DllName, EntryPoint = "MC_MoveRelative", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_MoveRelative(int cardHandle, int axis, double delta);

        [DllImport(DllName, EntryPoint = "MC_Home", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_Home(int cardHandle, int axis);

        [DllImport(DllName, EntryPoint = "MC_Stop", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_Stop(int cardHandle, int axis);

        // 多轴同步启动（批量写入目标位）
        [DllImport(DllName, EntryPoint = "MC_GroupMoveAbs", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_GroupMoveAbs(int cardHandle, int[] axes, double[] positions, int axisCount);

        // 查询轴状态（运动/停止/报警/限位等）
        [DllImport(DllName, EntryPoint = "MC_GetAxisStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_GetAxisStatus(int cardHandle, int axis, out int statusWord);

        // I/O 控制（输出）
        [DllImport(DllName, EntryPoint = "MC_SetDO", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_SetDO(int cardHandle, int doIndex, bool value);

        // I/O 读取（输入）
        [DllImport(DllName, EntryPoint = "MC_GetDI", CallingConvention = CallingConvention.Cdecl)]
        private static extern int MC_GetDI(int cardHandle, int diIndex, out bool value);

        private int _handle = -1; // 设备句柄，-1 表示未打开

        /// <summary>
        /// 打开运动控制卡设备。
        /// </summary>
        /// <param name="cardIndex">卡索引号，默认0</param>
        /// <returns>打开成功返回 true，否则 false</returns>
        public bool Open(int cardIndex = 0)
        {
            _handle = MC_OpenDevice(cardIndex);
            return _handle >= 0;
        }

        /// <summary>
        /// 设置指定轴的运动参数。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="velocity">速度</param>
        /// <param name="accel">加速度</param>
        /// <returns>设置成功返回 true</returns>
        public bool SetParam(int axis, double velocity, double accel)
        {
            return MC_SetMotionParam(_handle, axis, velocity, accel) == 0;
        }

        /// <summary>
        /// 绝对位置运动。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="position">目标位置</param>
        /// <returns>运动命令下发成功返回 true</returns>
        public bool MoveAbs(int axis, double position)
        {
            return MC_MoveAbsolute(_handle, axis, position) == 0;
        }

        /// <summary>
        /// 相对位置运动。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="delta">相对位移</param>
        /// <returns>运动命令下发成功返回 true</returns>
        public bool MoveRel(int axis, double delta)
        {
            return MC_MoveRelative(_handle, axis, delta) == 0;
        }

        /// <summary>
        /// 回原点。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>回原点命令下发成功返回 true</returns>
        public bool Home(int axis)
        {
            return MC_Home(_handle, axis) == 0;
        }

        /// <summary>
        /// 停止指定轴的运动。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>停止命令下发成功返回 true</returns>
        public bool Stop(int axis)
        {
            return MC_Stop(_handle, axis) == 0;
        }

        /// <summary>
        /// 多轴同步绝对位置运动。
        /// </summary>
        /// <param name="axes">轴号数组</param>
        /// <param name="positions">目标位置数组</param>
        /// <returns>命令下发成功返回 true</returns>
        public bool GroupMoveAbs(int[] axes, double[] positions)
        {
            if (axes.Length != positions.Length) return false;
            return MC_GroupMoveAbs(_handle, axes, positions, axes.Length) == 0;
        }

        /// <summary>
        /// 获取指定轴的状态字。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="statusWord">返回状态字</param>
        /// <returns>0 表示成功，其他为错误码</returns>
        public int GetAxisStatus(int axis, out int statusWord)
        {
            return MC_GetAxisStatus(_handle, axis, out statusWord);
        }

        /// <summary>
        /// 判断指定轴是否正在运动。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>运动中返回 true</returns>
        public bool IsMoving(int axis)
        {
            int status;
            if (MC_GetAxisStatus(_handle, axis, out status) == 0)
            {
                // 假设bit0为“运动中”状态
                return (status & 0x01) != 0;
            }
            return false;
        }

        /// <summary>
        /// 判断指定轴是否处于报警状态。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>报警返回 true</returns>
        public bool IsAlarm(int axis)
        {
            int status;
            if (MC_GetAxisStatus(_handle, axis, out status) == 0)
            {
                // 假设bit3为“报警”状态
                return (status & 0x08) != 0;
            }
            return false;
        }

        /// <summary>
        /// 判断指定轴是否触发限位。
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <returns>限位返回 true</returns>
        public bool IsLimit(int axis)
        {
            int status;
            if (MC_GetAxisStatus(_handle, axis, out status) == 0)
            {
                // 假设bit4为“限位”状态
                return (status & 0x10) != 0;
            }
            return false;
        }

        /// <summary>
        /// 设置数字输出（DO）。
        /// </summary>
        /// <param name="doIndex">输出点编号</param>
        /// <param name="value">输出值</param>
        /// <returns>设置成功返回 true</returns>
        public bool SetDO(int doIndex, bool value)
        {
            return MC_SetDO(_handle, doIndex, value) == 0;
        }

        /// <summary>
        /// 读取数字输入（DI）。
        /// </summary>
        /// <param name="diIndex">输入点编号</param>
        /// <param name="value">返回输入值</param>
        /// <returns>读取成功返回 true</returns>
        public bool GetDI(int diIndex, out bool value)
        {
            return MC_GetDI(_handle, diIndex, out value) == 0;
        }

        /// <summary>
        /// 释放资源，关闭设备。
        /// </summary>
        public void Dispose()
        {
            if (_handle >= 0)
            {
                MC_CloseDevice(_handle);
                _handle = -1;
            }
        }
    }
}