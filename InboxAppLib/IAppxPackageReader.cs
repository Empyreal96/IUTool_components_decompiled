using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000018 RID: 24
	[CLSCompliant(false)]
	[Guid("b5c49650-99bc-481c-9a34-3d53a4106708")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxPackageReader
	{
		// Token: 0x06000070 RID: 112
		IAppxBlockMapReader GetBlockMap();

		// Token: 0x06000071 RID: 113
		IAppxFile GetFootprintFile([In] APPX_FOOTPRINT_FILE_TYPE type);

		// Token: 0x06000072 RID: 114
		IAppxFile GetPayloadFile([MarshalAs(UnmanagedType.LPWStr)] [In] string fileName);

		// Token: 0x06000073 RID: 115
		IAppxFilesEnumerator GetPayloadFiles();

		// Token: 0x06000074 RID: 116
		IAppxManifestReader GetManifest();
	}
}
