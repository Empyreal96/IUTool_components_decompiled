using System;
using System.Collections.Generic;
using Microsoft.WindowsPhone.ImageUpdate.PkgCommon;

namespace Microsoft.WindowsPhone.ImageUpdate.Customization
{
	// Token: 0x02000003 RID: 3
	public class Customizations
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002188 File Offset: 0x00000388
		// (set) Token: 0x06000008 RID: 8 RVA: 0x00002190 File Offset: 0x00000390
		public string CustomizationXMLFilePath { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002199 File Offset: 0x00000399
		// (set) Token: 0x0600000A RID: 10 RVA: 0x000021A1 File Offset: 0x000003A1
		public string CustomizationPPKGFilePath { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000021AA File Offset: 0x000003AA
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000021B2 File Offset: 0x000003B2
		public string OutputDirectory { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000021BB File Offset: 0x000003BB
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000021C3 File Offset: 0x000003C3
		public string ImageDeviceName { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000021CC File Offset: 0x000003CC
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000021D4 File Offset: 0x000003D4
		public CpuId ImageCpuType { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000021DD File Offset: 0x000003DD
		// (set) Token: 0x06000012 RID: 18 RVA: 0x000021E5 File Offset: 0x000003E5
		public BuildType ImageBuildType { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000021EE File Offset: 0x000003EE
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000021F6 File Offset: 0x000003F6
		public VersionInfo ImageVersion { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000021FF File Offset: 0x000003FF
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002207 File Offset: 0x00000407
		public List<IPkgInfo> ImagePackages { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002210 File Offset: 0x00000410
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002218 File Offset: 0x00000418
		public List<string> ImageLanguages { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002221 File Offset: 0x00000421
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002228 File Offset: 0x00000428
		public static bool StrictSettingPolicies { get; set; }

		// Token: 0x04000003 RID: 3
		public const string PROVISIONING_PKG_EXT = ".ppkg";

		// Token: 0x04000004 RID: 4
		public const string CUSTOMIZATION_XML_EXT = ".xml";
	}
}
