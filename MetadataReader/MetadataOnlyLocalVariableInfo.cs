using System;
using System.Reflection.Mock;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000027 RID: 39
	internal class MetadataOnlyLocalVariableInfo : LocalVariableInfo
	{
		// Token: 0x0600020A RID: 522 RVA: 0x00005DDF File Offset: 0x00003FDF
		public MetadataOnlyLocalVariableInfo(int index, Type type, bool fPinned)
		{
			this.LocalType = type;
			this.LocalIndex = index;
			this.IsPinned = fPinned;
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00005DFE File Offset: 0x00003FFE
		public override bool IsPinned { get; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600020C RID: 524 RVA: 0x00005E06 File Offset: 0x00004006
		public override int LocalIndex { get; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600020D RID: 525 RVA: 0x00005E0E File Offset: 0x0000400E
		public override Type LocalType { get; }
	}
}
