using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SecondTask
{
	public class MappingGenerator
	{
		public Mapper<TSource, TDestination> Generate<TSource, TDestination>()
		{
			var sourceParam = Expression.Parameter(typeof(TSource), "source");
			var resultParam = Expression.Variable(
				typeof(TDestination),
				"result"
			);

			var initializationExpressions = new List<Expression>()
			{
				Expression.Assign(resultParam,
					Expression.New(typeof(TDestination)))
			};

			var sourceProperties = typeof(TSource).GetProperties();
			var destinationProperties = typeof(TDestination).GetProperties();

			foreach (var destinationProperty in destinationProperties)
			{
				var sourceProperty = sourceProperties.FirstOrDefault(
					o => o.Name == destinationProperty.Name && o.PropertyType == destinationProperty.PropertyType);

				if (sourceProperty != null)
				{
					var destinationPropertyExp = Expression.Property(resultParam, destinationProperty);
					var sourcePropertyExp = Expression.Property(sourceParam, sourceProperty);

					initializationExpressions.Add(Expression.Assign(
						destinationPropertyExp, sourcePropertyExp
					));
				}
			}

			initializationExpressions.Add(resultParam);

			var mapFunc = Expression.Lambda<Func<TSource, TDestination>>(
				Expression.Block(
					new[] {resultParam},
					initializationExpressions
				),
				sourceParam
			);
			return new Mapper<TSource, TDestination>(mapFunc.Compile());
		}
	}
}