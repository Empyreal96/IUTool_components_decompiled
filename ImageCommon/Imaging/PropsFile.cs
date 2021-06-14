using System;
using System.Xml.Serialization;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002C RID: 44
	public class PropsFile
	{
		// Token: 0x060001D4 RID: 468 RVA: 0x0000FE74 File Offset: 0x0000E074
		public override string ToString()
		{
			return this.Include;
		}

		// Token: 0x0400012C RID: 300
		[XmlAttribute("Include")]
		public string Include;

		// Token: 0x0400012D RID: 301
		public string InstallPath;

		// Token: 0x0400012E RID: 302
		public string MC_ARM_FRE;

		// Token: 0x0400012F RID: 303
		public string MC_ARM_CHK;

		// Token: 0x04000130 RID: 304
		public string MC_ARM64_FRE;

		// Token: 0x04000131 RID: 305
		public string MC_ARM64_CHK;

		// Token: 0x04000132 RID: 306
		public string MC_X86_FRE;

		// Token: 0x04000133 RID: 307
		public string MC_X86_CHK;

		// Token: 0x04000134 RID: 308
		public string MC_AMD64_FRE;

		// Token: 0x04000135 RID: 309
		public string MC_AMD64_CHK;

		// Token: 0x04000136 RID: 310
		public string Feature;

		// Token: 0x04000137 RID: 311
		public string Owner;

		// Token: 0x04000138 RID: 312
		public string BusinessReason;
	}
}
