using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace FirstTask.Tests
{
	[TestFixture]
	public class TransformerTests
	{
		private Transformer _transformer;

		[OneTimeSetUp]
		public void Init()
		{
			_transformer = new Transformer();
		}

		[Test]
		public void VisitAndConvert_TransformsIncreasingByOneToIncrement()
		{
			// Arrange
			_transformer.Parameters.Clear();
			Expression<Func<int, int>> sourceExpression = (a) => a + 1;

			// Act
			var resultExpression = _transformer.VisitAndConvert(sourceExpression, "Main");

			// Assert
			Assert.IsNotNull(resultExpression, "Result should not be null!");
			Assert.IsInstanceOf<LambdaExpression>(resultExpression, "Result should be a lambda expression!");
			Assert.IsInstanceOf<UnaryExpression>(resultExpression.Body, "Body should be an unary expression!");
			Assert.AreEqual(ExpressionType.Increment, resultExpression.Body.NodeType,
				"Body should be an increment expression!");
		}

		[Test]
		public void VisitAndConvert_TransformsDecreasingByOneToDecrement()
		{
			// Arrange
			_transformer.Parameters.Clear();
			Expression<Func<int, int>> sourceExpression = (a) => a - 1;

			// Act
			var resultExpression = _transformer.VisitAndConvert(sourceExpression, "Main");

			// Assert
			Assert.IsNotNull(resultExpression, "Result should not be null!");
			Assert.IsInstanceOf<LambdaExpression>(resultExpression, "Result should be a lambda expression!");
			Assert.IsInstanceOf<UnaryExpression>(resultExpression.Body, "Body should be an unary expression!");
			Assert.AreEqual(ExpressionType.Decrement, resultExpression.Body.NodeType,
				"Body should be an decrement expression!");
		}

		[Test]
		public void VisitAndConvert_DoNothingForOtherAdditions()
		{
			// Arrange
			_transformer.Parameters.Clear();
			Expression<Func<int, int>> sourceExpression = (a) => a + 5;

			// Act
			var resultExpression = _transformer.VisitAndConvert(sourceExpression, "Main");

			// Assert
			Assert.IsNotNull(resultExpression, "Result should not be null!");
			Assert.IsInstanceOf<LambdaExpression>(resultExpression, "Result should be a lambda expression!");
			Assert.IsInstanceOf<BinaryExpression>(resultExpression.Body, "Body should be a binary expression!");
			Assert.AreEqual(ExpressionType.Add, resultExpression.Body.NodeType,
				"Body should be an addition expression!");
		}

		[Test]
		public void VisitAndConvert_DoNothingForOtherSubstractions()
		{
			// Arrange
			_transformer.Parameters.Clear();
			Expression<Func<int, int>> sourceExpression = (a) => a - 5;

			// Act
			var resultExpression = _transformer.VisitAndConvert(sourceExpression, "Main");

			// Assert
			Assert.IsNotNull(resultExpression, "Result should not be null!");
			Assert.IsInstanceOf<LambdaExpression>(resultExpression, "Result should be a lambda expression!");
			Assert.IsInstanceOf<BinaryExpression>(resultExpression.Body, "Body should be a binary expression!");
			Assert.AreEqual(ExpressionType.Subtract, resultExpression.Body.NodeType,
				"Body should be a substraction expression!");
		}

		[Test]
		public void VisitAndConvert_ReplacesParametersWithConstants()
		{
			// Arrange
			_transformer.Parameters.Clear();
			_transformer.Parameters.Add("a", 8);
			_transformer.Parameters.Add("b", 5);
			Expression<Func<int, int, int>> sourceExpression = (a, b) => a - b;

			// Act
			var resultExpression = _transformer.VisitAndConvert(sourceExpression, "Main");

			// Assert
			Assert.IsNotNull(resultExpression, "Result should not be null!");
			Assert.IsInstanceOf<LambdaExpression>(resultExpression, "Result should be a lambda expression!");
			Assert.IsInstanceOf<BinaryExpression>(resultExpression.Body, "Body should be a binary expression!");

			var binaryBody = resultExpression.Body as BinaryExpression;
			Assert.IsInstanceOf<ConstantExpression>(binaryBody.Left, "Left should be a constant expression!");
			Assert.IsInstanceOf<ConstantExpression>(binaryBody.Right, "Right should be a constant expression!");
		}
	}
}