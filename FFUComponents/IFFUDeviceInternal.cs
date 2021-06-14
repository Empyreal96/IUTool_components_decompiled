using System;

namespace FFUComponents
{
	// Token: 0x02000027 RID: 39
	internal interface IFFUDeviceInternal : IFFUDevice, IDisposable
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000108 RID: 264
		string UsbDevicePath { get; }

		// Token: 0x06000109 RID: 265
		bool NeedsTimer();
	}
}
