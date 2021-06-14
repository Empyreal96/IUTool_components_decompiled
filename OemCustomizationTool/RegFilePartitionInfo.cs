using System;

namespace Microsoft.WindowsPhone.ImageUpdate.OemCustomizationTool
{
	// Token: 0x0200000D RID: 13
	internal class RegFilePartitionInfo
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00003FE7 File Offset: 0x000021E7
		public RegFilePartitionInfo(string filename, string partionString)
		{
			this.regFilename = filename;
			this.partition = partionString;
		}

		// Token: 0x0400003A RID: 58
		public string regFilename;

		// Token: 0x0400003B RID: 59
		public string partition;
	}
}
