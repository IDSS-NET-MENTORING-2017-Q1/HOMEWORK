using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AnimalsLibrary
{
	[ComVisible(true)]
	[Guid("AD4951E8-0EB6-4B33-ADD9-D9CFBF995546")]
	[ClassInterface(ClassInterfaceType.None)]
	public class Dog : Animal
	{
		public override string GetVoice()
		{
			return "Bark!";
		}
	}
}
