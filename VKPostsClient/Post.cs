using EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace VKPostsClient
{
	public class Post : IIntegrationEvent
	{
		public int Age { get; set; }
		public int Likes { get; set; }
		public string Locations { get; set; }
		public string Text { get; set; }
		public DateTime Date { get; set; }
	}
}
