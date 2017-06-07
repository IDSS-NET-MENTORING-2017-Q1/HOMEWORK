using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity.InterceptionExtension;
using System.IO;
using System.Configuration;

namespace CustomMessaging.Unity
{
	public class LoggingInterceptionBehavior : IInterceptionBehavior
	{
		public IMethodReturn Invoke(IMethodInvocation input,
		  GetNextInterceptionBehaviorDelegate getNext)
		{
			// Before invoking the method on the original target.
			WriteLog(String.Format(
			  "Invoking method {0} at {1}",
			  input.MethodBase, DateTime.Now.ToLongTimeString()));

			// Invoke the next behavior in the chain.
			var nextBehavior = getNext();
			var result = nextBehavior(input, getNext);

			// After invoking the method on the original target.
			if (result.Exception != null)
			{
				WriteLog(String.Format(
				  "Method {0} threw exception {1} at {2}",
				  input.MethodBase, result.Exception.Message,
				  DateTime.Now.ToLongTimeString()));
			}
			else
			{
				WriteLog(String.Format(
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

		private void WriteLog(string message)
		{
			var logPath = ConfigurationManager.AppSettings["logPath"];
			if (string.IsNullOrWhiteSpace(logPath))
			{
				logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			}
			File.AppendAllText(logPath, message);
		}
	}
}