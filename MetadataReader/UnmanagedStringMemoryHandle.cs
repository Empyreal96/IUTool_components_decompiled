using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.MetadataReader
{
	// Token: 0x02000019 RID: 25
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal sealed class UnmanagedStringMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		// Token: 0x06000105 RID: 261 RVA: 0x0000308B File Offset: 0x0000128B
		internal UnmanagedStringMemoryHandle() : base(true)
		{
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00003098 File Offset: 0x00001298
		internal UnmanagedStringMemoryHandle(int countBytes) : base(true)
		{
			bool flag = countBytes == 0;
			if (!flag)
			{
				IntPtr handle = Marshal.AllocHGlobal(countBytes);
				base.SetHandle(handle);
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x000030C8 File Offset: 0x000012C8
		protected override bool ReleaseHandle()
		{
			bool flag = this.handle != IntPtr.Zero;
			bool result;
			if (flag)
			{
				Marshal.FreeHGlobal(this.handle);
				this.handle = IntPtr.Zero;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000310C File Offset: 0x0000130C
		public string GetAsString(int countCharsNoNull)
		{
			return Marshal.PtrToStringUni(this.handle, countCharsNoNull);
		}
	}
}
