using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Tools.IO;

namespace Microsoft.Phone.Test.TestMetadata.ObjectModel
{
	// Token: 0x02000014 RID: 20
	[Serializable]
	public sealed class PackageDependency : Dependency
	{
		// Token: 0x0600004E RID: 78 RVA: 0x000032A8 File Offset: 0x000014A8
		public PackageDependency()
		{
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003330 File Offset: 0x00001530
		public PackageDependency(string name)
		{
			bool flag = name.EndsWith(".cab", StringComparison.OrdinalIgnoreCase) || name.EndsWith(".spkg", StringComparison.OrdinalIgnoreCase);
			if (flag)
			{
				this.Name = Path.Combine(LongPathPath.GetDirectoryName(name), LongPathPath.GetFileNameWithoutExtension(name));
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00003380 File Offset: 0x00001580
		// (set) Token: 0x06000051 RID: 81 RVA: 0x00003388 File Offset: 0x00001588
		[XmlAttribute]
		public string Name { get; set; }

		// Token: 0x06000052 RID: 82 RVA: 0x00003394 File Offset: 0x00001594
		public override bool Equals(object obj)
		{
			PackageDependency packageDependency = obj as PackageDependency;
			bool flag = packageDependency == null;
			return !flag && this.Name.Equals(packageDependency.Name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000033CC File Offset: 0x000015CC
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}
	}
}
