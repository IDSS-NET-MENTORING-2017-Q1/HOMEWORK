using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMessaging.Interfaces
{
	public interface IListener
	{
		void Start();
		void Stop();
	}

	public interface IListener<T> : IListener
	{
		event EventHandler<T> Received;
	}
}
