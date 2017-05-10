using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomMessaging.Classes
{
	public class DocumentPartition
	{
		public bool EndOfDocument { get; set; }
		public IEnumerable<byte> Content { get; set; }
		public string DocumentGuid { get; set; }
	}
}
