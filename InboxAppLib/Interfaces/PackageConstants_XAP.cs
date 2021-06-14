using System;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x0200003C RID: 60
	public struct PackageConstants_XAP
	{
		// Token: 0x04000068 RID: 104
		public const string Extension = ".xap";

		// Token: 0x04000069 RID: 105
		public const string InRomInstallBaseDestinationPath = "$(runtime.commonfiles)\\InboxApps";

		// Token: 0x0400006A RID: 106
		public const string DataPartitionInstallBaseDestinationPath = "$(runtime.data)\\Programs\\{0}\\install";
	}
}
