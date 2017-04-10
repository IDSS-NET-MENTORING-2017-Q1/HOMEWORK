using System;
using System.Runtime.InteropServices;

namespace FirstTask
{
	public class PowerManager
	{
		protected const uint STATUS_SUCCESS = 0;
		protected const int SystemPowerInformation = 12;
		protected const int SystemReserveHiberFile = 10;

		public struct SYSTEM_POWER_INFORMATION
		{
			public uint MaxIdlenessAllowed;
			public uint Idleness;
			public uint TimeRemaining;
			public byte CoolingMode;
		}

		[DllImport("powrprof.dll")]
		protected static extern uint CallNtPowerInformation(
			int InformationLevel,
			IntPtr lpInputBuffer,
			int nInputBufferSize,
			out SYSTEM_POWER_INFORMATION spi,
			int nOutputBufferSize
		);

		public SYSTEM_POWER_INFORMATION GetPowerInfo()
		{
			SYSTEM_POWER_INFORMATION powerInfo;
			uint result = CallNtPowerInformation(
				SystemPowerInformation,
				IntPtr.Zero,
				0,
				out powerInfo,
				Marshal.SizeOf(typeof(SYSTEM_POWER_INFORMATION))
			);

			if (result == STATUS_SUCCESS)
			{
				return powerInfo;
			}
			else
			{
				throw new InvalidOperationException("Unexpected error has occured!");
			}
		}

		public bool SetHibernationMode(bool enabled)
		{
			uint result = CallNtPowerInformation(
				SystemReserveHiberFile,
				enabled,
				Marshal.SizeOf(typeof(bool)),
				IntPtr.Zero,
				0
			);

			if (result == STATUS_SUCCESS)
			{
				return true;
			}
			else
			{
				throw new InvalidOperationException("Unexpected error has occured!");
			}
		}		
	}
}
