using System;
using System.Collections.Generic;

namespace FFUComponents
{
	// Token: 0x02000017 RID: 23
	internal class DisconnectTimer
	{
		// Token: 0x0600008C RID: 140 RVA: 0x000039BC File Offset: 0x00001BBC
		public DisconnectTimer()
		{
			this.devices = new Dictionary<Guid, DisconnectDevice>();
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000039D0 File Offset: 0x00001BD0
		public void StopAllTimers()
		{
			DisconnectDevice[] array = new DisconnectDevice[this.devices.Values.Count];
			this.devices.Values.CopyTo(array, 0);
			DisconnectDevice[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Cancel();
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003A20 File Offset: 0x00001C20
		public void StartTimer(IFFUDeviceInternal device)
		{
			Dictionary<Guid, DisconnectDevice> obj = this.devices;
			lock (obj)
			{
				DisconnectDevice disconnectDevice;
				if (this.devices.TryGetValue(device.DeviceUniqueID, out disconnectDevice))
				{
					throw new FFUException(device.DeviceFriendlyName, device.DeviceUniqueID, Resources.ERROR_MULTIPE_DISCONNECT_NOTIFICATIONS);
				}
				this.devices[device.DeviceUniqueID] = new DisconnectDevice(device, this.devices);
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00003AA4 File Offset: 0x00001CA4
		public IFFUDeviceInternal StopTimer(IFFUDeviceInternal device)
		{
			IFFUDeviceInternal result = null;
			Dictionary<Guid, DisconnectDevice> obj = this.devices;
			lock (obj)
			{
				DisconnectDevice disconnectDevice;
				if (this.devices.TryGetValue(device.DeviceUniqueID, out disconnectDevice))
				{
					disconnectDevice.Cancel();
					result = disconnectDevice.FFUDevice;
				}
			}
			return result;
		}

		// Token: 0x04000049 RID: 73
		private Dictionary<Guid, DisconnectDevice> devices;
	}
}
