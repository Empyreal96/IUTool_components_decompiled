using System;
using System.IO;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x0200001A RID: 26
	public class ImagePartition
	{
		// Token: 0x060000F2 RID: 242 RVA: 0x00004FC0 File Offset: 0x000031C0
		protected ImagePartition()
		{
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006CDD File Offset: 0x00004EDD
		public ImagePartition(string name, string root)
		{
			this.Name = name;
			this.Root = root;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x00006CF3 File Offset: 0x00004EF3
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x00006CFB File Offset: 0x00004EFB
		public string PhysicalDeviceId { get; protected set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00006D04 File Offset: 0x00004F04
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x00006D0C File Offset: 0x00004F0C
		public string Name { get; protected set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00006D15 File Offset: 0x00004F15
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00006D1D File Offset: 0x00004F1D
		public string Root { get; protected set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00006D26 File Offset: 0x00004F26
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00006D2E File Offset: 0x00004F2E
		public DriveInfo MountedDriveInfo { get; protected set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00006D38 File Offset: 0x00004F38
		public FileInfo[] Files
		{
			get
			{
				if (this._files == null && !string.IsNullOrEmpty(this.Root))
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(this.Root);
					this._files = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
				}
				return this._files;
			}
		}

		// Token: 0x0400004C RID: 76
		private FileInfo[] _files;
	}
}
