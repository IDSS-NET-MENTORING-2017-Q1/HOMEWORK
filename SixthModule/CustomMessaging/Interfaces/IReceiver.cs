using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMessaging.Interfaces
{
	public interface IReceiver<T>
	{
		T Receive();
	}
}
