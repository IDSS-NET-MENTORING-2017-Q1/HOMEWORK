using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AnimalsLibrary
{
	[ComVisible(true)]
	[Guid("CB9815CC-41B4-4F84-9EF3-D3B1255D7022")]
	[ClassInterface(ClassInterfaceType.None)]
	public class Cat : Animal
	{
		public override string GetVoice()
		{
			return "Meow!";
		}
	}
}
