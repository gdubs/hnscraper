using HNScraper.Domains;
using HNScraper.Utils;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HNScraper
{
	class Program
	{
		static void Main(string[] args)
		{

			Console.WriteLine("This app gets the top posts from Hacker News. Please enter a number from 0 to 100");

			int n;
			if (int.TryParse(Console.ReadLine(), out n))
			{
				// input top n posts.. from 1 and 100
				Console.WriteLine("Get the top " + n + " from HN");
				GetNPostsFromHackerNews(n);
			}
			else
			{
				Logging.Log("Wrong input. Please restart");
			}
			
		}


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

		static void GetNPostsFromHackerNews(int topNPosts)
		{
			if(topNPosts < 0 || topNPosts > 100)
			{
				Logging.Log("Invalid total number of posts");
				return;
			}


			try
			{
				Console.WriteLine("Requested top " + topNPosts + " posts to retrieve");


				var topPostUrl = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";

				// uncomment to test error
				// topPostUrl = "invalid url";

				if (!HttpHelper.ValidUrl(topPostUrl))
					throw new Exception($"{topPostUrl} is an invalid URL");

				var getTopPosts = Task.Run(() => HttpHelper.SendRequest(topPostUrl));
				getTopPosts.Wait();

				var topPostIds = JsonConvert.DeserializeObject<int[]>(getTopPosts.Result);
				var topPostJsonObjects = new StringBuilder();
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

				// uncomment to test error
				// throw new Exception("error test");


				// write to file

				var topPosts = $"[{topPostJsonObjects.ToString()}]";
				JsonUtil.WriteJsonFile(topPosts);

			}catch(Exception ex)
			{
				Logging.HandleExceptions(ex);
			}
			
		}
	}
}


