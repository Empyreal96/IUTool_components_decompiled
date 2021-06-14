using System;
using System.Threading;

namespace Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
	// Token: 0x02000012 RID: 18
	public class MobileCoreWIM : MobileCoreImage
	{
		// Token: 0x06000086 RID: 134 RVA: 0x00005388 File Offset: 0x00003588
		internal MobileCoreWIM(string path) : base(path)
		{
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005394 File Offset: 0x00003594
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
					this.MountWIM(readOnly);
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

		// Token: 0x06000088 RID: 136 RVA: 0x000053F0 File Offset: 0x000035F0
		public override void MountReadOnly()
		{
			bool readOnly = true;
			this.MountWithRetry(readOnly);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005408 File Offset: 0x00003608
		public override void Mount()
		{
			bool readOnly = false;
			this.MountWithRetry(readOnly);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005420 File Offset: 0x00003620
		private void MountWIM(bool readOnly)
		{
			object lockObj = MobileCoreWIM._lockObj;
			lock (lockObj)
			{
				this.m_partitions.Clear();
				string text = string.Empty;
				if (!readOnly)
				{
					text = FileUtils.GetTempDirectory();
				}
				string tempDirectory = FileUtils.GetTempDirectory();
				if (CommonUtils.MountWIM(this.m_mobileCoreImagePath, tempDirectory, text))
				{
					this.m_partitions.Add(new ImagePartition("WIM", tempDirectory));
					this._tmpDir = text;
					this._mountPoint = tempDirectory;
					this._commitChanges = !readOnly;
				}
				else
				{
					FileUtils.DeleteTree(tempDirectory);
					if (!string.IsNullOrEmpty(text))
					{
						FileUtils.DeleteTree(text);
					}
				}
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000054CC File Offset: 0x000036CC
		public override void Unmount()
		{
			object lockObj = MobileCoreWIM._lockObj;
			lock (lockObj)
			{
				CommonUtils.DismountWIM(this.m_mobileCoreImagePath, this._mountPoint, this._commitChanges);
				this.m_partitions.Clear();
				base.IsMounted = false;
				if (!string.IsNullOrEmpty(this._tmpDir))
				{
					FileUtils.DeleteTree(this._tmpDir);
				}
				this._tmpDir = null;
				this._mountPoint = null;
			}
		}

		// Token: 0x04000026 RID: 38
		private bool _commitChanges;

		// Token: 0x04000027 RID: 39
		private string _tmpDir;

		// Token: 0x04000028 RID: 40
		private string _mountPoint;

		// Token: 0x04000029 RID: 41
		private static readonly object _lockObj = new object();

		// Token: 0x0400002A RID: 42
		private const int SLEEP_1000 = 1000;

		// Token: 0x0400002B RID: 43
		private const int MAX_RETRY = 3;
	}
}
