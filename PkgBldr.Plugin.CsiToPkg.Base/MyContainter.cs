using System;
using System.Xml.Linq;

namespace Microsoft.CompPlat.PkgBldr.Plugins.CsiToPkg
{
	// Token: 0x0200000E RID: 14
	internal class MyContainter
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00003944 File Offset: 0x00001B44
		// (set) Token: 0x06000028 RID: 40 RVA: 0x0000394C File Offset: 0x00001B4C
		public Security Security { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00003955 File Offset: 0x00001B55
		// (set) Token: 0x0600002A RID: 42 RVA: 0x0000395D File Offset: 0x00001B5D
		public XElement RegKeys { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00003966 File Offset: 0x00001B66
		// (set) Token: 0x0600002C RID: 44 RVA: 0x0000396E File Offset: 0x00001B6E
		public XElement Files { get; set; }
	}
}
