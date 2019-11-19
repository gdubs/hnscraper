﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNScraper.Utils
{
	static public class JsonUtil
	{

		// write file
		static public void WriteJsonFile(string json)
		{

			Console.WriteLine("Writing file");

			long timeStamp = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
			string path = $"topposts_{timeStamp}.json";
			string parsedJson = string.Empty;

			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}

			using (StreamWriter writer = new StreamWriter(path, true))
			{
				parsedJson = json;
			}

			File.WriteAllText(path, parsedJson);
		}
	}
}
