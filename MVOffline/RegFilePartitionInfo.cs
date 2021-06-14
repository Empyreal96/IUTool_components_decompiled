using System;

namespace Microsoft.WindowsPhone.Multivariant.Offline
{
	// Token: 0x02000003 RID: 3
	public class RegFilePartitionInfo
	{
		// Token: 0x06000007 RID: 7 RVA: 0x000022EE File Offset: 0x000004EE
		public RegFilePartitionInfo(string filename, string partionString)
		{
			this.regFilename = filename;
			this.partition = partionString;
		}

		// Token: 0x04000005 RID: 5
		public string regFilename;

		// Token: 0x04000006 RID: 6
		public string partition;
	}
}
