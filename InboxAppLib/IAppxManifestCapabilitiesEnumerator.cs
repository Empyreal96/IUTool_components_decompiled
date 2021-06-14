using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000023 RID: 35
	[CLSCompliant(false)]
	[Guid("11D22258-F470-42C1-B291-8361C5437E41")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestCapabilitiesEnumerator
	{
		// Token: 0x060000A9 RID: 169
		APPX_CAPABILITIES GetCurrent();

		// Token: 0x060000AA RID: 170
		bool GetHasCurrent();

		// Token: 0x060000AB RID: 171
		bool MoveNext();
	}
}
