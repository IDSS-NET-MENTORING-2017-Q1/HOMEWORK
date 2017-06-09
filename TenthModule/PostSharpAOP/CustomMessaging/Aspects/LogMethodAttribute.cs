using System;
using System.Reflection;
using System.Text;
using NLog;
using PostSharp.Aspects;

namespace CustomMessaging.Aspects
{
	[Serializable]
	public class LogMethodAttribute : OnMethodBoundaryAspect
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		///     Method invoked before the target method is executed.
		/// </summary>
		/// <param name="args">Method execution context.</param>
		public override void OnEntry(MethodExecutionArgs args)
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append("Entering ");
			AppendCallInformation(args, stringBuilder);

			Logger.Info(stringBuilder.ToString());
		}


		/// <summary>
		///     Method invoked after the target method has successfully completed.
		/// </summary>
		/// <param name="args">Method execution context.</param>
		public override void OnSuccess(MethodExecutionArgs args)
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append("Exiting ");
			AppendCallInformation(args, stringBuilder);

			if (!args.Method.IsConstructor && ((MethodInfo)args.Method).ReturnType != typeof(void))
			{
				stringBuilder.Append(" with return value ");
				stringBuilder.Append(args.ReturnValue);
			}

			Logger.Info(stringBuilder.ToString());
		}

		/// <summary>
		///     Method invoked when the target method has failed.
		/// </summary>
		/// <param name="args">Method execution context.</param>
		public override void OnException(MethodExecutionArgs args)
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append("Exiting ");
			AppendCallInformation(args, stringBuilder);

			if (!args.Method.IsConstructor && ((MethodInfo)args.Method).ReturnType != typeof(void))
			{
				stringBuilder.Append(" with exception ");
				stringBuilder.Append(args.Exception.GetType().Name);
			}

			Logger.Info(stringBuilder.ToString());
		}

		private static void AppendCallInformation(MethodExecutionArgs args, StringBuilder stringBuilder)
		{
			var declaringType = args.Method.DeclaringType;
			Formatter.AppendTypeName(stringBuilder, declaringType);
			stringBuilder.Append('.');
			stringBuilder.Append(args.Method.Name);

			if (args.Method.IsGenericMethod)
			{
				var genericArguments = args.Method.GetGenericArguments();
				Formatter.AppendGenericArguments(stringBuilder, genericArguments);
			}

			var arguments = args.Arguments;

			Formatter.AppendArguments(stringBuilder, arguments);
		}
	}
}
