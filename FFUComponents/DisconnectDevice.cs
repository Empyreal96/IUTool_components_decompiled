using System;
using System.Collections.Generic;
using System.Threading;

namespace FFUComponents
{
	// Token: 0x02000016 RID: 22
	internal class DisconnectDevice
	{
		// Token: 0x06000083 RID: 131 RVA: 0x00003880 File Offset: 0x00001A80
		~DisconnectDevice()
		{
			this.cancelEvent.Set();
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000084 RID: 132 RVA: 0x000038B4 File Offset: 0x00001AB4
		// (set) Token: 0x06000085 RID: 133 RVA: 0x000038BC File Offset: 0x00001ABC
		public IFFUDeviceInternal FFUDevice { get; private set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000086 RID: 134 RVA: 0x000038C5 File Offset: 0x00001AC5
		public Guid DeviceUniqueId
		{
			get
			{
				return this.FFUDevice.DeviceUniqueID;
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000038D4 File Offset: 0x00001AD4
		public DisconnectDevice(IFFUDeviceInternal device, Dictionary<Guid, DisconnectDevice> collection)
		{
			this.FFUDevice = device;
			this.DiscCollection = collection;
			this.cancelEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
			this.removalThread = new Thread(new ParameterizedThreadStart(DisconnectDevice.WaitAndRemove));
			this.removalThread.Start(this);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00003925 File Offset: 0x00001B25
		public void Cancel()
		{
			this.cancelEvent.Set();
			this.Remove();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00003939 File Offset: 0x00001B39
		public bool WaitForReconnect()
		{
			return this.cancelEvent.WaitOne(30000, false);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000394C File Offset: 0x00001B4C
		private static void WaitAndRemove(object obj)
		{
			DisconnectDevice disconnectDevice = obj as DisconnectDevice;
			if (!disconnectDevice.WaitForReconnect())
			{
				disconnectDevice.Remove();
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003970 File Offset: 0x00001B70
		private void Remove()
		{
			Dictionary<Guid, DisconnectDevice> discCollection = this.DiscCollection;
			lock (discCollection)
			{
				this.DiscCollection.Remove(this.DeviceUniqueId);
			}
		}

		// Token: 0x04000046 RID: 70
		private EventWaitHandle cancelEvent;

		// Token: 0x04000047 RID: 71
		private Dictionary<Guid, DisconnectDevice> DiscCollection;

		// Token: 0x04000048 RID: 72
		private Thread removalThread;
	}
}
