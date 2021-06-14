using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.Test.TestMetadata.ObjectModel
{
	// Token: 0x02000016 RID: 22
	[Serializable]
	public sealed class EnvironmentPathDependnecy : Dependency
	{
		// Token: 0x06000066 RID: 102 RVA: 0x000032A8 File Offset: 0x000014A8
		public EnvironmentPathDependnecy()
		{
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003616 File Offset: 0x00001816
		public EnvironmentPathDependnecy(string name)
		{
			this.Name = name;
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00003628 File Offset: 0x00001828
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00003630 File Offset: 0x00001830
		[XmlAttribute]
		public string Name { get; set; }

		// Token: 0x0600006A RID: 106 RVA: 0x0000363C File Offset: 0x0000183C
		public override bool Equals(object obj)
		{
			EnvironmentPathDependnecy environmentPathDependnecy = obj as EnvironmentPathDependnecy;
			bool flag = environmentPathDependnecy == null;
			return !flag && this.Name.Equals(environmentPathDependnecy.Name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003674 File Offset: 0x00001874
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}
	}
}
