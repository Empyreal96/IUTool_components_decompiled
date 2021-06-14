using System;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x02000010 RID: 16
	[Guid("CBA774B0-D968-4363-898D-D7FCDCFBDDB2")]
	[ClassInterface(ClassInterfaceType.None)]
	[ComSourceInterfaces(typeof(IFlashableDeviceNotify))]
	[ComVisible(true)]
	public class FlashableDevice : IFlashableDevice, IDisposable
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000061 RID: 97 RVA: 0x00003268 File Offset: 0x00001468
		// (remove) Token: 0x06000062 RID: 98 RVA: 0x000032A0 File Offset: 0x000014A0
		public event ProgressHandler Progress;

		// Token: 0x06000063 RID: 99 RVA: 0x000032D5 File Offset: 0x000014D5
		public FlashableDevice(IFFUDevice ffuDev)
		{
			this.theDev = ffuDev;
			this.theDev.ProgressEvent += this.theDev_ProgressEvent;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000032FB File Offset: 0x000014FB
		public void Dispose()
		{
			this.theDev.ProgressEvent -= this.theDev_ProgressEvent;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003314 File Offset: 0x00001514
		private void theDev_ProgressEvent(object sender, ProgressEventArgs e)
		{
			if (this.Progress != null)
			{
				this.Progress(e.Position, e.Length);
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003335 File Offset: 0x00001535
		public string GetFriendlyName()
		{
			return this.theDev.DeviceFriendlyName;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003344 File Offset: 0x00001544
		public string GetUniqueIDStr()
		{
			return this.theDev.DeviceUniqueID.ToString();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000336C File Offset: 0x0000156C
		public string GetSerialNumberStr()
		{
			return this.theDev.SerialNumber.ToString();
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003394 File Offset: 0x00001594
		public bool FlashFFU(string filePath)
		{
			bool result = true;
			try
			{
				this.theDev.FlashFFUFile(filePath);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0400003F RID: 63
		private IFFUDevice theDev;
	}
}
