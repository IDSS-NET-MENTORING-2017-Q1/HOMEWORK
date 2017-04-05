using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Library.Validation
{
    public class Validator
    {
		private readonly Dictionary<string, string> _schemas = new Dictionary<string, string>();

	    public Dictionary<string, string> Schemas
	    {
		    get { return _schemas; }
	    }

	    public ValidationStatus Validate(string filePath)
	    {
			if (_schemas.Count <= 0)
				throw new ArgumentException("There are no schemas to validate!");

			var result = new ValidationStatus();
			var settings = new XmlReaderSettings();

		    foreach (var schema in _schemas)
			{
				settings.Schemas.Add(schema.Key, schema.Value);
		    }
			settings.ValidationEventHandler +=
				delegate(object sender, ValidationEventArgs e)
				{
					result.Status = false;
					result.Errors.Add(new ValidationError()
					{
						Message = e.Message,
						LineNumber = e.Exception.LineNumber
					});
				};

			settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
			settings.ValidationType = ValidationType.Schema;

			using (var reader = XmlReader.Create(filePath, settings))
			{
				while (reader.Read()) { }
			}

		    return result;
	    }
    }
}
