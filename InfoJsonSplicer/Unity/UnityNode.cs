using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InfoJsonSplicer.Unity
{
	public class UnityNode
	{
		/// <summary>
		/// The unique type name used in the <see cref = "SharedState"/> dictionaries
		/// </summary>
		public string TypeName { get => typeName; set => typeName = value ?? ""; }
		public string Name { get => name; set => name = value ?? ""; }
		public byte Level { get; set; }
		public int ByteSize { get; set; }
		public int Index { get; set; }
		public short Version { get; set; }
		public byte TypeFlags { get; set; }
		public int MetaFlag { get; set; }
		public List<UnityNode> SubNodes { get => subNodes; set => subNodes = value ?? new(); }

		private string typeName = "";
		private string name = "";
		private List<UnityNode> subNodes = new();

		/// <summary>
		/// Deep clones a node and all its subnodes<br/>
		/// Warning: Deep cloning a node with a circular hierarchy will cause an endless loop
		/// </summary>
		/// <returns>The new node</returns>
		public UnityNode DeepClone()
		{
			UnityNode? cloned = new UnityNode();
			cloned.TypeName = CloneString(TypeName);
			cloned.Name = CloneString(Name);
			cloned.Level = Level;
			cloned.ByteSize = ByteSize;
			cloned.Index = Index;
			cloned.Version = Version;
			cloned.TypeFlags = TypeFlags;
			cloned.MetaFlag = MetaFlag;
			cloned.SubNodes = SubNodes.ConvertAll(x => x.DeepClone());
			return cloned;
		}

		private static string CloneString(string? @string)
		{
			return @string == null ? "" : new string(@string);
		}
	}
}