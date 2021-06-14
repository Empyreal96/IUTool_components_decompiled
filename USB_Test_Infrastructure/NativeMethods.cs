using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace USB_Test_Infrastructure
{
	// Token: 0x02000018 RID: 24
	internal static class NativeMethods
	{
		// Token: 0x0600000C RID: 12
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern uint GetTimeZoneInformation(out TimeZoneInformation timeZoneInformation);

		// Token: 0x0600000D RID: 13
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern void GetSystemTime(out SystemTime systemTime);

		// Token: 0x0600000E RID: 14
		[DllImport("winusb.dll", EntryPoint = "WinUsb_Initialize", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbInitialize(SafeFileHandle deviceHandle, ref IntPtr interfaceHandle);

		// Token: 0x0600000F RID: 15
		[DllImport("winusb.dll", EntryPoint = "WinUsb_Free", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbFree(IntPtr interfaceHandle);

		// Token: 0x06000010 RID: 16
		[DllImport("winusb.dll", EntryPoint = "WinUsb_ControlTransfer", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbControlTransfer(IntPtr interfaceHandle, WinUsbSetupPacket setupPacket, IntPtr buffer, uint bufferLength, ref uint lengthTransferred, IntPtr overlapped);

		// Token: 0x06000011 RID: 17
		[DllImport("winusb.dll", EntryPoint = "WinUsb_ControlTransfer", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal unsafe static extern bool WinUsbControlTransfer(IntPtr interfaceHandle, WinUsbSetupPacket setupPacket, byte* buffer, uint bufferLength, ref uint lengthTransferred, IntPtr overlapped);

		// Token: 0x06000012 RID: 18
		[DllImport("winusb.dll", EntryPoint = "WinUsb_QueryInterfaceSettings", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbQueryInterfaceSettings(IntPtr interfaceHandle, byte alternateInterfaceNumber, ref WinUsbInterfaceDescriptor usbAltInterfaceDescriptor);

		// Token: 0x06000013 RID: 19
		[DllImport("winusb.dll", EntryPoint = "WinUsb_QueryPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbQueryPipe(IntPtr interfaceHandle, byte alternateInterfaceNumber, byte pipeIndex, ref WinUsbPipeInformation pipeInformation);

		// Token: 0x06000014 RID: 20
		[DllImport("winusb.dll", EntryPoint = "WinUsb_SetPipePolicy", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbSetPipePolicy(IntPtr interfaceHandle, byte pipeID, uint policyType, uint valueLength, ref bool value);

		// Token: 0x06000015 RID: 21
		[DllImport("winusb.dll", EntryPoint = "WinUsb_SetPipePolicy", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbSetPipePolicy(IntPtr interfaceHandle, byte pipeID, uint policyType, uint valueLength, ref uint value);

		// Token: 0x06000016 RID: 22
		[DllImport("winusb.dll", EntryPoint = "WinUsb_ResetPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbResetPipe(IntPtr interfaceHandle, byte pipeID);

		// Token: 0x06000017 RID: 23
		[DllImport("winusb.dll", EntryPoint = "WinUsb_AbortPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbAbortPipe(IntPtr interfaceHandle, byte pipeID);

		// Token: 0x06000018 RID: 24
		[DllImport("winusb.dll", EntryPoint = "WinUsb_FlushPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbFlushPipe(IntPtr interfaceHandle, byte pipeID);

		// Token: 0x06000019 RID: 25
		[DllImport("winusb.dll", EntryPoint = "WinUsb_ReadPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal unsafe static extern bool WinUsbReadPipe(IntPtr interfaceHandle, byte pipeID, byte* buffer, uint bufferLength, IntPtr lenghtTransferred, NativeOverlapped* overlapped);

		// Token: 0x0600001A RID: 26
		[DllImport("winusb.dll", EntryPoint = "WinUsb_WritePipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal unsafe static extern bool WinUsbWritePipe(IntPtr interfaceHandle, byte pipeID, byte* buffer, uint bufferLength, IntPtr lenghtTransferred, NativeOverlapped* overlapped);

		// Token: 0x0600001B RID: 27
		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, string enumerator, int parent, int flags);

		// Token: 0x0600001C RID: 28
		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, int deviceInfoData, ref Guid interfaceClassGuid, int memberIndex, ref DeviceInterfaceData deviceInterfaceData);

		// Token: 0x0600001D RID: 29
		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref DeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);

		// Token: 0x0600001E RID: 30
		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal unsafe static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref DeviceInterfaceData deviceInterfaceData, DeviceInterfaceDetailData* deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, ref DeviceInformationData deviceInfoData);

		// Token: 0x0600001F RID: 31
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool GetDiskFreeSpace(string pathName, out uint sectorsPerCluster, out uint bytesPerSector, out uint numberOfFreeClusters, out uint totalNumberOfClusters);

		// Token: 0x06000020 RID: 32
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle FindFirstFile(string fileName, ref FindFileData findFileData);

		// Token: 0x06000021 RID: 33
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool FindNextFile(SafeFileHandle findFileHandle, ref FindFileData findFileData);

		// Token: 0x06000022 RID: 34
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool FindClose(SafeFileHandle findFileHandle);

		// Token: 0x06000023 RID: 35
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFileHandle);

		// Token: 0x06000024 RID: 36
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern void CloseHandle(SafeHandle handle);

		// Token: 0x06000025 RID: 37
		[DllImport("kernel32.dll", SetLastError = true)]
		public unsafe static extern bool ReadFile(SafeFileHandle handle, byte* buffer, int numBytesToRead, IntPtr numBytesRead, NativeOverlapped* overlapped);

		// Token: 0x06000026 RID: 38
		[DllImport("kernel32.dll", SetLastError = true)]
		public unsafe static extern bool WriteFile(SafeFileHandle handle, byte* buffer, int numBytesToWrite, IntPtr numBytesWritten, NativeOverlapped* overlapped);

		// Token: 0x06000027 RID: 39
		[DllImport("iphlpapi.dll", ExactSpelling = true)]
		public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);
	}
}
