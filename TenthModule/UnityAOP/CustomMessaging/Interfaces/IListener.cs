using CustomMessaging.Unity;
using System;

namespace CustomMessaging.Interfaces
{
	public interface IListener : IDisposable
	{
		void Start();
	}
	
	public interface IDocumentListener : IListener
	{
		string OutputPath { get; set; }
	}

	public interface IListener<T> : IListener
	{
		event EventHandler<T> Received;
	}
}
