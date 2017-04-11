using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace AnimalsLibrary
{
	[ComVisible(true)]
	[Guid("B30D1C43-0A4A-4270-9AE7-D39DD62E8E53")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface ICreature
	{
		int GetSatiety();
		void Feed();

		int GetHealth();
		void Heal();

		string GetVoice();
	}
}
