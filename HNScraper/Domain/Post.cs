using Newtonsoft.Json;
using System.Collections.Generic;

namespace HNScraper.Domain
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


		// TODO: removed this from Post, looks ugly when converting to json
		//       maybe make sense to add somewhere else. Maybe another service inside domain??
		[JsonIgnore]
		List<string> ValidationErrors;
		public bool IsValid()
		{
			ValidationErrors = new List<string>();

			if (string.IsNullOrEmpty(this.Title))
				ValidationErrors.Add("\r\n Title is an empty string");

			if (string.IsNullOrEmpty(this.By))
				ValidationErrors.Add("\r\n Author is an empty string");

			if (this.Title?.Length >= 256)
				ValidationErrors.Add("\r\n Title is greater than 256 characters");

			if (this.By?.Length >= 256)
				ValidationErrors.Add("\r\n Author is greater than 256 characters");

			if (this.Rank < 0)
				ValidationErrors.Add("\r\n Invalid rank");

			if (this.Descendants < 0)
				ValidationErrors.Add("\r\n Invalid comments");

			if (this.Score < 0)
				ValidationErrors.Add("\r\n Invalid points");

			if (ValidationErrors.Count > 0)
				ValidationErrors.Insert(0, "Validation errors for id " + this.Id);

			return ValidationErrors.Count == 0;
		}

		public List<string> GetValidationErrors()
		{
			return ValidationErrors;
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
