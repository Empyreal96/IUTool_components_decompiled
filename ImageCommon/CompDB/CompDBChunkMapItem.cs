using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.CompDB
{
	// Token: 0x02000006 RID: 6
	public class CompDBChunkMapItem
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00004257 File Offset: 0x00002457
		public CompDBChunkMapItem()
		{
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000425F File Offset: 0x0000245F
		public CompDBChunkMapItem(CompDBChunkMapItem src)
		{
			this.ChunkName = src.ChunkName;
			this.Path = src.Path;
			this.Type = src.Type;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000428C File Offset: 0x0000248C
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.Path,
				" (",
				this.ChunkName,
				"\\",
				this.Type.ToString(),
				")"
			});
		}

		// Token: 0x04000024 RID: 36
		[XmlAttribute]
		public string ChunkName;

		// Token: 0x04000025 RID: 37
		[XmlAttribute]
		public string Path;

		// Token: 0x04000026 RID: 38
		[XmlAttribute]
		public DesktopCompDBGen.PackageTypes Type;
	}
}
