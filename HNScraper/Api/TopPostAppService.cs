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

				if (!IsValid(post))
					throw new Exception(String.Join(" ", ValidationErrors));

				topPostJsonObjects.Append(JsonConvert.SerializeObject(post, serializeSettings));

				if (r + 1 != topNPosts)
					topPostJsonObjects.Append(",");
			}

			return topPostJsonObjects.ToString();
		}


		// TODO: removed this from Post, looks ugly when converting to json
		//       maybe make sense to add somewhere else. Maybe another service inside domain??
		List<string> ValidationErrors;
		public bool IsValid(Post post)
		{
			ValidationErrors = new List<string>();

			if (string.IsNullOrEmpty(post.Title))
				ValidationErrors.Add("\r\n Title is an empty string");

			if (string.IsNullOrEmpty(post.By))
				ValidationErrors.Add("\r\n Author is an empty string");

			if (post.Title?.Length >= 256)
				ValidationErrors.Add("\r\n Title is greater than 256 characters");

			if (post.By?.Length >= 256)
				ValidationErrors.Add("\r\n Author is greater than 256 characters");

			if(post.Rank < 0)
				ValidationErrors.Add("\r\n Invalid rank");

			if (post.Descendants < 0)
				ValidationErrors.Add("\r\n Invalid comments");

			if (post.Score < 0)
				ValidationErrors.Add("\r\n Invalid points");

			if (ValidationErrors.Count > 0)
				ValidationErrors.Insert(0, "Validation errors for id " + post.Id);

			return ValidationErrors.Count == 0;
		}
	}
}

