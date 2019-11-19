namespace HNScraper.Domains
{
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
