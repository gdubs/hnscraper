using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HNScraper.Common
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

