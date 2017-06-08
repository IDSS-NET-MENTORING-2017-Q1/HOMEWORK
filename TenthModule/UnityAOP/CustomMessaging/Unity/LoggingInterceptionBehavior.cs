using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension;
using System.IO;
using System.Configuration;
using System.Linq;
using CustomMessaging.Interfaces;

namespace CustomMessaging.Unity
{
	public class LoggingInterceptionBehavior : IInterceptionBehavior
	{
		public IMethodReturn Invoke(IMethodInvocation input,
		  GetNextInterceptionBehaviorDelegate getNext)
		{
			// Before invoking the method on the original target.
			WriteLog(input, String.Format(
			  "Invoking method {0} at {1}",
			  input.MethodBase, DateTime.Now.ToLongTimeString()));

			// Invoke the next behavior in the chain.
			var nextBehavior = getNext();
			var result = nextBehavior(input, getNext);

			// After invoking the method on the original target.
			if (result.Exception != null)
			{
				WriteLog(input, String.Format(
				  "Method {0} threw exception {1} at {2}",
				  input.MethodBase, result.Exception.Message,
				  DateTime.Now.ToLongTimeString()));
			}
			else
			{
				WriteLog(input, String.Format(
				  "Method {0} returned {1} at {2}",
				  input.MethodBase, result.ReturnValue,
				  DateTime.Now.ToLongTimeString()));
			}

			return result;
		}

		public IEnumerable<Type> GetRequiredInterfaces()
		{
			return Type.EmptyTypes;
		}

		public bool WillExecute
		{
			get { return true; }
		}

		private void WriteLog(IMethodInvocation input, string message)
		{
			var logPath = ConfigurationManager.AppSettings["logFolder"];
			if (string.IsNullOrWhiteSpace(logPath))
			{
				logPath = AppDomain.CurrentDomain.BaseDirectory;
			}

			var fileNameAttribute = input.Target.GetType().GetCustomAttributes(typeof(LogFileNameAttribute), false).FirstOrDefault() as LogFileNameAttribute;
			if (fileNameAttribute != null)
			{
				var identifiableTarget = input.Target as IIdentifiable;
				logPath = Path.Combine(logPath, string.Format("{0} ({1}).txt", fileNameAttribute.Name, identifiableTarget.ObjectGuid.ToLower()));
			}
			else
			{
				logPath = Path.Combine(logPath, Guid.NewGuid().ToString() + ".txt");
			}

			using (var logFile = File.AppendText(logPath))
			{
				logFile.WriteLine(message);
			}
		}
	}
}