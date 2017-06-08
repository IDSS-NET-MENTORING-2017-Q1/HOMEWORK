using System;

namespace CustomMessaging.Unity
{
	public class LogFileNameAttribute : Attribute
	{
		private string _name;

		public LogFileNameAttribute(string name)
		{
			_name = string.Format("{0} ({1}).txt", name, Guid.NewGuid().ToString().ToLower());
		}

		public string Name
		{
			get
			{
				return _name;
			}

			set
			{
				_name = value;
			}
		}
	}
}
