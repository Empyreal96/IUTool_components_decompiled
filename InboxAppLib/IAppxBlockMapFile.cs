using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000021 RID: 33
	[CLSCompliant(false)]
	[Guid("277672ac-4f63-42c1-8abc-beae3600eb59")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxBlockMapFile
	{
		// Token: 0x060000A0 RID: 160
		IAppxBlockMapBlocksEnumerator GetBlocks();

		// Token: 0x060000A1 RID: 161
		uint GetLocalFileHeaderSize();

		// Token: 0x060000A2 RID: 162
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetName();

		// Token: 0x060000A3 RID: 163
		ulong GetUncompressedSize();

		// Token: 0x060000A4 RID: 164
		bool ValidateFileHash([In] IStream fileStream);
	}
}
