using System;

namespace FFUComponents
{
	// Token: 0x02000020 RID: 32
	public class ConnectEventArgs : EventArgs
	{
		// Token: 0x060000C4 RID: 196 RVA: 0x00003CFD File Offset: 0x00001EFD
		private ConnectEventArgs()
		{
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00003D05 File Offset: 0x00001F05
		public ConnectEventArgs(IFFUDevice device)
		{
			this.device = device;
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00003D14 File Offset: 0x00001F14
		public IFFUDevice Device
		{
			get
			{
				return this.device;
			}
		}

		// Token: 0x0400004E RID: 78
		private IFFUDevice device;
	}
}
