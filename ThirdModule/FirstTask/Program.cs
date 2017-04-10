using System;
using System.Runtime.InteropServices;

namespace FirstTask
{
	class Program
	{
		static void Main(string[] args)
		{
			var powerManager = new PowerManager();
			var powerInfo = powerManager.GetPowerInfo();

			Console.WriteLine(powerInfo.TimeRemaining);
			Console.ReadLine();
		}
	}
}
