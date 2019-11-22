using HNScraper.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNScraper.Api
{
	public class PostApi
	{
		public string GetTopNPosts(int topNPosts)
		{
			var topPostUrl = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";

			// uncomment to test error
			// topPostUrl = "invalid url";

			var topPostJsonObjects = new StringBuilder();

			if (!HttpHelper.ValidUrl(topPostUrl))
				throw new Exception($"{topPostUrl} is an invalid URL");

			var getTopPosts = Task.Run(() => HttpHelper.SendRequest(topPostUrl));
			getTopPosts.Wait();

			var topPostIds = JsonConvert.DeserializeObject<int[]>(getTopPosts.Result);

			var getPostUri = string.Empty;

			// setup settings for converting the right properties when serializing
			var jsonSerializerResolver = new JsonSerializeResolver();
			jsonSerializerResolver.RenameProperty(typeof(Post), "By", "Author");
			jsonSerializerResolver.RenameProperty(typeof(Post), "Score", "Points");
			jsonSerializerResolver.RenameProperty(typeof(Post), "Descendants", "Comments");

			var serializeSettings = new JsonSerializerSettings();
			serializeSettings.ContractResolver = jsonSerializerResolver;


			// start retrieving N  and serializing objects to json
			for (var r = 0; r < topNPosts; r++)
			{
				var id = topPostIds[r];

				Console.WriteLine("Processing id " + id + " count " + (r + 1) + " of " + topNPosts);

				getPostUri = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";


				// uncomment to test error
				// getPostUri = "another invalid url";

				if (!HttpHelper.ValidUrl(getPostUri))
					throw new Exception($"{getPostUri} is an invalid URL");


				var getPost = Task.Run(() => HttpHelper.SendRequest(getPostUri));
				getPost.Wait();


				var post = JsonConvert.DeserializeObject<Post>(getPost.Result);
				post.Rank = r + 1;

				// uncomment to test validation error
				//post.Title = "";
				//post.By = "";

				// uncomment to test 256 validation error
				//post.By = "knGvbM6sS5PKTTMJ7ENGjwnk3bPnIaHFjkAEsMJCFHLRT5UfT4Hyzbxuh3HJ7nfEgWjzWBaSYXXAWaBCSOouiL08ILqJ1qiWKFhOylv2GvbyqW2hM9UAWqcvnbQ5dLG7N4jzVH4MipjsHlNZ8MaPkntEwPInLMIjSlxth8cnqr5ydaopVAk6azahopJIl3GmfOFEdRN6Oxiv4QJQNvHfPficrdjh3SORCrEHzt5Ekd8x5XIEcyhrOsbOSwTG2f2t5";
				//post.Title = "knGvbM6sS5PKTTMJ7ENGjwnk3bPnIaHFjkAEsMJCFHLRT5UfT4Hyzbxuh3HJ7nfEgWjzWBaSYXXAWaBCSOouiL08ILqJ1qiWKFhOylv2GvbyqW2hM9UAWqcvnbQ5dLG7N4jzVH4MipjsHlNZ8MaPkntEwPInLMIjSlxth8cnqr5ydaopVAk6azahopJIl3GmfOFEdRN6Oxiv4QJQNvHfPficrdjh3SORCrEHzt5Ekd8x5XIEcyhrOsbOSwTG2f2t5";

				Validation.ValidatePost(post);


				topPostJsonObjects.Append(JsonConvert.SerializeObject(post, serializeSettings));

				if (r + 1 != topNPosts)
					topPostJsonObjects.Append(",");
			}
			return topPostJsonObjects.ToString();
		}
	}
}
