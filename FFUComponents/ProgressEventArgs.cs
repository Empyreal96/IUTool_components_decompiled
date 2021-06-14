using System;

namespace FFUComponents
{
	// Token: 0x02000040 RID: 64
	public class ProgressEventArgs : EventArgs
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00004E26 File Offset: 0x00003026
		// (set) Token: 0x0600013D RID: 317 RVA: 0x00004E2E File Offset: 0x0000302E
		public IFFUDevice Device { get; private set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600013E RID: 318 RVA: 0x00004E37 File Offset: 0x00003037
		// (set) Token: 0x0600013F RID: 319 RVA: 0x00004E3F File Offset: 0x0000303F
		public long Position { get; private set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000140 RID: 320 RVA: 0x00004E48 File Offset: 0x00003048
		// (set) Token: 0x06000141 RID: 321 RVA: 0x00004E50 File Offset: 0x00003050
		public long Length { get; private set; }

		// Token: 0x06000142 RID: 322 RVA: 0x00004E59 File Offset: 0x00003059
		public ProgressEventArgs(IFFUDevice device, long pos, long len)
		{
			this.Device = device;
			this.Position = pos;
			this.Length = len;
		}
	}
}
