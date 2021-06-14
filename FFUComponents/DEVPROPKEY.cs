using System;
using System.Runtime.InteropServices;

namespace FFUComponents
{
	// Token: 0x0200003A RID: 58
	[StructLayout(LayoutKind.Sequential)]
	public class DEVPROPKEY
	{
		// Token: 0x0600010B RID: 267 RVA: 0x00004B4E File Offset: 0x00002D4E
		public DEVPROPKEY(Guid a_fmtid, uint a_pid)
		{
			this.fmtid = a_fmtid;
			this.pid = a_pid;
		}

		// Token: 0x040000C5 RID: 197
		public Guid fmtid;

		// Token: 0x040000C6 RID: 198
		public uint pid;
	}
}
