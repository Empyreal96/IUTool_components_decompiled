using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200003C RID: 60
	public static class VirtualDiskLib
	{
		// Token: 0x0600016D RID: 365
		[CLSCompliant(false)]
		[DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int OpenVirtualDisk(ref VIRTUAL_STORAGE_TYPE VirtualStorageType, [MarshalAs(UnmanagedType.LPWStr)] string Path, VIRTUAL_DISK_ACCESS_MASK VirtualDiskAccessMask, OPEN_VIRTUAL_DISK_FLAG Flags, ref OPEN_VIRTUAL_DISK_PARAMETERS Parameters, ref IntPtr Handle);

		// Token: 0x0600016E RID: 366
		[CLSCompliant(false)]
		[DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int OpenVirtualDisk(ref VIRTUAL_STORAGE_TYPE VirtualStorageType, [MarshalAs(UnmanagedType.LPWStr)] string Path, VIRTUAL_DISK_ACCESS_MASK VirtualDiskAccessMask, OPEN_VIRTUAL_DISK_FLAG Flags, IntPtr Parameters, ref IntPtr Handle);

		// Token: 0x0600016F RID: 367
		[DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int GetVirtualDiskPhysicalPath(IntPtr VirtualDiskHandle, ref int DiskPathSizeInBytes, StringBuilder DiskPath);

		// Token: 0x06000170 RID: 368
		[CLSCompliant(false)]
		[DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int AttachVirtualDisk(IntPtr VirtualDiskHandle, IntPtr SecurityDescriptor, ATTACH_VIRTUAL_DISK_FLAG Flags, uint ProviderSpecificFlags, ref ATTACH_VIRTUAL_DISK_PARAMETERS Parameters, IntPtr Overlapped);

		// Token: 0x06000171 RID: 369
		[CLSCompliant(false)]
		[DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int DetachVirtualDisk(IntPtr VirtualDiskHandle, DETACH_VIRTUAL_DISK_FLAG Flags, uint ProviderSpecificFlags);

		// Token: 0x06000172 RID: 370
		[CLSCompliant(false)]
		[DllImport("Virtdisk.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int CreateVirtualDisk(ref VIRTUAL_STORAGE_TYPE VirtualStorageType, [MarshalAs(UnmanagedType.LPWStr)] string Path, VIRTUAL_DISK_ACCESS_MASK VirtualDiskAccessMask, IntPtr SecurityDescriptor, CREATE_VIRTUAL_DISK_FLAG Flags, uint ProviderSpecificFlags, ref CREATE_VIRTUAL_DISK_PARAMETERS Parameters, IntPtr Overlapped, ref IntPtr Handle);

		// Token: 0x06000173 RID: 371
		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);
	}
}
