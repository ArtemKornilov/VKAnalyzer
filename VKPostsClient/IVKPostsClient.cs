using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VKPostsClient
{
	public interface IVKPostsClient
	{
		Task<List<Post>> GrabWallPosts();
	}
}
