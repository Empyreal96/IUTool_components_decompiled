using System;
using System.Threading;
using FFUComponents;

namespace Microsoft.Windows.ImageTools
{
	// Token: 0x0200000C RID: 12
	internal class FlashParam
	{
		// Token: 0x04000033 RID: 51
		public IFFUDevice Device;

		// Token: 0x04000034 RID: 52
		public string LogFolderPath;

		// Token: 0x04000035 RID: 53
		public string FfuFilePath;

		// Token: 0x04000036 RID: 54
		public string WimPath;

		// Token: 0x04000037 RID: 55
		public AutoResetEvent WaitHandle;

		// Token: 0x04000038 RID: 56
		public int Result;

		// Token: 0x04000039 RID: 57
		public bool FastFlash;
	}
}
