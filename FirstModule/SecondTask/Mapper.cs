using System;

namespace SecondTask
{
	public class Mapper<TSource, TDestination>
	{
		private readonly Func<TSource, TDestination> _mapFunc;

		internal Mapper(Func<TSource, TDestination> mapFunc)
		{
			_mapFunc = mapFunc;
		}

		public TDestination Map(TSource source)
		{
			return _mapFunc(source);
		}
	}
}