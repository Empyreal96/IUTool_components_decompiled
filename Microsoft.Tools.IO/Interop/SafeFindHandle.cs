using System;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Tools.IO.Interop
{
	// Token: 0x02000008 RID: 8
	internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		// Token: 0x06000085 RID: 133 RVA: 0x00003790 File Offset: 0x00001990
		internal SafeFindHandle() : base(true)
		{
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00003799 File Offset: 0x00001999
		protected override bool ReleaseHandle()
		{
			return NativeMethods.FindClose(this.handle);
		}
	}
}
