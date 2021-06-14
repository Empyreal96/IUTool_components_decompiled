using System;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000011 RID: 17
	public class MobileCoreVHD : MobileCoreImage
	{
		// Token: 0x0600007F RID: 127 RVA: 0x000051AE File Offset: 0x000033AE
		internal MobileCoreVHD(string path) : base(path)
		{
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000051C4 File Offset: 0x000033C4
		private void MountWithRetry(bool readOnly)
		{
			if (base.IsMounted)
			{
				return;
			}
			int num = 0;
			int num2 = 3;
			bool flag = false;
			do
			{
				flag = false;
				try
				{
					this.MountVHD(readOnly);
				}
				catch (Exception)
				{
					num++;
					flag = (num < num2);
					if (!flag)
					{
						throw;
					}
					Thread.Sleep(1000);
				}
			}
			while (flag);
			base.IsMounted = true;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005220 File Offset: 0x00003420
		public override void MountReadOnly()
		{
			bool readOnly = true;
			this.MountWithRetry(readOnly);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00005238 File Offset: 0x00003438
		public override void Mount()
		{
			bool readOnly = false;
			this.MountWithRetry(readOnly);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005250 File Offset: 0x00003450
		private void MountVHD(bool readOnly)
		{
			object lockObj = MobileCoreVHD._lockObj;
			lock (lockObj)
			{
				this.m_partitions.Clear();
				try
				{
					this._hndlVirtDisk = CommonUtils.MountVHD(this.m_mobileCoreImagePath, readOnly);
					int capacity = 1024;
					StringBuilder stringBuilder = new StringBuilder(capacity);
					int virtualDiskPhysicalPath = VirtualDiskLib.GetVirtualDiskPhysicalPath(this._hndlVirtDisk, ref capacity, stringBuilder);
					if (0 < virtualDiskPhysicalPath)
					{
						throw new Win32Exception(virtualDiskPhysicalPath);
					}
					this.m_partitions.PopulateFromPhysicalDeviceId(stringBuilder.ToString());
					if (this.m_partitions.Count == 0)
					{
						throw new IUException("Could not retrieve logical drive information for {0}", new object[]
						{
							stringBuilder
						});
					}
				}
				catch (Exception)
				{
					this.Unmount();
					throw;
				}
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005318 File Offset: 0x00003518
		public override void Unmount()
		{
			object lockObj = MobileCoreVHD._lockObj;
			lock (lockObj)
			{
				CommonUtils.DismountVHD(this._hndlVirtDisk);
				this._hndlVirtDisk = IntPtr.Zero;
				this.m_partitions.Clear();
				base.IsMounted = false;
			}
		}

		// Token: 0x04000022 RID: 34
		private IntPtr _hndlVirtDisk = IntPtr.Zero;

		// Token: 0x04000023 RID: 35
		private static readonly object _lockObj = new object();

		// Token: 0x04000024 RID: 36
		private const int SLEEP_1000 = 1000;

		// Token: 0x04000025 RID: 37
		private const int MAX_RETRY = 3;
	}
}
