using System;
using System.Collections;
using System.ComponentModel;
using System.Management;
using System.Runtime.InteropServices;

namespace USB_Test_Infrastructure
{
	// Token: 0x0200001B RID: 27
	public class UsbConnectionManager
	{
		// Token: 0x06000030 RID: 48 RVA: 0x000021AB File Offset: 0x000003AB
		public UsbConnectionManager(OnDeviceConnect deviceConnectCallback, OnDeviceDisconnect deviceDisconnectCallback)
		{
			if (deviceConnectCallback == null)
			{
				throw new ArgumentNullException("deviceConnectCallback");
			}
			if (deviceDisconnectCallback == null)
			{
				throw new ArgumentNullException("deviceDisconnectCallback");
			}
			this.deviceConnectCallback = deviceConnectCallback;
			this.deviceDisconnectCallback = deviceDisconnectCallback;
			this.DevicesHash = new Hashtable();
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000021E8 File Offset: 0x000003E8
		public void Start()
		{
			this.DiscoverUsbDevices();
			this.StartUsbDeviceNotificationWatcher();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000021F6 File Offset: 0x000003F6
		public void Stop()
		{
			this.StopUsbDeviceNotificationWatcher();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002200 File Offset: 0x00000400
		private void DiscoverUsbDevices()
		{
			foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher(string.Format("SELECT PnPDeviceId FROM Win32_PnPEntity where ClassGuid ='{0}'", UsbConnectionManager.UsbClassGuid.ToString("B"))).Get())
			{
				string text = managementBaseObject["PnPDeviceId"] as string;
				if (text != null)
				{
					this.OnUsbDeviceConnect(text);
				}
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000227C File Offset: 0x0000047C
		private void StartUsbDeviceNotificationWatcher()
		{
			WqlEventQuery query = new WqlEventQuery(string.Format("SELECT * FROM __InstanceCreationEvent WITHIN 0.1 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.ClassGuid = '{0}'", UsbConnectionManager.UsbClassGuid.ToString("B")));
			this.connectWatcher = new ManagementEventWatcher(query);
			this.connectWatcher.EventArrived += delegate(object sender, EventArrivedEventArgs args)
			{
				ManagementBaseObject managementBaseObject = args.NewEvent["TargetInstance"] as ManagementBaseObject;
				if (managementBaseObject != null)
				{
					string pnpId = managementBaseObject.GetPropertyValue("PnPDeviceID").ToString();
					try
					{
						this.OnUsbDeviceConnect(pnpId);
					}
					catch (Exception)
					{
					}
				}
			};
			this.connectWatcher.Start();
			WqlEventQuery query2 = new WqlEventQuery(string.Format("SELECT * FROM __InstanceDeletionEvent WITHIN 0.1 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.ClassGuid = '{0}'", UsbConnectionManager.UsbClassGuid.ToString("B")));
			this.disconnectWatcher = new ManagementEventWatcher(query2);
			this.disconnectWatcher.EventArrived += delegate(object sender, EventArrivedEventArgs args)
			{
				ManagementBaseObject managementBaseObject = args.NewEvent["TargetInstance"] as ManagementBaseObject;
				if (managementBaseObject != null)
				{
					string pnpId = managementBaseObject.GetPropertyValue("PnPDeviceID").ToString();
					try
					{
						this.OnUsbDeviceDisconnect(pnpId, true);
					}
					catch (Exception)
					{
					}
				}
			};
			this.disconnectWatcher.Start();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002324 File Offset: 0x00000524
		private void StopUsbDeviceNotificationWatcher()
		{
			if (this.connectWatcher != null)
			{
				this.connectWatcher.Stop();
				this.connectWatcher.Dispose();
				this.connectWatcher = null;
			}
			if (this.disconnectWatcher != null)
			{
				this.disconnectWatcher.Stop();
				this.disconnectWatcher.Dispose();
				this.disconnectWatcher = null;
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000237C File Offset: 0x0000057C
		private void OnUsbDeviceConnect(string pnpId)
		{
			try
			{
				this.DevicesHash.Add(pnpId, this.GetUsbDevicePath(pnpId));
			}
			catch
			{
			}
			this.deviceConnectCallback(this.GetUsbDevicePath(pnpId));
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000023C4 File Offset: 0x000005C4
		private void OnUsbDeviceDisconnect(string pnpId, bool fCallCallback)
		{
			this.deviceDisconnectCallback((string)this.DevicesHash[pnpId]);
			this.DevicesHash.Remove(pnpId);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000023F0 File Offset: 0x000005F0
		private unsafe string GetUsbDevicePath(string pnpId)
		{
			IntPtr intPtr = NativeMethods.SetupDiGetClassDevs(ref UsbConnectionManager.UsbIfGuid, pnpId, 0, 18);
			if (IntPtr.Zero == intPtr)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			int memberIndex = 0;
			int num = 0;
			DeviceInterfaceData deviceInterfaceData = new DeviceInterfaceData
			{
				Size = Marshal.SizeOf(typeof(DeviceInterfaceData))
			};
			if (!NativeMethods.SetupDiEnumDeviceInterfaces(intPtr, 0, ref UsbConnectionManager.UsbIfGuid, memberIndex, ref deviceInterfaceData))
			{
				Marshal.GetLastWin32Error();
			}
			if (!NativeMethods.SetupDiGetDeviceInterfaceDetail(intPtr, ref deviceInterfaceData, IntPtr.Zero, 0, ref num, IntPtr.Zero))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 122)
				{
					throw new Win32Exception(lastWin32Error);
				}
			}
			DeviceInterfaceDetailData* ptr = (DeviceInterfaceDetailData*)((void*)Marshal.AllocHGlobal(num));
			if (IntPtr.Size == 4)
			{
				ptr->Size = 6;
			}
			else
			{
				ptr->Size = 8;
			}
			DeviceInformationData deviceInformationData = new DeviceInformationData
			{
				Size = Marshal.SizeOf(typeof(DeviceInformationData))
			};
			if (!NativeMethods.SetupDiGetDeviceInterfaceDetail(intPtr, ref deviceInterfaceData, ptr, num, ref num, ref deviceInformationData))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			string result = Marshal.PtrToStringAuto(new IntPtr((void*)(&ptr->DevicePath)));
			Marshal.FreeHGlobal((IntPtr)((void*)ptr));
			return result;
		}

		// Token: 0x04000084 RID: 132
		private static string UsbClassGuidString = "{88BAE032-5A81-49F0-BC3D-A4FF138216D6}";

		// Token: 0x04000085 RID: 133
		private static string UsbIfGuidString = "{82809DD0-51F5-11E1-B86C-0800200C9A66}";

		// Token: 0x04000086 RID: 134
		private static Guid UsbClassGuid = new Guid(UsbConnectionManager.UsbClassGuidString);

		// Token: 0x04000087 RID: 135
		private static Guid UsbIfGuid = new Guid(UsbConnectionManager.UsbIfGuidString);

		// Token: 0x04000088 RID: 136
		private OnDeviceConnect deviceConnectCallback;

		// Token: 0x04000089 RID: 137
		private OnDeviceDisconnect deviceDisconnectCallback;

		// Token: 0x0400008A RID: 138
		private ManagementEventWatcher connectWatcher;

		// Token: 0x0400008B RID: 139
		private ManagementEventWatcher disconnectWatcher;

		// Token: 0x0400008C RID: 140
		private Hashtable DevicesHash;
	}
}
