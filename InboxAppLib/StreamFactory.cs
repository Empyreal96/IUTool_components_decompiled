using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib
{
	// Token: 0x02000009 RID: 9
	[CLSCompliant(false)]
	public class StreamFactory
	{
		// Token: 0x06000049 RID: 73 RVA: 0x000046B1 File Offset: 0x000028B1
		public static IStream CreateFileStream(string fileName)
		{
			return StreamFactory.SHCreateStreamOnFileEx(fileName, StreamFactory.STGM.STGM_READ, 1U, false, null);
		}

		// Token: 0x0600004A RID: 74
		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
		private static extern IStream SHCreateStreamOnFileEx([In] string fileName, [In] StreamFactory.STGM mode, [In] uint attributes, [In] bool create, [In] IStream template);

		// Token: 0x02000050 RID: 80
		[Flags]
		private enum STGM
		{
			// Token: 0x040000E5 RID: 229
			STGM_READ = 0,
			// Token: 0x040000E6 RID: 230
			STGM_WRITE = 1,
			// Token: 0x040000E7 RID: 231
			STGM_READWRITE = 2
		}
	}
}
