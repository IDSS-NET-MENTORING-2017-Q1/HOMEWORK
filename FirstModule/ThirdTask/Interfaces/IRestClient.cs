using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThirdTask.Interfaces
{
	public interface IRestClient
	{
		Dictionary<string, string> Headers { get; }
		TResult Get<TResult>(string url);
		Task<TResult> GetAsync<TResult>(string url);
	}
}