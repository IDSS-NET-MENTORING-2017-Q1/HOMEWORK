using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ThirdTask
{
	public class GithubTranslator : ExpressionVisitor
	{
		private readonly StringBuilder _builder = new StringBuilder();

		public string Translate(Expression expression)
		{
			_builder.Clear();
			Visit(expression);
			return _builder.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.DeclaringType == typeof(Queryable))
			{
				switch (node.Method.Name)
				{
					case "Where":
						if (_builder.Length > 0)
							_builder.Append("&");

						_builder.Append("q=");
						var predicate = node.Arguments[1];
						Visit(predicate);
						return node;
				}
			}
			return base.VisitMethodCall(node);
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

				default:
					throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
			}
			;

			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Member.Name != "Name")
				_builder.Append(node.Member.Name).Append(":");
			return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			_builder.Append(node.Value);
			return node;
		}
	}
}