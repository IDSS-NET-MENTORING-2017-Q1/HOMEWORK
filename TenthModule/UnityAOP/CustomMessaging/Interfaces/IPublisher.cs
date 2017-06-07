namespace CustomMessaging.Interfaces
{
	public interface IPublisher<T>
	{
		void Publish(T value);
	}
}
