using System;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000007 RID: 7
	internal struct DELTA_INPUT
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00003760 File Offset: 0x00001960
		public DELTA_INPUT(IntPtr start, UIntPtr size, bool editable)
		{
			this.lpStart = start;
			this.uSize = size;
			this.Editable = editable;
		}

		// Token: 0x0400000B RID: 11
		public IntPtr lpStart;

		// Token: 0x0400000C RID: 12
		public UIntPtr uSize;

		// Token: 0x0400000D RID: 13
		public bool Editable;
	}
}
