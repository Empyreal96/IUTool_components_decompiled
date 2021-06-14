using System;

namespace Microsoft.WindowsPhone.ImageUpdate.InboxAppLib.Interfaces
{
	// Token: 0x0200003D RID: 61
	public struct PackageConstants_APPX
	{
		// Token: 0x0400006B RID: 107
		public const string AppxExtension = ".appx";

		// Token: 0x0400006C RID: 108
		public const string AppxBundleExtension = ".appxbundle";

		// Token: 0x0400006D RID: 109
		public const string AppxMetadataSubDir = "AppxMetadata";

		// Token: 0x0400006E RID: 110
		public const string InRomInstallBaseDestinationPath = "$(runtime.windows)\\InfusedApps";

		// Token: 0x0400006F RID: 111
		public const string InRomInstallApplicationsDestinationPath = "Applications";

		// Token: 0x04000070 RID: 112
		public const string InRomInstallFrameworksDestinationPath = "Frameworks";

		// Token: 0x04000071 RID: 113
		public const string InRomInstallPackagesDestinationPath = "Packages";

		// Token: 0x04000072 RID: 114
		public const string DataPartitionInstallBaseDestinationPath = "$(runtime.data)\\Programs\\WindowsApps";

		// Token: 0x04000073 RID: 115
		public const string AppxFrameworkMacroHeader = "appxframework.";

		// Token: 0x04000074 RID: 116
		public const string AppxFrameworkRegEx_VersionQuad = "(0|[1-9][0-9]{0,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])(\\.(0|[1-9][0-9]{0,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])){3}";
	}
}
