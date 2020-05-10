using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VKPostsClient;

namespace PostsGrabber.Controllers
{
	[ApiController]
	[Route("")]
	public class GrabberController : ControllerBase
	{
		private readonly IEventBus _eventBus;
		private readonly IVKPostsClient _vkClient;

		public GrabberController(IEventBus eventBus, IVKPostsClient vkClient)
		{
			_eventBus = eventBus;
			_vkClient = vkClient;
		}

		[HttpPost]
		[Route("grab")]
		public async Task Grab()
		{
			var posts = await _vkClient.GrabWallPosts();
			posts.ForEach(p => _eventBus.Publish(p, nameof(Post)));
		}
	}
}
