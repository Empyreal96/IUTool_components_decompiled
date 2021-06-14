using System;
using System.Xml.Linq;

namespace Microsoft.CompPlat.PkgBldr.Plugins.PkgToWm
{
	// Token: 0x02000009 RID: 9
	public class ComData
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002DF4 File Offset: 0x00000FF4
		// (set) Token: 0x06000012 RID: 18 RVA: 0x00002DFC File Offset: 0x00000FFC
		public XElement RegKeys { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002E05 File Offset: 0x00001005
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00002E0D File Offset: 0x0000100D
		public XElement Files { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002E16 File Offset: 0x00001016
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002E1E File Offset: 0x0000101E
		public XElement InProcServer { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002E27 File Offset: 0x00001027
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002E2F File Offset: 0x0000102F
		public XElement InProcServerClasses { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002E38 File Offset: 0x00001038
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002E40 File Offset: 0x00001040
		public XElement Interfaces { get; set; }
	}
}
