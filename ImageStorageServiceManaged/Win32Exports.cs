using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200005B RID: 91
	public sealed class Win32Exports
	{
		// Token: 0x06000407 RID: 1031 RVA: 0x00012867 File Offset: 0x00010A67
		public static bool FAILED(int hr)
		{
			return hr < 0;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0001286D File Offset: 0x00010A6D
		public static bool SUCCEEDED(int hr)
		{
			return hr >= 0;
		}

		// Token: 0x06000409 RID: 1033
		[DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle_Native(IntPtr handle);

		// Token: 0x0600040A RID: 1034 RVA: 0x00012878 File Offset: 0x00010A78
		public static void CloseHandle(IntPtr handle)
		{
			if (!Win32Exports.CloseHandle_Native(handle))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("{0} failed with error {1}", MethodBase.GetCurrentMethod().Name, lastWin32Error));
			}
		}

		// Token: 0x0600040B RID: 1035
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, EntryPoint = "CreateFile", SetLastError = true)]
		private static extern SafeFileHandle CreateFile_Native(string fileName, Win32Exports.DesiredAccess desiredAccess, Win32Exports.ShareMode shareMode, IntPtr securityAttributes, Win32Exports.CreationDisposition creationDisposition, Win32Exports.FlagsAndAttributes flagsAndAttributes, IntPtr templateFileHandle);

		// Token: 0x0600040C RID: 1036 RVA: 0x000128B4 File Offset: 0x00010AB4
		[CLSCompliant(false)]
		public static SafeFileHandle CreateFile(string fileName, Win32Exports.DesiredAccess desiredAccess, Win32Exports.ShareMode shareMode, Win32Exports.CreationDisposition creationDisposition, Win32Exports.FlagsAndAttributes flagsAndAttributes)
		{
			SafeFileHandle safeFileHandle = Win32Exports.CreateFile_Native(fileName, desiredAccess, shareMode, IntPtr.Zero, creationDisposition, flagsAndAttributes, IntPtr.Zero);
			if (safeFileHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("{0}({1}) failed with error {2}", MethodBase.GetCurrentMethod().Name, string.IsNullOrEmpty(fileName) ? "" : fileName, lastWin32Error));
			}
			return safeFileHandle;
		}

		// Token: 0x0600040D RID: 1037
		[DllImport("kernel32.dll", EntryPoint = "ReadFile", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ReadFile_Native(SafeFileHandle fileHandle, out byte[] buffer, uint bytesToRead, out uint bytesRead, IntPtr overlapped);

		// Token: 0x0600040E RID: 1038 RVA: 0x00012914 File Offset: 0x00010B14
		[CLSCompliant(false)]
		public static void ReadFile(SafeFileHandle fileHandle, out byte[] buffer, uint bytesToRead, out uint bytesRead)
		{
			if (!Win32Exports.ReadFile_Native(fileHandle, out buffer, bytesToRead, out bytesRead, IntPtr.Zero))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("ReadFile failed with error: {0}", lastWin32Error));
			}
		}

		// Token: 0x0600040F RID: 1039
		[DllImport("kernel32.dll", EntryPoint = "ReadFile", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ReadFile_Native(SafeFileHandle fileHandle, IntPtr buffer, uint bytesToRead, out uint bytesRead, IntPtr overlapped);

		// Token: 0x06000410 RID: 1040 RVA: 0x00012950 File Offset: 0x00010B50
		[CLSCompliant(false)]
		public static void ReadFile(SafeFileHandle fileHandle, IntPtr buffer, uint bytesToRead, out uint bytesRead)
		{
			if (!Win32Exports.ReadFile_Native(fileHandle, buffer, bytesToRead, out bytesRead, IntPtr.Zero))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("ReadFile failed with error: {0}", lastWin32Error));
			}
		}

		// Token: 0x06000411 RID: 1041
		[DllImport("kernel32.dll", EntryPoint = "WriteFile", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool WriteFile_Native(SafeFileHandle handle, IntPtr buffer, uint numBytesToWrite, out uint numBytesWritten, IntPtr overlapped);

		// Token: 0x06000412 RID: 1042 RVA: 0x0001298C File Offset: 0x00010B8C
		[CLSCompliant(false)]
		public static void WriteFile(SafeFileHandle fileHandle, IntPtr buffer, uint bytesToWrite, out uint bytesWritten)
		{
			if (!Win32Exports.WriteFile_Native(fileHandle, buffer, bytesToWrite, out bytesWritten, IntPtr.Zero))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("WriteFile failed with error: {0}", lastWin32Error));
			}
		}

		// Token: 0x06000413 RID: 1043
		[DllImport("kernel32.dll", EntryPoint = "SetFilePointerEx", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetFilePointerEx_Native(SafeFileHandle fileHandle, long distanceToMove, out long newFilePointer, Win32Exports.MoveMethod moveMethod);

		// Token: 0x06000414 RID: 1044 RVA: 0x000129C8 File Offset: 0x00010BC8
		[CLSCompliant(false)]
		public static void SetFilePointerEx(SafeFileHandle fileHandle, long distanceToMove, out long newFileLocation, Win32Exports.MoveMethod moveMethod)
		{
			if (!Win32Exports.SetFilePointerEx_Native(fileHandle, distanceToMove, out newFileLocation, moveMethod))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("SetFilePointerEx failed with error: {0}", lastWin32Error));
			}
		}

		// Token: 0x06000415 RID: 1045
		[DllImport("kernel32.dll", EntryPoint = "VirtualAlloc", SetLastError = true)]
		private static extern IntPtr VirtualAlloc_Native(IntPtr lpAddress, UIntPtr sizeInBytes, Win32Exports.AllocationType allocationType, Win32Exports.MemoryProtection memoryProtection);

		// Token: 0x06000416 RID: 1046 RVA: 0x000129FC File Offset: 0x00010BFC
		[CLSCompliant(false)]
		public static IntPtr VirtualAlloc(UIntPtr sizeInBytes, Win32Exports.AllocationType allocationType, Win32Exports.MemoryProtection memoryProtection)
		{
			IntPtr intPtr = Win32Exports.VirtualAlloc_Native(IntPtr.Zero, sizeInBytes, allocationType, memoryProtection);
			if (intPtr == IntPtr.Zero)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("{0} failed with error {1}", MethodBase.GetCurrentMethod().Name, lastWin32Error));
			}
			return intPtr;
		}

		// Token: 0x06000417 RID: 1047
		[DllImport("kernel32.dll", EntryPoint = "VirtualFree", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool VirtualFree_Native(IntPtr address, UIntPtr sizeInBytes, Win32Exports.FreeType freeType);

		// Token: 0x06000418 RID: 1048 RVA: 0x00012A4C File Offset: 0x00010C4C
		[CLSCompliant(false)]
		public static void VirtualFree(IntPtr address, Win32Exports.FreeType freeType)
		{
			UIntPtr zero = UIntPtr.Zero;
			if (!Win32Exports.VirtualFree_Native(address, zero, freeType))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("{0} failed with error {1}", MethodBase.GetCurrentMethod().Name, lastWin32Error));
			}
		}

		// Token: 0x06000419 RID: 1049
		[CLSCompliant(false)]
		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int memcmp(byte[] buffer1, IntPtr buffer2, UIntPtr count);

		// Token: 0x0600041A RID: 1050
		[DllImport("kernel32.dll", EntryPoint = "FlushFileBuffers", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FlushFileBuffers_Native(SafeFileHandle fileHandle);

		// Token: 0x0600041B RID: 1051 RVA: 0x00012A90 File Offset: 0x00010C90
		[CLSCompliant(false)]
		public static void FlushFileBuffers(SafeFileHandle fileHandle)
		{
			if (!Win32Exports.FlushFileBuffers_Native(fileHandle))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("{0} failed: {1}", MethodBase.GetCurrentMethod().Name, lastWin32Error));
			}
		}

		// Token: 0x0600041C RID: 1052
		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "DeviceIoControl", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeviceIoControl_Native(IntPtr hDevice, uint controlCode, byte[] inBuffer, int inBufferSize, byte[] outBuffer, int outBufferSize, out int bytesReturned, IntPtr lpOverlapped);

		// Token: 0x0600041D RID: 1053 RVA: 0x00012ACC File Offset: 0x00010CCC
		[CLSCompliant(false)]
		public static void DeviceIoControl(IntPtr handle, uint controlCode, byte[] inBuffer, int inBufferSize, byte[] outBuffer, int outBufferSize, out int bytesReturned)
		{
			if (!Win32Exports.DeviceIoControl_Native(handle, controlCode, inBuffer, inBufferSize, outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero))
			{
				throw new Win32ExportException(string.Format("{0}: Control code {1:x} failed with error code {2:x}.", MethodBase.GetCurrentMethod().Name, controlCode, Marshal.GetHRForLastWin32Error()));
			}
		}

		// Token: 0x0600041E RID: 1054
		[DllImport("Kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "DeviceIoControl", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeviceIoControl_Native(IntPtr hDevice, uint controlCode, IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, out int bytesReturned, IntPtr lpOverlapped);

		// Token: 0x0600041F RID: 1055 RVA: 0x00012B1C File Offset: 0x00010D1C
		[CLSCompliant(false)]
		public static void DeviceIoControl(IntPtr handle, uint controlCode, IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize, out int bytesReturned)
		{
			if (!Win32Exports.DeviceIoControl_Native(handle, controlCode, inBuffer, inBufferSize, outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero))
			{
				throw new Win32ExportException(string.Format("{0}: Control code {1:x} failed with error code {2:x}.", MethodBase.GetCurrentMethod().Name, controlCode, Marshal.GetHRForLastWin32Error()));
			}
		}

		// Token: 0x06000420 RID: 1056
		[DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess")]
		private static extern IntPtr GetCurrentProcess_Native();

		// Token: 0x06000421 RID: 1057 RVA: 0x00012B6C File Offset: 0x00010D6C
		public static IntPtr GetCurrentProcess()
		{
			IntPtr currentProcess_Native = Win32Exports.GetCurrentProcess_Native();
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (currentProcess_Native.ToInt32() != -1)
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, lastWin32Error));
			}
			return currentProcess_Native;
		}

		// Token: 0x06000422 RID: 1058
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool LookupPrivilegeValue(string systemName, string name, out Win32Exports.LUID luid);

		// Token: 0x06000423 RID: 1059 RVA: 0x00012BB0 File Offset: 0x00010DB0
		public static Win32Exports.LUID LookupPrivilegeValue(string privilegeName)
		{
			Win32Exports.LUID result = default(Win32Exports.LUID);
			if (!Win32Exports.LookupPrivilegeValue(null, privilegeName, out result))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, lastWin32Error));
			}
			return result;
		}

		// Token: 0x06000424 RID: 1060
		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges, ref Win32Exports.TOKEN_PRIVILEGES newState, uint bufferLengthInBytes, ref Win32Exports.TOKEN_PRIVILEGES previousState, out uint returnLengthInBytes);

		// Token: 0x06000425 RID: 1061 RVA: 0x00012BF8 File Offset: 0x00010DF8
		public static Win32Exports.TOKEN_PRIVILEGES AdjustTokenPrivileges(IntPtr tokenHandle, Win32Exports.TOKEN_PRIVILEGES privileges)
		{
			Win32Exports.TOKEN_PRIVILEGES result = default(Win32Exports.TOKEN_PRIVILEGES);
			uint num = 0U;
			if (!Win32Exports.AdjustTokenPrivileges(tokenHandle, false, ref privileges, (uint)Marshal.SizeOf(privileges), ref result, out num))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, lastWin32Error));
			}
			return result;
		}

		// Token: 0x06000426 RID: 1062
		[DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool OpenProcessToken_Native(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

		// Token: 0x06000427 RID: 1063 RVA: 0x00012C50 File Offset: 0x00010E50
		[CLSCompliant(false)]
		public static IntPtr OpenProcessToken(IntPtr processHandle, uint desiredAccess)
		{
			IntPtr result;
			if (!Win32Exports.OpenProcessToken_Native(processHandle, desiredAccess, out result))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, lastWin32Error));
			}
			return result;
		}

		// Token: 0x06000428 RID: 1064
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegLoadKey", SetLastError = true)]
		private static extern uint RegLoadKey_Native(SafeRegistryHandle registryKey, string subKeyName, string fileName);

		// Token: 0x06000429 RID: 1065 RVA: 0x00012C90 File Offset: 0x00010E90
		public static void RegLoadKey(SafeRegistryHandle registryKey, string subKeyName, string fileName)
		{
			uint num = Win32Exports.RegLoadKey_Native(registryKey, subKeyName, fileName);
			if ((ulong)num != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x0600042A RID: 1066
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegUnLoadKey", SetLastError = true)]
		private static extern uint RegUnLoadKey_Native(SafeRegistryHandle registryKey, string subKeyName);

		// Token: 0x0600042B RID: 1067 RVA: 0x00012CD0 File Offset: 0x00010ED0
		public static void RegUnloadKey(SafeRegistryHandle registryKey, string subKeyName)
		{
			uint num = Win32Exports.RegUnLoadKey_Native(registryKey, subKeyName);
			if ((ulong)num != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x0600042C RID: 1068
		[DllImport("offreg.dll", CharSet = CharSet.Unicode)]
		private static extern uint OROpenHive(string hivePath, out IntPtr rootKey);

		// Token: 0x0600042D RID: 1069 RVA: 0x00012D10 File Offset: 0x00010F10
		public static IntPtr OfflineRegistryOpenHive(string hivePath)
		{
			IntPtr zero = IntPtr.Zero;
			uint num = Win32Exports.OROpenHive(hivePath, out zero);
			if ((ulong)num != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x} for path: '{2}.", MethodBase.GetCurrentMethod().Name, num, hivePath));
			}
			return zero;
		}

		// Token: 0x0600042E RID: 1070
		[DllImport("offreg.dll", CharSet = CharSet.Unicode)]
		private static extern uint ORSaveHive(IntPtr hiveHandle, string hivePath, uint majorOsVersion, uint minorOSVersion);

		// Token: 0x0600042F RID: 1071 RVA: 0x00012D58 File Offset: 0x00010F58
		public static void OfflineRegistrySaveHive(IntPtr hiveHandle, string hivePath)
		{
			uint num = Win32Exports.ORSaveHive(hiveHandle, hivePath, 6U, 1U);
			if ((ulong)num != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x} for path: '{2}.", MethodBase.GetCurrentMethod().Name, num, hivePath));
			}
		}

		// Token: 0x06000430 RID: 1072
		[DllImport("offreg.dll", CharSet = CharSet.Unicode)]
		private static extern uint ORCloseHive(IntPtr rootKey);

		// Token: 0x06000431 RID: 1073 RVA: 0x00012D9C File Offset: 0x00010F9C
		public static void OfflineRegistryCloseHive(IntPtr registryKey)
		{
			uint num = Win32Exports.ORCloseHive(registryKey);
			if ((ulong)num != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x06000432 RID: 1074
		[DllImport("offreg.dll", CharSet = CharSet.Unicode)]
		private static extern uint OROpenKey(IntPtr keyHandle, string subKeyName, out IntPtr subKeyHandle);

		// Token: 0x06000433 RID: 1075 RVA: 0x00012DDC File Offset: 0x00010FDC
		public static IntPtr OfflineRegistryOpenSubKey(IntPtr keyHandle, string subKeyName)
		{
			IntPtr zero = IntPtr.Zero;
			uint num = Win32Exports.OROpenKey(keyHandle, subKeyName, out zero);
			if ((ulong)num == (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				return zero;
			}
			if (num == 2U)
			{
				return IntPtr.Zero;
			}
			throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
		}

		// Token: 0x06000434 RID: 1076
		[DllImport("offreg.dll", CharSet = CharSet.Unicode)]
		private static extern uint ORCloseKey(IntPtr keyHandle);

		// Token: 0x06000435 RID: 1077 RVA: 0x00012E30 File Offset: 0x00011030
		public static void OfflineRegistryCloseSubKey(IntPtr keyHandle)
		{
			uint num = Win32Exports.ORCloseKey(keyHandle);
			if ((ulong)num != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x06000436 RID: 1078
		[DllImport("offreg.dll", CharSet = CharSet.Unicode, EntryPoint = "OREnumKey")]
		private static extern uint OREnumKeySimple(IntPtr rootKey, uint index, StringBuilder subKeyName, ref uint subKeyCharacterCount, IntPtr subKeyClass, IntPtr subKeyClassCharacterCount, IntPtr fileTime);

		// Token: 0x06000437 RID: 1079 RVA: 0x00012E70 File Offset: 0x00011070
		[CLSCompliant(false)]
		public static string OfflineRegistryEnumKey(IntPtr registryKey, uint index)
		{
			StringBuilder stringBuilder = new StringBuilder("keyName", (int)ImageConstants.RegistryKeyMaxNameSize);
			uint registryKeyMaxNameSize = ImageConstants.RegistryKeyMaxNameSize;
			uint num = Win32Exports.OREnumKeySimple(registryKey, index, stringBuilder, ref registryKeyMaxNameSize, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if ((ulong)num == (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				return stringBuilder.ToString();
			}
			if ((ulong)num == (ulong)((long)Win32Exports.ERROR_NO_MORE_ITEMS))
			{
				return null;
			}
			throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
		}

		// Token: 0x06000438 RID: 1080
		[DllImport("offreg.dll", CharSet = CharSet.Unicode, EntryPoint = "OREnumValue")]
		private static extern uint OREnumValueSimple(IntPtr rootKey, uint index, StringBuilder valueName, ref uint valueCharacterCount, IntPtr valueType, IntPtr data, IntPtr dataSize);

		// Token: 0x06000439 RID: 1081 RVA: 0x00012EEC File Offset: 0x000110EC
		[CLSCompliant(false)]
		public static string OfflineRegistryEnumValue(IntPtr registryKey, uint index)
		{
			StringBuilder stringBuilder = new StringBuilder("valueName", (int)ImageConstants.RegistryValueMaxNameSize);
			uint registryValueMaxNameSize = ImageConstants.RegistryValueMaxNameSize;
			uint num = Win32Exports.OREnumValueSimple(registryKey, index, stringBuilder, ref registryValueMaxNameSize, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if ((ulong)num == (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				return stringBuilder.ToString();
			}
			if ((ulong)num == (ulong)((long)Win32Exports.ERROR_NO_MORE_ITEMS))
			{
				return null;
			}
			throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
		}

		// Token: 0x0600043A RID: 1082
		[DllImport("offreg.dll", CharSet = CharSet.Unicode, EntryPoint = "ORGetValue")]
		private static extern uint ORGetValueKind(IntPtr keyHandle, IntPtr subKey, string valueName, out uint valueType, IntPtr data, IntPtr dataLength);

		// Token: 0x0600043B RID: 1083 RVA: 0x00012F68 File Offset: 0x00011168
		[CLSCompliant(false)]
		public static uint OfflineRegistryGetValueKind(IntPtr keyHandle, string valueName)
		{
			uint result = 0U;
			uint num = Win32Exports.ORGetValueKind(keyHandle, IntPtr.Zero, valueName, out result, IntPtr.Zero, IntPtr.Zero);
			if ((ulong)num != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
			return result;
		}

		// Token: 0x0600043C RID: 1084
		[DllImport("offreg.dll", CharSet = CharSet.Unicode, EntryPoint = "ORGetValue")]
		private static extern uint ORGetValueSize(IntPtr keyHandle, IntPtr subKey, string valueName, IntPtr valueType, IntPtr data, ref uint dataLength);

		// Token: 0x0600043D RID: 1085 RVA: 0x00012FBC File Offset: 0x000111BC
		[CLSCompliant(false)]
		public static uint OfflineRegistryGetValueSize(IntPtr keyHandle, string valueName)
		{
			uint result = 0U;
			uint num = Win32Exports.ORGetValueSize(keyHandle, IntPtr.Zero, valueName, IntPtr.Zero, IntPtr.Zero, ref result);
			if ((ulong)num != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
			return result;
		}

		// Token: 0x0600043E RID: 1086
		[DllImport("offreg.dll", CharSet = CharSet.Unicode)]
		private static extern uint ORGetValue(IntPtr keyHandle, IntPtr subKey, string valueName, out uint valueType, byte[] data, ref uint dataLength);

		// Token: 0x0600043F RID: 1087 RVA: 0x00013010 File Offset: 0x00011210
		public static object OfflineRegistryGetValue(IntPtr keyHandle, string valueName)
		{
			uint num = 0U;
			uint num2 = Win32Exports.ORGetValueSize(keyHandle, IntPtr.Zero, valueName, IntPtr.Zero, IntPtr.Zero, ref num);
			if ((ulong)num2 != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num2));
			}
			byte[] array = new byte[num];
			uint num3;
			num2 = Win32Exports.ORGetValue(keyHandle, IntPtr.Zero, valueName, out num3, array, ref num);
			if ((ulong)num2 != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num2));
			}
			switch (num3)
			{
			case 1U:
				return Encoding.Unicode.GetString(array).Split(new char[1])[0];
			case 2U:
				return Environment.ExpandEnvironmentVariables(Encoding.Unicode.GetString(array)).Split(new char[1])[0];
			case 3U:
				return array;
			case 4U:
				return (uint)((int)array[3] << 24 | (int)array[2] << 16 | (int)array[1] << 8 | (int)array[0]);
			case 5U:
				return (uint)((int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3]);
			case 6U:
				return Encoding.Unicode.GetString(array).Split(new char[1])[0];
			case 7U:
			{
				List<string> list = new List<string>(Encoding.Unicode.GetString(array).Split(new char[1]));
				for (int i = 0; i < list.Count; i++)
				{
					if (string.IsNullOrEmpty(list[i]))
					{
						list.RemoveAt(i--);
					}
					else if (string.IsNullOrWhiteSpace(list[i]))
					{
						list.RemoveAt(i--);
					}
				}
				return list.ToArray();
			}
			case 8U:
			{
				List<string> list2 = new List<string>(Encoding.Unicode.GetString(array).Split(new char[1]));
				for (int j = 0; j < list2.Count; j++)
				{
					if (string.IsNullOrEmpty(list2[j]))
					{
						list2.RemoveAt(j--);
					}
					else if (string.IsNullOrWhiteSpace(list2[j]))
					{
						list2.RemoveAt(j--);
					}
				}
				return list2.ToArray();
			}
			case 9U:
				return Encoding.Unicode.GetString(array).Split(new char[1])[0];
			case 10U:
			{
				List<string> list3 = new List<string>(Encoding.Unicode.GetString(array).Split(new char[1]));
				for (int k = 0; k < list3.Count; k++)
				{
					if (string.IsNullOrEmpty(list3[k]))
					{
						list3.RemoveAt(k--);
					}
					else if (string.IsNullOrWhiteSpace(list3[k]))
					{
						list3.RemoveAt(k--);
					}
				}
				return list3.ToArray();
			}
			default:
				return array;
			}
		}

		// Token: 0x06000440 RID: 1088
		[DllImport("offreg.dll", CharSet = CharSet.Unicode)]
		private static extern uint ORSetValue(IntPtr keyHandle, string valueName, uint valueType, byte[] data, uint dataLength);

		// Token: 0x06000441 RID: 1089 RVA: 0x000132EC File Offset: 0x000114EC
		[CLSCompliant(false)]
		public static void OfflineRegistrySetValue(IntPtr keyHandle, string valueName, uint valueType, byte[] data)
		{
			uint num = Win32Exports.ORSetValue(keyHandle, valueName, valueType, data, (uint)data.Length);
			if ((ulong)num != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num));
			}
		}

		// Token: 0x06000442 RID: 1090
		[DllImport("offreg.dll", CharSet = CharSet.Unicode)]
		private static extern uint ORCreateKey(IntPtr keyHandle, string subKeyName, string className, uint options, IntPtr securityDescriptor, out IntPtr newKeyHandle, out uint creationDisposition);

		// Token: 0x06000443 RID: 1091 RVA: 0x00013330 File Offset: 0x00011530
		[CLSCompliant(false)]
		public static IntPtr OfflineRegistryCreateKey(IntPtr keyHandle, string subKeyName)
		{
			IntPtr zero = IntPtr.Zero;
			uint num = 0U;
			uint num2 = Win32Exports.ORCreateKey(keyHandle, subKeyName, null, 0U, IntPtr.Zero, out zero, out num);
			if ((ulong)num2 != (ulong)((long)Win32Exports.ERROR_SUCCESS))
			{
				throw new Win32ExportException(string.Format("{0}: This function failed with error 0x{1:x}.", MethodBase.GetCurrentMethod().Name, num2));
			}
			if (num != 1U)
			{
				throw new ImageStorageException(string.Format("{0}: The key '{1}' already exists.", MethodBase.GetCurrentMethod().Name, subKeyName));
			}
			return zero;
		}

		// Token: 0x04000232 RID: 562
		public const int S_OK = 0;

		// Token: 0x04000233 RID: 563
		public static int ERROR_SUCCESS = 0;

		// Token: 0x04000234 RID: 564
		public static int ERROR_NO_MORE_ITEMS = 259;

		// Token: 0x04000235 RID: 565
		public const int INFINITE = -1;

		// Token: 0x04000236 RID: 566
		public static string MountManagerPath = "\\\\.\\MountPointManager";

		// Token: 0x04000237 RID: 567
		private const uint MountManagerControlType = 109U;

		// Token: 0x04000238 RID: 568
		private const uint IoctlDiskBase = 7U;

		// Token: 0x04000239 RID: 569
		private const uint MethodBuffered = 0U;

		// Token: 0x0400023A RID: 570
		private const uint FileReadAccess = 1U;

		// Token: 0x0400023B RID: 571
		private const uint FileReadData = 1U;

		// Token: 0x0400023C RID: 572
		private const uint FileWriteAccess = 2U;

		// Token: 0x0400023D RID: 573
		private const uint FileAnyAccess = 0U;

		// Token: 0x0400023E RID: 574
		private const uint DeviceDisk = 7U;

		// Token: 0x0400023F RID: 575
		private const uint DiskBase = 7U;

		// Token: 0x04000240 RID: 576
		private const uint DiskUserStart = 2016U;

		// Token: 0x04000241 RID: 577
		public const int ANYSIZE_ARRAY = 1;

		// Token: 0x04000242 RID: 578
		public const int INVALID_HANDLE_VALUE = -1;

		// Token: 0x04000243 RID: 579
		[CLSCompliant(false)]
		public const uint SE_PRIVILEGE_ENABLED_BY_DEFAULT = 1U;

		// Token: 0x04000244 RID: 580
		[CLSCompliant(false)]
		public const uint SE_PRIVILEGE_ENABLED = 2U;

		// Token: 0x04000245 RID: 581
		[CLSCompliant(false)]
		public const uint SE_PRIVILEGE_REMOVED = 4U;

		// Token: 0x04000246 RID: 582
		[CLSCompliant(false)]
		public const uint SE_PRIVILEGE_USED_FOR_ACCESS = 2147483648U;

		// Token: 0x04000247 RID: 583
		[CLSCompliant(false)]
		public const uint STANDARD_RIGHTS_REQUIRED = 983040U;

		// Token: 0x04000248 RID: 584
		[CLSCompliant(false)]
		public const uint STANDARD_RIGHTS_READ = 131072U;

		// Token: 0x04000249 RID: 585
		[CLSCompliant(false)]
		public const uint TOKEN_ASSIGN_PRIMARY = 1U;

		// Token: 0x0400024A RID: 586
		[CLSCompliant(false)]
		public const uint TOKEN_DUPLICATE = 2U;

		// Token: 0x0400024B RID: 587
		[CLSCompliant(false)]
		public const uint TOKEN_IMPERSONATE = 4U;

		// Token: 0x0400024C RID: 588
		[CLSCompliant(false)]
		public const uint TOKEN_QUERY = 8U;

		// Token: 0x0400024D RID: 589
		[CLSCompliant(false)]
		public const uint TOKEN_QUERY_SOURCE = 16U;

		// Token: 0x0400024E RID: 590
		[CLSCompliant(false)]
		public const uint TOKEN_ADJUST_PRIVILEGES = 32U;

		// Token: 0x0400024F RID: 591
		[CLSCompliant(false)]
		public const uint TOKEN_ADJUST_GROUPS = 64U;

		// Token: 0x04000250 RID: 592
		[CLSCompliant(false)]
		public const uint TOKEN_ADJUST_DEFAULT = 128U;

		// Token: 0x04000251 RID: 593
		[CLSCompliant(false)]
		public const uint TOKEN_ADJUST_SESSIONID = 256U;

		// Token: 0x04000252 RID: 594
		[CLSCompliant(false)]
		public const uint TOKEN_READ = 131080U;

		// Token: 0x04000253 RID: 595
		[CLSCompliant(false)]
		public const uint TOKEN_ALL_ACCESS = 983551U;

		// Token: 0x04000254 RID: 596
		[CLSCompliant(false)]
		public const uint REG_NONE = 0U;

		// Token: 0x04000255 RID: 597
		[CLSCompliant(false)]
		public const uint REG_SZ = 1U;

		// Token: 0x04000256 RID: 598
		[CLSCompliant(false)]
		public const uint REG_EXPAND_SZ = 2U;

		// Token: 0x04000257 RID: 599
		[CLSCompliant(false)]
		public const uint REG_BINARY = 3U;

		// Token: 0x04000258 RID: 600
		[CLSCompliant(false)]
		public const uint REG_DWORD = 4U;

		// Token: 0x04000259 RID: 601
		[CLSCompliant(false)]
		public const uint REG_DWORD_BIG_ENDIAN = 5U;

		// Token: 0x0400025A RID: 602
		[CLSCompliant(false)]
		public const uint REG_LINK = 6U;

		// Token: 0x0400025B RID: 603
		[CLSCompliant(false)]
		public const uint REG_MULTI_SZ = 7U;

		// Token: 0x0400025C RID: 604
		[CLSCompliant(false)]
		public const uint REG_RESOURCE_LIST = 8U;

		// Token: 0x0400025D RID: 605
		[CLSCompliant(false)]
		public const uint REG_FULL_RESOURCE_DESCRIPTOR = 9U;

		// Token: 0x0400025E RID: 606
		[CLSCompliant(false)]
		public const uint REG_RESOURCE_REQUIREMENTS_LIST = 10U;

		// Token: 0x0400025F RID: 607
		[CLSCompliant(false)]
		public const uint REG_QWORD = 11U;

		// Token: 0x0200008C RID: 140
		private struct FILETIME
		{
			// Token: 0x040002F7 RID: 759
			public uint DateTimeLow;

			// Token: 0x040002F8 RID: 760
			public uint DateTimeHigh;
		}

		// Token: 0x0200008D RID: 141
		[CLSCompliant(false)]
		public enum MoveMethod : uint
		{
			// Token: 0x040002FA RID: 762
			FILE_BEGIN,
			// Token: 0x040002FB RID: 763
			FILE_CURRENT,
			// Token: 0x040002FC RID: 764
			FILE_END
		}

		// Token: 0x0200008E RID: 142
		[Flags]
		[CLSCompliant(false)]
		public enum DesiredAccess : uint
		{
			// Token: 0x040002FE RID: 766
			GENERIC_READ = 2147483648U,
			// Token: 0x040002FF RID: 767
			GENERIC_WRITE = 1073741824U
		}

		// Token: 0x0200008F RID: 143
		[Flags]
		[CLSCompliant(false)]
		public enum ShareMode : uint
		{
			// Token: 0x04000301 RID: 769
			FILE_SHARE_NONE = 0U,
			// Token: 0x04000302 RID: 770
			FILE_SHARE_READ = 1U,
			// Token: 0x04000303 RID: 771
			FILE_SHARE_WRITE = 2U,
			// Token: 0x04000304 RID: 772
			FILE_SHARE_DELETE = 4U
		}

		// Token: 0x02000090 RID: 144
		[Flags]
		[CLSCompliant(false)]
		public enum FlagsAndAttributes : uint
		{
			// Token: 0x04000306 RID: 774
			FILE_ATTRIBUTES_ARCHIVE = 32U,
			// Token: 0x04000307 RID: 775
			FILE_ATTRIBUTE_HIDDEN = 2U,
			// Token: 0x04000308 RID: 776
			FILE_ATTRIBUTE_NORMAL = 128U,
			// Token: 0x04000309 RID: 777
			FILE_ATTRIBUTE_OFFLINE = 4096U,
			// Token: 0x0400030A RID: 778
			FILE_ATTRIBUTE_READONLY = 1U,
			// Token: 0x0400030B RID: 779
			FILE_ATTRIBUTE_SYSTEM = 4U,
			// Token: 0x0400030C RID: 780
			FILE_ATTRIBUTE_TEMPORARY = 256U,
			// Token: 0x0400030D RID: 781
			FILE_FLAG_WRITE_THROUGH = 2147483648U,
			// Token: 0x0400030E RID: 782
			FILE_FLAG_OVERLAPPED = 1073741824U,
			// Token: 0x0400030F RID: 783
			FILE_FLAG_NO_BUFFERING = 536870912U,
			// Token: 0x04000310 RID: 784
			FILE_FLAG_RANDOM_ACCESS = 268435456U,
			// Token: 0x04000311 RID: 785
			FILE_FLAG_SEQUENTIAL_SCAN = 134217728U,
			// Token: 0x04000312 RID: 786
			FILE_FLAG_DELETE_ON = 67108864U,
			// Token: 0x04000313 RID: 787
			FILE_FLAG_POSIX_SEMANTICS = 16777216U,
			// Token: 0x04000314 RID: 788
			FILE_FLAG_OPEN_REPARSE_POINT = 2097152U,
			// Token: 0x04000315 RID: 789
			FILE_FLAG_OPEN_NO_CALL = 1048576U
		}

		// Token: 0x02000091 RID: 145
		[CLSCompliant(false)]
		public enum CreationDisposition : uint
		{
			// Token: 0x04000317 RID: 791
			CREATE_NEW = 1U,
			// Token: 0x04000318 RID: 792
			CREATE_ALWAYS,
			// Token: 0x04000319 RID: 793
			OPEN_EXISTING,
			// Token: 0x0400031A RID: 794
			OPEN_ALWAYS,
			// Token: 0x0400031B RID: 795
			TRUNCATE_EXSTING
		}

		// Token: 0x02000092 RID: 146
		[Flags]
		[CLSCompliant(false)]
		public enum AllocationType : uint
		{
			// Token: 0x0400031D RID: 797
			MEM_COMMIT = 4096U,
			// Token: 0x0400031E RID: 798
			MEM_RESERVE = 8192U,
			// Token: 0x0400031F RID: 799
			MEM_RESET = 524288U,
			// Token: 0x04000320 RID: 800
			MEM_LARGE_PAGES = 536870912U,
			// Token: 0x04000321 RID: 801
			MEM_PHYSICAL = 4194304U,
			// Token: 0x04000322 RID: 802
			MEM_TOP_DOWN = 1048576U,
			// Token: 0x04000323 RID: 803
			MEM_WRITE_WATCH = 2097152U
		}

		// Token: 0x02000093 RID: 147
		[Flags]
		[CLSCompliant(false)]
		public enum MemoryProtection : uint
		{
			// Token: 0x04000325 RID: 805
			PAGE_EXECUTE = 16U,
			// Token: 0x04000326 RID: 806
			PAGE_EXECUTE_READ = 32U,
			// Token: 0x04000327 RID: 807
			PAGE_EXECUTE_READWRITE = 64U,
			// Token: 0x04000328 RID: 808
			PAGE_EXECUTE_WRITECOPY = 128U,
			// Token: 0x04000329 RID: 809
			PAGE_NOACCESS = 1U,
			// Token: 0x0400032A RID: 810
			PAGE_READONLY = 2U,
			// Token: 0x0400032B RID: 811
			PAGE_READWRITE = 4U,
			// Token: 0x0400032C RID: 812
			PAGE_WRITECOPY = 8U,
			// Token: 0x0400032D RID: 813
			PAGE_GUARD = 256U,
			// Token: 0x0400032E RID: 814
			PAGE_NOCACHE = 512U,
			// Token: 0x0400032F RID: 815
			PAGE_WRITECOMBINE = 1024U
		}

		// Token: 0x02000094 RID: 148
		[Flags]
		[CLSCompliant(false)]
		public enum FreeType : uint
		{
			// Token: 0x04000331 RID: 817
			MEM_DECOMMIT = 16384U,
			// Token: 0x04000332 RID: 818
			MEM_RELEASE = 32768U
		}

		// Token: 0x02000095 RID: 149
		[Flags]
		[CLSCompliant(false)]
		public enum PartitionAttributes : ulong
		{
			// Token: 0x04000334 RID: 820
			GPT_ATTRIBUTE_PLATFORM_REQUIRED = 1UL,
			// Token: 0x04000335 RID: 821
			GPT_BASIC_DATA_ATTRIBUTE_NO_DRIVE_LETTER = 9223372036854775808UL,
			// Token: 0x04000336 RID: 822
			GPT_BASIC_DATA_ATTRIBUTE_HIDDEN = 4611686018427387904UL,
			// Token: 0x04000337 RID: 823
			GPT_BASIC_DATA_ATTRIBUTE_SHADOW_COPY = 2305843009213693952UL,
			// Token: 0x04000338 RID: 824
			GPT_BASIC_DATA_ATTRIBUTE_READ_ONLY = 1152921504606846976UL
		}

		// Token: 0x02000096 RID: 150
		[CLSCompliant(false)]
		public enum IoctlControlCode : uint
		{
			// Token: 0x0400033A RID: 826
			IoctlMountManagerScrubRegistry = 7192632U,
			// Token: 0x0400033B RID: 827
			IoctlDiskGetDriveLayoutEx = 458832U
		}

		// Token: 0x02000097 RID: 151
		public struct LUID
		{
			// Token: 0x0400033C RID: 828
			[CLSCompliant(false)]
			public uint LowPart;

			// Token: 0x0400033D RID: 829
			public int HighPart;
		}

		// Token: 0x02000098 RID: 152
		public struct LUID_AND_ATTRIBUTES
		{
			// Token: 0x0400033E RID: 830
			public Win32Exports.LUID Luid;

			// Token: 0x0400033F RID: 831
			[CLSCompliant(false)]
			public uint Attributes;
		}

		// Token: 0x02000099 RID: 153
		public struct TOKEN_PRIVILEGES
		{
			// Token: 0x04000340 RID: 832
			[CLSCompliant(false)]
			public uint PrivilegeCount;

			// Token: 0x04000341 RID: 833
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public Win32Exports.LUID_AND_ATTRIBUTES[] Privileges;
		}

		// Token: 0x0200009A RID: 154
		public enum PARTITION_STYLE
		{
			// Token: 0x04000343 RID: 835
			MasterBootRecord,
			// Token: 0x04000344 RID: 836
			GuidPartitionTable,
			// Token: 0x04000345 RID: 837
			Raw
		}

		// Token: 0x0200009B RID: 155
		[CLSCompliant(false)]
		public struct DRIVE_LAYOUT_INFORMATION_MBR
		{
			// Token: 0x04000346 RID: 838
			public uint DiskSignature;
		}

		// Token: 0x0200009C RID: 156
		[CLSCompliant(false)]
		public struct DRIVE_LAYOUT_INFORMATION_GPT
		{
			// Token: 0x04000347 RID: 839
			public Guid DiskId;

			// Token: 0x04000348 RID: 840
			public ulong StartingUsableOffset;

			// Token: 0x04000349 RID: 841
			public ulong UsableLength;

			// Token: 0x0400034A RID: 842
			public uint MaxPartitionCount;
		}

		// Token: 0x0200009D RID: 157
		[CLSCompliant(false)]
		[StructLayout(LayoutKind.Explicit)]
		public struct DRIVE_LAYOUT_INFORMATION_UNION
		{
			// Token: 0x0400034B RID: 843
			[FieldOffset(0)]
			public Win32Exports.DRIVE_LAYOUT_INFORMATION_MBR Mbr;

			// Token: 0x0400034C RID: 844
			[FieldOffset(0)]
			public Win32Exports.DRIVE_LAYOUT_INFORMATION_GPT Gpt;
		}

		// Token: 0x0200009E RID: 158
		[CLSCompliant(false)]
		public struct PARTITION_INFORMATION_MBR
		{
			// Token: 0x0400034D RID: 845
			public byte PartitionType;

			// Token: 0x0400034E RID: 846
			[MarshalAs(UnmanagedType.Bool)]
			public bool BootIndicator;

			// Token: 0x0400034F RID: 847
			[MarshalAs(UnmanagedType.Bool)]
			public bool RecognizedPartition;

			// Token: 0x04000350 RID: 848
			public uint HiddenSectors;
		}

		// Token: 0x0200009F RID: 159
		[CLSCompliant(false)]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct PARTITION_INFORMATION_GPT
		{
			// Token: 0x04000351 RID: 849
			public Guid PartitionType;

			// Token: 0x04000352 RID: 850
			public Guid PartitionId;

			// Token: 0x04000353 RID: 851
			public ulong Attributes;

			// Token: 0x04000354 RID: 852
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
			public string PartitionName;
		}

		// Token: 0x020000A0 RID: 160
		[StructLayout(LayoutKind.Explicit)]
		public struct PARTITION_INFORMATION_UNION
		{
			// Token: 0x04000355 RID: 853
			[CLSCompliant(false)]
			[FieldOffset(0)]
			public Win32Exports.PARTITION_INFORMATION_MBR Mbr;

			// Token: 0x04000356 RID: 854
			[CLSCompliant(false)]
			[FieldOffset(0)]
			public Win32Exports.PARTITION_INFORMATION_GPT Gpt;
		}

		// Token: 0x020000A1 RID: 161
		public struct PARTITION_INFORMATION_EX
		{
			// Token: 0x04000357 RID: 855
			public Win32Exports.PARTITION_STYLE PartitionStyle;

			// Token: 0x04000358 RID: 856
			public long StartingOffset;

			// Token: 0x04000359 RID: 857
			public long PartitionLength;

			// Token: 0x0400035A RID: 858
			public int PartitionNumber;

			// Token: 0x0400035B RID: 859
			public bool RewritePartition;

			// Token: 0x0400035C RID: 860
			public Win32Exports.PARTITION_INFORMATION_UNION DriveLayoutInformaiton;
		}

		// Token: 0x020000A2 RID: 162
		[CLSCompliant(false)]
		public struct DRIVE_LAYOUT_INFORMATION_EX
		{
			// Token: 0x0400035D RID: 861
			public Win32Exports.PARTITION_STYLE PartitionStyle;

			// Token: 0x0400035E RID: 862
			public int PartitionCount;

			// Token: 0x0400035F RID: 863
			[CLSCompliant(false)]
			public Win32Exports.DRIVE_LAYOUT_INFORMATION_UNION DriveLayoutInformation;

			// Token: 0x04000360 RID: 864
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.Struct)]
			public Win32Exports.PARTITION_INFORMATION_EX[] PartitionEntry;
		}
	}
}
