using System;

namespace Microsoft.CompPlat.PkgBldr.Base.Security.SecurityPolicy
{
	// Token: 0x02000054 RID: 84
	public class WnfValue
	{
		// Token: 0x0600019D RID: 413 RVA: 0x0000A76E File Offset: 0x0000896E
		public WnfValue(string WnfName, string WnfTag, string WnfScope, string WnfSequence)
		{
			this.Name = WnfName;
			this.Tag = WnfTag;
			this.Scope = WnfScope;
			this.Sequence = WnfSequence;
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600019E RID: 414 RVA: 0x0000A793 File Offset: 0x00008993
		// (set) Token: 0x0600019F RID: 415 RVA: 0x0000A79B File Offset: 0x0000899B
		public string Name { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000A7A4 File Offset: 0x000089A4
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x0000A7AC File Offset: 0x000089AC
		public string Tag { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x0000A7B5 File Offset: 0x000089B5
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x0000A7BD File Offset: 0x000089BD
		public string Scope { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x0000A7C6 File Offset: 0x000089C6
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x0000A7CE File Offset: 0x000089CE
		public string Sequence { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000A7D7 File Offset: 0x000089D7
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x0000A7DF File Offset: 0x000089DF
		public string SecurityDescriptor { get; set; }

		// Token: 0x060001A8 RID: 424 RVA: 0x0000A7E8 File Offset: 0x000089E8
		public string GetId()
		{
			return this.Tag + this.Scope + this.Sequence;
		}
	}
}
