using System;
using Microsoft.Windows.Flashing.Platform;

namespace FFUComponents
{
	// Token: 0x02000022 RID: 34
	public class NotificationCallback : DeviceNotificationCallback
	{
		// Token: 0x060000CB RID: 203 RVA: 0x00003D3B File Offset: 0x00001F3B
		public override void Connected(string devicePath)
		{
			FFUManager.OnDeviceConnect(devicePath);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00003D43 File Offset: 0x00001F43
		public override void Disconnected(string devicePath)
		{
			FFUManager.OnDeviceDisconnect(devicePath);
		}
	}
}
