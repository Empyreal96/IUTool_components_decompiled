using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000020 RID: 32
	[CLSCompliant(false)]
	[Guid("3b53a497-3c5c-48d1-9ea3-bb7eac8cd7d4")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestQualifiedResource
	{
		// Token: 0x0600009D RID: 157
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetLanguage();

		// Token: 0x0600009E RID: 158
		uint GetScale();

		// Token: 0x0600009F RID: 159
		DX_FEATURE_LEVEL GetDXFeatureLevel();
	}
}
