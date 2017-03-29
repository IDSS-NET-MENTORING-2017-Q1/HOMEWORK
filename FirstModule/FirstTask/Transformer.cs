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
			if (node.Left.NodeType != ExpressionType.Parameter || node.Right.NodeType != ExpressionType.Constant ||
			    !((ConstantExpression) node.Right).Value.Equals(1))
				return base.VisitBinary(node);


			switch (node.NodeType)
			{
				case ExpressionType.Add:
					return Expression.Increment(node.Left);
				case ExpressionType.Subtract:
					return Expression.Decrement(node.Left);
			}

			return base.VisitBinary(node);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (_parameters.ContainsKey(node.Name))
			{
				var value = _parameters[node.Name];
				return Expression.Constant(value);
			}
			return base.VisitParameter(node);
		}
	}
}