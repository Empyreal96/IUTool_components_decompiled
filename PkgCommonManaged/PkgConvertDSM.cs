using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Microsoft.WindowsPhone.ImageUpdate.PkgCommon
{
	// Token: 0x02000013 RID: 19
	public static class PkgConvertDSM
	{
		// Token: 0x060000A3 RID: 163
		[DllImport("ConvertDSMDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern void ConvertDSM_LogTo([MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString ErrorMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString WarningMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString InfoMsgHandler, [MarshalAs(UnmanagedType.FunctionPtr)] LogUtil.InteropLogString DebugMsgHandler);

		// Token: 0x060000A4 RID: 164 RVA: 0x0000661F File Offset: 0x0000481F
		public static bool FAILED(int hr)
		{
			return hr < 0;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00006625 File Offset: 0x00004825
		public static bool SUCCEEDED(int hr)
		{
			return hr >= 0;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00006630 File Offset: 0x00004830
		private static PkgConvertDSM.CONVERTDSM_PARAMETERS CreateParams(PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS Flags, string outputDir)
		{
			PkgConvertDSM.CONVERTDSM_PARAMETERS convertdsm_PARAMETERS = default(PkgConvertDSM.CONVERTDSM_PARAMETERS);
			convertdsm_PARAMETERS.cbSize = (uint)Marshal.SizeOf(convertdsm_PARAMETERS);
			convertdsm_PARAMETERS.dwFlags = (uint)Flags;
			convertdsm_PARAMETERS.pszOutDir = outputDir;
			convertdsm_PARAMETERS.pszFullOutName = string.Empty;
			convertdsm_PARAMETERS.cchFullOutNameSize = 0U;
			convertdsm_PARAMETERS.cchRequired = 0U;
			return convertdsm_PARAMETERS;
		}

		// Token: 0x060000A7 RID: 167
		[DllImport("ConvertDSMDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int ConvertExpandedPackages(string DsmFolder, string PackageRoot, ref PkgConvertDSM.CONVERTDSM_PARAMETERS pParams);

		// Token: 0x060000A8 RID: 168 RVA: 0x00006684 File Offset: 0x00004884
		public static void ConvertExpandedPackage(PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS Flags, string expandedPackage, string outputDir)
		{
			PkgConvertDSM.CONVERTDSM_PARAMETERS convertdsm_PARAMETERS = PkgConvertDSM.CreateParams(Flags, outputDir);
			int num;
			if (PkgConvertDSM.FAILED(num = PkgConvertDSM.ConvertExpandedPackages(expandedPackage, expandedPackage, ref convertdsm_PARAMETERS)))
			{
				throw new PackageException("ConvertDSM failed with error code: " + string.Format("{0} (0x{0:X})", num));
			}
		}

		// Token: 0x060000A9 RID: 169
		[DllImport("ConvertDSMDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int ConvertListOfSPKGs(string[] packagesList, uint cPackageList, ref PkgConvertDSM.CONVERTDSM_PARAMETERS pParams);

		// Token: 0x060000AA RID: 170 RVA: 0x000066CD File Offset: 0x000048CD
		public static List<string> ConvertPackagesToCBS(List<string> packageList, string outputDir)
		{
			return PkgConvertDSM.ConvertPackagesToCBS(PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_MAKE_CAB | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_SIGN_OUTPUT | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_SKIP_POLICY | PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS.CONVERTDSM_PARAMETERS_FLAGS_USE_FILENAME_AS_NAME, packageList, outputDir);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000066D8 File Offset: 0x000048D8
		public static List<string> ConvertPackagesToCBS(PkgConvertDSM.CONVERTDSM_PARAMETERS_FLAGS Flags, List<string> packageList, string outputDir)
		{
			PkgConvertDSM.CONVERTDSM_PARAMETERS convertdsm_PARAMETERS = PkgConvertDSM.CreateParams(Flags, outputDir);
			int num;
			if (PkgConvertDSM.FAILED(num = PkgConvertDSM.ConvertListOfSPKGs(packageList.ToArray(), (uint)packageList.Count, ref convertdsm_PARAMETERS)))
			{
				throw new PackageException("ConvertDSM failed with error code: " + string.Format("{0} (0x{0:X})", num));
			}
			if (string.IsNullOrEmpty(outputDir))
			{
				return (from pkg in packageList
				select Path.ChangeExtension(pkg, PkgConstants.c_strCBSPackageExtension)).ToList<string>();
			}
			return Directory.GetFiles(outputDir).ToList<string>();
		}

		// Token: 0x0400001A RID: 26
		private const string CONVERTDSMDLL_DLL = "ConvertDSMDLL.dll";

		// Token: 0x0400001B RID: 27
		public const int S_OK = 0;

		// Token: 0x02000047 RID: 71
		[Flags]
		public enum CONVERTDSM_PARAMETERS_FLAGS
		{
			// Token: 0x04000127 RID: 295
			CONVERTDSM_PARAMETERS_FLAGS_NONE = 0,
			// Token: 0x04000128 RID: 296
			CONVERTDSM_PARAMETERS_FLAGS_MAKE_CAB = 1,
			// Token: 0x04000129 RID: 297
			CONVERTDSM_PARAMETERS_FLAGS_SIGN_OUTPUT = 2,
			// Token: 0x0400012A RID: 298
			CONVERTDSM_PARAMETERS_FLAGS_WOW_MAP_ARCH = 4,
			// Token: 0x0400012B RID: 299
			CONVERTDSM_PARAMETERS_FLAGS_SKIP_POLICY = 8,
			// Token: 0x0400012C RID: 300
			CONVERTDSM_PARAMETERS_FLAGS_USE_FILENAME_AS_NAME = 16,
			// Token: 0x0400012D RID: 301
			CONVERTDSM_PARAMETERS_FLAGS_SINGLE_COMPONENT = 32,
			// Token: 0x0400012E RID: 302
			CONVERTDSM_PARAMETERS_FLAGS_METADATA_ONLY = 64,
			// Token: 0x0400012F RID: 303
			CONVERTDSM_PARAMETERS_FLAGS_LEAVE_SPKG_METADATA = 128,
			// Token: 0x04000130 RID: 304
			CONVERTDSM_PARAMETERS_FLAGS_CREATE_RECALL = 256,
			// Token: 0x04000131 RID: 305
			CONVERTDSM_PARAMETERS_FLAGS_INCLUDE_CATALOG = 512,
			// Token: 0x04000132 RID: 306
			CONVERTDSM_PARAMETERS_FLAGS_DO_NTSIGN = 1024,
			// Token: 0x04000133 RID: 307
			CONVERTDSM_PARAMETERS_FLAGS_OUTPUT_NEXT_TO_INPUT = 2048,
			// Token: 0x04000134 RID: 308
			CONVERTDSM_PARAMETERS_FLAGS_SIGN_TESTONLY = 4096,
			// Token: 0x04000135 RID: 309
			CONVERTDSM_PARAMETERS_MAX_FLAGS = 4096
		}

		// Token: 0x02000048 RID: 72
		public struct CONVERTDSM_PARAMETERS
		{
			// Token: 0x04000136 RID: 310
			public uint cbSize;

			// Token: 0x04000137 RID: 311
			public uint dwFlags;

			// Token: 0x04000138 RID: 312
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszOutDir;

			// Token: 0x04000139 RID: 313
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszFullOutName;

			// Token: 0x0400013A RID: 314
			public uint cchFullOutNameSize;

			// Token: 0x0400013B RID: 315
			public uint cchRequired;
		}
	}
}
