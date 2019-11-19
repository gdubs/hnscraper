using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HNScraper.Utils
{
	// renames specific properties to another name
	public class JsonSerializeResolver : DefaultContractResolver
	{
		readonly Dictionary<Type, Dictionary<string, string>> _renameMappings;

		public JsonSerializeResolver()
		{
			_renameMappings = new Dictionary<Type, Dictionary<string, string>>();
		}

		public void RenameProperty(Type type, string prop, string newProp)
		{
			if (!_renameMappings.ContainsKey(type))
				_renameMappings[type] = new Dictionary<string, string>();

			_renameMappings[type][prop] = newProp;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var prop = base.CreateProperty(member, memberSerialization);


			if (IsRenamed(prop.DeclaringType, prop.PropertyName, out var newProp))
				prop.PropertyName = newProp;

			return prop;
		}

		bool IsRenamed(Type type, string prop, out string newProp)
		{
			Dictionary<string, string> renamedProperties;

			if (!_renameMappings.TryGetValue(type, out renamedProperties) || !renamedProperties.TryGetValue(prop, out newProp))
			{
				newProp = null;
				return false;
			}

			return true;
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
