using System;

namespace Microsoft.MetadataReader
{
	// Token: 0x0200003C RID: 60
	internal class MethodSignatureDescriptor
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000443 RID: 1091 RVA: 0x0000DAF2 File Offset: 0x0000BCF2
		// (set) Token: 0x06000444 RID: 1092 RVA: 0x0000DAFA File Offset: 0x0000BCFA
		public CorCallingConvention CallingConvention { get; set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x0000DB03 File Offset: 0x0000BD03
		// (set) Token: 0x06000446 RID: 1094 RVA: 0x0000DB0B File Offset: 0x0000BD0B
		public int GenericParameterCount { get; set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000447 RID: 1095 RVA: 0x0000DB14 File Offset: 0x0000BD14
		// (set) Token: 0x06000448 RID: 1096 RVA: 0x0000DB1C File Offset: 0x0000BD1C
		public TypeSignatureDescriptor ReturnParameter { get; set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000449 RID: 1097 RVA: 0x0000DB25 File Offset: 0x0000BD25
		// (set) Token: 0x0600044A RID: 1098 RVA: 0x0000DB2D File Offset: 0x0000BD2D
		public TypeSignatureDescriptor[] Parameters { get; set; }
	}
}
