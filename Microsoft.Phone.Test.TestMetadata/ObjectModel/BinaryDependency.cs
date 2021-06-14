using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.Test.TestMetadata.ObjectModel
{
	// Token: 0x02000013 RID: 19
	[Serializable]
	public sealed class BinaryDependency : Dependency
	{
		// Token: 0x06000048 RID: 72 RVA: 0x000032A8 File Offset: 0x000014A8
		internal BinaryDependency()
		{
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000032B2 File Offset: 0x000014B2
		public BinaryDependency(string name)
		{
			this.Name = name;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000032C4 File Offset: 0x000014C4
		// (set) Token: 0x0600004B RID: 75 RVA: 0x000032CC File Offset: 0x000014CC
		[XmlAttribute]
		public string Name { get; set; }

		// Token: 0x0600004C RID: 76 RVA: 0x000032D8 File Offset: 0x000014D8
		public override bool Equals(object obj)
		{
			BinaryDependency binaryDependency = obj as BinaryDependency;
			bool flag = binaryDependency == null;
			return !flag && this.Name.Equals(binaryDependency.Name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003310 File Offset: 0x00001510
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}
	}
}
