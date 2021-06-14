using System;

namespace Microsoft.Composition.ToolBox
{
	// Token: 0x0200000E RID: 14
	public static class PkgConstants
	{
		// Token: 0x0400003B RID: 59
		public static readonly string CBSPackageExtension = ".cab";

		// Token: 0x0400003C RID: 60
		public static readonly string SPKGPackageExtension = ".spkg";

		// Token: 0x0400003D RID: 61
		public static readonly string CatExtension = ".cat";

		// Token: 0x0400003E RID: 62
		public static readonly string MumExtension = ".mum";

		// Token: 0x0400003F RID: 63
		public static readonly string ManifestExtension = ".manifest";

		// Token: 0x04000040 RID: 64
		public static readonly string MumPattern = "*" + PkgConstants.MumExtension;

		// Token: 0x04000041 RID: 65
		public static readonly string ManifestPattern = "*" + PkgConstants.ManifestExtension;

		// Token: 0x04000042 RID: 66
		public static readonly string UpdateMum = "update.mum";

		// Token: 0x04000043 RID: 67
		public static readonly string CMIV3NS = "urn:schemas-microsoft-com:asm.v3";

		// Token: 0x04000044 RID: 68
		public static readonly string RuntimeBootdrive = "$(runtime.bootdrive)";

		// Token: 0x04000045 RID: 69
		public static readonly string FileRedirectPath = "\\windows\\WinSxS\\";

		// Token: 0x04000046 RID: 70
		public static readonly string EmbeddedSigningCategory_None = "None";

		// Token: 0x04000047 RID: 71
		public static readonly string MainOsPartition = "MainOS";

		// Token: 0x04000048 RID: 72
		public static readonly string UpdateOsPartition = "UpdateOS";

		// Token: 0x04000049 RID: 73
		public static readonly string SxSVersionScope = "Sxs";

		// Token: 0x0400004A RID: 74
		public static readonly string NonSxSVersionScope = "nonSxS";

		// Token: 0x0400004B RID: 75
		public static readonly Version InvalidVersion = new Version("0.0.0.0");

		// Token: 0x0400004C RID: 76
		public static readonly string DefaultPublicKeyToken = "628844477771337a";

		// Token: 0x0400004D RID: 77
		public static readonly string NeutralCulture = "neutral";

		// Token: 0x0400004E RID: 78
		public static readonly string DefaultReleaseType = "Update";

		// Token: 0x0400004F RID: 79
		public static readonly string DefaultLanguagePattern = "_Lang_";

		// Token: 0x04000050 RID: 80
		public static readonly string DefaultResolutionPattern = "_Res_";

		// Token: 0x04000051 RID: 81
		public static readonly string DefaultWowPattern = "_Wow_";
	}
}
