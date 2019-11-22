using HNScraper.Api;
using HNScraper.Common;
using System;

namespace HNScraper
{
	class Program
	{
		static void Main(string[] args)
		{

			Console.WriteLine("This app gets the top posts from Hacker News. Please enter a number from 0 to 100");

			var topPostService = new TopPostAppService();

			int n;
			if (int.TryParse(Console.ReadLine(), out n))
			{
				// input top n posts.. from 1 and 100
				Console.WriteLine("Get the top " + n + " from HN");
				topPostService.CreateTopPostsJsonFile(n);
			}
			else
			{
				Logging.Log("Wrong input. Please restart");
			}
			
		}
	}
}


