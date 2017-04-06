using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace Library.Transformation
{
	public class Transformator
	{
		private string _stylesheet;

		public string Stylesheet
		{
			get
			{
				return _stylesheet;
			}
			set
			{
				_stylesheet = value;
			}
		}

		public Transformator()
		{

		}

		public Transformator(string stylesheet)
		{
			_stylesheet = stylesheet;
		}

		public void Transform(string sourcePath, string destinationPath)
		{
			var xsl = new XslCompiledTransform();
			xsl.Load(_stylesheet);
			xsl.Transform(sourcePath, destinationPath);
		}
	}
}
