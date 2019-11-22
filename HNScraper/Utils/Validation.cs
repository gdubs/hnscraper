using HNScraper.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HNScraper.Core
{
	static public class Validation
	{
		// validate property values
		static public bool ValidPostObject(Post post, out List<string> validationErrors)
		{
			validationErrors = new List<string>();

			if (string.IsNullOrEmpty(post.Title))
				validationErrors.Add("\r\n Title is an empty string");

			if (string.IsNullOrEmpty(post.By))
				validationErrors.Add("\r\n Author is an empty string");

			if (post.Title?.Length >= 256)
				validationErrors.Add("\r\n Title is greater than 256 characters");

			if (post.By?.Length >= 256)
				validationErrors.Add("\r\n Author is greater than 256 characters");

			if (validationErrors.Count > 0)
				validationErrors.Insert(0, "Validation errors for id " + post.Id);

			return validationErrors.Count == 0;
		}

		// validate instance of object
		static public void ValidatePost(Post post)
		{
			var validationErrors = new List<string>();
			if (!ValidPostObject(post, out validationErrors))
			{
				var errors = new StringBuilder();
				validationErrors.ForEach(e => errors.Append($"{e}"));
				throw new Exception(errors.ToString());
			}
		}

	}
}
