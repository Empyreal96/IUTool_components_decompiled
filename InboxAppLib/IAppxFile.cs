using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x0200001C RID: 28
	[CLSCompliant(false)]
	[Guid("91df827b-94fd-468f-827b-57f41b2f6f2e")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxFile
	{
		// Token: 0x06000090 RID: 144
		APPX_COMPRESSION_OPTION GetCompressionOption();

		// Token: 0x06000091 RID: 145
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetContentType();

		// Token: 0x06000092 RID: 146
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetName();

		// Token: 0x06000093 RID: 147
		ulong GetSize();

		// Token: 0x06000094 RID: 148
		IStream GetStream();
	}
}
