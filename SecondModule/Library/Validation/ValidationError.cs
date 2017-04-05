namespace Library.Validation
{
	public class ValidationError
	{
		public int LineNumber { get; set; }
		public int LinePosition { get; set; }
		public string Message { get; set; }
	}
}
