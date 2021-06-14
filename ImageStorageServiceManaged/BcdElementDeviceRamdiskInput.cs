using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000071 RID: 113
	public class BcdElementDeviceRamdiskInput
	{
		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x00014B3E File Offset: 0x00012D3E
		// (set) Token: 0x060004C2 RID: 1218 RVA: 0x00014B46 File Offset: 0x00012D46
		public BcdElementDeviceInput ParentDevice
		{
			get
			{
				return this._parent;
			}
			set
			{
				if (value.DeviceType == DeviceTypeChoice.RamdiskDevice)
				{
					throw new ImageStorageException("A RamDisk's parent device cannot be another ramdisk.");
				}
				this._parent = value;
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060004C3 RID: 1219 RVA: 0x00014B63 File Offset: 0x00012D63
		// (set) Token: 0x060004C4 RID: 1220 RVA: 0x00014B6B File Offset: 0x00012D6B
		public string FilePath { get; set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060004C5 RID: 1221 RVA: 0x00014B74 File Offset: 0x00012D74
		// (set) Token: 0x060004C6 RID: 1222 RVA: 0x00014B7C File Offset: 0x00012D7C
		public bool RamdiskOptions { get; set; }

		// Token: 0x04000299 RID: 665
		private BcdElementDeviceInput _parent;
	}
}
