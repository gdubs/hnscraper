using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HNScraper
{
	class Program
	{
		static void Main(string[] args)
		{
			// input top n posts.. from 1 and 100
			GetNPostsFromHackerNews(3);
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
				Log("Invalid total number of posts");
				return;
			}


			try
			{
				Console.WriteLine("Requested top " + topNPosts + " posts to retrieve");


				var topPostUrl = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";

				// uncomment to test error
				// topPostUrl = "invalid url";

				if (!ValidUrl(topPostUrl))
					throw new Exception($"{topPostUrl} is an invalid URL");

				var getTopPosts = Task.Run(() => SendRequest(topPostUrl));
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

					if (!ValidUrl(getPostUri))
						throw new Exception($"{getPostUri} is an invalid URL");


					var getPost = Task.Run(() => SendRequest(getPostUri));
					getPost.Wait();


					var post = JsonConvert.DeserializeObject<Post>(getPost.Result);
					post.Rank = r + 1;

					// uncomment to test error
					//post.Title = "";

					ValidatePost(post);


					topPostJsonObjects.Append(JsonConvert.SerializeObject(post, serializeSettings));

					if (r + 1 != topNPosts)
						topPostJsonObjects.Append(",");
				}

				// uncomment to test error
				// throw new Exception("error test");


				// write to file

				var topPosts = $"[{topPostJsonObjects.ToString()}]";
				WriteJsonFile(topPosts);

			}catch(Exception ex)
			{
				HandleExceptions(ex);
			}
			
		}


		//start http client for http request
		static HttpClient CreateHttpClient()
		{
			return new HttpClient();
		}

		// http request
		static async Task<string> SendRequest(string url)
		{
			try
			{
				var response = string.Empty;
				var client = CreateHttpClient();
				var responseMessage = await client.GetAsync(url); 

				if (responseMessage.IsSuccessStatusCode)
				{
					response = await responseMessage.Content.ReadAsStringAsync();
				}

				client.Dispose();

				return response;
			}
			catch(Exception ex)
			{
				HandleExceptions(ex);
			}

			return "";
		}

		// validate url exists
		static bool ValidUrl(string url)
		{
			try
			{
				var client = CreateHttpClient();
				var httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, url);
				client.SendAsync(httpRequestMessage);

				client.Dispose();

				return true;
			}
			catch
			{
				return false;
			}
		}

		// validate property values
		static bool ValidPostObject(Post post,out List<string> validationErrors)
		{
			validationErrors = new List<string>();

			if (string.IsNullOrEmpty(post.Title))
				validationErrors.Add("\r\n Title is an empty string");

			if (string.IsNullOrEmpty(post.By))
				validationErrors.Add("\r\n Author is an empty string");

			if(post.Title?.Length >= 256)
				validationErrors.Add("\r\n Title is greater than 256 characters");

			if (post.By?.Length >= 256)
				validationErrors.Add("\r\n Author is greater than 256 characters");

			if (validationErrors.Count > 0)
				validationErrors.Insert(0, "Validation errors for id " + post.Id);

			return validationErrors.Count == 0;
		}

		// validate instance of object
		static void ValidatePost(Post post)
		{
			var validationErrors = new List<string>();
			if (!ValidPostObject(post, out validationErrors))
			{
				var errors = new StringBuilder();
				validationErrors.ForEach(e => errors.Append($"{e}"));
				throw new Exception(errors.ToString());
			}
		}

		// write file
		static void WriteJsonFile(string json)
		{

			Console.WriteLine("Writing file");

			long timeStamp = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
			string path = $"topposts_{timeStamp}.json";
			string parsedJson = string.Empty;

			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}

			using(StreamWriter writer = new StreamWriter(path, true))
			{
				parsedJson = json;
			}

			File.WriteAllText(path, parsedJson);
		}

		// write log for exceptions
		static void Log(string message)
		{
			long timeStamp = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
			string path = $"log_{timeStamp}.txt";

			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}

			File.WriteAllText(path, message);
		}

		// catch exception handler
		static void HandleExceptions(Exception ex)
		{
			// this might not be the most ideal since it's a little confusing as to which catch it got triggered..
			// needs improvement. was just trying to avoid code repeat.

			Console.WriteLine("Something went wrong, please check logs");
			Log(ex.Message);
			throw ex;
		}

		public class Post
		{
			public int Id { get; set; }
			public string By { get; set; }
			public int Descendants { get; set; }
			public int[] Kids { get; set; }
			public int Score { get; set; }
			public int Time { get; set; }
			public string Title { get; set; }
			public string Type { get; set; }
			public string Url { get; set; }
			public int Rank { get; set; }
		}
	}


	// renames specific properties to another name
	public class JsonSerializeResolver : DefaultContractResolver
	{
		readonly Dictionary<Type, Dictionary<string, string>> _renameMappings;

		public JsonSerializeResolver()
		{
			_renameMappings = new Dictionary<Type, Dictionary<string, string>>();
		}

		public void RenameProperty(Type type, string prop, string newProp)
		{
			if (!_renameMappings.ContainsKey(type))
				_renameMappings[type] = new Dictionary<string, string>();

			_renameMappings[type][prop] = newProp;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var prop = base.CreateProperty(member, memberSerialization);


			if (IsRenamed(prop.DeclaringType, prop.PropertyName, out var newProp))
				prop.PropertyName = newProp;

			return prop;
		}

		bool IsRenamed(Type type, string prop, out string newProp)
		{
			Dictionary<string, string> renamedProperties;

			if(!_renameMappings.TryGetValue(type, out renamedProperties) || !renamedProperties.TryGetValue(prop, out newProp))
			{
				newProp = null;
				return false;
			}

			return true;
		}
	}
}



/*
 
	 {
  "by" : "Cthulhu_",
  "descendants" : 46,
  "id" : 21545425,
  "kids" : [ 21552962, 21552846, 21552959, 21552742, 21552764, 21552633, 21552811, 21552886, 21552815, 21552840, 21552921, 21552887, 21552730, 21552878 ],
  "score" : 80,
  "time" : 1573830832,
  "title" : "The Value in Go’s Simplicity",
  "type" : "story",
  "url" : "https://benjamincongdon.me/blog/2019/11/11/The-Value-in-Gos-Simplicity/"
}
	 
	 */
