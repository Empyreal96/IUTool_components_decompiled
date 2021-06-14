using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x0200001C RID: 28
	[Serializable]
	public class PackageDescription
	{
		// Token: 0x06000139 RID: 313 RVA: 0x00008B61 File Offset: 0x00006D61
		public PackageDescription()
		{
			this.RelativePath = string.Empty;
			this.Dependencies = new List<Dependency>();
			this.Binaries = new List<string>();
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600013A RID: 314 RVA: 0x00008B8F File Offset: 0x00006D8F
		// (set) Token: 0x0600013B RID: 315 RVA: 0x00008B97 File Offset: 0x00006D97
		[XmlAttribute(AttributeName = "Path")]
		public string RelativePath { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00008BA0 File Offset: 0x00006DA0
		// (set) Token: 0x0600013D RID: 317 RVA: 0x00008BA8 File Offset: 0x00006DA8
		[XmlElement]
		public List<Dependency> Dependencies { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600013E RID: 318 RVA: 0x00008BB1 File Offset: 0x00006DB1
		// (set) Token: 0x0600013F RID: 319 RVA: 0x00008BB9 File Offset: 0x00006DB9
		public List<string> Binaries { get; set; }
	}
}
