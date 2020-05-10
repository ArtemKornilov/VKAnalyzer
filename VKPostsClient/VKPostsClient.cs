using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKPostsClient
{
	public class VKPostsClient : IVKPostsClient
	{
		private readonly VkApi _client;
		private readonly string _accessToken;
		private readonly string _publicName;

		public VKPostsClient(IConfiguration config)
		{
			_accessToken = config["VKApi:Token"];
			_publicName = config["VKApi:PublicName"];

			_client = new VkApi();
			_client.Authorize(new ApiAuthParams 
			{ 
				AccessToken = _accessToken
			});
		}

		public async Task<List<Post>> GrabWallPosts()
		{
			var vkPosts = await _client.Wall.GetAsync(new WallGetParams
			{
				Domain = _publicName,
				Count = 100,
			});

			var posts = new List<Post>();

			foreach (var post in vkPosts.WallPosts)
			{
				posts.Add(new Post
				{
					Likes = post.Likes.Count,
					Age = RegexHelper.GetAge(post.Text),
					Locations = RegexHelper.GetLocations(post.Text),
					Text = post.Text,
					Date = post.Date.Value
				});

			}

			return posts;
		}
	}
}
