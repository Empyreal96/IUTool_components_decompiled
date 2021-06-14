using System;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000052 RID: 82
	[CLSCompliant(false)]
	public interface IVirtualHardDisk : IDisposable
	{
		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x060003C0 RID: 960
		uint SectorSize { get; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x060003C1 RID: 961
		ulong SectorCount { get; }

		// Token: 0x060003C2 RID: 962
		void FlushFile();

		// Token: 0x060003C3 RID: 963
		[CLSCompliant(false)]
		void ReadSector(ulong sector, byte[] buffer, uint offset);

		// Token: 0x060003C4 RID: 964
		[CLSCompliant(false)]
		void WriteSector(ulong sector, byte[] buffer, uint offset);
	}
}
