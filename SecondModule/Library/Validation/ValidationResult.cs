using System.Collections.Generic;

namespace Library.Validation
{
	public class ValidationResult
	{
		public bool Status { get; set; }
		public List<ValidationError> Errors { get; set; }

		public ValidationResult()
		{
			Status = true;
			Errors = new List<ValidationError>();
		}
	}
}
