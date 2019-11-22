using HNScraper.Api;
using HNScraper.Core;
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
				

				var postApi = new PostApi();
				var topPostJsonObjects = postApi.GetTopNPosts(topNPosts);

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


