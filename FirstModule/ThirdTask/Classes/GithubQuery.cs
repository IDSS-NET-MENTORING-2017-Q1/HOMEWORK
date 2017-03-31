using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ThirdTask.Classes
{
	public class GithubQuery<T> : IOrderedQueryable<T>
	{
		private Expression _expression;
		private Type _elementType;
		private IQueryProvider _provider;

		public Expression Expression
		{
			get { return _expression; }
			private set { _expression = value; }
		}

		public Type ElementType
		{
			get { return _elementType; }
			private set { _elementType = value; }
		}

		public IQueryProvider Provider
		{
			get { return _provider; }
			private set { _provider = value; }
		}

		public GithubQuery(IQueryProvider provider)
		{
			_provider = provider;
			_expression = Expression.Constant(this);
			_elementType = typeof(T);
		}

		public GithubQuery(Expression expression, IQueryProvider provider)
		{
			_provider = provider;
			_expression = expression;
			_elementType = typeof(T);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _provider.Execute<IEnumerable<T>>(_expression).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _provider.Execute<IEnumerable>(_expression).GetEnumerator();
		}
	}
}