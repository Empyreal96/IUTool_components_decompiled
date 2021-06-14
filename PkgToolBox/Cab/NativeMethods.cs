using System;
using System.Runtime.InteropServices;

namespace Microsoft.Composition.ToolBox.Cab
{
	// Token: 0x0200001F RID: 31
	internal class NativeMethods
	{
		// Token: 0x060000AE RID: 174
		[DllImport("CabApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint Cab_CreateCabSelected(string filename, string[] files, uint cFiles, string[] targetfiles, uint cTargetFiles, string tempWorkingFolder, string prefixToTrim, CabToolBox.CompressionType compressionType);

		// Token: 0x060000AF RID: 175
		[DllImport("CabApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint Cab_Extract(string filename, string outputDir);

		// Token: 0x060000B0 RID: 176
		[DllImport("CabApi.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint Cab_ExtractSelected(string filename, string outputDir, string[] filesToExtract, uint cFilesToExtract);

		// Token: 0x0400006F RID: 111
		private const string STRING_CABAPI_DLL = "CabApi.dll";

		// Token: 0x04000070 RID: 112
		private const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

		// Token: 0x04000071 RID: 113
		private const CharSet CHAR_SET = CharSet.Unicode;

		// Token: 0x04000072 RID: 114
		private const bool SET_LAST_ERROR = true;
	}
}
