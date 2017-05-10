using CustomMessaging.Interfaces;

namespace CustomMessaging.Classes
{
	public class StatusPublisher : IPublisher<ServiceStatuses>
	{
		public void Publish(ServiceStatuses value)
		{
			throw new System.NotImplementedException();
		}
	}
}
