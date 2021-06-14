using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x02000013 RID: 19
	[Guid("71A8CA8E-ED31-4C25-8CFF-689C40E6946E")]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public class FlashingManager : IFlashingManager
	{
		// Token: 0x06000072 RID: 114 RVA: 0x0000344B File Offset: 0x0000164B
		public bool Start()
		{
			FFUManager.Start();
			return true;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003453 File Offset: 0x00001653
		public bool Stop()
		{
			FFUManager.Stop();
			return true;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000345C File Offset: 0x0000165C
		public bool GetFlashableDevices(ref IEnumerator result)
		{
			ICollection<IFFUDevice> collection = new List<IFFUDevice>();
			FFUManager.GetFlashableDevices(ref collection);
			if (collection.Count == 0)
			{
				collection = null;
				return false;
			}
			result = new FlashableDeviceCollection(collection);
			return true;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000348C File Offset: 0x0000168C
		public IFlashableDevice GetFlashableDevice(string instancePath, bool enableFallback)
		{
			IFFUDevice flashableDevice = FFUManager.GetFlashableDevice(instancePath, enableFallback);
			if (flashableDevice == null)
			{
				return null;
			}
			return new FlashableDevice(flashableDevice);
		}
	}
}
