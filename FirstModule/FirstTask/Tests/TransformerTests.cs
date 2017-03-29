using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace FirstTask.Tests
{
	[TestFixture]
	public class TransformerTests
	{
		[Test]
		public void VisitAndConvert_TransformsIncreasingByOneToIncrement()
		{
			// Arrange
			Expression<Func<int, int>> sourceExpression = (a) => a + 1;

			// Act
			var resultExpression = new Transformer().VisitAndConvert(sourceExpression, "Main");

			// Assert
			Assert.IsNotNull(resultExpression, "Result should not be null!");
			Assert.IsInstanceOf<LambdaExpression>(resultExpression, "Result should be a lambda expression!");
			Assert.IsInstanceOf<UnaryExpression>(resultExpression.Body, "Result's body should be an unary expression!");
			Assert.AreEqual(ExpressionType.Increment, resultExpression.Body.NodeType,
				"Result's body should be an increment expression!");
		}

		[Test]
		public void VisitAndConvert_TransformsDecreasingByOneToDecrement()
		{
			// Arrange
			Expression<Func<int, int>> sourceExpression = (a) => a - 1;

			// Act
			var resultExpression = new Transformer().VisitAndConvert(sourceExpression, "Main");

			// Assert
			Assert.IsNotNull(resultExpression, "Result should not be null!");
			Assert.IsInstanceOf<LambdaExpression>(resultExpression, "Result should be a lambda expression!");
			Assert.IsInstanceOf<UnaryExpression>(resultExpression.Body, "Result's body should be an unary expression!");
			Assert.AreEqual(ExpressionType.Decrement, resultExpression.Body.NodeType,
				"Result's body should be an decrement expression!");
		}

		[Test]
		public void VisitAndConvert_DoNothingForOtherAdditions()
		{
			// Arrange
			Expression<Func<int, int>> sourceExpression = (a) => a + 5;

			// Act
			var resultExpression = new Transformer().VisitAndConvert(sourceExpression, "Main");

			// Assert
			Assert.IsNotNull(resultExpression, "Result should not be null!");
			Assert.IsInstanceOf<LambdaExpression>(resultExpression, "Result should be a lambda expression!");
			Assert.IsInstanceOf<BinaryExpression>(resultExpression.Body, "Result's body should be a binary expression!");
			Assert.AreEqual(ExpressionType.Add, resultExpression.Body.NodeType,
				"Result's body should be an addition expression!");
		}

		[Test]
		public void VisitAndConvert_DoNothingForOtherSubstractions()
		{
			// Arrange
			Expression<Func<int, int>> sourceExpression = (a) => a - 5;

			// Act
			var resultExpression = new Transformer().VisitAndConvert(sourceExpression, "Main");

			// Assert
			Assert.IsNotNull(resultExpression, "Result should not be null!");
			Assert.IsInstanceOf<LambdaExpression>(resultExpression, "Result should be a lambda expression!");
			Assert.IsInstanceOf<BinaryExpression>(resultExpression.Body, "Result's body should be a binary expression!");
			Assert.AreEqual(ExpressionType.Subtract, resultExpression.Body.NodeType,
				"Result's body should be a substraction expression!");
		}
	}
}