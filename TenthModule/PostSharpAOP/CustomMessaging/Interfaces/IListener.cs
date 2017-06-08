using System;

namespace CustomMessaging.Interfaces
{
	public interface IListener : IDisposable
	{
		void Start();
	}

	public interface IListener<T> : IListener
	{
		event EventHandler<T> Received;
	}
}
