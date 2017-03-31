using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FirstTask
{
	public class Transformer : ExpressionVisitor
	{
		private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

		public Dictionary<string, object> Parameters
		{
			get { return _parameters; }
		}

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			if (!node.Parameters.Any(o => _parameters.ContainsKey(o.Name)))
				return base.VisitLambda(node);

			var body = Visit(node.Body);
			var parameters = node.Parameters.Where(o => !_parameters.ContainsKey(o.Name));

			return Expression.Lambda(body, node.Name, parameters);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			var param = node.Operand as ParameterExpression;
			if (param != null && _parameters.ContainsKey(param.Name))
			{
				var paramValue = _parameters[param.Name];
				switch (node.NodeType)
				{
					case ExpressionType.Increment:
						return Expression.Add(
							Expression.Constant(paramValue),
							Expression.Constant(1)
						);
					case ExpressionType.Decrement:
						return Expression.Subtract(
							Expression.Constant(paramValue),
							Expression.Constant(1)
						);
				}
			}

			return base.VisitUnary(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			Expression leftExp, rightExp;
			var leftParam = node.Left as ParameterExpression;
			var rightParam = node.Right as ParameterExpression;

			if (leftParam != null && _parameters.ContainsKey(leftParam.Name))
				leftExp = Expression.Constant(_parameters[leftParam.Name]);
			else
				leftExp = node.Left;

			if (rightParam != null && _parameters.ContainsKey(rightParam.Name))
				rightExp = Expression.Constant(_parameters[rightParam.Name]);
			else
				rightExp = node.Right;

			if (leftExp is ConstantExpression && rightExp is ConstantExpression)
				return Expression.MakeBinary(node.NodeType, leftExp, rightExp);

			if (node.Left.NodeType == ExpressionType.Parameter &&
			    node.Right.NodeType == ExpressionType.Constant &&
			    ((ConstantExpression) node.Right).Value.Equals(1))
			{
				switch (node.NodeType)
				{
					case ExpressionType.Add:
						return Expression.Increment(node.Left);
					case ExpressionType.Subtract:
						return Expression.Decrement(node.Left);
				}
			}

			return Expression.MakeBinary(node.NodeType, leftExp, rightExp);
		}
	}
}