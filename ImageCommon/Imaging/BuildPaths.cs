using System;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.Imaging
{
	// Token: 0x02000017 RID: 23
	public class BuildPaths
	{
		// Token: 0x060000D8 RID: 216 RVA: 0x0000BC38 File Offset: 0x00009E38
		public static string GetImagingTempPath(string defaultPath)
		{
			string environmentVariable = Environment.GetEnvironmentVariable("BUILD_PRODUCT");
			string text = Environment.GetEnvironmentVariable("OBJECT_ROOT");
			if ((!string.IsNullOrEmpty(environmentVariable) && environmentVariable.Equals("nt", StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(text))
			{
				text = Environment.GetEnvironmentVariable("TEMP");
				if (string.IsNullOrEmpty(text))
				{
					text = Environment.GetEnvironmentVariable("TMP");
					if (string.IsNullOrEmpty(text))
					{
						text = defaultPath;
					}
				}
			}
			return FileUtils.GetTempFile(text);
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x0000BCA7 File Offset: 0x00009EA7
		public static string OEMKitFMSchema
		{
			get
			{
				return "OEMKitFM.xsd";
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000DA RID: 218 RVA: 0x0000BCAE File Offset: 0x00009EAE
		public static string PropsProjectSchema
		{
			get
			{
				return "PropsProject.xsd";
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000DB RID: 219 RVA: 0x0000BCB5 File Offset: 0x00009EB5
		public static string PropsGuidMappingsSchema
		{
			get
			{
				return "PropsGuidMappings.xsd";
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000DC RID: 220 RVA: 0x0000BCBC File Offset: 0x00009EBC
		public static string PublishingPackageInfoSchema
		{
			get
			{
				return "PublishingPackageInfo.xsd";
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000DD RID: 221 RVA: 0x0000BCC3 File Offset: 0x00009EC3
		public static string FMCollectionSchema
		{
			get
			{
				return "FMCollection.xsd";
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000DE RID: 222 RVA: 0x0000BCCA File Offset: 0x00009ECA
		public static string BuildCompDBSchema
		{
			get
			{
				return "BuildCompDB.xsd";
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000DF RID: 223 RVA: 0x0000BCD1 File Offset: 0x00009ED1
		public static string UpdateCompDBSchema
		{
			get
			{
				return "UpdateCompDB.xsd";
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x0000BCD8 File Offset: 0x00009ED8
		public static string BSPCompDBSchema
		{
			get
			{
				return "BSPCompDB.xsd";
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x0000BCDF File Offset: 0x00009EDF
		public static string DeviceCompDBSchema
		{
			get
			{
				return "DeviceCompDB.xsd";
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x0000BCE6 File Offset: 0x00009EE6
		public static string CompDBChunkMappingSchema
		{
			get
			{
				return "CompDBChunkMapping.xsd";
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x0000BCED File Offset: 0x00009EED
		public static string CompDBPublishingInfoSchema
		{
			get
			{
				return "CompDBPublishingInfo.xsd";
			}
		}
	}
}
