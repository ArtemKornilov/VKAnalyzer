using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VKPostsClient;

namespace PostsHandler.IntegrationEvents
{
	public class PostIntegrationEventHandler : IIntegrationEventHandler<Post>
	{
		private readonly IElasticClient<Post> _elasticClient;
		public PostIntegrationEventHandler(IElasticClient<Post> elasticClient)
		{
			_elasticClient = elasticClient;
		}

		public async Task HandleAsync(Post @event)
		{
			await _elasticClient.Index(@event);
			Console.WriteLine("Indexed");
		}
	}
}
