using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace FFUComponents
{
	// Token: 0x0200003E RID: 62
	internal static class NativeMethods
	{
		// Token: 0x0600010F RID: 271 RVA: 0x00004BC0 File Offset: 0x00002DC0
		internal static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
		{
			return DeviceType << 16 | Access << 14 | Function << 2 | Method;
		}

		// Token: 0x06000110 RID: 272
		[DllImport("winusb.dll", EntryPoint = "WinUsb_Initialize", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbInitialize(SafeFileHandle deviceHandle, ref IntPtr interfaceHandle);

		// Token: 0x06000111 RID: 273
		[DllImport("winusb.dll", EntryPoint = "WinUsb_Free", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbFree(IntPtr interfaceHandle);

		// Token: 0x06000112 RID: 274
		[DllImport("winusb.dll", EntryPoint = "WinUsb_ControlTransfer", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbControlTransfer(IntPtr interfaceHandle, WinUsbSetupPacket setupPacket, IntPtr buffer, uint bufferLength, ref uint lengthTransferred, IntPtr overlapped);

		// Token: 0x06000113 RID: 275
		[DllImport("winusb.dll", EntryPoint = "WinUsb_ControlTransfer", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal unsafe static extern bool WinUsbControlTransfer(IntPtr interfaceHandle, WinUsbSetupPacket setupPacket, byte* buffer, uint bufferLength, ref uint lengthTransferred, IntPtr overlapped);

		// Token: 0x06000114 RID: 276
		[DllImport("winusb.dll", EntryPoint = "WinUsb_QueryInterfaceSettings", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbQueryInterfaceSettings(IntPtr interfaceHandle, byte alternateInterfaceNumber, ref WinUsbInterfaceDescriptor usbAltInterfaceDescriptor);

		// Token: 0x06000115 RID: 277
		[DllImport("winusb.dll", EntryPoint = "WinUsb_QueryPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbQueryPipe(IntPtr interfaceHandle, byte alternateInterfaceNumber, byte pipeIndex, ref WinUsbPipeInformation pipeInformation);

		// Token: 0x06000116 RID: 278
		[DllImport("winusb.dll", EntryPoint = "WinUsb_SetPipePolicy", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbSetPipePolicy(IntPtr interfaceHandle, byte pipeID, uint policyType, uint valueLength, ref bool value);

		// Token: 0x06000117 RID: 279
		[DllImport("winusb.dll", EntryPoint = "WinUsb_SetPipePolicy", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbSetPipePolicy(IntPtr interfaceHandle, byte pipeID, uint policyType, uint valueLength, ref uint value);

		// Token: 0x06000118 RID: 280
		[DllImport("winusb.dll", EntryPoint = "WinUsb_ResetPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbResetPipe(IntPtr interfaceHandle, byte pipeID);

		// Token: 0x06000119 RID: 281
		[DllImport("winusb.dll", EntryPoint = "WinUsb_AbortPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbAbortPipe(IntPtr interfaceHandle, byte pipeID);

		// Token: 0x0600011A RID: 282
		[DllImport("winusb.dll", EntryPoint = "WinUsb_FlushPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool WinUsbFlushPipe(IntPtr interfaceHandle, byte pipeID);

		// Token: 0x0600011B RID: 283
		[DllImport("winusb.dll", EntryPoint = "WinUsb_ReadPipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal unsafe static extern bool WinUsbReadPipe(IntPtr interfaceHandle, byte pipeID, byte* buffer, uint bufferLength, IntPtr lenghtTransferred, NativeOverlapped* overlapped);

		// Token: 0x0600011C RID: 284
		[DllImport("winusb.dll", EntryPoint = "WinUsb_WritePipe", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal unsafe static extern bool WinUsbWritePipe(IntPtr interfaceHandle, byte pipeID, byte* buffer, uint bufferLength, IntPtr lenghtTransferred, NativeOverlapped* overlapped);

		// Token: 0x0600011D RID: 285
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFileHandle);

		// Token: 0x0600011E RID: 286
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		// Token: 0x0600011F RID: 287
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CancelIo(SafeFileHandle handle);

		// Token: 0x06000120 RID: 288
		[DllImport("iphlpapi.dll", ExactSpelling = true)]
		public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

		// Token: 0x06000121 RID: 289
		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SetupDiGetClassDevs(IntPtr ClassGuid, string Enumerator, IntPtr hwndParent, uint Flags);

		// Token: 0x06000122 RID: 290
		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

		// Token: 0x06000123 RID: 291
		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, SP_DEVINFO_DATA DeviceInfoData);

		// Token: 0x06000124 RID: 292
		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool SetupDiGetDeviceProperty(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, DEVPROPKEY PropertyKey, out uint PropertyType, out uint PropertyBuffer, uint PropertyBufferSize, IntPtr RequiredSize, uint Flags);

		// Token: 0x06000125 RID: 293
		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, uint MemberIndex, SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

		// Token: 0x06000126 RID: 294
		[DllImport("cfgmgr32.dll", CharSet = CharSet.Auto)]
		internal static extern uint CM_Get_Parent(out uint pdnDevInst, uint dnDevInst, uint ulFlags);

		// Token: 0x06000127 RID: 295
		[DllImport("cfgmgr32.dll", CharSet = CharSet.Auto)]
		internal static extern uint CM_Get_Device_ID_Size(out uint pulLen, uint dnDevInst, uint ulFlags);

		// Token: 0x06000128 RID: 296
		[DllImport("cfgmgr32.dll", CharSet = CharSet.Auto)]
		internal static extern uint CM_Get_Device_ID(uint dnDevInst, StringBuilder Buffer, uint BufferLen, uint ulFlags);

		// Token: 0x06000129 RID: 297
		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool SetupDiGetDeviceInterfaceDetailW(IntPtr DeviceInfoSet, SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, [In] [Out] SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, uint DeviceInterfaceDetailDataSize, IntPtr RequiredSize, IntPtr DeviceInfoData);

		// Token: 0x0600012A RID: 298
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, USB_NODE_CONNECTION_INFORMATION_EX_V2 lpInBuffer, uint nInBufferSize, USB_NODE_CONNECTION_INFORMATION_EX_V2 lpOutBuffer, uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

		// Token: 0x040000D1 RID: 209
		internal static DEVPROPKEY DEVPKEY_Device_Address = new DEVPROPKEY(new Guid(2757502286U, 57116, 20221, 128, 32, 103, 209, 70, 168, 80, 224), 30U);

		// Token: 0x040000D2 RID: 210
		internal const uint CR_SUCCESS = 0U;

		// Token: 0x040000D3 RID: 211
		internal static IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);

		// Token: 0x040000D4 RID: 212
		internal static Guid GUID_DEVINTERFACE_USB_HUB = new Guid(4052356744U, 49932, 4560, 136, 21, 0, 160, 201, 6, 190, 216);

		// Token: 0x040000D5 RID: 213
		internal const uint GENERIC_WRITE = 1073741824U;

		// Token: 0x040000D6 RID: 214
		internal const uint FILE_SHARE_READ = 1U;

		// Token: 0x040000D7 RID: 215
		internal const uint FILE_SHARE_WRITE = 2U;

		// Token: 0x040000D8 RID: 216
		internal const uint FILE_SHARE_DELETE = 4U;

		// Token: 0x040000D9 RID: 217
		internal const uint USB_GET_NODE_CONNECTION_INFORMATION_EX_V2 = 279U;

		// Token: 0x040000DA RID: 218
		internal const uint FILE_DEVICE_USB = 34U;

		// Token: 0x040000DB RID: 219
		internal const uint FILE_DEVICE_UNKNOWN = 34U;

		// Token: 0x040000DC RID: 220
		internal const uint METHOD_BUFFERED = 0U;

		// Token: 0x040000DD RID: 221
		internal const uint FILE_ANY_ACCESS = 0U;

		// Token: 0x040000DE RID: 222
		internal static uint IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX_V2 = NativeMethods.CTL_CODE(34U, 279U, 0U, 0U);
	}
}
