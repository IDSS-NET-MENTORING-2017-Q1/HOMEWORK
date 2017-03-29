using System.Linq.Expressions;

namespace FirstTask
{
	public class Transformer : ExpressionVisitor
	{
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
	}
}