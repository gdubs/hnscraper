using System;
using System.IO;

namespace HNScraper.Utils
{
	static public class Logging
	{
		// write log for exceptions
		static public void Log(string message)
		{
			Console.WriteLine(message);
			Console.WriteLine("Writing on log");

			long timeStamp = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
			string path = $"log_{timeStamp}.txt";

			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}

			File.WriteAllText(path, message);

			// wait for user to keypress before closing to make sure user reads the instruction
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();
		}

		// catch exception handler
		static public void HandleExceptions(Exception ex)
		{
			// this might not be the most ideal since it's a little confusing as to which catch it got triggered..
			// needs improvement. was just trying to avoid code repeat.

			Console.WriteLine("Something went wrong, please check logs");
			Log(ex.Message);
			//throw ex;
		}
	}
}
