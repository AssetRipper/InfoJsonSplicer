using InfoJsonSplicer.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfoJsonSplicer
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				UnityInfo spliced = SpliceJsons();
				spliced.RecalculateDerived();
				spliced.SerializeToFile("genshin.json");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			Console.WriteLine("done");
			Console.ReadLine();
		}

		private static UnityInfo GetReleaseUnityInfo()
		{
			return UnityInfo.DeserializeFromPath(@"E:\repos\InfoJsonSplicer\InfoJsonSplicer\bin\Debug\net6.0\info_modified_nes.json")
				?? throw new Exception("Could not load the release json");
		}

		private static UnityInfo GetEditorUnityInfo()
		{
			return UnityInfo.DeserializeFromPath(@"E:\repos\InfoJsonSplicer\InfoJsonSplicer\bin\Debug\net6.0\2017.4.30f1.json")
				?? throw new Exception("Could not load the editor json");
		}

		private static UnityInfo SpliceJsons()
		{
			UnityInfo result = new();
			UnityInfo releaseInfo = GetReleaseUnityInfo();
			UnityInfo editorInfo = GetEditorUnityInfo();

			result.Strings = releaseInfo.Strings;
			result.Version = releaseInfo.Version;

			Dictionary<int, UnityClass> releaseClasses = releaseInfo.Classes.ToDictionary(c => c.TypeID, c => c);
			Dictionary<int, UnityClass> editorClasses = editorInfo.Classes.ToDictionary(c => c.TypeID, c => c);
			List<int> typeIDs = releaseClasses.Keys.Union(editorClasses.Keys).ToList();
			typeIDs.Sort();
			foreach(int id in typeIDs)
			{
				bool hasRelease = releaseClasses.TryGetValue(id, out UnityClass? releaseClass);
				bool hasEditor = editorClasses.TryGetValue(id, out UnityClass? editorClass);
				if (hasRelease && hasEditor)
				{
					releaseClass!.EditorRootNode = editorClass!.EditorRootNode;
					result.Classes.Add(releaseClass);
				}
				else if (hasRelease)
				{
					releaseClass!.EditorRootNode = null;
					result.Classes.Add(releaseClass);
				}
				else if (hasEditor)
				{
					editorClass!.ReleaseRootNode = null;
					editorClass.IsEditorOnly = true;
					result.Classes.Add(editorClass);
				}
				else
				{
					throw new Exception(id.ToString());
				}
			}
			return result;
		}

		private static void RecalculateDerived(this UnityInfo info)
		{
			foreach(UnityClass unityClass in info.Classes)
			{
				unityClass.DescendantCount = 0;
				unityClass.Derived = info.Classes.Where(c => c.Base == unityClass.Name).Select(x => x.Name).ToList();
			}

			Dictionary<string, UnityClass> classDictionary = info.Classes.ToDictionary(x => x.Name, x => x);
			foreach (UnityClass unityClass in info.Classes)
			{
				unityClass.FixDescendantCount(classDictionary);
			}
		}

		private static void FixDescendantCount(this UnityClass unityClass, Dictionary<string, UnityClass> classDictionary)
		{
			if (unityClass.DescendantCount == 0)
			{
				foreach (string derivedClassName in unityClass.Derived!)
				{
					UnityClass derivedClass = classDictionary[derivedClassName];
					derivedClass.FixDescendantCount(classDictionary);
					unityClass.DescendantCount += derivedClass.DescendantCount;
				}
				unityClass.DescendantCount++;
			}
		}
	}
}