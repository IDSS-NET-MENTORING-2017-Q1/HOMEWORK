using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ThirdTask.Attributes;
using ThirdTask.Enums;
using ThirdTask.Interfaces;

namespace ThirdTask.Classes
{
	public class GithubTranslator : ExpressionVisitor, IGithubTranslator
	{
		private readonly Dictionary<string, ProcessingOperation> _supportedMethods = new Dictionary
			<string, ProcessingOperation>()
			{
				{"Where", ProcessingOperation.Filtering},
				{"OrderBy", ProcessingOperation.Sorting},
				{"OrderByDescending", ProcessingOperation.Sorting}
			};

		private ProcessingOperation _operation = ProcessingOperation.None;
		private readonly StringBuilder _builder = new StringBuilder();

		public string Translate(Expression expression)
		{
			_builder.Clear();
			Visit(expression);
			return _builder.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var methodName = node.Method.Name;
			if (node.Method.DeclaringType != typeof(Queryable) || !_supportedMethods.ContainsKey(methodName))
				return base.VisitMethodCall(node);

			var previousCall = node.Arguments[0] as MethodCallExpression;
			if (previousCall != null)
				Visit(previousCall);

			_operation = _supportedMethods[methodName];

			if (_builder.Length > 0)
				_builder.Append("&");

			var predicate = node.Arguments[1];

			switch (methodName)
			{
				case "Where":
					_builder.Append("q=");
					Visit(predicate);
					break;
				case "OrderBy":
					_builder.Append("sort=");
					Visit(predicate);
					_builder.Append("&order=asc");
					break;
				case "OrderByDescending":
					_builder.Append("sort=");
					Visit(predicate);
					_builder.Append("&order=desc");
					break;
			}

			_operation = ProcessingOperation.None;

			return node;
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			switch (node.NodeType)
			{
				case ExpressionType.Equal:
					if (node.Left.NodeType != ExpressionType.MemberAccess)
						throw new NotSupportedException(string.Format("Left operand should be property or field", node.NodeType));

					if (node.Right.NodeType != ExpressionType.Constant)
						throw new NotSupportedException(string.Format("Right operand should be constant", node.NodeType));

					Visit(node.Left);
					Visit(node.Right);
					break;
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					Visit(node.Left);
					_builder.Append("+");
					Visit(node.Right);
					break;
				default:
					throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
			}

			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			var nodeMember = node.Member;
			switch (_operation)
			{
				case ProcessingOperation.Filtering:
					var facetAttribute = nodeMember
						.GetCustomAttributes(typeof(GithubFacetAttribute), true)
						.FirstOrDefault() as GithubFacetAttribute;
					if (facetAttribute != null)
					{
						_builder.Append(facetAttribute.Pattern);
					}
					break;
				case ProcessingOperation.Sorting:
					var sorterAttribute = nodeMember
						.GetCustomAttributes(typeof(GithubSorterAttribute), true)
						.FirstOrDefault() as GithubSorterAttribute;
					if (sorterAttribute != null)
					{
						_builder.Append(sorterAttribute.Name);
					}
					break;
			}
			return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			var stringValue = node.Value.ToString();
			_builder.Replace("{value}", stringValue);
			return node;
		}
	}
}