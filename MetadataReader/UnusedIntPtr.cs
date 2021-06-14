using System;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000016 RID: 22
	internal struct UnusedIntPtr
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00003064 File Offset: 0x00001264
		public static UnusedIntPtr Zero
		{
			get
			{
				return new UnusedIntPtr
				{
					_zeroPtr = IntPtr.Zero
				};
			}
		}

		// Token: 0x04000044 RID: 68
		private IntPtr _zeroPtr;
	}
}
