using System.Collections.Generic;

namespace CustomMessaging.DTO
{
	public class DocumentPartitionDTO
	{
		public bool EndOfDocument { get; set; }
		public IEnumerable<byte> Content { get; set; }
		public string DocumentGuid { get; set; }
	}
}
