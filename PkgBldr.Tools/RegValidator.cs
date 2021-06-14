using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.CompPlat.PkgBldr.Tools
{
	// Token: 0x02000018 RID: 24
	public static class RegValidator
	{
		// Token: 0x06000103 RID: 259
		[DllImport("UpdateDLL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern int ValidateRegFiles(string[] rgRegFiles, int cRegFiles, string[] rgaFiles, int cRgaFiles);

		// Token: 0x06000104 RID: 260 RVA: 0x00005748 File Offset: 0x00003948
		public static void Validate(IEnumerable<string> regFiles, IEnumerable<string> rgaFiles)
		{
			string[] array = (regFiles != null) ? regFiles.ToArray<string>() : new string[0];
			string[] array2 = (rgaFiles != null) ? rgaFiles.ToArray<string>() : new string[0];
			if (array.Length == 0 && array2.Length == 0)
			{
				return;
			}
			string[] array3 = array;
			int cRegFiles = array3.Length;
			string[] array4 = array2;
			int num = RegValidator.ValidateRegFiles(array3, cRegFiles, array4, array4.Length);
			if (num != 0)
			{
				throw new IUException("Registry validation failed, check output log for detailed failure information, err '0x{0:X8}'", new object[]
				{
					num
				});
			}
		}
	}
}
