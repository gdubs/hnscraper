using HNScraper.Common;
using HNScraper.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HNScraper.Api
{
	public class TopPostAppService
	{
		readonly HackerNewsAppService _hnService = new HackerNewsAppService();

		/***
		 *	1. Get all top posts id
		 *  2. Get post details by looping through n posts needed
		 * 
		 *   NOTE:
		 *    - logs are created for any exceptions
		 *    - urls are being validated
		 *    - validation fails will also thow an exception and will be logged
		 *    - results can be found in the bin folder with a timestamp
		 * 
		 * ***/

		public void CreateTopPostsJsonFile(int topNPosts)
		{
			if (topNPosts < 0 || topNPosts > 100)
			{
				throw new Exception("Invalid total number of posts");
			}


			try
			{
				Console.WriteLine("Requested top " + topNPosts + " posts to retrieve");

				var topPostJsonObjects = GetTopNPosts(topNPosts);
				var topPosts = $"[{topPostJsonObjects.ToString()}]";

				// create file
				JsonUtil.WriteJsonFile(topPosts);

			}
			catch (Exception ex)
			{
				Logging.HandleExceptions(ex);
			}

		}


		public string GetTopNPosts(int topNPosts)
		{
			// uncomment to test error
			// topPostUrl = "invalid url";

			var topPostIds = _hnService.GetTopPostIds();
			var topPostJsonObjects = new StringBuilder();

			// setup settings for converting the right properties when serializing
			var jsonSerializerResolver = new JsonSerializeResolver();
			jsonSerializerResolver.RenameProperty(typeof(Post), "By", "Author");
			jsonSerializerResolver.RenameProperty(typeof(Post), "Score", "Points");
			jsonSerializerResolver.RenameProperty(typeof(Post), "Descendants", "Comments");
			jsonSerializerResolver.RenameProperty(typeof(Post), "Url", "Uri");

			var serializeSettings = new JsonSerializerSettings() { ContractResolver = jsonSerializerResolver };

			var getPostUri = string.Empty;

			// start retrieving N  and serializing objects to json
			for (var r = 0; r < topNPosts; r++)
			{
				var id = topPostIds[r];

				Console.WriteLine("Processing id " + id + " count " + (r + 1) + " of " + topNPosts);

				var postStringObject = _hnService.GetTopPostObject(id);
				var post = JsonConvert.DeserializeObject<Post>(postStringObject);
				post.Rank = r + 1;

				if (!post.IsValid())
					throw new Exception(String.Join(" ", post.GetValidationErrors()));

				topPostJsonObjects.Append(JsonConvert.SerializeObject(post, serializeSettings));

				if (r + 1 != topNPosts)
					topPostJsonObjects.Append(",");
			}

			return topPostJsonObjects.ToString();
		}
		
	}
}

