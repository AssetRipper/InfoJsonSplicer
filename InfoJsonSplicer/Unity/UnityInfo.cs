using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InfoJsonSplicer.Unity
{
	public class UnityInfo
	{
		private string version = "";
		private List<UnityString> strings = new();
		private List<UnityClass> classes = new();

		public string Version { get => version; set => version = value ?? ""; }
		public List<UnityString> Strings { get => strings; set => strings = value ?? new(); }
		public List<UnityClass> Classes { get => classes; set => classes = value ?? new(); }

		public string Serialize()
		{
			JsonSerializerOptions options = new JsonSerializerOptions();
			options.WriteIndented = true;
			return JsonSerializer.Serialize(this, options);
		}

		public void SerializeToFile(string path)
		{
			JsonSerializerOptions options = new JsonSerializerOptions();
			options.WriteIndented = true;
			using var stream = File.Create(path);
			JsonSerializer.Serialize(stream, this, options);
		}

		public static UnityInfo? DeserializeFromPath(string path)
		{
			using var stream = File.OpenRead(path);
			return JsonSerializer.Deserialize<UnityInfo>(stream);
		}
	}
}