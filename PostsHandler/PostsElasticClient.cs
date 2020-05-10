using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VKPostsClient;

namespace PostsHandler
{
	public class PostsElasticClient : IElasticClient<Post>
	{
		private readonly ElasticClient _client;
		private readonly string _indexName;

		public PostsElasticClient(IConfiguration config)
		{
			_indexName = config["Elastic:IndexName"];

			_client = new ElasticClient(new Uri(config["Elastic:Host"]));			

			bool indexExists = _client.Indices.Exists(_indexName).Exists;
			if (!indexExists)
			{
				_client.Indices.Create(_indexName, 
										index => index.Map<Post>(m => m.AutoMap()));
			}
		}

		public async ValueTask<bool> Index(Post post)
		{
			var result = await _client.IndexAsync(post, i => i.Index(_indexName));
			return result.IsValid;
		}
	}
}
