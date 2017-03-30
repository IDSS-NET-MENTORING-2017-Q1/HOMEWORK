using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ThirdTask.Attributes;

namespace ThirdTask
{
	public class GithubTranslator : ExpressionVisitor
	{
		private bool _processingOrder = false;
		private bool _processingWhere = false;

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
				Expression predicate; 
				switch (node.Method.Name)
				{
					case "Where":
						_processingWhere = true;
						if (_builder.Length > 0)
							_builder.Append("&");

						_builder.Append("q=");
						predicate = node.Arguments[1];
						Visit(predicate);
						return node;
					case "OrderBy":
						_processingOrder = true;
						if (_builder.Length > 0)
							_builder.Append("&");

						_builder.Append("sort=");
						predicate = node.Arguments[1];
						Visit(predicate);
						_builder.Append("order=asc");
						return node;
					case "OrderByDescending":
						_processingOrder = true;
						if (_builder.Length > 0)
							_builder.Append("&");

						_builder.Append("sort=");
						predicate = node.Arguments[1];
						Visit(predicate);
						_builder.Append("order=desc");
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

		protected override Expression VisitUnary(UnaryExpression node)
		{
			switch (node.NodeType)
			{
				case ExpressionType.Quote:
					if (node.Operand.NodeType == ExpressionType.Lambda)
					{
						var lambdaOperand = node.Operand as LambdaExpression;
						if (lambdaOperand != null && lambdaOperand.Body.NodeType == ExpressionType.MemberAccess)
							Visit(node.Operand);
					}
					break;
			}

			return base.VisitUnary(node);
		}

		protected override Expression VisitLambda<T>(Expression<T> node)
		{

			return base.VisitLambda(node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			var nodeMember = node.Member;
			if (nodeMember
				.GetCustomAttributes(typeof(GithubFacetAttribute), true)
				.FirstOrDefault() is GithubFacetAttribute facetAttribute) { _builder.Append(facetAttribute.Prefix); }
			return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			_builder.Append(node.Value);
			return node;
		}
	}
}