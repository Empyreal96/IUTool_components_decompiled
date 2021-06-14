using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace FFUComponents
{
	// Token: 0x02000015 RID: 21
	public class DeviceSet
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00003554 File Offset: 0x00001754
		public DeviceSet(string DevinstId)
		{
			this.Initialize(DevinstId);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00003564 File Offset: 0x00001764
		public uint GetAddress()
		{
			uint num = 0U;
			uint num2;
			if (!NativeMethods.SetupDiGetDeviceProperty(this.deviceSetHandle, this.deviceInfoData, NativeMethods.DEVPKEY_Device_Address, out num2, out num, (uint)Marshal.SizeOf<uint>(num), IntPtr.Zero, 0U))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new FFUException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_SETUP_DI_GET_DEVICE_PROPERTY, new object[]
				{
					lastWin32Error
				}));
			}
			return num;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000035C8 File Offset: 0x000017C8
		public string GetParentId()
		{
			uint devinst;
			uint num = NativeMethods.CM_Get_Parent(out devinst, this.deviceInfoData.DevInst, 0U);
			if (num != 0U)
			{
				throw new FFUException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_CM_GET_PARENT, new object[]
				{
					num
				}));
			}
			return DeviceSet.DeviceIdFromCmDevinst(devinst);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00003618 File Offset: 0x00001818
		public string GetHubDevicePath()
		{
			SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
			if (!NativeMethods.SetupDiEnumDeviceInterfaces(this.deviceSetHandle, IntPtr.Zero, ref NativeMethods.GUID_DEVINTERFACE_USB_HUB, 0U, deviceInterfaceData))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new FFUException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_SETUP_DI_ENUM_DEVICE_INTERFACES, new object[]
				{
					lastWin32Error
				}));
			}
			SP_DEVICE_INTERFACE_DETAIL_DATA sp_DEVICE_INTERFACE_DETAIL_DATA = new SP_DEVICE_INTERFACE_DETAIL_DATA();
			if (!NativeMethods.SetupDiGetDeviceInterfaceDetailW(this.deviceSetHandle, deviceInterfaceData, sp_DEVICE_INTERFACE_DETAIL_DATA, (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DETAIL_DATA>(sp_DEVICE_INTERFACE_DETAIL_DATA), IntPtr.Zero, IntPtr.Zero))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new FFUException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_SETUP_DI_GET_DEVICE_INTERFACE_DETAIL_W, new object[]
				{
					lastWin32Error
				}));
			}
			return sp_DEVICE_INTERFACE_DETAIL_DATA.DevicePath.ToString();
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000036C8 File Offset: 0x000018C8
		private static string DeviceIdFromCmDevinst(uint devinst)
		{
			uint num2;
			uint num = NativeMethods.CM_Get_Device_ID_Size(out num2, devinst, 0U);
			if (num != 0U)
			{
				throw new FFUException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_CM_GET_DEVICE_ID_SIZE, new object[]
				{
					num
				}));
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = (int)(num2 + 1U);
			StringBuilder stringBuilder2 = stringBuilder;
			num = NativeMethods.CM_Get_Device_ID(devinst, stringBuilder2, (uint)stringBuilder2.Capacity, 0U);
			if (num != 0U)
			{
				throw new FFUException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_CM_GET_DEVICE_ID, new object[]
				{
					num
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003754 File Offset: 0x00001954
		private void GetDeviceInfoData()
		{
			SP_DEVINFO_DATA sp_DEVINFO_DATA = new SP_DEVINFO_DATA();
			if (NativeMethods.SetupDiEnumDeviceInfo(this.deviceSetHandle, 0U, sp_DEVINFO_DATA))
			{
				this.deviceInfoData = sp_DEVINFO_DATA;
				return;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			throw new FFUException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_SETUP_DI_ENUM_DEVICE_INFO, new object[]
			{
				lastWin32Error
			}));
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000037A8 File Offset: 0x000019A8
		private void Initialize(string DevinstId)
		{
			if (string.IsNullOrEmpty(DevinstId))
			{
				throw new FFUException(Resources.ERROR_NULL_OR_EMPTY_STRING);
			}
			this.deviceInfoData = null;
			this.deviceSetHandle = NativeMethods.SetupDiGetClassDevs(IntPtr.Zero, DevinstId, IntPtr.Zero, 22U);
			if (this.deviceSetHandle == NativeMethods.INVALID_HANDLE_VALUE)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new FFUException(string.Format(CultureInfo.CurrentCulture, Resources.ERROR_INVALID_HANDLE, new object[]
				{
					DevinstId,
					lastWin32Error
				}));
			}
			this.GetDeviceInfoData();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00003830 File Offset: 0x00001A30
		~DeviceSet()
		{
			if (NativeMethods.INVALID_HANDLE_VALUE != this.deviceSetHandle)
			{
				NativeMethods.SetupDiDestroyDeviceInfoList(this.deviceSetHandle);
				this.deviceSetHandle = NativeMethods.INVALID_HANDLE_VALUE;
			}
		}

		// Token: 0x04000043 RID: 67
		private IntPtr deviceSetHandle;

		// Token: 0x04000044 RID: 68
		private SP_DEVINFO_DATA deviceInfoData;
	}
}
