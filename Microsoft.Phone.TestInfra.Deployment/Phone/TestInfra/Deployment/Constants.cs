using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.Phone.TestInfra.Deployment
{
	// Token: 0x02000013 RID: 19
	public static class Constants
	{
		// Token: 0x060000EF RID: 239 RVA: 0x0000670C File Offset: 0x0000490C
		static Constants()
		{
			try
			{
				string environmentVariable = Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
				Constants.numOfLoaders = int.Parse(environmentVariable);
			}
			catch
			{
				Constants.numOfLoaders = 1;
			}
			Constants.AssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x00006794 File Offset: 0x00004994
		public static string BinaryLocationCacheExtension
		{
			get
			{
				return ".bin.loc.json";
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x000067AC File Offset: 0x000049AC
		public static string PackageLocationCacheExtension
		{
			get
			{
				return ".pkg.loc.json";
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x000067C4 File Offset: 0x000049C4
		public static string ManifestFileExtension
		{
			get
			{
				return ".man.dsm.xml";
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x000067DC File Offset: 0x000049DC
		public static string SpkgFileExtension
		{
			get
			{
				return ".spkg";
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000067F4 File Offset: 0x000049F4
		public static string CabFileExtension
		{
			get
			{
				return ".cab";
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x0000680C File Offset: 0x00004A0C
		public static string DepFileExtension
		{
			get
			{
				return ".dep.xml";
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00006824 File Offset: 0x00004A24
		public static List<string> DependencyProjects
		{
			get
			{
				return Constants.dependencyProjects;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000683C File Offset: 0x00004A3C
		public static int NumOfLoaders
		{
			get
			{
				return Constants.numOfLoaders;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00006854 File Offset: 0x00004A54
		public static string SupressionFileName
		{
			get
			{
				return "pkgdep_supress.txt";
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x0000686C File Offset: 0x00004A6C
		public static string PrebuiltFolderName
		{
			get
			{
				return "Prebuilt";
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00006884 File Offset: 0x00004A84
		public static string GeneralPackageLocationCacheFileName
		{
			get
			{
				return "DeployTest.GeneralPackageLocationCache.json";
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060000FB RID: 251 RVA: 0x0000689C File Offset: 0x00004A9C
		public static string GeneralBinaryPackageMappingCacheFileName
		{
			get
			{
				return "DeployTest.GeneralBinaryLocationCache.json";
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060000FC RID: 252 RVA: 0x000068B3 File Offset: 0x00004AB3
		// (set) Token: 0x060000FD RID: 253 RVA: 0x000068BA File Offset: 0x00004ABA
		public static string AssemblyDirectory { get; private set; }

		// Token: 0x0400005D RID: 93
		private static List<string> dependencyProjects = new List<string>
		{
			"Windows",
			"Test",
			"unittests"
		};

		// Token: 0x0400005E RID: 94
		private static int numOfLoaders;
	}
}
