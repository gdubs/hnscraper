using System;
using HNScraper.Api;
using HNScraper.Common;
using HNScraper.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HNScraper.Test
{
	[TestClass]
	public class UnitTest1
	{
		TopPostAppService toppostService = new TopPostAppService();
		


		[TestMethod]
		public void InvalidUrl()
		{
			string url = "thisisaninvalidurl";

			Assert.IsFalse(HttpHelper.ValidUrl(url));
		}

		[TestMethod]
		public void InvalidPost_EmptyTitle()
		{
			Post post = new Post
			{
				By = "JohnSmith",
				Title = "",
				Score = 1,
				Descendants = 1,
				Rank = 1
			};

			Assert.IsFalse(post.IsValid());
		}

		[TestMethod]
		public void InvalidPost_256Title()
		{
			Post post = new Post
			{
				By = "JohnSmith",
				Title = "knGvbM6sS5PKTTMJ7ENGjwnk3bPnIaHFjkAEsMJCFHLRT5UfT4Hyzbxuh3HJ7nfEgWjzWBaSYXXAWaBCSOouiL08ILqJ1qiWKFhOylv2GvbyqW2hM9UAWqcvnbQ5dLG7N4jzVH4MipjsHlNZ8MaPkntEwPInLMIjSlxth8cnqr5ydaopVAk6azahopJIl3GmfOFEdRN6Oxiv4QJQNvHfPficrdjh3SORCrEHzt5Ekd8x5XIEcyhrOsbOSwTG2f2t5",
				Score = 1,
				Descendants = 1,
				Rank = 1
			};

			Assert.IsFalse(post.IsValid());
		}

		[TestMethod]
		public void InvalidPost_EmptyAuthor()
		{
			Post post = new Post
			{
				By = "",
				Title = "Title",
				Score = 1,
				Descendants = 1,
				Rank = 1
			};

			Assert.IsFalse(post.IsValid());
		}

		[TestMethod]
		public void InvalidPost_256Author()
		{
			Post post = new Post
			{
				By = "knGvbM6sS5PKTTMJ7ENGjwnk3bPnIaHFjkAEsMJCFHLRT5UfT4Hyzbxuh3HJ7nfEgWjzWBaSYXXAWaBCSOouiL08ILqJ1qiWKFhOylv2GvbyqW2hM9UAWqcvnbQ5dLG7N4jzVH4MipjsHlNZ8MaPkntEwPInLMIjSlxth8cnqr5ydaopVAk6azahopJIl3GmfOFEdRN6Oxiv4QJQNvHfPficrdjh3SORCrEHzt5Ekd8x5XIEcyhrOsbOSwTG2f2t5",
				Title = "Title",
				Score = 1,
				Descendants = 1,
				Rank = 1
			};

			Assert.IsFalse(post.IsValid());
		}

		[TestMethod]
		public void InvalidPost_Points0()
		{
			Post post = new Post
			{
				By = "JohnSmith",
				Title = "Title",
				Score = -1,
				Descendants = 1,
				Rank = 1
			};

			Assert.IsFalse(post.IsValid());
		}

		[TestMethod]
		public void InvalidPost_Comments0()
		{
			Post post = new Post
			{
				By = "JohnSmith",
				Title = "Title",
				Score = 1,
				Descendants = -1,
				Rank = 1
			};

			Assert.IsFalse(post.IsValid());
		}

		[TestMethod]
		public void InvalidPost_Rank0()
		{
			Post post = new Post
			{
				By = "JohnSmith",
				Title = "Title",
				Score = 1,
				Descendants = 1,
				Rank = -1
			};

			Assert.IsFalse(post.IsValid());
		}

		[TestMethod]
		public void InvalidPost()
		{
			try
			{
				toppostService.CreateTopPostsJsonFile(-1);
				Assert.Fail();
			}catch(Exception ex)
			{
				Assert.AreEqual("Invalid total number of posts", ex.Message);
			}
			
		}
	}
}
