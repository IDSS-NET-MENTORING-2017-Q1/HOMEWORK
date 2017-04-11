using System;

namespace FirstTask
{
	class Program
	{
		static void Main(string[] args)
		{
			var powerManager = new PowerManager();
			var powerInfo = powerManager.GetPowerInfo();

			Console.WriteLine("Max Idleness Allowed: {0}", powerInfo.MaxIdlenessAllowed);
			Console.WriteLine("Idleness: {0}", powerInfo.Idleness);
			Console.WriteLine("Time Remaining: {0}", powerInfo.TimeRemaining);
			Console.WriteLine("CoolingMode: {0}", powerInfo.CoolingMode);

			var lastSleepTime = powerManager.GetLastSleepTime();
			Console.WriteLine("Last Sleep Time: {0}", lastSleepTime);

			var lastWakeTime = powerManager.GetLastWakeTime();
			Console.WriteLine("Last Wake Time: {0}", lastWakeTime);

			Console.ReadLine();
		}
	}
}
