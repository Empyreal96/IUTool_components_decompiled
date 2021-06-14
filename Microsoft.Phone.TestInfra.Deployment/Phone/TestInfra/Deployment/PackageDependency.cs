using System;
using System.Xml.Serialization;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000036 RID: 54
	public class PackageDependency : Dependency
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000256 RID: 598 RVA: 0x0000EB06 File Offset: 0x0000CD06
		// (set) Token: 0x06000257 RID: 599 RVA: 0x0000EB0E File Offset: 0x0000CD0E
		[XmlIgnore]
		public string PkgName { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000EB17 File Offset: 0x0000CD17
		// (set) Token: 0x06000259 RID: 601 RVA: 0x0000EB1F File Offset: 0x0000CD1F
		[XmlAttribute(AttributeName = "Name")]
		public string RelativePath { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600025A RID: 602 RVA: 0x0000EB28 File Offset: 0x0000CD28
		// (set) Token: 0x0600025B RID: 603 RVA: 0x0000EB30 File Offset: 0x0000CD30
		[XmlIgnore]
		public string AbsolutePath { get; set; }

		// Token: 0x0600025C RID: 604 RVA: 0x0000EB3C File Offset: 0x0000CD3C
		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = obj.GetType() != base.GetType();
				result = (!flag2 && this.Equals(obj as PackageDependency));
			}
			return result;
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000EB84 File Offset: 0x0000CD84
		public bool Equals(PackageDependency other)
		{
			return string.Compare(this.PkgName, other.PkgName, true) == 0;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000EBAC File Offset: 0x0000CDAC
		public override int GetHashCode()
		{
			return this.PkgName.ToLowerInvariant().GetHashCode();
		}
	}
}
