using System.Collections.Generic;

namespace Library.Validation
{
	public class ValidationStatus
	{
		public bool Status { get; set; }
		public List<ValidationError> Errors { get; set; }

		public ValidationStatus()
		{
			Status = true;
			Errors = new List<ValidationError>();
		}
	}
}
