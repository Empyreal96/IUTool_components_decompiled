using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x0200001D RID: 29
	[CLSCompliant(false)]
	[Guid("03faf64d-f26f-4b2c-aaf7-8fe7789b8bca")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestProperties
	{
		// Token: 0x06000095 RID: 149
		bool GetBoolValue(string name);

		// Token: 0x06000096 RID: 150
		void GetStringValue(string name, [MarshalAs(UnmanagedType.LPWStr)] out string value);
	}
}
