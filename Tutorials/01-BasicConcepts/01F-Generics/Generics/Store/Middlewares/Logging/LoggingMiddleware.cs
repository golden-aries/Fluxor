using Fluxor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BasicConcepts.Generics.Store.Middlewares.Logging
{
	public class LoggingMiddleware : Middleware
	{
		private IStore Store;

		public override Task InitializeAsync(IStore store)
		{
			Store = store;
			return Task.CompletedTask;
		}

		public override void AfterDispatch(object action)
		{
			Console.WriteLine();
			Log($"After {ObjectInfo(action)}");
			foreach (KeyValuePair<string, IFeature> feature in Store.Features)
			{
				PropertyInfo propertyInfo = feature.Value.GetState().GetType().GetProperty("IsBusyCount");
				var count = (uint)propertyInfo.GetValue(feature.Value.GetState());
				Console.WriteLine($"\t{feature.Key}.IsBusyCount is now {count}");
			}
			Console.WriteLine();
		}

		private string ObjectInfo(object obj)
			=> ": " + ExpandTypeName(obj.GetType()) + " " + JsonConvert.SerializeObject(obj, Formatting.Indented);

		private static void Log(string text)
		{
			Console.WriteLine($"Middleware > {text}");
		}

		private static string ExpandTypeName(Type t) =>
			!t.IsGenericType || t.IsGenericTypeDefinition
			? !t.IsGenericTypeDefinition ? t.Name : t.Name.Remove(t.Name.IndexOf('`'))
			: $"{ExpandTypeName(t.GetGenericTypeDefinition())}<{string.Join(',', t.GetGenericArguments().Select(x => ExpandTypeName(x)))}>";
	}

}
