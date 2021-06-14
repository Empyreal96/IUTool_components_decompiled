using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x0200002A RID: 42
	public class OutputWrapper : IPayloadWrapper
	{
		// Token: 0x060001C4 RID: 452 RVA: 0x0000F838 File Offset: 0x0000DA38
		public OutputWrapper(string path)
		{
			this.path = path;
			this.writes = new Queue<IAsyncResult>();
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000F852 File Offset: 0x0000DA52
		public void InitializeWrapper(long payloadSize)
		{
			this.fileStream = new FileStream(this.path, FileMode.Create, FileAccess.Write, FileShare.None, 1048576, true);
			this.fileStream.SetLength(payloadSize);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000F87A File Offset: 0x0000DA7A
		public void ResetPosition()
		{
			this.fileStream.Seek(0L, SeekOrigin.Begin);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000F88C File Offset: 0x0000DA8C
		public void Write(byte[] data)
		{
			while (this.writes.Count > 0 && this.writes.Peek().IsCompleted)
			{
				this.fileStream.EndWrite(this.writes.Dequeue());
			}
			IAsyncResult item = this.fileStream.BeginWrite(data, 0, data.Length, null, null);
			this.writes.Enqueue(item);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000F8F0 File Offset: 0x0000DAF0
		public void FinalizeWrapper()
		{
			while (this.writes.Count > 0)
			{
				this.fileStream.EndWrite(this.writes.Dequeue());
			}
			if (this.fileStream != null)
			{
				this.fileStream.Close();
				this.fileStream = null;
			}
		}

		// Token: 0x04000121 RID: 289
		private string path;

		// Token: 0x04000122 RID: 290
		private FileStream fileStream;

		// Token: 0x04000123 RID: 291
		private Queue<IAsyncResult> writes;
	}
}
