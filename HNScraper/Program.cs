using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HNScraper
{
	class Program
	{
		static void Main(string[] args)
		{
			// input top n posts.. from 1 and 100
			GetNPostsFromHackerNews(12);
		}


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


		/***
		 *	1. Get all top posts id
		 *  2. Get post details by looping through n posts needed
		 * 
		 *   NOTE:
		 *    - logs are created for any exceptions
		 *    - urls are being validated
		 *    - validation fails will also thow an exception and will be logged
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

				//var topPostUrl = "invalid url";

				if (!ValidUrl(topPostUrl))
					throw new Exception($"{topPostUrl} is an invalid URL");

				var getTopPosts = Task.Run(() => SendRequest(topPostUrl));
				getTopPosts.Wait();

				var topPostIds = JsonConvert.DeserializeObject<int[]>(getTopPosts.Result);
				var topPostJsonObjects = new StringBuilder();
				var getPostUri = string.Empty;

				for (var r = 0; r < topNPosts; r++)
				{
					var id = topPostIds[r];

					Console.WriteLine("Processing id " + id + " count " + (r + 1) + " of " + topNPosts);

					getPostUri = $"https://hacker-news.firebaseio.com/v0/item/{id}.json?print=pretty";


					//var getPostUri = "another invalid url";

					if (!ValidUrl(getPostUri))
						throw new Exception($"{getPostUri} is an invalid URL");


					var getPost = Task.Run(() => SendRequest(getPostUri));
					getPost.Wait();

					var post = JsonConvert.DeserializeObject<Post>(getPost.Result);

					// test post.Title = "";

					ValidatePost(post);
						


					topPostJsonObjects.Append(getPost.Result);

					if (r + 1 != topNPosts)
						topPostJsonObjects.Append(",");
				}

				//throw new Exception("error test");

				var topPosts = $"[{topPostJsonObjects.ToString()}]";
				WriteJsonFile(topPosts);
			}catch(Exception ex)
			{
				Console.WriteLine("Something went wrong, please check logs");
				Log(ex.Message);
				throw ex;
			}
			
		}

		static async Task<string> SendRequest(string url)
		{
			try
			{
				var response = string.Empty;
				var client = CreateHttpClient();
				var responseMessage = await client.GetAsync(url); //.ConfigureAwait(false);

				if (responseMessage.IsSuccessStatusCode)
				{
					response = await responseMessage.Content.ReadAsStringAsync();
				}

				client.Dispose();

				return response;
			}
			catch(Exception ex)
			{
				Console.WriteLine("Something went wrong, please check logs");
				Log(ex.Message);
				throw ex;
			}
		}

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
			catch(Exception ex)
			{
				return false;
			}
		}

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

		static HttpClient CreateHttpClient()
		{
			return new HttpClient();
		}

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
