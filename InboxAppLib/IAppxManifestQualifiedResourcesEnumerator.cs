using System;
using System.Runtime.InteropServices;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x0200001F RID: 31
	[CLSCompliant(false)]
	[Guid("8ef6adfe-3762-4a8f-9373-2fc5d444c8d2")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxManifestQualifiedResourcesEnumerator
	{
		// Token: 0x0600009A RID: 154
		IAppxManifestQualifiedResource GetCurrent();

		// Token: 0x0600009B RID: 155
		bool GetHasCurrent();

		// Token: 0x0600009C RID: 156
		bool MoveNext();
	}
}
