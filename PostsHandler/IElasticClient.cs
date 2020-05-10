using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostsHandler
{
	public interface IElasticClient<T>
	{
		ValueTask<bool> Index(T objectToIndex);
	}
}
