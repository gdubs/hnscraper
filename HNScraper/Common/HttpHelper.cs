using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HNScraper.Common
{
	static public class HttpHelper
	{
		//start http client for http request
		static public HttpClient CreateHttpClient()
		{
			return new HttpClient();
		}

		// http request
		static public async Task<string> SendRequest(string url)
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
			catch (Exception ex)
			{
				Logging.HandleExceptions(ex);
			}

			return "";
		}

		// validate url exists
		static public bool ValidUrl(string url)
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
	}
}
