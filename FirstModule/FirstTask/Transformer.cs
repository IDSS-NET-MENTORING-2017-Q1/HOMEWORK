using System.Collections.Generic;
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

		protected override Expression VisitBinary(BinaryExpression node)
		{
			Expression resultLeft, resultRight;
			var sourceLeft = node.Left as ParameterExpression;
			var sourceRight = node.Right as ParameterExpression;

			if (sourceLeft != null && _parameters.ContainsKey(sourceLeft.Name))
				resultLeft = Expression.Constant(_parameters[sourceLeft.Name]);
			else
				resultLeft = sourceLeft;

			if (sourceRight != null && _parameters.ContainsKey(sourceRight.Name))
				resultRight = Expression.Constant(_parameters[sourceRight.Name]);
			else
				resultRight = sourceRight;

			if (resultLeft is ConstantExpression && resultRight is ConstantExpression)
				return Expression.MakeBinary(node.NodeType, resultLeft, resultRight);

			if (node.Left.NodeType == ExpressionType.Parameter &&
				node.Right.NodeType == ExpressionType.Constant &&
				((ConstantExpression)node.Right).Value.Equals(1))
			{
				switch (node.NodeType)
				{
					case ExpressionType.Add:
						return Expression.Increment(node.Left);
					case ExpressionType.Subtract:
						return Expression.Decrement(node.Left);
				}
			}

			if (resultLeft == null || resultRight == null)
				return base.VisitBinary(node);

			return Expression.MakeBinary(node.NodeType, resultLeft, resultRight);
		}
	}
}