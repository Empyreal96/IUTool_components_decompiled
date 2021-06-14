using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200003E RID: 62
	public static class IntPtrExtensions
	{
		// Token: 0x0600027A RID: 634 RVA: 0x0000B62C File Offset: 0x0000982C
		public static IntPtr Increment(this IntPtr ptr, int cbSize)
		{
			return new IntPtr(ptr.ToInt64() + (long)cbSize);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000B63D File Offset: 0x0000983D
		public static IntPtr Increment<T>(this IntPtr ptr)
		{
			return ptr.Increment(Marshal.SizeOf(typeof(T)));
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000B654 File Offset: 0x00009854
		public static T ElementAt<T>(this IntPtr ptr, int index)
		{
			int cbSize = Marshal.SizeOf(typeof(T)) * index;
			return (T)((object)Marshal.PtrToStructure(ptr.Increment(cbSize), typeof(T)));
		}
	}
}
