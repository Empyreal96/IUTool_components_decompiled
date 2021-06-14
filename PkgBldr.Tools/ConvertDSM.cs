using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x0200002F RID: 47
	public class ConvertDSM
	{
		// Token: 0x06000196 RID: 406 RVA: 0x00007686 File Offset: 0x00005886
		public static bool FAILED(int hr)
		{
			return hr < 0;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000768C File Offset: 0x0000588C
		public static bool SUCCEEDED(int hr)
		{
			return hr >= 0;
		}

		// Token: 0x06000198 RID: 408
		[DllImport("ConvertDSMDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern int ConvertOneSPKG(string packages, ref ConvertDSM.CONVERTDSM_PARAMETERS pParams);

		// Token: 0x06000199 RID: 409 RVA: 0x00007698 File Offset: 0x00005898
		public static void RunDsmConverter(string input, string output, bool wow, bool ignoreConvertDsmError, uint flags)
		{
			ConvertDSM.CONVERTDSM_PARAMETERS convertdsm_PARAMETERS = default(ConvertDSM.CONVERTDSM_PARAMETERS);
			convertdsm_PARAMETERS.cbSize = (uint)Marshal.SizeOf(convertdsm_PARAMETERS);
			convertdsm_PARAMETERS.dwFlags = flags;
			convertdsm_PARAMETERS.pszOutDir = output;
			convertdsm_PARAMETERS.pszFullOutName = string.Empty;
			convertdsm_PARAMETERS.cchFullOutNameSize = 0U;
			convertdsm_PARAMETERS.cchRequired = 0U;
			if (wow)
			{
				convertdsm_PARAMETERS.dwFlags |= 4U;
			}
			int num;
			if (ConvertDSM.FAILED(num = ConvertDSM.ConvertOneSPKG(input, ref convertdsm_PARAMETERS)) && !ignoreConvertDsmError)
			{
				throw new IUException("ConvertDSM failed with error code: " + string.Format(CultureInfo.InvariantCulture, "{0} (0x{0:X})", new object[]
				{
					num
				}));
			}
		}

		// Token: 0x0400007C RID: 124
		public const string CONVERTDSMDLL_DLL = "ConvertDSMDLL.dll";

		// Token: 0x0400007D RID: 125
		public const int S_OK = 0;

		// Token: 0x02000044 RID: 68
		[Flags]
		public enum CONVERTDSM_PARAMETERS_FLAGS
		{
			// Token: 0x040000B9 RID: 185
			CONVERTDSM_PARAMETERS_FLAGS_NONE = 0,
			// Token: 0x040000BA RID: 186
			CONVERTDSM_PARAMETERS_FLAGS_MAKE_CAB = 1,
			// Token: 0x040000BB RID: 187
			CONVERTDSM_PARAMETERS_FLAGS_SIGN_OUTPUT = 2,
			// Token: 0x040000BC RID: 188
			CONVERTDSM_PARAMETERS_FLAGS_WOW_MAP_ARCH = 4,
			// Token: 0x040000BD RID: 189
			CONVERTDSM_PARAMETERS_FLAGS_SKIP_POLICY = 8,
			// Token: 0x040000BE RID: 190
			CONVERTDSM_PARAMETERS_FLAGS_USE_FILENAME_AS_NAME = 16,
			// Token: 0x040000BF RID: 191
			CONVERTDSM_PARAMETERS_FLAGS_SINGLE_COMPONENT = 32,
			// Token: 0x040000C0 RID: 192
			CONVERTDSM_PARAMETERS_FLAGS_METADATA_ONLY = 64,
			// Token: 0x040000C1 RID: 193
			CONVERTDSM_PARAMETERS_FLAGS_LEAVE_SPKG_METADATA = 128,
			// Token: 0x040000C2 RID: 194
			CONVERTDSM_PARAMETERS_FLAGS_CREATE_RECALL = 256,
			// Token: 0x040000C3 RID: 195
			CONVERTDSM_PARAMETERS_FLAGS_INCLUDE_CATALOG = 512,
			// Token: 0x040000C4 RID: 196
			CONVERTDSM_PARAMETERS_FLAGS_SIGN_TESTONLY = 4096,
			// Token: 0x040000C5 RID: 197
			CONVERTDSM_PARAMETERS_MAX_FLAGS = 512
		}

		// Token: 0x02000045 RID: 69
		public struct CONVERTDSM_PARAMETERS
		{
			// Token: 0x040000C6 RID: 198
			public uint cbSize;

			// Token: 0x040000C7 RID: 199
			public uint dwFlags;

			// Token: 0x040000C8 RID: 200
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszOutDir;

			// Token: 0x040000C9 RID: 201
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszFullOutName;

			// Token: 0x040000CA RID: 202
			public uint cchFullOutNameSize;

			// Token: 0x040000CB RID: 203
			public uint cchRequired;
		}
	}
}
