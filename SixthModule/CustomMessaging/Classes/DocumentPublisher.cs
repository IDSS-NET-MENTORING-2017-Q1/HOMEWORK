using System.Collections.Generic;
using CustomMessaging.Interfaces;

namespace CustomMessaging.Classes
{
	public class DocumentPublisher : IPublisher<IEnumerable<byte>>
	{
		public void Publish(IEnumerable<byte> value)
		{
			throw new System.NotImplementedException();
		}
	}
}
