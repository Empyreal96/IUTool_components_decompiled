using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000022 RID: 34
	[CLSCompliant(false)]
	[Guid("5efec991-bca3-42d1-9ec2-e92d609ec22a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IAppxBlockMapReader
	{
		// Token: 0x060000A5 RID: 165
		IAppxBlockMapFile GetFile([MarshalAs(UnmanagedType.LPWStr)] [In] string filename);

		// Token: 0x060000A6 RID: 166
		IAppxBlockMapFilesEnumerator GetFiles();

		// Token: 0x060000A7 RID: 167
		IUri GetHashMethod();

		// Token: 0x060000A8 RID: 168
		IStream GetStream();
	}
}
