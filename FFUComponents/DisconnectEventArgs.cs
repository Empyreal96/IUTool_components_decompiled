using System;

namespace FFUComponents
{
	// Token: 0x02000021 RID: 33
	public class DisconnectEventArgs : EventArgs
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x00003CFD File Offset: 0x00001EFD
		private DisconnectEventArgs()
		{
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00003D1C File Offset: 0x00001F1C
		public DisconnectEventArgs(Guid deviceUniqueId)
		{
			this.deviceUniqueId = deviceUniqueId;
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00003D2B File Offset: 0x00001F2B
		public Guid DeviceUniqueId
		{
			get
			{
				return this.deviceUniqueId;
			}
		}

		// Token: 0x0400004F RID: 79
		private Guid deviceUniqueId;
	}
}
