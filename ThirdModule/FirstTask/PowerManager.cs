using System;
using System.Runtime.InteropServices;

namespace FirstTask
{
	public class PowerManager
	{
		public struct SYSTEM_POWER_INFORMATION
		{
			public uint MaxIdlenessAllowed;
			public uint Idleness;
			public uint TimeRemaining;
			public byte CoolingMode;
		}

		[DllImport("powrprof.dll", SetLastError = true)]
		protected static extern uint CallNtPowerInformation(
			int InformationLevel,
			IntPtr lpInputBuffer,
			int nInputBufferSize,
			out SYSTEM_POWER_INFORMATION lpOutputBuffer,
			int nOutputBufferSize
		);

		[DllImport("powrprof.dll", SetLastError = true)]
		protected static extern uint CallNtPowerInformation(
			int InformationLevel,
			bool lpInputBuffer,
			int nInputBufferSize,
			IntPtr lpOutputBuffer,
			int nOutputBufferSize
		);

		[DllImport("powrprof.dll", SetLastError = true)]
		protected static extern uint CallNtPowerInformation(
			int InformationLevel,
			IntPtr lpInputBuffer,
			int nInputBufferSize,
			out long lpOutputBuffer,
			int nOutputBufferSize
		);

		[DllImport("powrprof.dll", SetLastError = true)]
		protected static extern uint SetSuspendState(
			bool Hibernate,
			bool ForceCritical,
			bool DisableWakeEvent
		);

		protected long GetScalarInfo(PowerInfoTypes infoType)
		{
			long result;
			uint status = CallNtPowerInformation(
				(int) infoType,
				IntPtr.Zero,
				0,
				out result,
				Marshal.SizeOf(typeof(long))
			);

			if (status == (uint) ExecutionStatuses.Success)
			{
				return result;
			}
			else
			{
				throw new InvalidOperationException("Error code is: " + Marshal.GetLastWin32Error().ToString());
			}
		}

		public long GetLastSleepTime()
		{
			return GetScalarInfo(PowerInfoTypes.LastSleepTime);
		}

		public long GetLastWakeTime()
		{
			return GetScalarInfo(PowerInfoTypes.LastWakeTime);
		}

		public SYSTEM_POWER_INFORMATION GetPowerInfo()
		{
			SYSTEM_POWER_INFORMATION result;
			uint status = CallNtPowerInformation(
				(int) PowerInfoTypes.SystemPowerInformation,
				IntPtr.Zero,
				0,
				out result,
				Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION))
			);

			if (status == (uint) ExecutionStatuses.Success)
			{
				return result;
			}
			else
			{
				throw new InvalidOperationException("Error code is: " + Marshal.GetLastWin32Error().ToString());
			}
		}

		public bool SetHibernationMode(bool enabled)
		{
			uint status = CallNtPowerInformation(
				(int) PowerInfoTypes.SystemReserveHiberFile,
				enabled,
				Marshal.SizeOf(typeof(bool)),
				IntPtr.Zero,
				0
			);

			if (status == (uint) ExecutionStatuses.Success)
			{
				return true;
			}
			else
			{
				throw new InvalidOperationException("Error code is: " + Marshal.GetLastWin32Error().ToString());
			}
		}

		public void SetSuspensionState(bool hibernate, bool forceCritical, bool disableWakeEvent)
		{
			uint status = SetSuspendState(
				hibernate,
				forceCritical,
				disableWakeEvent
			);
		}
	}
}
