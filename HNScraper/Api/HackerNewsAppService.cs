using HNScraper.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNScraper.Api
{
	public class HackerNewsAppService
	{
		public int[] GetTopPostIds()
		{
			var topPostUrl = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";


			if (!HttpHelper.ValidUrl(topPostUrl))
				throw new Exception($"{topPostUrl} is an invalid URL");

			var getTopPosts = Task.Run(() => HttpHelper.SendRequest(topPostUrl));
			getTopPosts.Wait();

			return JsonConvert.DeserializeObject<int[]>(getTopPosts.Result);
		}

		public string GetTopPostObject(int id)
		{
			string uri = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";

			if (!HttpHelper.ValidUrl(uri))
				throw new Exception($"{uri} is an invalid URL");

			var getPost = Task.Run(() => HttpHelper.SendRequest(uri));
			getPost.Wait();

			return getPost.Result;
		}
	}
}
