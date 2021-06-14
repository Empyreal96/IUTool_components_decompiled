using System;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200003B RID: 59
	internal class TypeSignatureDescriptor
	{
		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x0000DABF File Offset: 0x0000BCBF
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x0000DAC7 File Offset: 0x0000BCC7
		public Type Type { get; set; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x0000DAD0 File Offset: 0x0000BCD0
		// (set) Token: 0x0600043F RID: 1087 RVA: 0x0000DAD8 File Offset: 0x0000BCD8
		public CustomModifiers CustomModifiers { get; set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x0000DAE1 File Offset: 0x0000BCE1
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x0000DAE9 File Offset: 0x0000BCE9
		public bool IsPinned { get; set; }
	}
}
