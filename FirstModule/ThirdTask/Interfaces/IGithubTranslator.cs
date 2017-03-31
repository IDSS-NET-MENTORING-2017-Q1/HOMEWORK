using System.Linq.Expressions;

namespace ThirdTask.Interfaces
{
	public interface IGithubTranslator
	{
		string Translate(Expression expression);
	}
}