using System;

namespace FirstTask
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Please press one of the following buttons:");
			Console.WriteLine("1. Get Power Information");
			Console.WriteLine("2. Get Last Sleep Time");
			Console.WriteLine("3. Get Last Wake Time");
			Console.WriteLine("4. Set Hibernation File Mode");
			Console.WriteLine("5. Go to Sleep");

			var input = Console.ReadKey().KeyChar.ToString();
			var isValid = Int32.TryParse(input, out int key);
			while (!isValid)
			{
				input = Console.ReadKey().KeyChar.ToString();
				isValid = Int32.TryParse(input, out key);
			}

			Console.WriteLine();
			Console.WriteLine("Result:");

			var powerManager = new PowerManager();

			switch (key)
			{
				case 1:
					var powerInfo = powerManager.GetPowerInfo();

					Console.WriteLine("Max Idleness Allowed: {0}", powerInfo.MaxIdlenessAllowed);
					Console.WriteLine("Idleness: {0}", powerInfo.Idleness);
					Console.WriteLine("Time Remaining: {0}", powerInfo.TimeRemaining);
					Console.WriteLine("CoolingMode: {0}", powerInfo.CoolingMode);
					break;
				case 2:
					var lastSleepTime = powerManager.GetLastSleepTime();
					Console.WriteLine("Last Sleep Time: {0}", lastSleepTime);
					break;
				case 3:
					var lastWakeTime = powerManager.GetLastWakeTime();
					Console.WriteLine("Last Wake Time: {0}", lastWakeTime);
					break;
				case 4:
					Console.WriteLine("Enable Hibernation File (Y/N):");
					var enable = Console.ReadKey().KeyChar.ToString().Equals("y", StringComparison.OrdinalIgnoreCase);
					powerManager.SetHibernationMode(enable);
					break;
				case 5:
					Console.WriteLine("Hibernate (Y/N):");
					var hibernate = Console.ReadKey().KeyChar.ToString().Equals("y", StringComparison.OrdinalIgnoreCase);
					var forceCritical = false;
					var disableWakeEvent = false;
					powerManager.SetSuspensionState(hibernate, forceCritical, disableWakeEvent);
					break;
			}
			
			Console.ReadLine();
		}
	}
}
