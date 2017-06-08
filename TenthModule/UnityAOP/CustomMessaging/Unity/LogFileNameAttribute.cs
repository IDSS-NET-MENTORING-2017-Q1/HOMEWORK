﻿using System;

namespace CustomMessaging.Unity
{
	public class LogFileNameAttribute : Attribute
	{
		private string _name;

		public LogFileNameAttribute(string name)
		{
			_name = name;
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