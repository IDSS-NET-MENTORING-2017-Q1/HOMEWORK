using System;
using System.Linq;
using System.Linq.Expressions;

namespace ThirdTask
{
	public class GithubQueryProvider : IQueryProvider
	{
		private readonly GithubClient _client;

		public GithubQueryProvider(GithubClient githubClient)
		{
			_client = githubClient;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			throw new NotImplementedException();
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return new GithubQuery<TElement>(expression, this);
		}

		public object Execute(Expression expression)
		{
			throw new NotImplementedException();
		}

		public TResult Execute<TResult>(Expression expression)
		{
			var translator = new GithubTranslator();
			var queryString = translator.Translate(expression);
			return (TResult) _client.SearchRepositories(queryString);
		}
	}
}
